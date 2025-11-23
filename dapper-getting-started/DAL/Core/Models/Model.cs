using DAL.Core.Models.Interfaces;
using DAL.Core.Results;
using DAL.Core.Validation.Interfaces;

namespace DAL.Core.Models;

/// <summary>
/// Represents the minimal base class for all domain models.
/// </summary>
/// <remarks>
/// Provides a basic <see cref="Validate"/> method that derived classes can override
/// to enforce domain-specific integrity rules.
/// </remarks>
public abstract class Model : IModel, IValidatable
{
    /// <inheritdoc/>
    public virtual int Id { get; set; }

    /// <summary>
    /// Validates the model's internal state.
    /// </summary>
    /// <param name="isCreate">
    /// If <c>true</c>, skips checks that would be invalid during creation (e.g., unset <see cref="Id"/>).
    /// </param>
    public virtual void Validate(bool isCreate = false, MessageCollection? messages = null)
    {
        messages ??= [];
        
        if (!isCreate && Id <= 0)
            messages.AddError($"{nameof(Id)} must be greater than 0 (zero) when updating or deleting.");
    }
}
