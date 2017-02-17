namespace PurchaseApproval.Infrastructure.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using Domain;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Microsoft.EntityFrameworkCore.Metadata;
    using Microsoft.EntityFrameworkCore.Metadata.Internal;

    public class EfDbContext : DbContext
    {
        internal DbSet<Request> Requests { get; set; }

        internal DbSet<Approval> Approvals { get; set; }

        internal const string RequestToApprovalsNavigationName = "_approvals";

        public EfDbContext()
        {
        }

        public EfDbContext(DbContextOptions<EfDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured)
            {
                return;
            }
            var connectionString = ConfigurationManager.ConnectionStrings["PurchaseApproval"]?.ConnectionString
                ?? @"Server=(localdb)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|PurchaseApproval.mdf;Database=PurchaseApproval;Trusted_Connection=True;";
            optionsBuilder.UseSqlServer(connectionString);
            optionsBuilder.ConfigureWarnings(wa => wa.Ignore(CoreEventId.ModelValidationWarning));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            const string approvalRequestId = "RequestId";

            modelBuilder.Entity<Request>(r =>
            {
                r.HasKey(e => e.Id);
                r.Property(e => e.Status).IsRequired().IsUnicode(false);
                r.Ignore(e => e.Approvals);
                r.HasMany(typeof(Approval), RequestToApprovalsNavigationName)
                    .WithOne().HasForeignKey(approvalRequestId).OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<Approval>(a =>
            {
                a.Property<Guid>(approvalRequestId);
                a.Property(e => e.Number);
                a.HasKey(approvalRequestId, nameof(Approval.Number));
                a.Property(e => e.Decision).IsRequired().IsUnicode(false);
            });
        }
    }
}
