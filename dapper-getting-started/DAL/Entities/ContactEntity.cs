using Dapper.Contrib.Extensions;

namespace DAL.Entities;

/// <summary>
/// Represents the Contact entity from the database, mapped to the 'Contact' table.
/// This class uses Dapper.Contrib attributes to handle discrepancies between C# class names
/// (e.g., ContactEntity) and actual table names (Contact). Specifically:
/// - [Table("Contact")] overrides the default table name convention, ensuring Contrib's
///   auto-generated SQL (e.g., InsertAsync, GetAsync) targets the correct table without
///   hardcoding strings in repositories.
/// - [Computed] or [Write(false)] on non-DB properties (e.g., flags like IsNew or navigation collections)
///   excludes them from INSERT/UPDATE operations, preventing "Invalid column name" errors
///   during CRUD, as Contrib scans all public properties by default.
/// This setup enables seamless use of Contrib's convenience methods while maintaining
/// compatibility with pure Dapper queries (attributes are ignored there).
/// </summary>
[Table("Contact")]
public class ContactEntity
{
  public int ID           {  get; set; }
  public string FirstName { get; set; } = string.Empty;
  public string LastName  { get; set; } = string.Empty;

  public string Email     { get; set; } = string.Empty;
  public string Company   { get; set; } = string.Empty;
  public string Title     { get; set; } = string.Empty;

  [Computed]  // Exclude from insert/update operations, not a real column in the database
  public bool IsNew => ID == default;

  [Write(false)]  // Exclude from insert/update operations, not a real column in the database
  public bool IsDeleted   { get; set; }

  [Write(false)]  // Exclude from insert/update operations, navigation property, not insertable
  public List<AddressEntity> Addresses { get; set; } = [];
}
