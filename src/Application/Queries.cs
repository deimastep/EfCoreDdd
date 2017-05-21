namespace PurchaseApproval.Application
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Infrastructure.Persistence;
    using Microsoft.EntityFrameworkCore;

    public class Queries : IQueries
    {
        // The good place for Automapper to do its job.
        internal static readonly Expression<Func<Domain.PurchaseApproval, PurchaseApproval>> MapExpression = a => new PurchaseApproval
        {
            Status = a.Status,
            Decision = a.Decisions.OrderByDescending(d => d.CreatedAt).Select(
                d => new Decision
                {
                    Answer = d.Answer,
                    ValidTill = d.ValidTill,
                    CreatedAt = d.CreatedAt
                }).FirstOrDefault(),
            CustomerId = a.Data.CustomerId,
            Id = a.Id,
            CreatedAt = a.CreatedAt
        };

        private readonly DomainDbContext _db;

        public Queries(DomainDbContext db) => _db = db;

        public async Task<PurchaseApproval> GetById(Guid id)
        {
            return await _db.Approvals.AsNoTracking()
                .Where(a => a.Id == id)
                .Select(MapExpression)
                .SingleOrDefaultAsync()
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<PurchaseApproval>> GetByStatus(string status)
        {
            return await _db.Approvals.AsNoTracking()
                .Where(a => a.Status == status)
                .Select(MapExpression)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<PurchaseApproval>> GetByCustomer(string customerId)
        {
            return await _db.Approvals.AsNoTracking()
                .Where(a => a.Data.CustomerId == customerId)
                .Select(MapExpression)
                .ToListAsync()
                .ConfigureAwait(false);
        }
    }
}