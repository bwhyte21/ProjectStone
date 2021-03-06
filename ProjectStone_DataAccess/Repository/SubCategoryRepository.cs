using ProjectStone_DataAccess.Data;
using ProjectStone_DataAccess.Repository.IRepository;
using ProjectStone_Models;

namespace ProjectStone_DataAccess.Repository;

public class SubCategoryRepository : Repository<SubCategory>, ISubCategoryRepository
{
    private readonly ApplicationDbContext _db;

    public SubCategoryRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public void Update(SubCategory categoryObj)
    {
        var objFromDb = FirstOrDefault(u => u.Id == categoryObj.Id);

        if (objFromDb is not null) { objFromDb.Name = categoryObj.Name; }
    }
}