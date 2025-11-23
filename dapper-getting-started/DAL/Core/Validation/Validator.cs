using DAL.Core.Validation.Interfaces;
using DAL.Core.Results;

namespace DAL.Core.Validation;

/// <summary>
/// Provides a minimal, extensible base class for implementing validators.
/// </summary>
/// <typeparam name="T">The type being validated.</typeparam>
/// <remarks>
/// Derived validators can compose multiple <see cref="IValidationRule{T}"/> instances
/// to build comprehensive validation workflows.
/// </remarks>
/// <inheritdoc cref="IValidator{T}"/>
public abstract class Validator<T> : IValidator<T>
{
    /// <inheritdoc />
    public abstract Result Validate(T target, bool isCreate = false);
}