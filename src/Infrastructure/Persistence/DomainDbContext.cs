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
        public DomainDbContext()
        {
        }

        public DomainDbContext(DbContextOptions<DomainDbContext> options) : base(options)
        {
        }

        public DbSet<PurchaseApproval> Approvals { get; set; }

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
            modelBuilder.Entity<PurchaseApproval>(
                b => {
                    b.ToTable("Approvals");
                    b.HasKey(e => e.Id);
                    b.Property(e => e.Status).HasMaxLength(20).IsRequired().IsUnicode(false);

                    // Foreign keys in those relationships are required because they are part of the PK
                    // in the related entities.
                    b.HasOne(e => e.Data)
                        .WithOne().HasForeignKey<ApprovalData>("Id").OnDelete(DeleteBehavior.Cascade);
                    b.HasMany(e => e.Decisions)
                        .WithOne().HasForeignKey("ApprovalId").OnDelete(DeleteBehavior.Cascade);
                });
            modelBuilder.Entity<ApprovalData>(
                b => {
                    b.ToTable("ApprovalData");
                    b.Property<Guid>("Id");
                    // PK is shadow property "Id"
                    b.HasKey("Id");
                    b.Property(e => e.CustomerId).HasMaxLength(10).IsRequired().IsUnicode(false);
                    b.HasIndex(e => e.CustomerId).HasName("IX_CustomerId");
                });
            modelBuilder.Entity<Decision>(
                b => {
                    b.ToTable("Decisions");
                    // PK is composite key with shadow (the same time as foreign key) and real properties.
                    b.HasKey("ApprovalId", nameof(Decision.Number));
                    b.Property(e => e.Answer).HasMaxLength(20).IsRequired().IsUnicode(false);
                });

            // This is the way in EF Core 1.1 to set backing field for the **navigation** property
            // https://blog.oneunicorn.com/2016/10/28/collection-navigation-properties-and-fields-in-ef-core-1-1/
            // https://github.com/aspnet/EntityFramework/issues/6674
            // BEWARE: should be done at the end of model configuration because of the bug
            // https://github.com/aspnet/EntityFramework/issues/7674
            var decisionNavigation = modelBuilder.Entity<PurchaseApproval>().Metadata.FindNavigation(nameof(PurchaseApproval.Decisions));
            decisionNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}