using DAL.Core.Infrastructure;
using DAL.Entities;
using DAL.Repositories;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace Runner;

internal class Program
{
  // Private static IConfigurationRoot field (global, outside Main—as per tutorial)
  private static IConfigurationRoot? config;

  static void Main(string[] args)
  {
    // Build config once in Main
    BuildConfiguration();

    //GetAll_ShouldReturn_6Contacts();
    //Insert_ShouldAssignIdentity_ToNewEntity();

    //var id = 8;
    //GetByID_ShouldReturn_ContactEntity(id);

    //var ids = new List<int> { 1, 3, 5, 7 };
    //GetByIDs_ShouldReturn_ContactEntities(ids);

    //var id = 8;
    //Update_ShouldModify_ExistingEntity(id);

    //var id = 8;
    //Delete_ShouldRemove_ExistingEntity(id);

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
    var repository = ContactRepository();


    // Act
    var contacts = repository.GetAllAsync()
                             .GetAwaiter()
                             .GetResult();

    // Assert
    Console.WriteLine($"Total Contacts: {contacts.Count()}");
    Debug.Assert(contacts.Count() == 6, "Expected 6 contacts in the database.");
    contacts.Output();
  }

  private static void Insert_ShouldAssignIdentity_ToNewEntity() 
  {
    // Arrange
    IContactRepository repository = ContactRepository();
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
  }

  private static void GetByID_ShouldReturn_ContactEntity(int id)
  {
    // Arrange
    IContactRepository repository = ContactRepository();
    
    // Act
    var contact = repository.GetByIDAsync(id)
                            .GetAwaiter()
                            .GetResult();
    // Assert
    Console.WriteLine("*** GetByID ContactEntity ***");
    Debug.Assert(contact != null, $"Contact with ID {id} should exist.");
    contact.Output();

    Debug.Assert(contact.FirstName == "Joe", "First name should be Joe.");
    Debug.Assert(contact.FirstName == "Blow", "Last name should be Blow.");
  }

  private static void GetByIDs_ShouldReturn_ContactEntities(IEnumerable<int> ids)
  {
    // Arrange
    IContactRepository repository = ContactRepository();
    
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
    IContactRepository repository = ContactRepository();
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
    IContactRepository repository2 = ContactRepository();
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
    IContactRepository repository = ContactRepository();
    
    // Act: Delete
    var result = repository.DeleteAsync(id)
                           .GetAwaiter()
                           .GetResult();

    Debug.Assert(result == true, $"Contact with ID '{id}' should be deleted.");

    // Try to retrieve again to verify deletion
    IContactRepository repository2 = ContactRepository();
    var deleted_contact = repository2.GetByIDAsync(id)
                                     .GetAwaiter()
                                     .GetResult();
    // Assert
    Console.WriteLine("*** Contact Deleted ***");
    Debug.Assert(deleted_contact == null, $"Contact with ID '{id}' should be deleted.");
  }

  private static void BuildConfiguration()
  {
    var builder = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                                            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

    config = builder.Build();
  }

  private static IContactRepository ContactRepository()
  {
    //NOTE: using connection string from appsettings.json
    //string dbConnectionString = config?.GetConnectionString("DefaultConnection") ?? 
    //                            throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

    //NOTE: building connection string manually via SqlFactory
    string dbConnectionString = new SqlFactory().BuildSqlConnection( server: "localhost\\SQL2022"
                                                                    ,database: "Pluralsight_ContactDB"
                                                                    ,username: "sa"
                                                                    ,password: "SQLP@ssw0rd"
                                                                    ,encrypt: true
                                                                    ,trust_certificate: true).ConnectionString;

    return new DAL.Repositories.ContactRepository(dbConnectionString);
  }
}
