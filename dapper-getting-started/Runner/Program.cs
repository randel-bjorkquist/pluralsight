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

    GetAll_ShouldReturn_6Contacts();

//
//    // Example usage: Access config values
//    Console.WriteLine("App Setting: " + config["AppSettings:Setting1"]);
//
//    // Your main app logic here...
//    Console.WriteLine("Press any key to exit.");
//    Console.ReadKey();
  }

  private static void GetAll_ShouldReturn_6Contacts()
  {
    // Arrange
    var repository = CreateContactRepository();


    // Act
    var contacts = repository.GetAllAsync()
                             .GetAwaiter()
                             .GetResult();

    // Assert
    Console.WriteLine($"Total Contacts: {contacts.Count()}");
    Debug.Assert(contacts.Count() == 6, "Expected 6 contacts in the database.");
    contacts.Output();
  }


  private static void BuildConfiguration()
  {
    var builder = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                                            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

    config = builder.Build();
  }

  private static IContactRepository CreateContactRepository()
  {
    string dbConnectionString = config.GetConnectionString("DefaultConnection") ?? 
                                throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    return new DAL.Repositories.ContactRepository(dbConnectionString);
  }
}
