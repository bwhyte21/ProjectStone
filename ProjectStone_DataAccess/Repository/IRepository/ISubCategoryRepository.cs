using ProjectStone_Models;

namespace ProjectStone_DataAccess.Repository.IRepository;

public interface ISubCategoryRepository : IRepository<SubCategory>
{
    void Update(SubCategory obj);
}