namespace LAB06_RodrigoLupo.Repository.Unit;

public interface IUnitOfWork
{
    IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class;
    Task<int> Complete();
}