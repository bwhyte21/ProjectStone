using ProjectStone_Models;

namespace ProjectStone_DataAccess.Repository.IRepository;

public interface IInquiryHeaderRepository : IRepository<InquiryHeader>
{
    void Update(InquiryHeader obj);
}