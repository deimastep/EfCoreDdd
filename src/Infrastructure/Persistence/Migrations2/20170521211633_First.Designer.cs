using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using PurchaseApproval.Infrastructure.Persistence;

namespace PurchaseApproval.Infrastructure.Persistence.Migrations2
{
    [DbContext(typeof(DomainDbContext2))]
    [Migration("20170521211633_First")]
    partial class First
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("PurchaseApproval.Domain.ApprovalData", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<Guid?>("ApprovalId");

                    b.Property<string>("CustomerId")
                        .IsRequired()
                        .HasMaxLength(10)
                        .IsUnicode(false);

                    b.Property<string>("Data");

                    b.HasKey("Id");

                    b.HasIndex("ApprovalId")
                        .IsUnique();

                    b.HasIndex("CustomerId")
                        .HasName("IX_CustomerId");

                    b.ToTable("ApprovalData");
                });

            modelBuilder.Entity("PurchaseApproval.Domain.Decision", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Answer")
                        .IsRequired()
                        .HasMaxLength(20)
                        .IsUnicode(false);

                    b.Property<Guid?>("ApprovalId")
                        .IsRequired();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("Number");

                    b.Property<DateTime>("ValidTill");

                    b.HasKey("Id");

                    b.HasIndex("ApprovalId");

                    b.ToTable("Decisions");
                });

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

            modelBuilder.Entity("PurchaseApproval.Domain.ApprovalData", b =>
                {
                    b.HasOne("PurchaseApproval.Domain.PurchaseApproval")
                        .WithOne("Data")
                        .HasForeignKey("PurchaseApproval.Domain.ApprovalData", "ApprovalId")
                        .OnDelete(DeleteBehavior.Cascade);
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
