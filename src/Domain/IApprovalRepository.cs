namespace PurchaseApproval.Domain
{
    using System;

    public interface IApprovalRepository
    {
        Approval GetById(Guid id);

        void Add(Approval approval);

        void Save();
    }
}
