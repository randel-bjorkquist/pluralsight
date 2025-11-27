using DAL.Core.Models.Options;
using DAL.Entities;

namespace DAL.Repositories;

public interface IContactRepository
{
  Task<IEnumerable<ContactEntity>> GetAllAsync();

  Task<ContactEntity?> GetByIDAsync(int id, FillOptions<ContactEntity>? options = default);
  Task<IEnumerable<ContactEntity>> GetByIDsAsync(IEnumerable<int> ids, FillOptions<ContactEntity>? options = default);

  Task<ContactEntity> CreateAsync(ContactEntity entity);
  Task<ContactEntity> UpdateAsync(ContactEntity entity);
  Task<bool> DeleteAsync(int id);

  Task<ContactEntity> SaveAsync(ContactEntity entity);
}
