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
    => (await GetByIDsAsync([id], options)).FirstOrDefault();
  
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
      {
        //NOTE: using the new 'collection expression'
        contact.Addresses = [.. addresses.Where(a => a.ContactID == contact.ID)];
        
        //contact.Addresses = addresses.Where(a => a.ContactID == contact.ID)
        //                             .ToList();
      }
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

    await dbConnection.ExecuteAsync( stored_procedure_name
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

    await dbConnection.ExecuteAsync( stored_procedure_name
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

    foreach (var address in contact.Addresses)
      address.ContactID = contact.ID;

    await SaveAsync(contact.Addresses);

    #region COMMENTED OUT: original code
    //
    //var contact_parameters = new DynamicParameters();
    //contact_parameters.Add("@ID", value: contact.ID, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
    //
    //contact_parameters.Add("@FirstName", contact.FirstName);
    //contact_parameters.Add("@LastName", contact.LastName);
    //
    //contact_parameters.Add("@Email", contact.Email);
    //contact_parameters.Add("@Company", contact.Company);
    //contact_parameters.Add("@Title", contact.Title);
    //
    //await dbConnection.ExecuteAsync( "ContactSave"
    //                                ,contact_parameters
    //                                ,commandType: CommandType.StoredProcedure );
    ////var contact_affected_rows = await dbConnection.ExecuteAsync( "ContactSave"
    ////                                                            ,contact_parameters
    ////                                                            ,commandType: CommandType.StoredProcedure );
    //
    //contact.ID = contact_parameters.Get<int>("@ID");
    //
    //var addresses_to_delete  = contact.Addresses.Where(a =>  a.IsDeleted);
    //var addresses_to_save    = contact.Addresses.Where(a => !a.IsDeleted);
    //
    //foreach(var address in addresses_to_save)
    //{
    //  address.ContactID = contact.ID;
    //
    //  var address_parameters = new DynamicParameters( new { address.ContactID
    //                                                       ,address.AddressType 
    //                                                       ,address.StreetAddress
    //                                                       ,address.City
    //                                                       ,address.StateID
    //                                                       ,address.PostalCode });
    //
    //  address_parameters.Add("@ID", address.ID, DbType.Int32, ParameterDirection.InputOutput);
    //  
    //  await dbConnection.ExecuteAsync( "AddressSave"
    //                                  ,address_parameters
    //                                  ,commandType: CommandType.StoredProcedure );
    //  //var address_affected_rows = await dbConnection.ExecuteAsync( "AddressSave"
    //  //                                                            ,address_parameters
    //  //                                                            ,commandType: CommandType.StoredProcedure );
    //
    //  address.ID = address_parameters.Get<int>("@ID");
    //}
    //
    //foreach(var address in addresses_to_delete)
    //{
    //  await dbConnection.ExecuteAsync( "AddressDelete"
    //                                  ,new { address.ID }
    //                                  ,commandType: CommandType.StoredProcedure);
    //  //var deleted_rows = await dbConnection.ExecuteAsync( "AddressDelete"
    //  //                                                   ,new { ID = address.ID }
    //  //                                                   ,commandType: CommandType.StoredProcedure);
    //}
    //
    #endregion
    
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

  public async Task<AddressEntity> CreateAsync(AddressEntity address)
  {
    var stored_procedure_name = "AddressInsert";
    var address_parameters    = new DynamicParameters();

    address_parameters.Add("@ID", value: address.ID, dbType: DbType.Int32, direction: ParameterDirection.Output);

    address_parameters.Add("@ContactID", address.ContactID);
    address_parameters.Add("@AddressType", address.AddressType);

    address_parameters.Add("@StreetAddress", address.StreetAddress);
    address_parameters.Add("@City", address.City);
    address_parameters.Add("@StateID", address.StateID);
    address_parameters.Add("@PostalCode", address.PostalCode);

    await dbConnection.ExecuteAsync( stored_procedure_name
                                    ,address_parameters
                                    ,commandType: CommandType.StoredProcedure );

    address.ID = address_parameters.Get<int>("@ID");
    
    return address;
  }

  public async Task<AddressEntity> UpdateAsync(AddressEntity address)
  {
    var stored_procedure_name = "AddressUpdate";
    var address_parameters    = new DynamicParameters();

    address_parameters.Add("@ID", address.ID);
    address_parameters.Add("@ContactID", address.ContactID);
    address_parameters.Add("@AddressType", address.AddressType);

    address_parameters.Add("@StreetAddress", address.StreetAddress);
    address_parameters.Add("@City", address.City);
    address_parameters.Add("@StateID", address.StateID);
    address_parameters.Add("@PostalCode", address.PostalCode);

    await dbConnection.ExecuteAsync( stored_procedure_name
                                    ,address_parameters
                                    ,commandType: CommandType.StoredProcedure );

    return address;
  }

  public async Task<bool> DeleteAsync(AddressEntity address)
    => await DeleteAsync([address]);

  public async Task<bool> DeleteAsync(IEnumerable<AddressEntity> addresses)
  {
    if (addresses.IsNullOrEmpty())
      return true;

    var stored_procedure_name = "AddressDelete";
    var address_parameters    = new DynamicParameters();

    var separator = ',';
    
    var ids = addresses.Where(e => !e.IsDeleted)
                       .Select(e => e.ID)
                       .Distinct()
                       .ToList();

    address_parameters.Add("@IDs", ids);
    address_parameters.Add("@separator", separator);

    var affected_rows = await dbConnection.ExecuteAsync( stored_procedure_name
                                                        ,address_parameters
                                                        ,commandType: CommandType.StoredProcedure );
    
    return affected_rows == ids.Count;
  }

  public async Task<AddressEntity?> SaveAsync(AddressEntity address)
    => (await SaveAsync([address])).FirstOrDefault();

  public async Task<IEnumerable<AddressEntity>> SaveAsync(IEnumerable<AddressEntity> addresses)
  {
    using var transaction_scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

    var addresses_to_delete = addresses.Where(e =>  e.IsDeleted).ToList();
    var addresses_to_save   = addresses.Where(e => !e.IsDeleted).ToList();

    await DeleteAsync(addresses_to_delete);

    foreach (var address in addresses_to_save)
    {
      if (address.IsNew)
        await CreateAsync(address);
      else
        await UpdateAsync(address);
    }

    transaction_scope.Complete();

    return addresses_to_save;
  }

  #endregion
}
