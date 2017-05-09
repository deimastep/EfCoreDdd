namespace PurchaseApproval.Domain
{
    using System;
    using System.Threading.Tasks;

    public interface IApprovalRepository
    {
        Task<PurchaseApproval> GetById(Guid id);

        void Add(PurchaseApproval approval);

        Task Save();
    }
}
