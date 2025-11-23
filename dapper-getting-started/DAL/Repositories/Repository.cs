using Microsoft.Data.SqlClient;
using System.Data;

namespace DAL.Repositories;

public abstract class Repository
{
  public readonly IDbConnection dbConnection;

  protected Repository(string db_connection_string)
  {
    dbConnection = new SqlConnection(db_connection_string);
  }
}
