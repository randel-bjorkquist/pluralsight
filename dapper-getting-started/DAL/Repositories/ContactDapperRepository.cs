using DAL.Core.Models.Options;
using DAL.Core.Utilities;
using DAL.Entities;
using Dapper;

namespace DAL.Repositories;

public class ContactDapperRepository : Repository, IContactRepository
{
  public ContactDapperRepository(string db_connection_string) 
    : base(db_connection_string) { }

  public async Task<IEnumerable<ContactEntity>> GetAllAsync(CancellationToken token = default)
    => await dbConnection.QueryAsync<ContactEntity>("SELECT * FROM [Contact]");

  public async Task<ContactEntity?> GetByIDAsync(int id, FillOptions<ContactEntity>? options = default, CancellationToken token = default)
  {
    var fill_options = options as ContactFillOptions ?? new ContactFillOptions();
    
    var command = "SELECT C.* FROM [Contact] AS C WHERE ID = @ID;"
                + "SELECT A.ID, A.ContactID, A.AddressType AS 'Type', A.StreetAddress AS 'Street', A.City, A.StateID, A.PostalCode FROM [Address] AS A WHERE ContactID = @ID";

    using (var multiple_results = await dbConnection.QueryMultipleAsync(command, new { ID = id }))
    {
      var contact = multiple_results.Read<ContactEntity>().SingleOrDefault();

      if (contact is not null && fill_options.IncludeAddressesProperty)
      {
        var addresses = multiple_results.Read<AddressEntity>().ToList();
        contact.Addresses.AddRange(addresses ?? []);
      }

      return contact;
    }
  }

  //public async Task<ContactEntity?> GetByIDAsync(int id, FillOptions<ContactEntity>? options = default, CancellationToken token = default)
  //{
  //  var results = await GetByIDsAsync([id], options, token);
  //  return results.FirstOrDefault();
  //}

  public async Task<IEnumerable<ContactEntity>> GetByIDsAsync(IEnumerable<int> ids, FillOptions<ContactEntity>? options = default, CancellationToken token = default)
  {
    var separator  = ',';

    var command = $"SELECT C.* "
                + $"FROM [Contact] AS C "
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

    #region COMMENTED OUT: R&D code (check affected rows)
    //
    //    var affected_rows = await dbConnection.ExecuteAsync(command, entity);
    //    
    //    if(affected_rows != 1)
    //      Debug.WriteLine($"While updating Contact ({entity.ID}), more than 1 row was affected.");
    //      Debug.WriteLine(throw new InvalidOperationException($"While updating Contact with ID {entity.ID}, more than 1 row was affected.");
    //
    #endregion

    await dbConnection.ExecuteAsync(command, entity);

    return entity;

    //return await dbConnection.ExecuteAsync(command, entity)
    //                         .ContinueWith(_ => entity, token);
  }

  public async Task<bool> DeleteAsync(int id, CancellationToken token = default)
  {
    var command = "DELETE FROM [Contact] WHERE ID = @ID";
    var affected_rows = await dbConnection.ExecuteAsync( command
                                                        ,new { ID = id });
    
    return affected_rows == 1;
  }
}
