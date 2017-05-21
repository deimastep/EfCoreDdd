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
    public sealed class Approval2PersistenceTest : IDisposable
    {
        #region Fixture ...

        private readonly ITestOutputHelper _console;
        private readonly List<Domain2DbContext> _dbContexts;

        public Approval2PersistenceTest(ITestOutputHelper console)
        {
            _console = console;
            _dbContexts = new List<Domain2DbContext>();
        }

        public void Dispose()
        {
            foreach (var db in _dbContexts)
            {
                db.Dispose();
            }
        }

        private Domain2DbContext GetDb()
        {
            var db = new Domain2DbContext();
            _dbContexts.Add(db);
            return db;
        }

        #endregion

        [Fact]
        public async Task Approval_CanBeCreated()
        {
            var expected = new PurchaseApproval2(Guid.NewGuid(), "CustomerId", DateTime.Now);
            expected.NewDecision("YwC");

            var db = GetDb();
            db.Add(expected);
            await db.SaveChangesAsync();

            db = GetDb();
            var actual = await db.Approvals
                .Include(Domain2DbContext.ApprovalToDecisionsNavigationName)
                .Include(Domain2DbContext.ApprovalToDataNavigationName)
                .SingleOrDefaultAsync(a => a.Id == expected.Id);
            Assert.True(expected.EqualByProperties(actual, _console));
        }

        [Fact]
        public async Task Approval_CanBeChanged()
        {
            var expected = new PurchaseApproval2(Guid.NewGuid(), "CustomerId", DateTime.Now);
            expected.NewDecision("YwC");
            var id = expected.Id;

            var db = GetDb();
            db.Add(expected);
            await db.SaveChangesAsync();

            db = GetDb();
            expected = await db.Approvals
                .Include(Domain2DbContext.ApprovalToDecisionsNavigationName)
                .Include(Domain2DbContext.ApprovalToDataNavigationName)
                .SingleOrDefaultAsync(a => a.Id == id);
            expected.Close();
            await db.SaveChangesAsync();

            db = GetDb();
            var actual = await db.Approvals
                .Include(Domain2DbContext.ApprovalToDecisionsNavigationName)
                .Include(Domain2DbContext.ApprovalToDataNavigationName)
                .SingleOrDefaultAsync(a => a.Id == id);
            Assert.True(expected.EqualByProperties(actual, _console));
        }

        [Fact]
        public async Task Decision_CanBeAdded()
        {
            var expected = new PurchaseApproval2(Guid.NewGuid(), "CustomerId", DateTime.Now);
            var db = GetDb();
            db.Add(expected);
            await db.SaveChangesAsync();
            var id = expected.Id;

            db = GetDb();
            expected = await db.Approvals
                .Include(Domain2DbContext.ApprovalToDecisionsNavigationName)
                .Include(Domain2DbContext.ApprovalToDataNavigationName)
                .SingleOrDefaultAsync(a => a.Id == id);
            expected.NewDecision("Yes");
            await db.SaveChangesAsync();

            db = GetDb();
            var actual = await db.Approvals
                .Include(Domain2DbContext.ApprovalToDecisionsNavigationName)
                .Include(Domain2DbContext.ApprovalToDataNavigationName)
                .SingleOrDefaultAsync(a => a.Id == id);
            Assert.True(expected.EqualByProperties(actual, _console));
        }

        [Fact]
        public async Task Decision_CanBeRemoved()
        {
            var expected = new PurchaseApproval2(Guid.NewGuid(), "CustomerId", DateTime.Now);
            expected.NewDecision("YwC");
            var db = GetDb();
            db.Add(expected);
            await db.SaveChangesAsync();
            var id = expected.Id;

            db = GetDb();
            expected = await db.Approvals
                .Include(Domain2DbContext.ApprovalToDecisionsNavigationName)
                .Include(Domain2DbContext.ApprovalToDataNavigationName)
                .SingleOrDefaultAsync(a => a.Id == id);
            expected.CancelDecision(1);
            await db.SaveChangesAsync();

            db = GetDb();
            var actual = await db.Approvals
                .Include(Domain2DbContext.ApprovalToDecisionsNavigationName)
                .Include(Domain2DbContext.ApprovalToDataNavigationName)
                .SingleOrDefaultAsync(a => a.Id == id);
            Assert.True(expected.EqualByProperties(actual, _console));
        }
    }
}