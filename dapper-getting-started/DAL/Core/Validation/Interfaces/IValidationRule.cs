namespace DAL.Core.Validation.Interfaces;

/// <summary>
/// Defines a single, atomic rule used by validators or domain models
/// to enforce specific constraints.
/// </summary>
/// <typeparam name="T">The type being evaluated.</typeparam>
/// <remarks>
/// Validation rules are typically composed together by <see cref="IValidator{T}"/> implementations
/// to form more complex validation behaviors.
/// </remarks>
public interface IValidationRule<T>
{
    /// <summary>
    /// Evaluates the rule against the specified target instance.
    /// </summary>
    /// <param name="target">The instance to evaluate.</param>
    /// <returns>
    /// <c>true</c> if the target satisfies this rule; otherwise, <c>false</c>.
    /// </returns>
    bool IsValid(T target);

    /// <summary>
    /// An error message describing the rule violation, used when <see cref="IsValid"/> returns <c>false</c>.
    /// </summary>
    string Message { get; }
}
