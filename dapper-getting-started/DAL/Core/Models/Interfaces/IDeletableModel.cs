namespace DAL.Core.Models.Interfaces;

/// <summary> Interface for soft-deletable domain models (tracks deletion state). </summary>
public interface IDeletableModel : IModel
{
    /// <summary> Indicates if the model is marked as deleted (soft delete). </summary>
    bool Deleted { get; set;  }
}
