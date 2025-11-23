using Dapper.Contrib.Extensions;

namespace DAL.Entities;

/// <summary>
/// Represents the State entity from the database, mapped to the 'State' table.
/// This class uses Dapper.Contrib attributes to handle discrepancies between C# class names
/// (e.g., StateEntity) and actual table names (State). Specifically:
/// - [Table("State")] overrides the default table name convention, ensuring Contrib's
///   auto-generated SQL (e.g., InsertAsync, GetAsync) targets the correct table without
///   hardcoding strings in repositories.
/// - [Computed] or [Write(false)] on non-DB properties (if any, e.g., computed flags or navigations)
///   excludes them from INSERT/UPDATE operations, preventing "Invalid column name" errors
///   during CRUD, as Contrib scans all public properties by default.
/// This setup enables seamless use of Contrib's convenience methods while maintaining
/// compatibility with pure Dapper queries (attributes are ignored there).
/// </summary>
[Table("State")]
public class StateEntity
{
  public int ID               { get; set; }
  public string Name          { get; set; } = string.Empty;
  public string Abbreviation  { get; set; } = string.Empty;

  [Computed]  // Exclude from insert/update operations, not a real column in the database
  public bool IsNew => ID == default;

  [Write(false)]  // Exclude from insert/update operations, not a real column in the database
  public bool IsDeleted { get; set; }
}
