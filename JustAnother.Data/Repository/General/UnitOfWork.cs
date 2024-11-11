
using JustAnother.DataAccess.Data;
using JustAnother.DataAccess.Repository.CategoryRepo;

namespace JustAnother.DataAccess.Repository.General;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _db;

    public ICategoryRepository Category { get; set; }
    public IMovieRepository Movie { get; set; }

    public UnitOfWork(ApplicationDbContext db)
    {
        _db = db;
        Category = new CategoryRepository(_db);
        Movie = new MovieRepository(_db);
    }

    public async Task Save()
    {
       await _db.SaveChangesAsync();
    }
}
