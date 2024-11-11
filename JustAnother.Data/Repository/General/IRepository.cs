using System.Linq.Expressions;

namespace JustAnother.DataAccess.Repository.General;

public interface IRepository<T> where T : class
{
    Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, bool tracked = true, string? includedProperties = null, int pageSize = 0, int pageNumber = 1);
    Task<T> GetAsync(Expression<Func<T, bool>>? filter = null, bool tracked = true, string? includedProperties = null, int pageSize = 0, int pageNumber = 1);
    Task CreateAsync(T entity);
    void Remove(T entity);
}
