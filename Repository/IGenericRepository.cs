using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace LAB06_RodrigoLupo.Repository;

public interface IGenericRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAll(
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null);
    Task<T?> GetById(int id);
    Task<T?> GetByIdString(string id);
    Task<List<T>> GetByIds(
        IEnumerable<int> ids,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null);
    Task Add(T entity);
    Task Update(T entity);
    Task Delete(int id);
}