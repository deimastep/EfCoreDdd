namespace PurchaseApproval.Infrastructure.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Tests.Core;
    using Xunit;
    using Xunit.Abstractions;

    public class ApprovalPersistence2Test : IDisposable
    {
        #region Fixture ...

        private readonly ITestOutputHelper _console;
        private readonly List<DomainDbContext2> _dbContexts;

        public ApprovalPersistence2Test(ITestOutputHelper console)
        {
            _console = console;
            _dbContexts = new List<DomainDbContext2>();

            using (var db = new DomainDbContext2())
            {
                var serviceProvider = db.GetInfrastructure();
                var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
                loggerFactory.AddProvider(new DebugLoggerProvider());
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
            }
        }

        public void Dispose()
        {
            foreach (var db in _dbContexts)
            {
                db.Dispose();
            }
        }

        private DomainDbContext2 GetDb()
        {
            var db = new DomainDbContext2();
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
            var decisionId = db.Entry(expected.Decisions.First()).Property("Id").CurrentValue;

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

            // Check if removed decision actually is deleted in the db
            db = GetDb();
            var decision = await db.Set<Decision>().FindAsync(decisionId);
            Assert.Null(decision);
            decision = await db.Set<Decision>().FirstOrDefaultAsync(d => EF.Property<Guid>(d, "ApprovalId") == actual.Id);
            Assert.Null(decision);
        }

        [Fact]
        public async Task Data_CanBeRemoved()
        {
            var expected = new PurchaseApproval(Guid.NewGuid(), "customerId", DateTime.Now);
            expected.NewDecision("YwC");
            var db = GetDb();
            db.Add(expected);
            Assert.Equal(3, await db.SaveChangesAsync());
            var id = expected.Id;
            var dataId = db.Entry(expected.Data).Property("Id").CurrentValue;

            db = GetDb();
            expected = await db.Approvals
                .Include(a => a.Decisions)
                .Include(a => a.Data)
                .SingleOrDefaultAsync(a => a.Id == id);
            expected.StupidMethodToRemoveMandatoryData();
            Assert.Equal(1, await db.SaveChangesAsync());

            db = GetDb();
            var actual = await db.Approvals
                .Include(a => a.Decisions)
                .Include(a => a.Data)
                .SingleOrDefaultAsync(a => a.Id == id);
            Assert.True(expected.EqualByProperties(actual, _console));

            // Check if removed data actually is deleted in the db
            db = GetDb();
            var data = await db.Set<ApprovalData>().FindAsync(dataId);
            Assert.Null(data);
            data = await db.Set<ApprovalData>().FirstOrDefaultAsync(d => EF.Property<Guid>(d, "ApprovalId") == actual.Id);
            Assert.Null(data);
        }
    }
}
