using DAL.Core.Models.Options;
using DAL.Core.Utilities;
using DAL.Entities;
using Dapper;
using System.Transactions;

namespace DAL.Repositories;

public class ContactDapperRepository : Repository, IContactRepository
{
  public ContactDapperRepository(string db_connection_string) 
    : base(db_connection_string) { }

  #region Contact CRUD Operations

  public async Task<IEnumerable<ContactEntity>> GetAllAsync(CancellationToken token = default)
    => await dbConnection.QueryAsync<ContactEntity>("SELECT * FROM [Contact]");

  public async Task<ContactEntity?> GetByIDAsync(int id, FillOptions<ContactEntity>? options = default, CancellationToken token = default)
  {
    var fill_options = options as ContactFillOptions ?? new ContactFillOptions();
    
    var command = "SELECT C.* FROM [Contact] AS C WHERE ID = @ID;"
                + "SELECT A.* FROM [Address] AS A WHERE ContactID = @ID";

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

  #region COMMENTED OUT: R&D code
  //
  //public async Task<IEnumerable<ContactEntity>> GetByIDsAsync(IEnumerable<int> ids, FillOptions<ContactEntity>? options = default, CancellationToken token = default)
  //{
  //  var separator  = ',';
  //
  //  var command = $"SELECT C.* "
  //              + $"FROM [Contact] AS C "
  //              + $"INNER JOIN fn_CsvToInt(@IDs, @separator) AS IDs "
  //              + $"ON C.ID = IDs.[value];"
  //              + ""
  //              + "SELECT A.* "
  //              + "FROM [Address] AS A "
  //              + "INNER JOIN fn_CsvToInt()"
  //              + ""
  //              + ""
  //              + ";"
  //              ;
  //
  //  return await dbConnection.QueryAsync<ContactEntity>( command
  //                                                      ,new { IDs = ids.Join(separator)
  //                                                            ,separator });
  //}
  //
  #endregion

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

  public async Task<ContactEntity> SaveAsync(ContactEntity contact, CancellationToken token = default)
  {
    using var transaction_scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

    if (contact.IsNew)
      await CreateAsync(contact, token);
    else
      await UpdateAsync(contact, token);

    //NOTE: Ensure each Address has the correct 'ContactID'
    foreach (var address in contact.Addresses)
    {
      address.ContactID = contact.ID;

      //NOTE: Refactored to use below 'SaveAsync' method for 'Addresses'
      //if (address.IsNew)          await CreateAsync(address, token);
      //else if (address.IsDeleted) await DeleteAsync(address.ID, token);
      //else                        await UpdateAsync(address, token);
    }

    await SaveAsync(contact.Addresses, token);
      
    transaction_scope.Complete();

    return contact;
  }

  #endregion

  #region Address CRUD Operations

  public async Task<AddressEntity?> SaveAsync(AddressEntity entity, CancellationToken token = default)
  {
    var result = await SaveAsync([entity], token);
    return result.FirstOrDefault();
  }

  public async Task<IEnumerable<AddressEntity>> SaveAsync(IEnumerable<AddressEntity> entities, CancellationToken token = default)
  {
    using var transaction_scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
    //var result = new List<AddressEntity>();

    var addresses_to_delete = entities.Where(e =>  e.IsDeleted).ToList();
    var addresses_to_save   = entities.Where(e => !e.IsDeleted).ToList();

    await DeleteAsync(addresses_to_delete, token);

    foreach (var entity in addresses_to_save)
    {
      if (entity.IsNew)
        await CreateAsync(entity, token);
      else
        await UpdateAsync(entity, token);
      
      //result.Add(entity);
    }

    transaction_scope.Complete();

    //return result;
    return addresses_to_save;
  }

  public async Task<AddressEntity> CreateAsync(AddressEntity entity, CancellationToken token = default)
  {
    var command = "INSERT INTO [Address] (ContactID, AddressType, StreetAddress, City, StateID, PostalCode) " 
                + "VALUES (@ContactID, @AddressType, @StreetAddress, @City, @StateID, @PostalCode); " 
                + "SELECT CAST(SCOPE_IDENTITY() AS INT);";
    
    var id = await dbConnection.QuerySingleAsync<int>(command, entity);
    entity.ID = id;
    
    return entity;
  }

  public async Task<AddressEntity> UpdateAsync(AddressEntity entity, CancellationToken token = default)
  {
    var command = "UPDATE [Address] "
                + "SET AddressType   = @AddressType, "
                + "    StreetAddress = @StreetAddress, "
                + "    City          = @City, "
                + "    StateID       = @StateID, "
                + "    PostalCode    = @PostalCode "
                + "WHERE ID = @ID";
    await dbConnection.ExecuteAsync(command, entity);

    return entity;
  }

  public async Task<bool> DeleteAsync(AddressEntity entity, CancellationToken token = default)
  {
    return await DeleteAsync([entity], token);

    //var command = "DELETE FROM [Address] WHERE ID = @ID";
    //var affected_rows = await dbConnection.ExecuteAsync( command
    //                                                    ,new { entity.ID });
    //
    //return affected_rows == 1;
  }

  public async Task<bool> DeleteAsync(IEnumerable<AddressEntity> entities, CancellationToken token = default)
  {
    if (entities.IsNullOrEmpty())
      return true;

    var separator = ',';
    
    var ids = entities.Where(e => !e.IsDeleted)
                      .Select(e => e.ID)
                      .Distinct()
                      .ToList();
    
    var command = "DELETE A FROM [Address] AS A "
                + "INNER JOIN fn_CsvToInt(@IDs, @separatOr) AS IDs "
                + "ON A.ID = IDs.[value]";
    
    var affected_rows = await dbConnection.ExecuteAsync( command
                                                        ,new { IDs = ids.Join(separator)
                                                              ,separator });
    
    return affected_rows == ids.Count;
  }

  #endregion
}
