﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Shared.Context;

#nullable disable

namespace Shared.Migrations.TenantDb
{
    [DbContext(typeof(TenantDbContext))]
    [Migration("20250402133658_Third")]
    partial class Third
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Shared.Models.Tenant", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("T_ID");

                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("T_CODE");

                    b.Property<string>("ConnectionString")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("T_CONNECTION_STRING");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("T_NAME");

                    b.Property<string>("OwnerId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

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
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("A_EMAIL");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("A_PASSWORD_HASH");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("A_PHONE_NUMBER");

                    b.Property<bool>("Registred")
                        .HasColumnType("bit")
                        .HasColumnName("A_REGISTRED");

                    b.Property<int>("Role")
                        .HasColumnType("int")
                        .HasColumnName("A_ROLE");

                    b.Property<string>("Seed")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("A_SEED");

                    b.Property<string>("TenantId")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("A_TENANT_ID");

                    b.Property<string>("Username")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("A_USERNAME");

                    b.HasKey("Id");

                    b.ToTable("ACCOUNT");
                });

            modelBuilder.Entity("Shared.Models.Tenant", b =>
                {
                    b.HasOne("Shared.Models.User", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId");

                    b.Navigation("Owner");
                });
#pragma warning restore 612, 618
        }
    }
}
