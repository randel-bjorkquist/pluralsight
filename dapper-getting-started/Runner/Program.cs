using DAL.Core.Infrastructure;
using DAL.Core.Models.Options;
using DAL.Entities;
using DAL.Repositories;
using System.Diagnostics;

namespace Runner;

internal class Program
{
  static async Task Main()
  {
    //NOTE: defines which repository type to use: 'Dapper', 'DapperContrib', or 'DapperStoredProcedure'
    RepositoryFactory.Type = RepositoryType.DapperStoredProcedure;

    #region Initial Test Calls, simply CRUD operations

    //await GetAll_ShouldReturn_6Contacts_Async();
    //await Insert_ShouldAssignIdentity_ToNewEntity_Async();

    //var id = 6;
    //var id = await Insert_ShouldAssignIdentity_ToNewEntity_Async();
    //await GetByID_ShouldReturn_ContactEntity_Async(id);

    //var ids = new List<int> { 1, 3, 5, 7 };
    //await GetByIDs_ShouldReturn_ContactEntities_Async(ids);

    //var id = 8;
    //await Update_ShouldModify_ExistingEntity_Async(id);

    //var id = 8;
    //await Delete_ShouldRemove_ExistingEntity_Async(id);

    #endregion

    #region Test Call: GetByID with ContactFillOptions to include Addresses
    //
    //var repository = RepositoryFactory.CreateContactRepository();
    //
    //var mjs_id  = 1;
    //var options = new ContactFillOptions { IncludeAddressesProperty = true };
    //
    //var mj = await repository.GetByIDAsync(mjs_id, options);
    //mj?.Output();
    //
    #endregion

    #region Test Call: Insert Contact with Address via Save method(s)
    //
    //var id = await Save_ShouldAssignIdentity_ToNewEntity_Async();
    //await Save_ShouldModify_ExistingEntity_Async(id);
    //
    #endregion

    #region Test Call: GetAll example usage
    //
    //var contacts = await RepositoryFactory.CreateContactRepository()
    //                                      .GetAllAsync();
    //contacts.Output();
    //
    #endregion

    #region Test Call: Beyond the basics: DapperExtraRepository methods
    //
    //await ListSupport_ShouldProduce_CorrectResults_Async();
    await DynamicSupport_ShouldProduce_CorrectResults_Async();
    
    //await BulkInsert_ShouldInsert_4Rows_Async();
    //
    //await GetIllionoisAddresses_ShouldReturn_CorrectResults_Async();
    //
    //await GetAll_ShouldReturn_6Contacts_WithAddresses_Async();
    //
    #endregion

    #region COMMENTED OUT: R&D code for future reference

    //    // Example usage: Access config values
    //    Console.WriteLine("App Setting: " + config["AppSettings:Setting1"]);
    //
    //    // Your main app logic here...
    //    Console.WriteLine("Press any key to exit.");
    //    Console.ReadKey();

    #endregion
  }

  private static async Task GetAll_ShouldReturn_6ContactsAsync()
  {
    // Arrange
    var row_count   = 6;
    var repository  = RepositoryFactory.CreateContactRepository();


    // Act
    var contacts = await repository.GetAllAsync();

    // Assert
    Console.WriteLine($"Total Contacts: {contacts.Count()}");
    Debug.Assert(contacts.Count() == row_count, $"Expected '{row_count}' contacts in the database.");
    contacts.Output();
  }

  private static async Task GetAll_ShouldReturn_6Contacts_Async()
  {
    // Arrange
    var row_count   = 6;
    var repository  = RepositoryFactory.CreateContactRepository();


    // Act
    var contacts = await repository.GetAllAsync();

    // Assert
    Console.WriteLine($"Total Contacts: {contacts.Count()}");
    Debug.Assert(contacts.Count() == row_count, $"Expected '{row_count}' contacts in the database.");
    contacts.Output();
  }

  private static async Task<int> Insert_ShouldAssignIdentity_ToNewEntity_Async() 
  {
    // Arrange
    IContactRepository repository = RepositoryFactory.CreateContactRepository();
    var contact = new ContactEntity { FirstName = "Joe"
                                     ,LastName  = "Blow"
                                     ,Email     = "joe.blow@gmail.com"
                                     ,Company   = "Microsoft"
                                     ,Title     = "Developer" };

    // Act
    await repository.CreateAsync(contact);

    // Assert
    Debug.Assert(contact.ID != 0);
    Console.WriteLine("*** Contact Inserted ***");
    Console.WriteLine($"New ID: {contact.ID}");

    contact.Output();
    return contact.ID;
  }

