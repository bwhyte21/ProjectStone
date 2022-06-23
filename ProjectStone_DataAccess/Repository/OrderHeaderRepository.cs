using ProjectStone_DataAccess.Data;
using ProjectStone_DataAccess.Repository.IRepository;
using ProjectStone_Models;

namespace ProjectStone_DataAccess.Repository;

public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
{
    private readonly ApplicationDbContext _db;

    public OrderHeaderRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public void Update(OrderHeader obj)
    {
        _db.OrderHeader.Update(obj);
    }
}