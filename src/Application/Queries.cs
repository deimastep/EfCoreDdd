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
        private readonly DomainDbContext _db;
        private readonly Expression<Func<Domain.PurchaseApproval, PurchaseApproval>> _map = a =>
            new PurchaseApproval
            {
                Status = a.Status,
                Decision = a.Decisions.OrderByDescending(d => d.CreatedAt).Select(d => new Decision
                {
                    Answer = d.Answer,
                    ValidTill = d.ValidTill,
                    CreatedAt = d.CreatedAt
                }).FirstOrDefault(),
                Id = a.Id,
                CreatedAt = a.CreatedAt
            };

        public Queries(DomainDbContext db)
        {
            _db = db;
        }

        public async Task<PurchaseApproval> GetById(Guid id)
        {
            return await _db.Approvals.AsNoTracking()
                .Where(a => a.Id == id)
                .Select(_map)
                .SingleOrDefaultAsync();
        }

        public IEnumerable<PurchaseApproval> GetByStatus(string status)
        {
            return _db.Approvals.AsNoTracking()
                .Where(a => a.Status == status)
                .Select(_map);
        }
    }
}
