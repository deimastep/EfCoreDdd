namespace PurchaseApproval.Infrastructure.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Domain;
    using Tests.Core;
    using Xunit;
    using Xunit.Abstractions;

    [Collection(GlobalFixture.CollectionName)]
    public sealed class ApprovalRepositoryTest : IDisposable
    {
        #region Fixture ...

        private readonly ITestOutputHelper _console;
        private readonly List<DomainDbContext> _dbContexts;

        public ApprovalRepositoryTest(ITestOutputHelper console)
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

        private IApprovalRepository GetRepo()
        {
            var db = new DomainDbContext();
            _dbContexts.Add(db);
            return new ApprovalRepository(db);
        }

        #endregion

        [Fact]
        public async Task Approval_CanBeCreated()
        {
            var expected = new PurchaseApproval(Guid.NewGuid(), DateTime.Now);
            expected.NewDecision("YwC");

            var repo = GetRepo();
            repo.Add(expected);
            await repo.Save();

            repo = GetRepo();
            var actual = await repo.GetById(expected.Id);
            Assert.True(expected.EqualByProperties(actual, _console));
        }

        [Fact]
        public async Task Approval_CanBeChanged()
        {
            var expected = new PurchaseApproval(Guid.NewGuid(), DateTime.Now);
            expected.NewDecision("YwC");

            var repo = GetRepo();
            repo.Add(expected);
            await repo.Save();

            repo = GetRepo();
            expected = await repo.GetById(expected.Id);
            expected.Close();
            await repo.Save();

            repo = GetRepo();
            var actual = await repo.GetById(expected.Id);
            Assert.True(expected.EqualByProperties(actual, _console));
        }

        [Fact]
        public async Task Decision_CanBeAdded()
        {
            var expected = new PurchaseApproval(Guid.NewGuid(), DateTime.Now);
            var repo = GetRepo();
            repo.Add(expected);
            await repo.Save();

            repo = GetRepo();
            expected = await repo.GetById(expected.Id);
            expected.NewDecision("Yes");
            await repo.Save();

            repo = GetRepo();
            var actual = await repo.GetById(expected.Id);
            Assert.True(expected.EqualByProperties(actual, _console));
        }

        [Fact]
        public async Task Decision_CanBeRemoved()
        {
            var expected = new PurchaseApproval(Guid.NewGuid(), DateTime.Now);
            expected.NewDecision("YwC");
            var repo = GetRepo();
            repo.Add(expected);
            await repo.Save();

            repo = GetRepo();
            expected = await repo.GetById(expected.Id);
            expected.CancelDecision(1);
            await repo.Save();

            repo = GetRepo();
            var actual = await repo.GetById(expected.Id);
            Assert.True(expected.EqualByProperties(actual, _console));
        }
    }
}
