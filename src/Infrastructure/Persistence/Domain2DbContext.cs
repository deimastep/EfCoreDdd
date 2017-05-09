namespace PurchaseApproval.Infrastructure.Persistence
{
    using System;
    using System.Configuration;
    using Domain;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Microsoft.EntityFrameworkCore.Metadata;

    public class Domain2DbContext : DbContext
    {
        public DbSet<PurchaseApproval2> Approvals { get; set; }

        internal const string ApprovalToDecisionsNavigationName = "DecisionsInternal";

        public Domain2DbContext()
        {
        }

        public Domain2DbContext(DbContextOptions<DomainDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured)
            {
                return;
            }
            var connectionString = ConfigurationManager.ConnectionStrings["PurchaseApproval"]?.ConnectionString
                ?? @"Server=(localdb)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|PurchaseApprovalDB.mdf;Database=PurchaseApproval-TEST;Trusted_Connection=True;";
            optionsBuilder.UseSqlServer(connectionString);
            optionsBuilder
                .ConfigureWarnings(wa => wa.Log(CoreEventId.ModelValidationWarning))
                .ConfigureWarnings(wa => wa.Throw(CoreEventId.IncludeIgnoredWarning))
                .ConfigureWarnings(wa => wa.Throw(RelationalEventId.QueryClientEvaluationWarning));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            const string approvalId = "ApprovalId";

            modelBuilder.Entity<PurchaseApproval2>(b =>
            {
                b.HasKey(e => e.Id);
                b.Property(e => e.Status).HasMaxLength(20).IsRequired().IsUnicode(false);
                b.Ignore(e => e.Decisions);
                b.HasMany(typeof(Decision), ApprovalToDecisionsNavigationName)
                    .WithOne().HasForeignKey(approvalId).OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<Decision>(b =>
            {
                b.Property<Guid>(approvalId);
                b.Property(e => e.Number);
                b.HasKey(approvalId, nameof(Decision.Number));
                b.Property(e => e.Answer).HasMaxLength(20).IsRequired().IsUnicode(false);
            });
        }
    }
}
