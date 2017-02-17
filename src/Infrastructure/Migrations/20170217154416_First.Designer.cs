using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using PurchaseApproval.Infrastructure.Persistence;

namespace PurchaseApproval.Infrastructure.Migrations
{
    [DbContext(typeof(EfDbContext))]
    [Migration("20170217154416_First")]
    partial class First
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("PurchaseApproval.Domain.Approval", b =>
                {
                    b.Property<Guid>("RequestId");

                    b.Property<int>("Number");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("Decision")
                        .IsRequired()
                        .HasMaxLength(20)
                        .IsUnicode(false);

                    b.Property<DateTime>("ValidTill");

                    b.HasKey("RequestId", "Number");

                    b.ToTable("Approvals");
                });

            modelBuilder.Entity("PurchaseApproval.Domain.Request", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(20)
                        .IsUnicode(false);

                    b.HasKey("Id");

                    b.ToTable("Requests");
                });

            modelBuilder.Entity("PurchaseApproval.Domain.Approval", b =>
                {
                    b.HasOne("PurchaseApproval.Domain.Request")
                        .WithMany("_approvals")
                        .HasForeignKey("RequestId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
