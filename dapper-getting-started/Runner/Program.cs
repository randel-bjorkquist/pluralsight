using DAL.Core.Infrastructure;
using DAL.Core.Models.Options;
using DAL.Entities;
using DAL.Repositories;
using System.Diagnostics;

namespace Runner;

internal class Program
{
  static void Main()
  {
    //NOTE: defines which repository type to use: 'Dapper', 'DapperContrib', or 'DapperStoredProcedure'
    RepositoryFactory.Type = RepositoryType.DapperStoredProcedure;

    #region Initial Test Calls, simply CRUD operations

    //GetAll_ShouldReturn_6Contacts();
    //Insert_ShouldAssignIdentity_ToNewEntity();

    //var id = 8;
    //var id = Insert_ShouldAssignIdentity_ToNewEntity();
    //GetByID_ShouldReturn_ContactEntity(id);

    //var ids = new List<int> { 1, 3, 5, 7 };
    //GetByIDs_ShouldReturn_ContactEntities(ids);

    //var id = 8;
    //Update_ShouldModify_ExistingEntity(id);

    //var id = 8;
    //Delete_ShouldRemove_ExistingEntity(id);

    #endregion

    #region Test Call: GetByID with ContactFillOptions to include Addresses
    //
    //var repository = RepositoryFactory.CreateContactRepository();
    //
    //var mjs_id  = 1;
    //var options = new ContactFillOptions { IncludeAddressesProperty = true };
    //
    //var mj = repository.GetByIDAsync(mjs_id, options)
    //                   .GetAwaiter()
    //                   .GetResult();
    //mj?.Output();
    //
    #endregion


    var id = Save_ShouldAssignIdentity_ToNewEntity();
    Save_ShouldModify_ExistingEntity(id);


    #region COMMENTED OUT: R&D code for future reference

    //    // Example usage: Access config values
    //    Console.WriteLine("App Setting: " + config["AppSettings:Setting1"]);
    //
    //    // Your main app logic here...
    //    Console.WriteLine("Press any key to exit.");
    //    Console.ReadKey();

    #endregion
  }

  private static void GetAll_ShouldReturn_6Contacts()
  {
    // Arrange
    var row_count   = 6;
    var repository  = RepositoryFactory.CreateContactRepository();


    // Act
    var contacts = repository.GetAllAsync()
                             .GetAwaiter()
                             .GetResult();

    // Assert
    Console.WriteLine($"Total Contacts: {contacts.Count()}");
    Debug.Assert(contacts.Count() == row_count, $"Expected '{row_count}' contacts in the database.");
    contacts.Output();
  }

  private static int Insert_ShouldAssignIdentity_ToNewEntity() 
  {
    // Arrange
    IContactRepository repository = RepositoryFactory.CreateContactRepository();
    var contact = new ContactEntity { FirstName = "Joe"
                                     ,LastName  = "Blow"
                                     ,Email     = "joe.blow@gmail.com"
                                     ,Company   = "Microsoft"
                                     ,Title     = "Developer" };

    // Act
    var new_contact_entity = repository.CreateAsync(contact)
                                       .GetAwaiter()
                                       .GetResult();

    // Assert
    Debug.Assert(contact.ID != 0);
    Console.WriteLine("*** Contact Inserted ***");
    Console.WriteLine($"New ID: {contact.ID}");

    contact.Output();
    new_contact_entity.Output();

    return new_contact_entity.ID;
  }

  private static void GetByID_ShouldReturn_ContactEntity(int id)
  {
    // Arrange
    IContactRepository repository = RepositoryFactory.CreateContactRepository();

    // Act
    var contact = repository.GetByIDAsync(id)
                            .GetAwaiter()
                            .GetResult();
    // Assert
    Console.WriteLine("*** GetByID ContactEntity ***");
    Debug.Assert(contact != null, $"Contact with ID {id} should exist.");
    contact.Output();

    Debug.Assert(contact.FirstName == "Joe"  ,"First name should be Joe.");
    Debug.Assert(contact.LastName  == "Blow" ,"Last name should be Blow.");
  }

