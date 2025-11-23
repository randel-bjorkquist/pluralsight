using DAL.Core.Validation.Interfaces;

namespace DAL.Core.Entities.Interfaces;

/// <summary> Base interface for all DB entities with a unique ID. </summary>
public interface IdbEntity : IValidatable
{
    /// <summary> The unique identifier for the entity. </summary>
    int Id { get; set; }
}
