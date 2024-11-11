using JustAnother.DataAccess.Repository.CategoryRepo;

namespace JustAnother.DataAccess.Repository.General
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category {  get; }
        IMovieRepository Movie {  get; }


        Task Save();
    }
}
