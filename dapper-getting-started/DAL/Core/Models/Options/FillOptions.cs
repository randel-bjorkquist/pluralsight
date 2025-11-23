namespace DAL.Core.Models.Options;

public abstract class FillOptions<T> where T : class
{
    // Base: No-op; features add props like public bool IncludeFoo { get; set; }
    // Default constructor ensures empty options = no fills
}
