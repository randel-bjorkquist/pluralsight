using DAL.Core.Validation.Interfaces;

namespace DAL.Core.Models.Interfaces;

/// <summary> Base interface for all domain models with a unique ID. </summary>
public interface IModel : IValidatable
{
    /// <summary> The unique identifier for the model. </summary>
    int Id { get; set; }
}
