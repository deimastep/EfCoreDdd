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
        internal DbSet<Approval> Approvals { get; set; }

        internal DbSet<Decision> Decisions { get; set; }

        internal const string RequestToApprovalsNavigationName = "_decisions";

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

            modelBuilder.Entity<Approval>(r =>
            {
                r.HasKey(e => e.Id);
                r.Property(e => e.Status).IsRequired().IsUnicode(false);

                /*r.Ignore(e => e.Decisions);
                r.HasMany(typeof(Decision), RequestToApprovalsNavigationName)
                    .WithOne().HasForeignKey(approvalId).OnDelete(DeleteBehavior.Cascade);*/
                //r.Property(e => e.Decisions).HasField(RequestToApprovalsNavigationName);
                r.HasMany(e => e.Decisions)
                    .WithOne().HasForeignKey(approvalId).OnDelete(DeleteBehavior.Cascade); ;
            });
            modelBuilder.Entity<Decision>(a =>
            {
                a.Property<Guid>(approvalId);
                a.Property(e => e.Number);
                a.HasKey(approvalId, nameof(Decision.Number));
                a.Property(e => e.Answer).IsRequired().IsUnicode(false);
            });
        }
    }
}
