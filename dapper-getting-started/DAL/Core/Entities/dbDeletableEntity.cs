using DAL.Core.Entities.Interfaces;
using DAL.Core.Results;

namespace DAL.Core.Entities;

/// <summary>
/// DB entity with soft-delete flag, inheriting from dbEntity. Mirrors DB schema for deletable tables.
/// </summary>
public abstract class dbDeletableEntity : dbEntity, IdbDeletableEntity
{
  public virtual bool Deleted { get; set; }

  /// <summary>
  /// Validates the entity, including soft-delete rules.
  /// </summary>
  /// <param name="isCreate">True if validating during creation.</param>
  public override void Validate(bool isCreate = false, MessageCollection? messages = null)
  {
    messages ??= [];
        
    Validate(isCreate, false, messages);
  }

  /// <summary>
  /// Validates the entity, including soft-delete rules.
  /// </summary>
  /// <param name="isCreate">True if validating during creation.</param>
  /// <param name="allowDeletedOnCreate">True to permit Deleted=true on new entities (e.g., batch imports).</param>
  public virtual void Validate(bool isCreate = false, bool allowDeletedOnCreate = false, MessageCollection? messages = null)
  {
    messages ??= [];

    base.Validate(isCreate, messages);

    // Core soft-delete check: Prevent accidental deleted inserts
    if (isCreate && Deleted && !allowDeletedOnCreate)
      messages.AddError($"{nameof(Deleted)}: new entities cannot have Deleted = true unless explicitly allowed (e.g., for imports).");

    // For updates: Allow deleted entities, but optionally enforce no non-audit changes (expand as needed)
    if (!isCreate && Deleted)
    {
      // Example: if (HasNonAuditChanges()) throw new InvalidOperationException("Cannot modify non-audit properties on deleted entities.");
      // Or log: Console.WriteLine("Warning: Updating deleted entity - verify audit trail.");
    }

    // Optional: If you add DeletedAt DateTime?, auto-set or validate here
    // if (Deleted && DeletedAt == null && isCreate) DeletedAt = DateTime.UtcNow;
  }
}