  private static async Task GetByID_ShouldReturn_ContactEntity_Async(int id)
  {
    // Arrange
    IContactRepository repository = RepositoryFactory.CreateContactRepository();

    // Act
    var contact = await repository.GetByIDAsync(id);
    // Assert
    Console.WriteLine("*** GetByID ContactEntity ***");
    Debug.Assert(contact != null, $"Contact with ID {id} should exist.");
    contact.Output();

    Debug.Assert(contact.FirstName == "Joe"  ,"First name should be Joe.");
    Debug.Assert(contact.LastName  == "Blow" ,"Last name should be Blow.");
  }

  private static async Task GetByIDs_ShouldReturn_ContactEntities_Async(IEnumerable<int> ids)
  {
    // Arrange
    IContactRepository repository = RepositoryFactory.CreateContactRepository();

    // Act
    var contacts = await repository.GetByIDsAsync(ids);

    // Assert
    Console.WriteLine("*** GetByIDs ContactEntities ***");
    Debug.Assert(contacts.Count() == ids.Count(), $"Should return {ids.Count()} contacts.");
    contacts.Output();
  }

  private static async Task Update_ShouldModify_ExistingEntity_Async(int id)
  {
    // Arrange
    IContactRepository repository = RepositoryFactory.CreateContactRepository();
    var contact = repository.GetByIDAsync(id)
                            .GetAwaiter()
                            .GetResult();
    contact?.Output();

    Debug.Assert(contact != null, $"Contact with ID '{id}' should exist.");
    contact?.Company = "Updated Company";

    // 1st Act: Update
    await repository.UpdateAsync(contact!);

    // 2nd Act: Retrieve again to verify
    IContactRepository repository2 = RepositoryFactory.CreateContactRepository();
    var updated_contact = await repository2.GetByIDAsync(id);

    // Assert
    Console.WriteLine("*** Contact Updated ***");
    updated_contact?.Output();

    Debug.Assert( updated_contact?.Company == "Updated Company"
                 ,"Company should be updated." );
  }

  private static async Task Delete_ShouldRemove_ExistingEntity_Async(int id)
  {
    // Arrange
    IContactRepository repository = RepositoryFactory.CreateContactRepository();

    // Act: Delete
    var result = await repository.DeleteAsync(id);

    Debug.Assert(result == true, $"Contact with ID '{id}' should be deleted.");

    // Try to retrieve again to verify deletion
    IContactRepository repository2 = RepositoryFactory.CreateContactRepository();
    var deleted_contact = await repository2.GetByIDAsync(id);

    // Assert
    Console.WriteLine("*** Contact Deleted ***");
    Debug.Assert(deleted_contact == null, $"Contact with ID '{id}' should be deleted.");
  }

  private static async Task<int> Save_ShouldAssignIdentity_ToNewEntity_Async() 
  {
    // Arrange
    IContactRepository repository = RepositoryFactory.CreateContactRepository();
    var contact = new ContactEntity { FirstName = "Joe"
                                     ,LastName  = "Blow"
                                     ,Email     = "joe.blow@gmail.com"
                                     ,Company   = "Microsoft"
                                     ,Title     = "Developer" };

    var address = new AddressEntity { AddressType   = "Home"
                                     ,StreetAddress = "123 Main Street"
                                     ,City          = "Baltimore"
                                     ,StateID       = 1
                                     ,PostalCode    = "22222" };

    contact.Addresses.Add(address);

    // Act
    await repository.SaveAsync(contact);

    // Assert
    Debug.Assert(contact.ID != 0);
    Console.WriteLine("*** Contact Inserted ***");
    Console.WriteLine($"New ID: {contact.ID}");

    contact.Output();

    return contact.ID;
  }

