using DAL.Core.Entities.Interfaces;
using DAL.Core.Results;

namespace DAL.Core.Entities;

/// <summary>
/// Abstract base class implementing IdbEntity for shared ID logic (e.g., validation).
/// Domain objects can inherit this for concrete impl.
/// </summary>
public abstract class dbEntity : IdbEntity
{
    public virtual int Id { get; set; }

  /// <summary>
  /// Validates the entity's internal state (override in derived for custom rules).
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
