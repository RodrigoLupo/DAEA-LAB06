using System.Collections;
using LAB06_RodrigoLupo.Models;

namespace LAB06_RodrigoLupo.Repository.Unit;

public class UnitOfWork: IUnitOfWork
{
    private readonly Hashtable? _repositories;
    private readonly JwtDbContext _context;
    public UnitOfWork(JwtDbContext context)
    {
        _context = context;
        _repositories = new Hashtable();
    }
    public Task<int> Complete()
    {
        return _context.SaveChangesAsync();
    }
    
    public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class
    {
        var type = typeof(TEntity).Name;
        if (_repositories.ContainsKey(type))
        {
            return (IGenericRepository<TEntity>)_repositories[type];
        }
        var repositoryType = typeof(GenericRepository<>);
        var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity)), _context);
        if (repositoryInstance != null)
        {
            _repositories.Add(type, repositoryInstance);
            return (IGenericRepository<TEntity>)repositoryInstance;
        }
        throw new Exception($"No se pudo crear la instancia del repositorio para el tipo {type}");
    }
}