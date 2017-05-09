namespace PurchaseApproval.Application
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IQueries
    {
        Task<PurchaseApproval> GetById(Guid id);

        IEnumerable<PurchaseApproval> GetByStatus(string status);
    }
}