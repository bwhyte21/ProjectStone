using ProjectStone_DataAccess.Data;
using ProjectStone_DataAccess.Repository.IRepository;
using ProjectStone_Models;

namespace ProjectStone_DataAccess.Repository;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    private readonly ApplicationDbContext _db;

    // base(db) is the AppDbContext from Repository.cs
    public CategoryRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public void Update(Category categoryObj)
    {
        // The original way, we can do this because we're in the Category Repo.
        //var objFromDb = _db.Category.FirstOrDefault(u => u.Id == categoryObj.Id);

        // BUT, since we have FirstOrDefault in our Repo already, we can use base.FirstOrDefault.
        //var objFromDb = base.FirstOrDefault(u => u.Id == categoryObj.Id);

        // ReSharper claims we do not need to call the qualifier "base" as it is a redundant call, so we will go with that.
        var objFromDb = FirstOrDefault(u => u.Id == categoryObj.Id);

        if (objFromDb is not null)
        {
            objFromDb.Name = categoryObj.Name;
            objFromDb.DisplayOrder = categoryObj.DisplayOrder;
        }
    }
}