  private static async Task Save_ShouldModify_ExistingEntity_Async(int id)
  {
    // Arrange
    IContactRepository repository = RepositoryFactory.CreateContactRepository();

    var options = new ContactFillOptions { IncludeAddressesProperty = true };
    var contact = await repository.GetByIDAsync(id, options);

    contact?.Output();

    Debug.Assert(contact != null, $"Contact with ID '{id}' should exist.");
    
    contact?.Company = "Updated Company";
    contact?.Addresses[0].StreetAddress = "456 Main Street";

    // 1st Act: Update
    await repository.SaveAsync(contact!);

    // 2nd Act: Retrieve again to verify
    IContactRepository repository2 = RepositoryFactory.CreateContactRepository();
    var updated_contact = await repository2.GetByIDAsync(id, options);

    // Assert
    Console.WriteLine("*** Contact Updated ***");
    updated_contact?.Output();

    Debug.Assert( updated_contact?.Company == "Updated Company"
                 ,"Company should be updated." );

    Debug.Assert(updated_contact?.Addresses.First().StreetAddress == "456 Main Street"
                 ,"Address should be updated." );
  }

  #region ContactDapperExtraRepository specific methods

  private static async Task ListSupport_ShouldProduce_CorrectResults_Async()
  {
    // Arrange
    var repository = RepositoryFactory.CreateContactDapperExtraRepository();
    var ids        = new int[] { 1, 2, 4 };
    var id         = ids.First();

    // Act
    var contact  = await repository.GetContactByIDAsync(id);
    var contacts = await repository.GetContactsByIDsAsync(ids);

    // Assert 1: single 'contact'
    Console.WriteLine("*** GetContactByID ContactEntities via id == '1' ***");

    Debug.Assert( contact is not null
                 ,$"Should have return 1 contact." );
    
    contact.Output();

    // Assert 2: list of 'contacts'
    Console.WriteLine("*** GetContactsByIDs ContactEntities via ids = new int[] {1, 2, 4} ***");
    
    Debug.Assert( contacts.Count() == ids.Length
                 ,$"Should return {ids.Length} contacts." );

    contacts.Output();
  }

  private static async Task DynamicSupport_ShouldProduce_CorrectResults_Async()
  {
    // Arrange
    var repository = RepositoryFactory.CreateContactDapperExtraRepository();
    var ids        = new int[] { 1, 2, 4 };
    var id         = ids.First();

    // Act
    var contact  = await repository.GetDynamicContactByIDAsync(id);
    var contacts = await repository.GetDynamicContactsByIDsAsync(ids);

    // Assert 1: single dynamic 'contact'
    Console.WriteLine("*** GetDynamicContactsByIDs ContactEntities via new int[] {1, 2, 4} ***");
    
    Debug.Assert( contacts is not null
                 ,$"Should have return 1 contact.");

    Console.WriteLine($"First 'FirstName': {contacts.First().FirstName}");
    (contact as object)?.Output();  //NOTE: Output single dynamic 'contact' IS NOT WORKING properly. I honestly
                                    //      don't know why, but Visual Studio says it's "due to YamlDotNet serialization
                                    //      issue"

    // Assert 2: list of dynamic 'contacts'
    Console.WriteLine("*** GetDynamicContactsByIDs ContactEntities via new int[] {1, 2, 4} ***");
    
    Debug.Assert( contacts.Count() == ids.Length
                 ,$"Should return {ids.Length} contacts.");

    Console.WriteLine($"First 'FirstName': {contacts.First().FirstName}");
    contacts.Output();
  }

  private static async Task BulkInsert_ShouldInsert_4Rows_Async()
  {
    // Arrange
    var repository = RepositoryFactory.CreateContactDapperExtraRepository();
    var contacts   = new List<ContactEntity> { new() { FirstName = "Charles", LastName = "Barkly" }
                                              ,new() { FirstName = "Scottie", LastName = "Pippen" }
                                              ,new() { FirstName = "Tim", LastName = "Duncan" }
                                              ,new() { FirstName = "Patrick", LastName = "Ewing" }};

    // Act  
    var inserted_rows = await repository.BulkInsertContacts(contacts);

    // Assert
    Console.WriteLine("*** Bulk Insert Contacts ***");
    Console.WriteLine($"Rows Inserted: {inserted_rows}");

    Debug.Assert( inserted_rows == contacts.Count
                 ,$"Should have inserted {contacts.Count} rows." );
  }

