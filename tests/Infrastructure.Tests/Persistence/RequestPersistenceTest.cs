namespace PurchaseApproval.Infrastructure.Persistence
{
    using System;
    using System.Linq;
    using Domain;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Xunit;
    using Tests.Core;
    using Xunit.Abstractions;

    public sealed class RequestPersistenceTest
    {
        private readonly ITestOutputHelper _console;

        public RequestPersistenceTest(ITestOutputHelper console)
        {
            _console = console;
            using (var db = new EfDbContext())
            {
                var serviceProvider = db.GetInfrastructure();
                var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
                loggerFactory.AddProvider(new DebugLoggerProvider());
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
            }
        }

        private static EfDbContext NewDbContext()
        {
            return new EfDbContext();
        }

        [Fact]
        public void Request_can_be_created()
        {
            var expected = new Request(Guid.NewGuid(), DateTime.Now);
            expected.NewApproval("YwC");
            Request actual;

            using (var db = NewDbContext())
            {
                db.Add(expected);
                db.SaveChanges();
            }
            using (var db = NewDbContext())
            {
                actual = db.Requests
                    .Include(EfDbContext.RequestToApprovalsNavigationName)
                    .AsNoTracking()
                    .Single();
            }
            Assert.True(expected.EqualByProperties(actual, _console));
        }
    }
}
