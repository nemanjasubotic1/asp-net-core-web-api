using JustAnother.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace JustAnother.DataAccess.Repository.General;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly ApplicationDbContext _db;

    private DbSet<T> _dbSet;
    public Repository(ApplicationDbContext db)
    {
        _db = db;
        _dbSet = _db.Set<T>();
    }

    public async Task CreateAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, bool tracked = true, string? includedProperties = null, int pageSize = 0, int pageNumber = 1)
    {
        IQueryable<T> query = _dbSet;

        if (!string.IsNullOrEmpty(includedProperties))
        {
            foreach (var property in includedProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(property);
            }
        }

        if (filter != null)
        {
            query = query.Where(filter);
        }

        if (pageSize > 0)
        {
            if (pageSize > 100)
            {
                pageSize = 100;
            }

            query = query.Skip(pageSize * (pageNumber - 1)).Take(pageSize);
        }

        return await query.ToListAsync();
    }

    public async Task<T> GetAsync(Expression<Func<T, bool>>? filter = null, bool tracked = true, string? includedProperties = null, int pageSize = 0, int pageNumber = 1)
    {
        IQueryable<T> query = _dbSet;

        if (!tracked)
        {
            query = query.AsNoTracking();
        }

        if (!string.IsNullOrEmpty(includedProperties))
        {
            foreach (var property in includedProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(property);
            }
        }

        if (filter != null)
        {
            query = query.Where(filter);
        }

        return await query.FirstOrDefaultAsync();

    }

    public void Remove(T entity)
    {
        _dbSet.Remove(entity);
    }
}
