using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using PurchaseApproval.Infrastructure.Persistence;

namespace PurchaseApproval.Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(DomainDbContext))]
    [Migration("20170508114714_First")]
    partial class First
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("PurchaseApproval.Domain.PurchaseApproval", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(20)
                        .IsUnicode(false);

                    b.HasKey("Id");

                    b.ToTable("Approvals");
                });

            modelBuilder.Entity("PurchaseApproval.Domain.Decision", b =>
                {
                    b.Property<Guid>("ApprovalId");

                    b.Property<int>("Number");

                    b.Property<string>("Answer")
                        .IsRequired()
                        .HasMaxLength(20)
                        .IsUnicode(false);

                    b.Property<DateTime>("CreatedAt");

                    b.Property<DateTime>("ValidTill");

                    b.HasKey("ApprovalId", "Number");

                    b.ToTable("Decisions");
                });

            modelBuilder.Entity("PurchaseApproval.Domain.Decision", b =>
                {
                    b.HasOne("PurchaseApproval.Domain.PurchaseApproval")
                        .WithMany("Decisions")
                        .HasForeignKey("ApprovalId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
