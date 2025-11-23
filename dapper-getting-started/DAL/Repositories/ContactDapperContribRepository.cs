using DAL.Core.Utilities;
using DAL.Entities;
using Dapper;
using Dapper.Contrib.Extensions;

namespace DAL.Repositories;

/// <summary>
/// A repository for ContactEntity using Dapper.Contrib for simplified CRUD operations.
/// This class leverages Dapper.Contrib's extension methods (e.g., InsertAsync, GetAsync)
/// on IDbConnection to auto-generate SQL, reducing boilerplate while respecting entity
/// attributes like [Table] for table mapping and [Computed]/[Write(false)] to exclude
/// non-DB properties (e.g., IsNew flag or Addresses navigation) from INSERT/UPDATE.
/// Why Contrib? It provides out-of-the-box async CRUD for simple scenarios, with automatic
/// PK handling (e.g., SCOPE_IDENTITY for Id assignment on insert), but falls back to
/// core Dapper for complex queries (e.g., joins). Attributes ensure no "Invalid column name"
/// errors by skipping irrelevant properties during SQL generation.
/// Compatible with pure Dapper repos—Contrib methods are optional here.
/// </summary>
public class ContactDapperContribRepository : Repository, IContactRepository
{
  public ContactDapperContribRepository(string connection_string) 
    : base(connection_string) { }

  public async Task<IEnumerable<ContactEntity>> GetAllAsync(CancellationToken token = default)
    => await dbConnection.GetAllAsync<ContactEntity>();

  public async Task<ContactEntity?> GetByIDAsync(int id, CancellationToken token = default)
    => await dbConnection.GetAsync<ContactEntity?>(id);

  public async Task<IEnumerable<ContactEntity>> GetByIDsAsync(IEnumerable<int> ids, CancellationToken token = default)
  {
    var separator = ',';
    var commant = "SELECT C.* "
                + "FROM [Contact] AS C "
                + "INNER JOIN fn_CsvToInt(@IDs, @separator) AS IDs "
                + "ON C.ID = IDs.[value]";

    return await dbConnection.QueryAsync<ContactEntity>( commant
                                                        ,new { IDs = ids.Join(separator)
                                                              ,separator });
  }

  public async Task<ContactEntity> CreateAsync(ContactEntity entity, CancellationToken token = default)
  {
    var id = await dbConnection.InsertAsync(entity);

    entity.ID = id;
    return entity;

    //var id = await dbConnection.InsertAsync(entity)
    //                     .GetAwaiter()
    //                     .GetResult();
    //
    //entity.ID = id;
    //return Task.FromResult(entity);
  }

  public async Task<bool> DeleteAsync(int id, CancellationToken token = default)
    => await dbConnection.DeleteAsync<ContactEntity>(new ContactEntity { ID = id });

  public async Task<ContactEntity> UpdateAsync(ContactEntity entity, CancellationToken token = default)
  {
    var result = await dbConnection.UpdateAsync<ContactEntity>(entity);

    return result ? entity : throw new Exception("Update failed");
  }
}
