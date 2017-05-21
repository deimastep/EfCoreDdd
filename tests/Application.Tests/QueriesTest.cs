namespace PurchaseApproval.Application
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Infrastructure.Persistence;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Tests.Core;
    using Xunit;
    using Xunit.Abstractions;

    public sealed class QueriesTest : IClassFixture<QueriesTest.Fixture>
    {
        #region Fixture ...

        private readonly Fixture _fixture;
        private readonly ITestOutputHelper _console;

        public QueriesTest(Fixture fixture, ITestOutputHelper console)
        {
            _fixture = fixture;
            _console = console;
        }

        public sealed class Fixture : IDisposable
        {
            private static Domain.PurchaseApproval[] SourceEntities { get; }

            public static IList<PurchaseApproval> Entities { get; }

            private readonly DomainDbContext _db;

            public IQueries Queries { get; }

            static Fixture()
            {
                SourceEntities = new Domain.PurchaseApproval[4];
                var date = DateTime.UtcNow;

                SourceEntities[0] = new Domain.PurchaseApproval(
                    new Guid("00000000-0000-0000-0000-000000000001"),
                    "customer1",
                    date.AddSeconds(1));

                SourceEntities[1] = new Domain.PurchaseApproval(
                    new Guid("00000000-0000-0000-0000-000000000002"),
                    "customer2",
                    date.AddSeconds(2));
                SourceEntities[1].NewDecision("Yes");

                SourceEntities[2] = new Domain.PurchaseApproval(
                    new Guid("00000000-0000-0000-0000-000000000003"),
                    "customer1",
                    date.AddSeconds(3));
                SourceEntities[2].Close();

                SourceEntities[3] = new Domain.PurchaseApproval(
                    new Guid("00000000-0000-0000-0000-000000000004"),
                    "customer4",
                    date.AddSeconds(4));

                var mapper = Application.Queries.MapExpression.Compile();
                Entities = SourceEntities.Select(mapper).ToList();
            }

            public Fixture()
            {
                var options = new DbContextOptionsBuilder<DomainDbContext>()
                    .UseInMemoryDatabase()
                    .ConfigureWarnings(wa => wa.Log(CoreEventId.ModelValidationWarning))
                    .ConfigureWarnings(wa => wa.Throw(CoreEventId.IncludeIgnoredWarning))
                    .ConfigureWarnings(wa => wa.Throw(RelationalEventId.QueryClientEvaluationWarning))
                    .Options;

                _db = new DomainDbContext(options);
                Queries = new Queries(_db);

                using (var db = new DomainDbContext(options))
                {
                    foreach (var entity in SourceEntities)
                    {
                        db.Add(entity);
                    }
                    db.SaveChanges();
                }
            }

            public void Dispose()
            {
                _db.Dispose();
            }
        }

        #endregion

        [Theory, MemberData(nameof(GetByIdShouldReturnEntityData))]
        public async Task GetById_ShouldReturnEntity(Guid id, PurchaseApproval expected)
        {
            var actual = await _fixture.Queries.GetById(id).ConfigureAwait(false);
            Assert.True(expected.EqualByProperties(actual, _console));
        }

        private static IEnumerable<object[]> GetByIdShouldReturnEntityData()
        {
            var entity = Fixture.Entities[0];
            yield return new object[] { entity.Id, entity };

            entity = Fixture.Entities[1];
            yield return new object[] { entity.Id, entity };

            entity = Fixture.Entities[2];
            yield return new object[] { entity.Id, entity };

            entity = Fixture.Entities[3];
            yield return new object[] { entity.Id, entity };
        }

        [Fact]
        public async Task GetById_ShouldNotReturnWhenEntityDoesNotExists()
        {
            var actual = await _fixture.Queries.GetById(Guid.Empty).ConfigureAwait(false);
            Assert.Null(actual);
        }

        [Theory, MemberData(nameof(GetByStatusShouldReturnOnlyWithCertainStatusData))]
        public async Task GetByStatus_ShouldReturnOnlyWithCertainStatus(
            string status,
            PurchaseApproval[] expected)
        {
            var actual = await _fixture.Queries.GetByStatus(status).ConfigureAwait(false);
            Assert.True(expected.EqualByProperties(actual.ToArray(), _console));
        }

        private static IEnumerable<object[]> GetByStatusShouldReturnOnlyWithCertainStatusData()
        {
            var entity = Fixture.Entities[0];
            yield return new object[]
            {
                entity.Status, new[] { entity, Fixture.Entities[3] }
            };

            entity = Fixture.Entities[1];
            yield return new object[]
            {
                entity.Status, new[] { entity }
            };

            entity = Fixture.Entities[2];
            yield return new object[]
            {
                entity.Status, new[] { entity }
            };
        }

        [Fact]
        public async Task GetByStatus_ShouldNotReturnWhenEntityWithCertainStatusDoesNotExists()
        {
            var actual = await _fixture.Queries.GetByStatus("ABC").ConfigureAwait(false);
            Assert.Empty(actual);
        }

        [Theory, MemberData(nameof(GetByCustomerShouldReturnOnlyWithCertainCustomerData))]
        public async Task GetByCustomer_ShouldReturnOnlyWithCertainCustomer(
            string customerId,
            PurchaseApproval[] expected)
        {
            var actual = await _fixture.Queries.GetByCustomer(customerId).ConfigureAwait(false);
            Assert.True(expected.EqualByProperties(actual.ToArray(), _console));
        }

        private static IEnumerable<object[]> GetByCustomerShouldReturnOnlyWithCertainCustomerData()
        {
            var entity = Fixture.Entities[0];
            yield return new object[]
            {
                entity.CustomerId, new[] { entity, Fixture.Entities[2] }
            };

            entity = Fixture.Entities[1];
            yield return new object[]
            {
                entity.CustomerId, new[] { entity }
            };

            entity = Fixture.Entities[3];
            yield return new object[]
            {
                entity.CustomerId, new[] { entity }
            };
        }

        [Fact]
        public async Task GetByCustomer_ShouldNotReturnWhenEntityWithCertainCustomerDoesNotExists()
        {
            var actual = await _fixture.Queries.GetByCustomer("ABC").ConfigureAwait(false);
            Assert.Empty(actual);
        }
    }
}