using JustAnother.DataAccess.Data;
using JustAnother.DataAccess.Repository.General;
using JustAnother.Model.Entity;

namespace JustAnother.DataAccess.Repository.CategoryRepo;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    private readonly ApplicationDbContext _db;

    public CategoryRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public void Update(Category category)
    {
        _db.Update<Category>(category);
    }
}
