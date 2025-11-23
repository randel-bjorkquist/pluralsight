using DAL.Entities;

namespace DAL.Repositories;

public interface IContactRepository
{
  Task<IEnumerable<ContactEntity>> GetAll(CancellationToken token = default);
  Task<ContactEntity> GetByIDAsync(int id, CancellationToken token = default);
  Task<ContactEntity> GetByIDsAsync(IEnumerable<int> ids, CancellationToken token = default);

  Task<ContactEntity> CreateAsync(ContactEntity entity, CancellationToken token = default);
  Task<ContactEntity> DeleteAsync(ContactEntity entity, CancellationToken token = default);
  Task<ContactEntity> UpdateAsync(ContactEntity entity, CancellationToken token = default);
}
