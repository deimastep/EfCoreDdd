namespace PurchaseApproval.Infrastructure.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain;
    using Microsoft.EntityFrameworkCore;
    using Tests.Core;
    using Xunit;
    using Xunit.Abstractions;

    [Collection(GlobalFixture.CollectionName)]
    public sealed class ApprovalPersistenceTest : IDisposable
    {
        #region Fixture ...

        private readonly ITestOutputHelper _console;
        private readonly List<DomainDbContext> _dbContexts;

        public ApprovalPersistenceTest(ITestOutputHelper console)
        {
            _console = console;
            _dbContexts = new List<DomainDbContext>();
        }

        public void Dispose()
        {
            foreach (var db in _dbContexts)
            {
                db.Dispose();
            }
        }

        private DomainDbContext GetDb()
        {
            var db = new DomainDbContext();
            _dbContexts.Add(db);
            return db;
        }

        #endregion

        [Fact]
        public async Task Approval_CanBeCreated()
        {
            var expected = new PurchaseApproval(Guid.NewGuid(), "customerId", DateTime.Now);
            expected.NewDecision("YwC");

            var db = GetDb();
            db.Add(expected);
            Assert.Equal(3, await db.SaveChangesAsync());

            db = GetDb();
            var actual = await db.Approvals
                .Include(a => a.Decisions)
                .Include(a => a.Data)
                .SingleOrDefaultAsync(a => a.Id == expected.Id);
            Assert.True(expected.EqualByProperties(actual, _console));
        }

        [Fact]
        public async Task Approval_CanBeChanged()
        {
            var expected = new PurchaseApproval(Guid.NewGuid(), "customerId", DateTime.Now);
            expected.NewDecision("YwC");
            var id = expected.Id;

            var db = GetDb();
            db.Add(expected);
            Assert.Equal(3, await db.SaveChangesAsync());

            db = GetDb();
            expected = await db.Approvals
                .Include(a => a.Decisions)
                .Include(a => a.Data)
                .SingleOrDefaultAsync(a => a.Id == id);
            expected.Close();
            Assert.Equal(1, await db.SaveChangesAsync());

            db = GetDb();
            var actual = await db.Approvals
                .Include(a => a.Decisions)
                .Include(a => a.Data)
                .SingleOrDefaultAsync(a => a.Id == id);
            Assert.True(expected.EqualByProperties(actual, _console));
        }

        [Fact]
        public async Task Decision_CanBeAdded()
        {
            var expected = new PurchaseApproval(Guid.NewGuid(), "customerId", DateTime.Now);
            var db = GetDb();
            db.Add(expected);
            Assert.Equal(2, await db.SaveChangesAsync());
            var id = expected.Id;

            db = GetDb();
            expected = await db.Approvals
                .Include(a => a.Decisions)
                .Include(a => a.Data)
                .SingleOrDefaultAsync(a => a.Id == id);
            expected.NewDecision("Yes");
            Assert.Equal(2, await db.SaveChangesAsync());

            db = GetDb();
            var actual = await db.Approvals
                .Include(a => a.Decisions)
                .Include(a => a.Data)
                .SingleOrDefaultAsync(a => a.Id == id);
            Assert.True(expected.EqualByProperties(actual, _console));
        }

        [Fact]
        public async Task Decision_CanBeRemoved()
        {
            var expected = new PurchaseApproval(Guid.NewGuid(), "customerId", DateTime.Now);
            expected.NewDecision("YwC");
            var db = GetDb();
            db.Add(expected);
            Assert.Equal(3, await db.SaveChangesAsync());
            var id = expected.Id;

            db = GetDb();
            expected = await db.Approvals
                .Include(a => a.Decisions)
                .Include(a => a.Data)
                .SingleOrDefaultAsync(a => a.Id == id);
            Assert.True(expected.CancelDecision(1));
            Assert.Equal(1, await db.SaveChangesAsync());

            db = GetDb();
            var actual = await db.Approvals
                .Include(a => a.Decisions)
                .Include(a => a.Data)
                .SingleOrDefaultAsync(a => a.Id == id);
            Assert.True(expected.EqualByProperties(actual, _console));
        }
    }
}