﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Shared.Context;

#nullable disable

namespace Shared.Migrations.TenantDb
{
    [DbContext(typeof(TenantDbContext))]
    partial class TenantDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Shared.Models.Join.UserTenantRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("J_ID");

                    b.Property<string>("TenantId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("role")
                        .HasColumnType("int")
                        .HasColumnName("J_USER_TENANT_ROLE");

                    b.Property<string>("userId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("TenantId");

                    b.HasIndex("userId");

                    b.ToTable("JOIN_TENANT_USER");
                });

            modelBuilder.Entity("Shared.Models.Tenant", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("T_ID");

                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("T_CODE");

                    b.Property<string>("ConnectionString")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("T_CONNECTION_STRING");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("T_NAME");

                    b.Property<string>("OWNER_ID")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("Code")
                        .IsUnique()
                        .HasFilter("[T_CODE] IS NOT NULL");

                    b.HasIndex("OWNER_ID");

                    b.ToTable("Tenant");
                });

            modelBuilder.Entity("Shared.Models.User", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("A_ID");

                    b.Property<bool>("Active")
                        .HasColumnType("bit")
                        .HasColumnName("A_ACTIVE");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("A_EMAIL");

                    b.Property<bool>("IsAdmin")
                        .HasColumnType("bit");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("A_PASSWORD_HASH");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("A_PHONE_NUMBER");

                    b.Property<bool>("Registred")
                        .HasColumnType("bit")
                        .HasColumnName("A_REGISTRED");

                    b.Property<string>("Seed")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("A_SEED");

                    b.Property<string>("Username")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("A_USERNAME");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique()
                        .HasFilter("[A_EMAIL] IS NOT NULL");

                    b.ToTable("ACCOUNT");
                });

            modelBuilder.Entity("Shared.Models.Join.UserTenantRole", b =>
                {
                    b.HasOne("Shared.Models.Tenant", "Tenant")
                        .WithMany()
                        .HasForeignKey("TenantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Shared.Models.User", "user")
                        .WithMany("Roles")
                        .HasForeignKey("userId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Tenant");

                    b.Navigation("user");
                });

            modelBuilder.Entity("Shared.Models.Tenant", b =>
                {
                    b.HasOne("Shared.Models.User", "Owner")
                        .WithMany()
                        .HasForeignKey("OWNER_ID");

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("Shared.Models.User", b =>
                {
                    b.Navigation("Roles");
                });
#pragma warning restore 612, 618
        }
    }
}
