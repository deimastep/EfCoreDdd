namespace PurchaseApproval.Infrastructure.Persistence
{
    using System;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using Domain;

    public sealed class ApprovalRepository : IApprovalRepository
    {
        private readonly EfDbContext _db;

        public ApprovalRepository(EfDbContext db)
        {
            _db = db;
        }

        public Approval GetById(Guid id)
        {
            return _db.Approvals
                .Include(a => a.Decisions)
                .Single(a => a.Id == id);
        }

        public void Add(Approval approval)
        {
            _db.Add(approval);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
