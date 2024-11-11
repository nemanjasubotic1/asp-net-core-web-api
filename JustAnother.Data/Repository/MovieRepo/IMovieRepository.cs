using JustAnother.DataAccess.Repository.General;
using JustAnother.Model.Entity;

namespace JustAnother.DataAccess.Repository.CategoryRepo;

public interface IMovieRepository : IRepository<Movie>
{
    void Update(Movie movie);
}
