using DAL.Core.Models.Options;
using DAL.Core.Utilities;
using DAL.Entities;
using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using System.Transactions;


namespace DAL.Repositories;

public class ContactDapperStoredProcedureRepository : Repository, IContactRepository
{
  public ContactDapperStoredProcedureRepository(string db_connection_string) 
    : base(db_connection_string) { }

  #region Contact CRUD Operations

  public async Task<IEnumerable<ContactEntity>> GetAllAsync(CancellationToken token = default)
  {
      throw new NotImplementedException();
  }

  public async Task<ContactEntity?> GetByIDAsync(int id, FillOptions<ContactEntity>? options = null, CancellationToken token = default)
  {
    var fill_options = options as ContactFillOptions ?? new ContactFillOptions();
    var stored_procedure_name = "ContactGetByID";

    using (var multiple_results = await dbConnection.QueryMultipleAsync( stored_procedure_name
                                                                        ,new { ID = id }
                                                                        ,commandType: System.Data.CommandType.StoredProcedure ))
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

  public async Task<IEnumerable<ContactEntity>> GetByIDsAsync(IEnumerable<int> ids, FillOptions<ContactEntity>? options = null, CancellationToken token = default)
  {
      throw new NotImplementedException();
  }

  public async Task<ContactEntity> CreateAsync(ContactEntity entity, CancellationToken token = default)
  {
      throw new NotImplementedException();
  }

  public async Task<ContactEntity> UpdateAsync(ContactEntity entity, CancellationToken token = default)
  {
      throw new NotImplementedException();
  }

  public async Task<bool> DeleteAsync(int id, CancellationToken token = default)
  {
    var affected_rows = await dbConnection.ExecuteAsync( "ContactDelete"
                                                        ,new { ID = id }
                                                        ,commandType: CommandType.StoredProcedure );
    return affected_rows == 1;
  }

  public async Task<ContactEntity> SaveAsync(ContactEntity contact, CancellationToken token = default)
  {
    using var transaction_scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

    var contact_parameters = new DynamicParameters();
    contact_parameters.Add("@ID", value: contact.ID, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);

    contact_parameters.Add("@FirstName", contact.FirstName);
    contact_parameters.Add("@LastName", contact.LastName);

    contact_parameters.Add("@Company", contact.Company);
    contact_parameters.Add("@Title", contact.Title);
    contact_parameters.Add("@Email", contact.Email);

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

      var address_parameters = new DynamicParameters( new { ContactID     = address.ContactID
                                                           ,AddressType   = address.AddressType 
                                                           ,StreetAddress = address.StreetAddress
                                                           ,City          = address.City
                                                           ,StateID       = address.StateID
                                                           ,PostalCode    = address.PostalCode });

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
                                      ,new { ID = address.ID }
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

    foreach (var address in addresses_to_save)
    {
      if (address.IsNew)
        await CreateAsync(address, token);
      else
        await UpdateAsync(address, token);
      
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
