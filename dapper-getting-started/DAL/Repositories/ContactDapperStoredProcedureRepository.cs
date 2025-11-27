using DAL.Core.Models.Options;
using DAL.Core.Utilities;
using DAL.Entities;
using Dapper;
using System.Data;
using System.Transactions;

namespace DAL.Repositories;

public class ContactDapperStoredProcedureRepository : Repository, IContactRepository
{
  public ContactDapperStoredProcedureRepository(string db_connection_string) 
    : base(db_connection_string) { }

  #region Contact CRUD Operations

  public async Task<IEnumerable<ContactEntity>> GetAllAsync()
    => await dbConnection.QueryAsync<ContactEntity>( "ContactGetAll"
                                                    ,commandType: CommandType.StoredProcedure );

  public async Task<ContactEntity?> GetByIDAsync(int id, FillOptions<ContactEntity>? options = null)
    => GetByIDsAsync([id], options).GetAwaiter()
                                   .GetResult()
                                   .FirstOrDefault();
  
  public async Task<IEnumerable<ContactEntity>> GetByIDsAsync(IEnumerable<int> ids, FillOptions<ContactEntity>? options = null)
  {
    var fill_options = options as ContactFillOptions ?? new ContactFillOptions();

    var separator   = ',';
    var contact_ids = ids.Join(separator);
    var parameters  = new { IDs = contact_ids, separator };

    var stored_procedure_name = "ContactGetByID";

    var contacts = await dbConnection.QueryAsync<ContactEntity>( sql: stored_procedure_name
                                                                ,param: parameters
                                                                ,commandType: CommandType.StoredProcedure);
    
    if (!contacts.IsNullOrEmpty() && fill_options.IncludeAddressesProperty)
    {
      var addresses = await AddressGetByContactIDsAsync(ids);

      foreach (var contact in contacts)
        contact.Addresses = addresses.Where(a => a.ContactID == contact.ID).ToList();
    }
    
    return contacts;
  }

  public async Task<ContactEntity?> GetByID_MultiRead_Async(int id, FillOptions<ContactEntity>? options = null)
  {  
    var fill_options = options as ContactFillOptions ?? new ContactFillOptions();
    var stored_procedure_name = "ContactGetByID_MultiRead";

    using var multiple_results = await dbConnection.QueryMultipleAsync( stored_procedure_name
                                                                       ,new { ID = id }
                                                                       ,commandType: CommandType.StoredProcedure);
    
    var contact = multiple_results.Read<ContactEntity>().SingleOrDefault();
    
    if (contact is not null && fill_options.IncludeAddressesProperty)
    {
      var addresses = multiple_results.Read<AddressEntity>().ToList();
      contact.Addresses.AddRange(addresses ?? []);
    }
    
    return contact;
  }

  public async Task<ContactEntity> CreateAsync(ContactEntity contact)
  {
    var stored_procedure_name = "ContactInsert";
    var contact_parameters    = new DynamicParameters();

    contact_parameters.Add("@ID", value: contact.ID, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);

    contact_parameters.Add("@FirstName", contact.FirstName);
    contact_parameters.Add("@LastName", contact.LastName);

    contact_parameters.Add("@Company", contact.Company);
    contact_parameters.Add("@Title", contact.Title);
    contact_parameters.Add("@Email", contact.Email);

    var affected_rows = await dbConnection.ExecuteAsync( stored_procedure_name
                                                        ,contact_parameters
                                                        ,commandType: CommandType.StoredProcedure );
    
    contact.ID = contact_parameters.Get<int>("@ID");

    return contact;
  }

  public async Task<ContactEntity> UpdateAsync(ContactEntity contact)
  {
    var stored_procedure_name = "ContactUpdate";
    var contact_parameters    = new DynamicParameters();

    contact_parameters.Add("@ID", contact.ID);
    contact_parameters.Add("@FirstName", contact.FirstName);
    contact_parameters.Add("@LastName", contact.LastName);

    contact_parameters.Add("@Email", contact.Email);
    contact_parameters.Add("@Company", contact.Company);
    contact_parameters.Add("@Title", contact.Title);

    #region OPTION (Dynamic Parameters) 2:
    //
    //var contact_parameters = new DynamicParameters( new { contact.ID
    //                                                     ,contact.FirstName
    //                                                     ,contact.LastName
    //                                                     ,contact.Email
    //                                                     ,contact.Company
    //                                                     ,contact.Title });
    //
    #endregion

    await dbConnection.ExecuteAsync(stored_procedure_name
                                    ,param: contact_parameters
                                    ,commandType: CommandType.StoredProcedure );

    return contact;
  }

  public async Task<bool> DeleteAsync(int id)
  {
    var affected_rows = await dbConnection.ExecuteAsync( "ContactDelete"
                                                        ,new { ID = id }
                                                        ,commandType: CommandType.StoredProcedure );
    return affected_rows == 1;
  }

