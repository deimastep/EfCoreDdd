namespace PurchaseApproval.Infrastructure.Persistence
{
    using System;
    using System.Configuration;
    using Domain;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Microsoft.EntityFrameworkCore.Metadata;

    public class DomainDbContext : DbContext
    {
        public DbSet<PurchaseApproval> Approvals { get; set; }

        public DomainDbContext()
        {
        }

        public DomainDbContext(DbContextOptions<DomainDbContext> options) : base(options)
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

            modelBuilder.Entity<PurchaseApproval>(b =>
            {
                b.HasKey(e => e.Id);
                b.Property(e => e.Status).HasMaxLength(20).IsRequired().IsUnicode(false);
                b.HasMany(e => e.Decisions)
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
