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
        internal const string ApprovalToDataNavigationName = "DataInternal";

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
            modelBuilder.Entity<PurchaseApproval2>(b =>
            {
                b.ToTable("Approvals");
                b.HasKey(e => e.Id);
                b.Property(e => e.Status).HasMaxLength(20).IsRequired().IsUnicode(false);
                b.Ignore(e => e.Decisions);
                b.HasMany(typeof(Decision), ApprovalToDecisionsNavigationName)
                    .WithOne().HasForeignKey("ApprovalId").OnDelete(DeleteBehavior.Cascade);
                b.HasOne(typeof(ApprovalData), ApprovalToDataNavigationName)
                    .WithOne().HasForeignKey(typeof(ApprovalData), "Id").OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<ApprovalData>(b =>
            {
                b.ToTable("ApprovalData");
                b.Property<Guid>("Id");
                b.HasKey("Id");
                b.Property(e => e.CustomerId).HasMaxLength(10).IsRequired().IsUnicode(false);
                b.HasIndex(e => e.CustomerId).HasName("IX_CustomerId");
            });
            modelBuilder.Entity<Decision>(b =>
            {
                b.ToTable("Decisions");
                b.Property<Guid>("ApprovalId");
                b.Property(e => e.Number);
                b.HasKey("ApprovalId", nameof(Decision.Number));
                b.Property(e => e.Answer).HasMaxLength(20).IsRequired().IsUnicode(false);
            });
        }
    }
}
