namespace PurchaseApproval.Infrastructure.Persistence
{
    using System;
    using System.Collections.Generic;
    using Domain;
    using Tests.Core;
    using Xunit;
    using Xunit.Abstractions;

    [Collection(GlobalFixture.CollectionName)]
    public sealed class ApprovalPersistenceTest : IDisposable
    {
        #region Fixture ...

        private readonly ITestOutputHelper _console;
        private readonly List<EfDbContext> _dbContexts;

        public ApprovalPersistenceTest(ITestOutputHelper console)
        {
            _console = console;
            _dbContexts = new List<EfDbContext>();
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
            var db = new EfDbContext();
            _dbContexts.Add(db);
            return new ApprovalRepository(db);
        }

        #endregion

        [Fact]
        public void Approval_CanBeCreated()
        {
            var expected = new Approval(Guid.NewGuid(), DateTime.Now);
            expected.NewDecision("YwC");

            var repo = GetRepo();
            repo.Add(expected);
            repo.Save();

            repo = GetRepo();
            var actual = repo.GetById(expected.Id);
            Assert.True(expected.EqualByProperties(actual, _console));
        }

        [Fact]
        public void Approval_CanBeChanged()
        {
            var expected = new Approval(Guid.NewGuid(), DateTime.Now);
            expected.NewDecision("YwC");

            var repo = GetRepo();
            repo.Add(expected);
            repo.Save();

            repo = GetRepo();
            expected = repo.GetById(expected.Id);
            expected.InProgress();
            repo.Save();

            repo = GetRepo();
            var actual = repo.GetById(expected.Id);
            Assert.True(expected.EqualByProperties(actual, _console));
        }

        [Fact]
        public void Decision_CanBeAdded()
        {
            var expected = new Approval(Guid.NewGuid(), DateTime.Now);
            var repo = GetRepo();
            repo.Add(expected);
            repo.Save();

            repo = GetRepo();
            expected = repo.GetById(expected.Id);
            expected.NewDecision("Yes");
            repo.Save();

            repo = GetRepo();
            var actual = repo.GetById(expected.Id);
            Assert.True(expected.EqualByProperties(actual, _console));
        }

        [Fact]
        public void Decision_CanBeRemoved()
        {
            var expected = new Approval(Guid.NewGuid(), DateTime.Now);
            expected.NewDecision("YwC");
            var repo = GetRepo();
            repo.Add(expected);
            repo.Save();

            repo = GetRepo();
            expected = repo.GetById(expected.Id);
            expected.CancelDecision(1);
            repo.Save();

            repo = GetRepo();
            var actual = repo.GetById(expected.Id);
            Assert.True(expected.EqualByProperties(actual, _console));
        }
    }
}
