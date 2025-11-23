using DAL.Core.Results;

namespace DAL.Core.Validation.Interfaces;

/// <summary>
/// Defines a contract for objects that can perform self-validation.
/// </summary>
/// <remarks>
/// Implementations are responsible for determining whether their current state is valid.  
/// A validation failure should typically result in an exception or an error result.
/// </remarks>
public interface IValidatable
{
    /// <summary>
    /// Performs validation and throws an exception if the object is invalid. Implementations 
    /// should not return a value — they either succeed or throw.
    /// </summary>
    /// <param name="isCreate">
    /// If <c>true</c>, skips checks that would be invalid during creation (e.g., unset <see cref="Id"/>).
    /// </param>
    void Validate(bool isCreate = false, MessageCollection? messages = null);
}
