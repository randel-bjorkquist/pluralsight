using DAL.Core.Results;

namespace DAL.Core.Validation.Interfaces;

/// <summary>
/// Represents a generic validator capable of evaluating a target object
/// and returning a structured <see cref="Result"/>.
/// </summary>
/// <typeparam name="T">The type being validated.</typeparam>
/// <remarks>
/// Validators encapsulate reusable validation logic that can be applied to multiple object types.
/// </remarks>
public interface IValidator<T>
{
    /// <summary>
    /// Validates the specified target instance and returns a <see cref="Result"/>
    /// describing the outcome of the validation process.
    /// </summary>
    /// <param name="target">The instance to validate.</param>
    /// <param name="isCreate">
    /// When <c>true</c>, applies rules relevant to object creation and skips those
    /// only enforced for updates or deletions.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> indicating validation success or failure.
    /// </returns>
    Result Validate(T target, bool isCreate = false);
}