  private static async Task GetIllionoisAddresses_ShouldReturn_CorrectResults_Async()
  {
    // Arrange
    var repository = RepositoryFactory.CreateContactDapperExtraRepository();
    var state_id   = 17; //Illinois

    // Act
    var addresses = await repository.GetAddressesByStateAsync(state_id);

    // Assert
    Console.WriteLine("*** GetAddressesByStateCode 'IL' ***");
    Console.WriteLine($"Total Addresses: {addresses.Count()}");
    
    Debug.Assert( addresses.All(a => a.StateID == state_id)
                 ,$"All addresses should have the StateID of '{state_id}'.");

    addresses.Output();
  }

  private static async Task GetAll_ShouldReturn_6Contacts_WithAddresses_Async()
  {
    // Arrange
    var repository = RepositoryFactory.CreateContactDapperExtraRepository();

    // Act
    var contacts = await repository.GetAllContactsWithAddressesAsync();

    // Assert
    Console.WriteLine("*** GetAllContactsWithAddresses ***");
    Console.WriteLine($"Total Contacts: {contacts.Count()}");
    contacts.Output();

    Debug.Assert( contacts.Count() == 6
                 ,$"There should be 6 'contacts' with 'addresses'.");

    Debug.Assert( contacts.First().Addresses.Count == 2
                 ,$"The first contact, 'MJ', should have 2 'addresses'.");
  }
  
  #endregion
  
  private enum RepositoryType : short
  {
     Dapper                 = 1
    ,DapperContrib          = 2
    ,DapperStoredProcedure  = 3
  }
  
  private static class RepositoryFactory
  {
    public static RepositoryType Type = RepositoryType.Dapper;

    //NOTE: Builds a connection string manually via SqlFactory;
    //      DO NOT use the 'expression-bodied' syntax here!
    private static readonly string ConnectionString 
      = new SqlFactory().BuildSqlConnection( server: "localhost\\SQL2022"
                                            ,database: "Pluralsight_ContactDB"
                                            ,username: "sa"
                                            ,password: "SQLP@ssw0rd"
                                            ,encrypt: true
                                            ,trust_certificate: true)
                        .ConnectionString;

    public static IContactRepository CreateContactRepository()
      => Type switch { RepositoryType.Dapper                => CreateContactDapperRepository()
                      ,RepositoryType.DapperContrib         => CreateContactDapperContribRepository()
                      ,RepositoryType.DapperStoredProcedure => CreateContactDapperStoredProcedureRepository()
                      ,_                                    => throw new NotSupportedException("The specified repository type is not supported.") }; 

    public static IContactRepository CreateContactDapperRepository()
      => new ContactDapperRepository(ConnectionString);

    public static IContactRepository CreateContactDapperContribRepository()
      => new ContactDapperContribRepository(ConnectionString);

    public static IContactRepository CreateContactDapperStoredProcedureRepository()
      => new ContactDapperStoredProcedureRepository(ConnectionString);

    public static ContactDapperExtraRepository CreateContactDapperExtraRepository()
     => new(ConnectionString);
  }
  
  #region COMMENTED OUT: R&D code (BuildConfiguration and ContactDapperRepository methods)
  //
  //References needed:
  //using Microsoft.Extensions.Configuration;
  //
  //Place into: Main method:
  //Build config once in Main
  //BuildConfiguration();
  //
  //Private static IConfigurationRoot field (global, outside Main—as per tutorial)
  //private static IConfigurationRoot? config;
  //
  //private static void BuildConfiguration()
  //{
  //  var builder = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
  //                                          .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
  //
  //  config = builder.Build();
  //}
  //
  //private static IContactRepository ContactDapperRepository()
  //{
  //  //NOTE: using connection string from appsettings.json
  //  //string dbConnectionString = config?.GetConnectionString("DefaultConnection") ?? 
  //  //                            throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
  //
  //  //NOTE: building connection string manually via SqlFactory
  //  string dbConnectionString = new SqlFactory().BuildSqlConnection( server: "localhost\\SQL2022"
  //                                                                  ,database: "Pluralsight_ContactDB"
  //                                                                  ,username: "sa"
  //                                                                  ,password: "SQLP@ssw0rd"
  //                                                                  ,encrypt: true
  //                                                                  ,trust_certificate: true).ConnectionString;
  //
  //  return new DAL.Repositories.ContactDapperRepository(dbConnectionString);
  //}
  //
  #endregion
}