  public async Task<ContactEntity> SaveAsync(ContactEntity contact)
  {
    using var transaction_scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

    var contact_parameters = new DynamicParameters();
    contact_parameters.Add("@ID", value: contact.ID, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);

    contact_parameters.Add("@FirstName", contact.FirstName);
    contact_parameters.Add("@LastName", contact.LastName);

    contact_parameters.Add("@Email", contact.Email);
    contact_parameters.Add("@Company", contact.Company);
    contact_parameters.Add("@Title", contact.Title);

    await dbConnection.ExecuteAsync( "ContactSave"
                                    ,contact_parameters
                                    ,commandType: CommandType.StoredProcedure );
    //var contact_affected_rows = await dbConnection.ExecuteAsync( "ContactSave"
    //                                                            ,contact_parameters
    //                                                            ,commandType: CommandType.StoredProcedure );

    contact.ID = contact_parameters.Get<int>("@ID");

    var addresses_to_delete  = contact.Addresses.Where(a =>  a.IsDeleted);
    var addresses_to_save    = contact.Addresses.Where(a => !a.IsDeleted);

    foreach(var address in addresses_to_save)
    {
      address.ContactID = contact.ID;

      var address_parameters = new DynamicParameters( new { address.ContactID
                                                           ,address.AddressType 
                                                           ,address.StreetAddress
                                                           ,address.City
                                                           ,address.StateID
                                                           ,address.PostalCode });

      address_parameters.Add("@ID", address.ID, DbType.Int32, ParameterDirection.InputOutput);
      
      await dbConnection.ExecuteAsync( "AddressSave"
                                      ,address_parameters
                                      ,commandType: CommandType.StoredProcedure );
      //var address_affected_rows = await dbConnection.ExecuteAsync( "AddressSave"
      //                                                            ,address_parameters
      //                                                            ,commandType: CommandType.StoredProcedure );

      address.ID = address_parameters.Get<int>("@ID");
    }

    foreach(var address in addresses_to_delete)
    {
      await dbConnection.ExecuteAsync( "AddressDelete"
                                      ,new { address.ID }
                                      ,commandType: CommandType.StoredProcedure);
      //var deleted_rows = await dbConnection.ExecuteAsync( "AddressDelete"
      //                                                   ,new { ID = address.ID }
      //                                                   ,commandType: CommandType.StoredProcedure);
    }

    //var delete_result = await DeleteAsync(addresses_to_delete);
    //await SaveAsync(contact.Addresses, token);

    transaction_scope.Complete();

    return contact;
  }

  #endregion

  #region Address CRUD Operations

  public async Task<IEnumerable<AddressEntity>> AddressGetByIDsAsync(IEnumerable<int> ids)
  {
    var stored_procedure_name = "AddressGetByID";
    
    var separator   = ',';
    var address_ids = ids.Join(separator);
    var parameters  = new { IDs = address_ids, separator };
    
    return await dbConnection.QueryAsync<AddressEntity>( sql: stored_procedure_name
                                                        ,param: parameters
                                                        ,commandType: CommandType.StoredProcedure );
  }

  public async Task<IEnumerable<AddressEntity>> AddressGetByContactIDsAsync(IEnumerable<int> ids)
  {
    var stored_procedure_name = "AddressGetByContactID";
    
    var separator   = ',';
    var contact_ids = ids.Join(separator);
    var parameters  = new { IDs = contact_ids, separator };
    
    return await dbConnection.QueryAsync<AddressEntity>( sql: stored_procedure_name
                                                        ,param: parameters
                                                        ,commandType: CommandType.StoredProcedure );
  }

  public async Task<AddressEntity?> SaveAsync(AddressEntity entity)
  {
    var result = await SaveAsync([entity]);
    return result.FirstOrDefault();
  }

  public async Task<IEnumerable<AddressEntity>> SaveAsync(IEnumerable<AddressEntity> entities)
  {
    using var transaction_scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
    //var result = new List<AddressEntity>();

    var addresses_to_delete = entities.Where(e =>  e.IsDeleted).ToList();
    var addresses_to_save   = entities.Where(e => !e.IsDeleted).ToList();

    await DeleteAsync(addresses_to_delete);

    foreach (var address in addresses_to_save)
    {
      if (address.IsNew)
        await CreateAsync(address);
      else
        await UpdateAsync(address);
      
      //result.Add(entity);
    }

    transaction_scope.Complete();

    //return result;
    return addresses_to_save;
  }

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
  {
    return await DeleteAsync([entity]);

    //var command = "DELETE FROM [Address] WHERE ID = @ID";
    //var affected_rows = await dbConnection.ExecuteAsync( command
    //                                                    ,new { entity.ID });
    //
    //return affected_rows == 1;
  }

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

  #endregion
}
