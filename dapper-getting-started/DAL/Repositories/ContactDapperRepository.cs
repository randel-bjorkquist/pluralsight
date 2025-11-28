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

  public async Task<IEnumerable<ContactEntity>> GetAllAsync()
    => await dbConnection.QueryAsync<ContactEntity>("SELECT * FROM [Contact]");

  //NOTE: Using 'QueryMultipleAsync' to support multi-fill options
  public async Task<ContactEntity?> GetByIDAsync(int id, FillOptions<ContactEntity>? options = default)
  {
    var fill_options = options as ContactFillOptions ?? new ContactFillOptions();
    
    var command = "SELECT C.* FROM [Contact] AS C WHERE ID = @ID;"
                + "SELECT A.* FROM [Address] AS A WHERE ContactID = @ID";

    using var multiple_results = await dbConnection.QueryMultipleAsync(command, new { ID = id });

    var contact = multiple_results.Read<ContactEntity>().SingleOrDefault();

    if (contact is not null && fill_options.IncludeAddressesProperty)
    {
      var addresses = multiple_results.Read<AddressEntity>().ToList();
      contact.Addresses.AddRange(addresses ?? []);
    }

    return contact;

    #region COMMENTED OUT: Original Code
    //
    //using (var multiple_results = await dbConnection.QueryMultipleAsync(command, new { ID = id }))
    //{
    //  var contact = multiple_results.Read<ContactEntity>().SingleOrDefault();
    //
    //  if (contact is not null && fill_options.IncludeAddressesProperty)
    //  {
    //    var addresses = multiple_results.Read<AddressEntity>().ToList();
    //    contact.Addresses.AddRange(addresses ?? []);
    //  }
    //
    //  return contact;
    //}
    //
    #endregion
  }

  #region COMMENTED OUT: R&D CODE (WIP, multi-read/multi-fill)
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

  public async Task<IEnumerable<ContactEntity>> GetByIDsAsync(IEnumerable<int> ids, FillOptions<ContactEntity>? options = default)
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

  public async Task<ContactEntity> CreateAsync(ContactEntity entity)
  {
    var command = "INSERT INTO [Contact] (FirstName, LastName, Email, Company, Title) " 
                + "VALUES (@FirstName, @LastName, @Email, @Company, @Title); " 
                + "SELECT CAST(SCOPE_IDENTITY() AS INT);";

    var id = await dbConnection.QuerySingleAsync<int>(command, entity);
    entity.ID = id;

    return entity;
  }

  public async Task<ContactEntity> UpdateAsync(ContactEntity entity)
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

  public async Task<bool> DeleteAsync(int id)
  {
    var command = "DELETE FROM [Contact] WHERE ID = @ID";
    var affected_rows = await dbConnection.ExecuteAsync( command
                                                        ,new { ID = id });
    
    return affected_rows == 1;
  }

  public async Task<ContactEntity?> SaveAsync(ContactEntity contact)
  {
    using var transaction_scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

    if (contact.IsDeleted)
    {
      await DeleteAsync(contact.ID);
      return null;
    }

    if (contact.IsNew)
      await CreateAsync(contact);
    else
      await UpdateAsync(contact);

    //NOTE: Ensure each Address has the correct 'ContactID'
    foreach (var address in contact.Addresses)
    {
      address.ContactID = contact.ID;

      //NOTE: Refactored to use below 'SaveAsync' method for 'Addresses'
      //if (address.IsNew)          await CreateAsync(address, token);
      //else if (address.IsDeleted) await DeleteAsync(address.ID, token);
      //else                        await UpdateAsync(address, token);
    }

    await SaveAsync(contact.Addresses);
      
    transaction_scope.Complete();

    return contact;
  }

  #endregion

  #region Address CRUD Operations

  public async Task<AddressEntity> CreateAsync(AddressEntity entity)
  {
    var command = "INSERT INTO [Address] (ContactID, AddressType, StreetAddress, City, StateID, PostalCode) " 
                + "VALUES (@ContactID, @AddressType, @StreetAddress, @City, @StateID, @PostalCode); " 
                + "SELECT CAST(SCOPE_IDENTITY() AS INT);";
    
    var id = await dbConnection.QuerySingleAsync<int>(command, entity);
    entity.ID = id;
    
    return entity;
  }

  public async Task<AddressEntity> UpdateAsync(AddressEntity entity)
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

  public async Task<bool> DeleteAsync(AddressEntity entity)
    => await DeleteAsync([entity]);

  public async Task<bool> DeleteAsync(IEnumerable<AddressEntity> entities)
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

  public async Task<AddressEntity?> SaveAsync(AddressEntity entity)
    => (await SaveAsync([entity])).FirstOrDefault();

  public async Task<IEnumerable<AddressEntity>> SaveAsync(IEnumerable<AddressEntity> entities)
  {
    using var transaction_scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

    var addresses_to_delete = entities.Where(e =>  e.IsDeleted).ToList();
    var addresses_to_save   = entities.Where(e => !e.IsDeleted).ToList();

    await DeleteAsync(addresses_to_delete);

    foreach (var entity in addresses_to_save)
    {
      if (entity.IsNew)
        await CreateAsync(entity);
      else
        await UpdateAsync(entity);
    }

    transaction_scope.Complete();

    return addresses_to_save;
  }

  #endregion
}
