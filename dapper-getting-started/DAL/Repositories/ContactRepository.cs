using DAL.Entities;
using System.Data;

namespace DAL.Repositories;

public class ContactRepository : IContactRepository
{
  //private readonly IDbConnection dbConnection;

  public Task<ContactEntity> CreateAsync(ContactEntity entity, CancellationToken token = default)
  {
      throw new NotImplementedException();
  }

  public Task<ContactEntity> DeleteAsync(ContactEntity entity, CancellationToken token = default)
  {
      throw new NotImplementedException();
  }

  public Task<IEnumerable<ContactEntity>> GetAll(CancellationToken token = default)
  {
      throw new NotImplementedException();
  }

  public Task<ContactEntity> GetByIDAsync(int id, CancellationToken token = default)
  {
      throw new NotImplementedException();
  }

  public Task<ContactEntity> GetByIDsAsync(IEnumerable<int> ids, CancellationToken token = default)
  {
      throw new NotImplementedException();
  }

  public Task<ContactEntity> UpdateAsync(ContactEntity entity, CancellationToken token = default)
  {
      throw new NotImplementedException();
  }
}
