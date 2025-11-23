using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace DAL.Core.Infrastructure;

public class SqlFactory : ISqlFactory
{
  public SqlConnection BuildSqlConnection(string? settingName = null)
    => settingName == null
                    ? new SqlConnection(ConfigurationManager.AppSettings["connectionString"])
                    : new SqlConnection(ConfigurationManager.AppSettings[settingName]);

    
  public SqlConnection BuildSqlConnection(string server, string database, string username, string password, bool encrypt = true, bool trust_certificate = false)
    => BuildSqlConnection(server, database, username, password, encrypt, trust_certificate, integrated_security: false);
      
  public SqlConnection BuildSqlConnection(string server, string database, bool encrypt = true, bool trust_certificate = false)
    => BuildSqlConnection(server, database, username: null, password: null, encrypt, trust_certificate, integrated_security: true);
    
  private SqlConnection BuildSqlConnection( string server
                                          ,string database
                                          ,string? username
                                          ,string? password
                                          ,bool encrypt             = true     // Explicitly disable encryption (false) by overriding the default value (true)
                                          ,bool trust_certificate   = false    // Optional: Disable (true) trust server certificate default value (false) if needed (for non-SSL connections)
                                          ,bool integrated_security = false )
  {
    switch(integrated_security)
    {
      case true when username is not null || 
                      password is not null:
        throw new ArgumentException("Cannot specify username and/or password when using IntegratedSecurity (Windows Authentication).");
        
      case false when string.IsNullOrEmpty(username) ||
                      string.IsNullOrEmpty(password):
        throw new ArgumentException("Username and password are required for SQL Server Authentication.");        
    }
      
    var builder = new SqlConnectionStringBuilder { DataSource             = server
                                                  ,InitialCatalog         = database
                                                  ,IntegratedSecurity     = integrated_security
                                                  ,Encrypt                = encrypt
                                                  ,TrustServerCertificate = trust_certificate };
      
    if(!integrated_security)
    {
      builder.UserID    = username;
      builder.Password  = password;
    }
      
    var connection = new SqlConnection(builder.ConnectionString);
    
    #region COMMENTED OUT: Optional logging for debugging (random length masked password)
    //
    //var org_length   = builder.Password.Length;
    //var min_length   = (int)Math.Ceiling(org_length / 2.0); //e.g., 8 -> 4
    //var max_length   = org_length * 2;                      //e.g., 8 -> 16
    //var random_lenth = Random.Shared.Next(min_length, max_length + 1);
    //
    //string masked_connection_string = string.IsNullOrWhiteSpace(builder.Password)
    //                                ? builder.ConnectionString
    //                                : builder.ConnectionString
    //                                         .Replace( builder.Password
    //                                                  ,new string('*', random_lenth));
    //
    //Debug.WriteLine($"Connection built: {masked_connection_string}");
    //
    #endregion
      
    return connection;
  }
    
    
  public SqlCommand BuildSqlCommand(string procedureName, SqlConnection connection)
    => BuildSqlCommand(procedureName, connection, null);
    
  public SqlCommand BuildSqlCommand(string procedureName, SqlConnection connection, SqlTransaction? transaction)
  {
    SqlCommand command = transaction == null
                        ? new SqlCommand(procedureName, connection) 
                        : new SqlCommand(procedureName, connection, transaction);

    command.CommandType    = CommandType.StoredProcedure;                        
    command.CommandTimeout = int.Parse(ConfigurationManager.AppSettings["commandTimeOut"] ?? "30");
          
    return command;
  }
}
