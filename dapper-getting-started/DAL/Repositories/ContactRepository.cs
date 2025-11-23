using DAL.Entities;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Linq;

namespace DAL.Repositories;

public class ContactRepository : IContactRepository
{
  private readonly IDbConnection dbConnection;

  public ContactRepository(string db_connection_string)
  {
    dbConnection = new SqlConnection(db_connection_string);
  }

  public async Task<ContactEntity> CreateAsync(ContactEntity entity, CancellationToken token = default)
  {
    throw new NotImplementedException();
  }

  public async Task<ContactEntity> DeleteAsync(ContactEntity entity, CancellationToken token = default)
  {
    throw new NotImplementedException();
  }

  public async Task<IEnumerable<ContactEntity>> GetAllAsync(CancellationToken token = default)
    => dbConnection.QueryAsync<ContactEntity>("SELECT * FROM Contact")
                   .GetAwaiter()
                   .GetResult()
                   .ToList();

  public async Task<ContactEntity> GetByIDAsync(int id, CancellationToken token = default)
  {
    throw new NotImplementedException();
  }

  public async Task<ContactEntity> GetByIDsAsync(IEnumerable<int> ids, CancellationToken token = default)
  {
    throw new NotImplementedException();
  }

  public async Task<ContactEntity> UpdateAsync(ContactEntity entity, CancellationToken token = default)
  {
    throw new NotImplementedException();
  }
}
