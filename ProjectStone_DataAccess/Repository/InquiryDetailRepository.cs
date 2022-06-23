using ProjectStone_DataAccess.Data;
using ProjectStone_DataAccess.Repository.IRepository;
using ProjectStone_Models;

namespace ProjectStone_DataAccess.Repository;

public class InquiryDetailRepository : Repository<InquiryDetail>, IInquiryDetailRepository
{
    private readonly ApplicationDbContext _db;

    public InquiryDetailRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public void Update(InquiryDetail obj)
    {
        _db.InquiryDetail.Update(obj);
    }
}