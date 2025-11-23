using DAL.Core.Entities.Interfaces;
using DAL.Core.Models.Interfaces;

namespace DAL.Core.Extensions;

public static class EntityModelExtensions
{
    /// <summary>
    /// Converts a <typeparamref name="TEntity"/> instance to a <typeparamref name="TModel"/> instance.
    /// </summary>
    /// <typeparam name="TModel">The target model type, must implement IModel.</typeparam>
    /// <typeparam name="TEntity">The source entity type, must derive from dbEntity.</typeparam>
    /// <param name="entity">The entity to convert.</param>
    /// <returns>The converted model, or null if the entity is null.</returns>
    public static TModel? ToModel<TModel, TEntity>(this TEntity? entity)
        where TModel  : class, IModel, new()  // Assumes models have parameterless ctor; adjust if using factories
        where TEntity : class, IdbEntity
    {
        if (entity == null) return null;

        // Create model instance
        var model = new TModel { Id = entity.Id };

        // Handle Deleted if applicable
        if (model is IDeletableModel deletableModel && entity is IdbDeletableEntity deletableEntity)
        {
            deletableModel.Deleted = deletableEntity.Deleted;
        }

        // Base validation (optional; can be called post-conversion in service)
        // model.Validate();

        return model;
    }

    /// <summary>
    /// Batch conversion for collections.
    /// </summary>
    public static IEnumerable<TModel?> ToModels<TModel, TEntity>(this IEnumerable<TEntity>? entities)
        where TModel  : class, IModel, new()
        where TEntity : class, IdbEntity
    {
        if (entities == null) yield break;

        foreach (var entity in entities)
        {
            yield return entity.ToModel<TModel, TEntity>();
        }
    }

    /// <summary>
    /// Converts a <typeparamref name="TModel"/> instance to a <typeparamref name="TEntity"/> instance.
    /// </summary>
    /// <typeparam name="TEntity">The target entity type, must derive from dbEntity or implement IdbEntity.</typeparam>
    /// <typeparam name="TModel">The source model type, must implement IModel.</typeparam>
    /// <param name="model">The model to convert.</param>
    /// <returns>The converted entity, or null if the model is null.</returns>
    public static TEntity? ToEntity<TEntity, TModel>(this TModel? model)
        where TEntity : IdbEntity, new()  // Assumes parameterless ctor; adjust for factories. Use 'class, IdbEntity, new()' if loosening further.
        where TModel  : class, IModel
    {
        if (model == null) return default;

        // Create entity instance
        var entity = new TEntity { Id = model.Id };

        // Handle Deleted if applicable
        if (model is IDeletableModel deletableModel && entity is IdbDeletableEntity deletableEntity)
        {
            deletableEntity.Deleted = deletableModel.Deleted;
        }

        // Optional: Trigger validation post-conversion (e.g., for creates)
        // entity.Validate(isCreate: entity.Id == 0);

        return entity;
    }

    /// <summary>
    /// Batch conversion for collections.
    /// </summary>
    public static IEnumerable<TEntity?> ToEntities<TEntity, TModel>(this IEnumerable<TModel>? models)
        where TEntity : IdbEntity, new()
        where TModel  : class, IModel
    {
        if (models == null) yield break;

        foreach (var model in models)
        {
            yield return model.ToEntity<TEntity, TModel>();
        }
    }
}
