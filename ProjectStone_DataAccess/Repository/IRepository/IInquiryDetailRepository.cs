using ProjectStone_Models;

namespace ProjectStone_DataAccess.Repository.IRepository;

public interface IInquiryDetailRepository : IRepository<InquiryDetail>
{
    void Update(InquiryDetail obj);
}