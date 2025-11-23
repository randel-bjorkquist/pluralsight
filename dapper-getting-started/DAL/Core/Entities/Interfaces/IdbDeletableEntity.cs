namespace DAL.Core.Entities.Interfaces;

/// <summary> Interface for soft-deletable entities (tracks deletion state). </summary>
public interface IdbDeletableEntity : IdbEntity
{
    /// <summary> Indicates if the entity is marked as deleted (soft delete). </summary>
    bool Deleted { get; set;  }
}
