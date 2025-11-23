using DAL.Core.Validation.Interfaces;

namespace DAL.Core.Validation;

/// <summary>
/// Provides a simple base class for reusable validation rules.
/// </summary>
/// <typeparam name="T">The type being evaluated.</typeparam>
/// <remarks>
/// Implementations should define their validation logic within <see cref="IsValid"/>
/// and supply a corresponding <see cref="Message"/> describing the failure.
/// </remarks>
/// <inheritdoc cref="IValidationRule{T}" />
public abstract class ValidationRule<T> : IValidationRule<T>
{
    /// <inheritdoc />
    public abstract bool IsValid(T target);

    /// <inheritdoc />
    public abstract string Message { get; }
}