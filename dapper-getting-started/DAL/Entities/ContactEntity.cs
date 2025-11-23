namespace DAL.Entities;

public class ContactEntity
{
  public int ID           {  get; set; }
  public string FirstName { get; set; } = string.Empty;
  public string LastName  { get; set; } = string.Empty;

  public string Email     { get; set; } = string.Empty;
  public string Company   { get; set; } = string.Empty;
  public string Title     { get; set; } = string.Empty;

  public bool IsNew => ID == default;
  public List<AddressEntity> Addresses { get; set; } = [];
}
