namespace DAL.Core.Interfaces.Mappings;

public interface IEntityMapper<TEntity, TModel>
{
    TModel? ToModel(TEntity entity);
    IEnumerable<TModel> ToModels(IEnumerable<TEntity> entities);

    TEntity? ToEntity(TModel model);
    IEnumerable<TEntity> ToEntities(IEnumerable<TModel> models);
}
