using Dapper.Contrib.Extensions;

namespace DAL.Entities;

[Table("Address")]
public class AddressEntity
{
  public int ID             { get; set; }
  public string Type        { get; set; } = string.Empty;
  public string Street      { get; set; } = string.Empty;
  public string City        { get; set; } = string.Empty;
  public int StateID        { get; set; }
  public string PostalCode  { get; set; } = string.Empty;

  [Computed]  // Exclude from insert/update operations, not a real column in the database
  internal bool IsNew => ID == default;

  [Write(false)]  // Exclude from insert/update operations, not a real column in the database
  public bool IsDeleted {  get; set; }
}
