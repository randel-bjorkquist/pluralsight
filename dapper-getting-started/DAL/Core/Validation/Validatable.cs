using DAL.Core.Validation.Interfaces;
using DAL.Core.Results;

namespace DAL.Core.Validation;

/// <summary>
/// Provides a lightweight base implementation for self-validating objects.
/// </summary>
/// <remarks>
/// Classes inheriting from <see cref="Validatable"/> can override <see cref="Validate"/> 
/// to perform domain-specific or aggregate validation logic.
/// </remarks>
/// <inheritdoc cref="IValidatable" />
public abstract class Validatable : IValidatable
{
    /// <inheritdoc/>
    public abstract void Validate(bool isCreate = false, MessageCollection? messages = null);
}
