namespace DAL.Entities;

public class AddressEntity
{
  public int ID             { get; set; }
  public string Type        { get; set; } = string.Empty;
  public string Street      { get; set; } = string.Empty;
  public string City        { get; set; } = string.Empty;
  public int StateID        { get; set; }
  public string PostalCode  { get; set; } = string.Empty;

  internal bool IsNew => ID == default;
  public bool IsDeleted {  get; set; }
}
