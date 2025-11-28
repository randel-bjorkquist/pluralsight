using DAL.Entities;
using Dapper;

namespace DAL.Repositories;

public class ContactDapperExtraRepository : Repository
{
  public ContactDapperExtraRepository(string db_connection_string)
    : base(db_connection_string) { }

  #region GetContactByID | GetContactsByIDs

  public async Task<ContactEntity?> GetContactByIDAsync(int id)
    => (await GetContactsByIDsAsync([id])).FirstOrDefault();

  public async Task<IEnumerable<ContactEntity>> GetContactsByIDsAsync(params int[] ids)
    => await dbConnection.QueryAsync<ContactEntity>("SELECT * FROM [Contact] WHERE [ID] IN @IDs"
                                                    , new { IDs = ids });

  #endregion

  #region GetDynamicContactByID | GetDynamicContactsByIDs

  public async Task<dynamic?> GetDynamicContactByIDAsync(int id)
    => (await GetDynamicContactsByIDsAsync([id])).FirstOrDefault();

  public async Task<IEnumerable<dynamic>> GetDynamicContactsByIDsAsync(params int[] ids)
    => await dbConnection.QueryAsync("SELECT * FROM [Contact] WHERE [ID] IN @IDs"
                                     , new { IDs = ids });

  #endregion

  #region BulkInsertContacts

  /// <summary>
  /// IMPORTANT: Dapper still executes individual insert statements for each entity in the collection.
  ///            It IS NOT a true bulk insert operation; it's more syntax sugar for multiple inserts.
  /// FYI:  
  ///   1. This method does not set the IDs of the inserted 'contacts' back to the entities; so if you
  ///      you need the IDs, consider using a different approach or modify the method accordingly.
  ///   2. The method returns the number of rows affected, which should equal the number of contacts inserted.
  ///   3. I've removed the SCOPE_IDENTITY() retrieval to simplify the bulk insert operation.
  /// </summary>
  /// <param name="contacts"></param>
  /// <returns></returns>
  public async Task<int> BulkInsertContacts(IEnumerable<ContactEntity> contacts)
  {
    var command = "INSERT INTO [Contact] ([FirstName], [LastName], [Email], [Company], [Title]) "
                + "VALUES (@FirstName, @LastName, @Email, @Company, @Title);";

    #region COMMENTED OUT: original SQL command
    //
    //var command = "INSERT INTO [Contact] ([FirstName], [LastName], [Email], [Company], [Title]) "
    //            + "VALUES (@FirstName, @LastName, @Email, @Company, @Title) "
    //            + "SELECT CAST(SCOPE_IDENTITY() AS INT);";
    //
    #endregion

    return await dbConnection.ExecuteAsync(command, contacts);
  }

  #endregion

  #region GetAddressesByState

  /// <summary>
  /// Literal Replacements in Dapper Queries using {=PropertyName} syntax, only for Dapper v2.0.35+, and
  /// only for boolean and numric types (int, long, double, etc.)
  /// </summary>
  /// <param name="id"></param>
  /// <returns></returns>
  public async Task<IEnumerable<AddressEntity>> GetAddressesByStateAsync(int id)
    => await dbConnection.QueryAsync<AddressEntity>( "SELECT * FROM [Address] WHERE [StateID] = {=StateID}"
                                                    ,new { StateID = id });

  #endregion

  #region GetAllContactsWithAddresses

  public async Task<IEnumerable<ContactEntity>> GetAllContactsWithAddressesAsync(bool isOneToOne = false)
  {
    var command = "SELECT * FROM [Contact] AS C INNER JOIN [Address] AS A ON C.ID = A.ContactID";

    if(isOneToOne)
    {
      // IMPORTANT: This is the correct syntax if we only have a one-to-one relationship.  For the 
      //            correct syntax for a one-to-many, look below ...
      var contacts_1_to_1 = dbConnection.Query<ContactEntity, AddressEntity, ContactEntity>(command, (contact, address) 
                              => { contact.Addresses.Add(address);
                                   return contact; });
  
      return contacts_1_to_1;
    }
    else 
    {
      var contact_dictionary = new Dictionary<int, ContactEntity>();
      var contacts_1_to_many = dbConnection.Query<ContactEntity, AddressEntity, ContactEntity>(command, (contact, address) 
                                 => {
                                      //IMPORTANT: Check to see if the 'contact' has already been inserted into the dictionary;
                                      //           if not, insert it AND per normal, assigned the 'address' to the 'contact'.
                                      if(!contact_dictionary.TryGetValue(contact.ID, out var current_contact))
                                      {
                                        current_contact = contact;
                                        contact_dictionary.Add(current_contact.ID, current_contact);
                                      }
                                      
                                      current_contact.Addresses.Add(address);
                                      
                                      return current_contact; 
                                    });
      
      return contacts_1_to_many.Distinct();
    }
  }

  #endregion
}
