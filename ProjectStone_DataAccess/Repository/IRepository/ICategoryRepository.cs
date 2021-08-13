using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectStone_Models;

namespace ProjectStone_DataAccess.Repository.IRepository
{
    public interface ICategoryRepository : IRepository<Category>
    {
        // Since all repositories do not need an Update(), but this one does, we will add the update() here.
        // Add, Remove, Save is used globally, but this Update will not, so there will be one here.
        void Update(Category obj);
    }
}