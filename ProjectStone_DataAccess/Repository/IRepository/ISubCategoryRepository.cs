using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectStone_Models;

namespace ProjectStone_DataAccess.Repository.IRepository
{
    public interface ISubCategoryRepository : IRepository<SubCategory>
    {
        void Update(SubCategory obj);
    }
}