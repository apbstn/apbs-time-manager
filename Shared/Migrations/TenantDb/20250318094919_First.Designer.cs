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
    [Migration("20250318094919_First")]
    partial class First
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

                    b.HasKey("Id");

                    b.ToTable("Tenant");
                });

            modelBuilder.Entity("Shared.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("A_ID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("Active")
                        .HasColumnType("bit")
                        .HasColumnName("A_ACTIVE");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("A_EMAIL");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("A_PASSWORD_HASH");

                    b.Property<bool>("Registred")
                        .HasColumnType("bit")
                        .HasColumnName("A_REGISTRED");

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
#pragma warning restore 612, 618
        }
    }
}
