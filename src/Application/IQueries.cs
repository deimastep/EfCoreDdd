namespace PurchaseApproval.Application
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IQueries
    {
        Task<PurchaseApproval> GetById(Guid id);

        Task<IEnumerable<PurchaseApproval>> GetByStatus(string status);

        Task<IEnumerable<PurchaseApproval>> GetByCustomer(string customerId);
    }
}