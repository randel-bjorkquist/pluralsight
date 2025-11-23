using DAL.Core.Models.Interfaces;
using DAL.Core.Results;

namespace DAL.Core.Models;

/// <summary>
/// Extends <see cref="Model"/> with soft-delete semantics.
/// </summary>
public abstract class DeletableModel : Model, IDeletableModel
{
    /// <inheritdoc/>
    public virtual bool Deleted { get; set; }

    /// <inheritdoc/>
    public override void Validate(bool isCreate = false, MessageCollection? messages = null)
    {
        messages ??= [];

        base.Validate(isCreate, messages);
    }
}
