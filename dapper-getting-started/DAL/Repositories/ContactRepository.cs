using DAL.Core.Utilities;
using DAL.Entities;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DAL.Repositories;

public class ContactRepository : IContactRepository
{
  private readonly IDbConnection dbConnection;

  public ContactRepository(string db_connection_string)
  {
    dbConnection = new SqlConnection(db_connection_string);
  }

  public async Task<IEnumerable<ContactEntity>> GetAllAsync(CancellationToken token = default)
    => dbConnection.QueryAsync<ContactEntity>("SELECT * FROM [Contact]")
                   .GetAwaiter()
                   .GetResult()
                   .ToList();

  public async Task<ContactEntity?> GetByIDAsync(int id, CancellationToken token = default)
  {
    var results = await GetByIDsAsync([id], token);
    return results.FirstOrDefault();
  }

  public async Task<IEnumerable<ContactEntity>> GetByIDsAsync(IEnumerable<int> ids, CancellationToken token = default)
  {
    var separator  = ',';

    var command = $"SELECT C.* "
                + $"FROM Contact AS C "
                + $"INNER JOIN fn_CsvToInt(@IDs, @separator) AS IDs "
                + $"ON C.ID = IDs.[value]";

    return await dbConnection.QueryAsync<ContactEntity>( command
                                                        ,new { IDs = ids.Join(separator)
                                                              ,separator });
  }

  public async Task<ContactEntity> CreateAsync(ContactEntity entity, CancellationToken token = default)
  {
    var command = "INSERT INTO [Contact] (FirstName, LastName, Email, Company, Title) " 
                + "VALUES (@FirstName, @LastName, @Email, @Company, @Title); " 
                + "SELECT CAST(SCOPE_IDENTITY() AS INT);";

    var id = await dbConnection.QuerySingleAsync<int>(command, entity);
    entity.ID = id;

    return entity;
  }

  public async Task<ContactEntity> UpdateAsync(ContactEntity entity, CancellationToken token = default)
  {
    var command = "UPDATE [Contact] "
                + "SET FirstName = @FirstName, "
                + "    LastName  = @LastName, "
                + "    Email     = @Email, "
                + "    Company   = @Company, "
                + "    Title     = @Title "
                + "WHERE ID = @ID";

    var affected_rows = await dbConnection.ExecuteAsync(command, entity);
    
//    if(affected_rows != 1)
//      Debug.WriteLine($"While updating Contact ({entity.ID}), more than 1 row was affected.");
//      Debug.WriteLine(throw new InvalidOperationException($"While updating Contact with ID {entity.ID}, more than 1 row was affected.");

    return entity;

    //return await dbConnection.ExecuteAsync(command, entity)
    //                         .ContinueWith(_ => entity, token);
  }

  public async Task<bool> DeleteAsync(int id, CancellationToken token = default)
  {
    //var affected_rows = await dbConnection.ExecuteAsync( "DELETE FROM [Contact] WHERE ID = @ID"
    //                                                    ,new { ID = id } );
    
    var command = "DELETE FROM [Contact] WHERE ID = @ID";
    var affected_rows = await dbConnection.ExecuteAsync( command
                                                        ,new { ID = id });
    
    return affected_rows == 1;
  }
}
