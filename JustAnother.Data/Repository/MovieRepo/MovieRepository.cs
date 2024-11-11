using JustAnother.DataAccess.Data;
using JustAnother.DataAccess.Repository.General;
using JustAnother.Model.Entity;

namespace JustAnother.DataAccess.Repository.CategoryRepo;

public class MovieRepository : Repository<Movie>, IMovieRepository
{
    private readonly ApplicationDbContext _db;

    public MovieRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public void Update(Movie Movie)
    {
        _db.Update<Movie>(Movie);
    }
}
