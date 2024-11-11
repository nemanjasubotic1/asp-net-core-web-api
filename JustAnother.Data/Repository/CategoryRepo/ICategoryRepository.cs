using JustAnother.DataAccess.Repository.General;
using JustAnother.Model.Entity;

namespace JustAnother.DataAccess.Repository.CategoryRepo;

public interface ICategoryRepository : IRepository<Category>
{
    void Update(Category category);
}