  private static void GetByIDs_ShouldReturn_ContactEntities(IEnumerable<int> ids)
  {
    // Arrange
    IContactRepository repository = RepositoryFactory.CreateContactRepository();

    // Act
    var contacts = repository.GetByIDsAsync(ids)
                             .GetAwaiter()
                             .GetResult();
    // Assert
    Console.WriteLine("*** GetByIDs ContactEntities ***");
    Debug.Assert(contacts.Count() == ids.Count(), $"Should return {ids.Count()} contacts.");
    contacts.Output();
  }

  private static void Update_ShouldModify_ExistingEntity(int id)
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
    repository.UpdateAsync(contact!)
              .GetAwaiter()
              .GetResult();

    // 2nd Act: Retrieve again to verify
    IContactRepository repository2 = RepositoryFactory.CreateContactRepository();
    var updated_contact = repository2.GetByIDAsync(id)
                                     .GetAwaiter()
                                     .GetResult();

    // Assert
    Console.WriteLine("*** Contact Updated ***");
    updated_contact?.Output();

    Debug.Assert( updated_contact?.Company == "Updated Company"
                 ,"Company should be updated." );
  }

  private static void Delete_ShouldRemove_ExistingEntity(int id)
  {
    // Arrange
    IContactRepository repository = RepositoryFactory.CreateContactRepository();

    // Act: Delete
    var result = repository.DeleteAsync(id)
                           .GetAwaiter()
                           .GetResult();

    Debug.Assert(result == true, $"Contact with ID '{id}' should be deleted.");

    // Try to retrieve again to verify deletion
    IContactRepository repository2 = RepositoryFactory.CreateContactRepository();
    var deleted_contact = repository2.GetByIDAsync(id)
                                     .GetAwaiter()
                                     .GetResult();
    // Assert
    Console.WriteLine("*** Contact Deleted ***");
    Debug.Assert(deleted_contact == null, $"Contact with ID '{id}' should be deleted.");
  }

  private static int Save_ShouldAssignIdentity_ToNewEntity() 
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
    var new_contact_entity = repository.SaveAsync(contact)
                                       .GetAwaiter()
                                       .GetResult();

    // Assert
    Debug.Assert(contact.ID != 0);
    Console.WriteLine("*** Contact Inserted ***");
    Console.WriteLine($"New ID: {contact.ID}");

    contact.Output();
    new_contact_entity.Output();

    return new_contact_entity.ID;
  }

  private static void Save_ShouldModify_ExistingEntity(int id)
  {
    // Arrange
    IContactRepository repository = RepositoryFactory.CreateContactRepository();

    var options = new ContactFillOptions { IncludeAddressesProperty = true };
    var contact = repository.GetByIDAsync(id, options)
                            .GetAwaiter()
                            .GetResult();
    contact?.Output();

    Debug.Assert(contact != null, $"Contact with ID '{id}' should exist.");
    
    contact?.Company = "Updated Company";
    contact?.Addresses[0].StreetAddress = "456 Main Street";

    // 1st Act: Update
    repository.SaveAsync(contact!)
              .GetAwaiter()
              .GetResult();

    // 2nd Act: Retrieve again to verify
    IContactRepository repository2 = RepositoryFactory.CreateContactRepository();
    var updated_contact = repository2.GetByIDAsync(id, options)
                                     .GetAwaiter()
                                     .GetResult();

    // Assert
    Console.WriteLine("*** Contact Updated ***");
    updated_contact?.Output();

    Debug.Assert( updated_contact?.Company == "Updated Company"
                 ,"Company should be updated." );

    Debug.Assert(updated_contact?.Addresses.First().StreetAddress == "456 Main Street"
                 ,"Address should be updated." );
  }

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
