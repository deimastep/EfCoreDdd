namespace PurchaseApproval.Infrastructure.Persistence
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Domain;

    public sealed class ApprovalRepository : IApprovalRepository
    {
        private readonly DomainDbContext _db;

        public ApprovalRepository(DomainDbContext db)
        {
            _db = db;
        }

        public async Task<PurchaseApproval> GetById(Guid id)
        {
            return await _db.Approvals
                .Include(a => a.Decisions)
                .Include(a => a.Data)
                .SingleAsync(a => a.Id == id);
        }

        public void Add(PurchaseApproval approval)
        {
            _db.Add(approval);
        }

        public async Task Save()
        {
            await _db.SaveChangesAsync();
        }
    }
}
