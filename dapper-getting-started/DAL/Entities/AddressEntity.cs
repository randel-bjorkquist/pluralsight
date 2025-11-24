using Dapper.Contrib.Extensions;

namespace DAL.Entities;

/// <summary>
/// Represents the Address entity from the database, mapped to the 'Address' table.
/// This class uses Dapper.Contrib attributes to handle discrepancies between C# class names
/// (e.g., AddressEntity) and actual table names (Address). Specifically:
/// - [Table("Address")] overrides the default table name convention, ensuring Contrib's
///   auto-generated SQL (e.g., InsertAsync, GetAsync) targets the correct table without
///   hardcoding strings in repositories.
/// - [Computed] or [Write(false)] on non-DB properties (e.g., navigation collections like Addresses)
///   excludes them from INSERT/UPDATE operations, preventing "Invalid column name" errors
///   during CRUD, as Contrib scans all public properties by default.
/// This setup enables seamless use of Contrib's convenience methods while maintaining
/// compatibility with pure Dapper queries (attributes are ignored there).
/// </summary>
[Table("Address")]
public class AddressEntity
{
  public int ID               { get; set; }
  public string AddressType   { get; set; } = string.Empty;
  public string StreetAddress { get; set; } = string.Empty;
  public string City          { get; set; } = string.Empty;
  public int StateID          { get; set; }
  public string PostalCode    { get; set; } = string.Empty;

  [Computed]  // Exclude from insert/update operations, not a real column in the database
  internal bool IsNew => ID == default;

  [Write(false)]  // Exclude from insert/update operations, not a real column in the database
  public bool IsDeleted       { get; set; }
}
