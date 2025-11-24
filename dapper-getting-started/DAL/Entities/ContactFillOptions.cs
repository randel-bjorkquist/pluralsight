using DAL.Core.Models.Options;

namespace DAL.Entities;

public class ContactFillOptions : FillOptions<ContactEntity>
{
  public bool IncludeAddressesProperty { get; set; } = false; // default: do not include related Addresses
}
