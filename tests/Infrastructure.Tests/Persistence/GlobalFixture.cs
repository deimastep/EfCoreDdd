namespace PurchaseApproval.Infrastructure.Persistence
{
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Xunit;

    public class GlobalFixture
    {
        public const string CollectionName = "PersistenceGlobalFixture";

        public GlobalFixture()
        {
            using (var db = new DomainDbContext())
            {
                var serviceProvider = db.GetInfrastructure();
                var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
                loggerFactory.AddProvider(new DebugLoggerProvider());
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
            }
        }

        [CollectionDefinition(CollectionName)]
        public class CollectionFixture : ICollectionFixture<GlobalFixture>
        {
        }
    }
}
