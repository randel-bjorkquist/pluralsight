using Dapper.Contrib.Extensions;

namespace DAL.Entities;

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

  [Write(false)]  // Exclude from insert/update operations, navigation property, not insertable
  public List<AddressEntity> Addresses { get; set; } = [];
}
