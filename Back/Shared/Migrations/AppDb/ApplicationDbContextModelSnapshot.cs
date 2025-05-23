﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Shared.Context;

#nullable disable

namespace Shared.Migrations.AppDb
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.4")
                .HasAnnotation("Proxies:ChangeTracking", false)
                .HasAnnotation("Proxies:CheckEquality", false)
                .HasAnnotation("Proxies:LazyLoading", true)
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Shared.Models.LeaveRequest", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("L_ID");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("L_END");

                    b.Property<int>("NumberOfDays")
                        .HasColumnType("integer")
                        .HasColumnName("L_NUMBEROFDAYS");

                    b.Property<string>("Reason")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("L_REASON");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("L_START");

                    b.Property<int>("Status")
                        .HasColumnType("integer")
                        .HasColumnName("L_STATUS");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("L_TYPE");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("L_USER_ID");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("LEAVEREQUEST");
                });

            modelBuilder.Entity("Shared.Models.PlannerBase", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("P_ID");

                    b.Property<int>("PlanType")
                        .HasColumnType("integer")
                        .HasColumnName("P_TYPE");

                    b.Property<string>("PlannerName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("P_Name");

                    b.HasKey("Id");

                    b.ToTable("Planners");

                    b.UseTptMappingStrategy();
                });

            modelBuilder.Entity("Shared.Models.Planners.FixedDayPlanner", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool>("Active")
                        .HasColumnType("boolean");

                    b.Property<TimeOnly?>("EndTime")
                        .HasColumnType("time without time zone");

                    b.Property<TimeOnly?>("StartTime")
                        .HasColumnType("time without time zone");

                    b.HasKey("Id");

                    b.ToTable("fixedDayPlanners");
                });

            modelBuilder.Entity("Shared.Models.Planners.FlexibleDayPlanner", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool>("Active")
                        .HasColumnType("boolean");

                    b.Property<TimeOnly?>("NumberOfHours")
                        .HasColumnType("time without time zone");

                    b.HasKey("Id");

                    b.ToTable("flexibleDayPlanners");
                });

            modelBuilder.Entity("Shared.Models.Team", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.HasKey("Id");

                    b.ToTable("Teams");
                });

            modelBuilder.Entity("Shared.Models.TimeLog", b =>
                {
                    b.Property<Guid>("TM_Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("TM_ID");

                    b.Property<bool>("Activ")
                        .HasColumnType("boolean")
                        .HasColumnName("TM_ACTIV");

                    b.Property<DateTime>("Time")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("TM_TIME");

                    b.Property<TimeSpan?>("TotalHours")
                        .HasColumnType("interval")
                        .HasColumnName("TM_TOTALHOURS");

                    b.Property<int>("Type")
                        .HasColumnType("integer")
                        .HasColumnName("TM_TYPE");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("TM_USER_ID");

                    b.HasKey("TM_Id");

                    b.HasIndex("UserId");

                    b.ToTable("TIMELOG");
                });

            modelBuilder.Entity("Shared.Models.UserTenant", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("A_ID");

                    b.Property<string>("Email")
                        .HasColumnType("text")
                        .HasColumnName("A_EMAIL");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text")
                        .HasColumnName("A_PHONE_NUMBER");

                    b.Property<int?>("Role")
                        .HasColumnType("integer")
                        .HasColumnName("A_ROLE");

                    b.Property<Guid?>("TeamId")
                        .HasColumnType("uuid")
                        .HasColumnName("A_TEAM");

                    b.Property<string>("Username")
                        .HasColumnType("text")
                        .HasColumnName("A_USERNAME");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("TeamId");

                    b.ToTable("T_ACCOUNT");
                });

            modelBuilder.Entity("Shared.Models.Planners.FixedPlanner", b =>
                {
                    b.HasBaseType("Shared.Models.PlannerBase");

                    b.Property<Guid>("FridayId")
                        .HasColumnType("uuid")
                        .HasColumnName("P_FRIDAY_ID");

                    b.Property<Guid>("MondayId")
                        .HasColumnType("uuid")
                        .HasColumnName("P_MONDAY_ID");

                    b.Property<Guid>("SaturdayId")
                        .HasColumnType("uuid")
                        .HasColumnName("P_SATURDAY_ID");

                    b.Property<Guid>("SundayId")
                        .HasColumnType("uuid")
                        .HasColumnName("P_SUNDAY_ID");

                    b.Property<Guid>("ThursdayId")
                        .HasColumnType("uuid")
                        .HasColumnName("P_THURSDAY_ID");

                    b.Property<Guid>("TuesdayId")
                        .HasColumnType("uuid")
                        .HasColumnName("P_TUESDAY_ID");

                    b.Property<Guid>("WendsdayId")
                        .HasColumnType("uuid")
                        .HasColumnName("P_WENDSDAY_ID");

                    b.HasIndex("FridayId");

                    b.HasIndex("MondayId");

                    b.HasIndex("SaturdayId");

                    b.HasIndex("SundayId");

                    b.HasIndex("ThursdayId");

                    b.HasIndex("TuesdayId");

                    b.HasIndex("WendsdayId");

                    b.ToTable("FixedPlanners");
                });

            modelBuilder.Entity("Shared.Models.Planners.FlexiblePlanner", b =>
                {
                    b.HasBaseType("Shared.Models.PlannerBase");

                    b.Property<Guid>("FridayId")
                        .HasColumnType("uuid")
                        .HasColumnName("P_FRIDAY_ID");

                    b.Property<Guid>("MondayId")
                        .HasColumnType("uuid")
                        .HasColumnName("P_MONDAY_ID");

                    b.Property<Guid>("SaturdayId")
                        .HasColumnType("uuid")
                        .HasColumnName("P_SATURDAY_ID");

                    b.Property<Guid>("SundayId")
                        .HasColumnType("uuid")
                        .HasColumnName("P_SUNDAY_ID");

                    b.Property<Guid>("ThursdayId")
                        .HasColumnType("uuid")
                        .HasColumnName("P_THURSDAY_ID");

                    b.Property<Guid>("TuesdayId")
                        .HasColumnType("uuid")
                        .HasColumnName("P_TUESDAY_ID");

                    b.Property<Guid>("WendsdayId")
                        .HasColumnType("uuid")
                        .HasColumnName("P_WENDSDAY_ID");

                    b.HasIndex("FridayId");

                    b.HasIndex("MondayId");

                    b.HasIndex("SaturdayId");

                    b.HasIndex("SundayId");

                    b.HasIndex("ThursdayId");

                    b.HasIndex("TuesdayId");

                    b.HasIndex("WendsdayId");

                    b.ToTable("FlexiblePlanners");
                });

            modelBuilder.Entity("Shared.Models.Planners.WeeklyPlanner", b =>
                {
                    b.HasBaseType("Shared.Models.PlannerBase");

                    b.Property<TimeOnly>("WeeklyHours")
                        .HasColumnType("time without time zone")
                        .HasColumnName("P_WEEKLY_HOURS");

                    b.ToTable("WeeklyPlanners");
                });

            modelBuilder.Entity("Shared.Models.LeaveRequest", b =>
                {
                    b.HasOne("Shared.Models.UserTenant", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Shared.Models.TimeLog", b =>
                {
                    b.HasOne("Shared.Models.UserTenant", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Shared.Models.UserTenant", b =>
                {
                    b.HasOne("Shared.Models.Team", "Team")
                        .WithMany()
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.Navigation("Team");
                });

            modelBuilder.Entity("Shared.Models.Planners.FixedPlanner", b =>
                {
                    b.HasOne("Shared.Models.Planners.FixedDayPlanner", "Friday")
                        .WithMany()
                        .HasForeignKey("FridayId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Shared.Models.PlannerBase", null)
                        .WithOne()
                        .HasForeignKey("Shared.Models.Planners.FixedPlanner", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Shared.Models.Planners.FixedDayPlanner", "Monday")
                        .WithMany()
                        .HasForeignKey("MondayId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Shared.Models.Planners.FixedDayPlanner", "Saturday")
                        .WithMany()
                        .HasForeignKey("SaturdayId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Shared.Models.Planners.FixedDayPlanner", "Sunday")
                        .WithMany()
                        .HasForeignKey("SundayId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Shared.Models.Planners.FixedDayPlanner", "Thursday")
                        .WithMany()
                        .HasForeignKey("ThursdayId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Shared.Models.Planners.FixedDayPlanner", "Tuesday")
                        .WithMany()
                        .HasForeignKey("TuesdayId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Shared.Models.Planners.FixedDayPlanner", "Wendsday")
                        .WithMany()
                        .HasForeignKey("WendsdayId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Friday");

                    b.Navigation("Monday");

                    b.Navigation("Saturday");

                    b.Navigation("Sunday");

                    b.Navigation("Thursday");

                    b.Navigation("Tuesday");

                    b.Navigation("Wendsday");
                });

            modelBuilder.Entity("Shared.Models.Planners.FlexiblePlanner", b =>
                {
                    b.HasOne("Shared.Models.Planners.FlexibleDayPlanner", "Friday")
                        .WithMany()
                        .HasForeignKey("FridayId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Shared.Models.PlannerBase", null)
                        .WithOne()
                        .HasForeignKey("Shared.Models.Planners.FlexiblePlanner", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Shared.Models.Planners.FlexibleDayPlanner", "Monday")
                        .WithMany()
                        .HasForeignKey("MondayId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Shared.Models.Planners.FlexibleDayPlanner", "Saturday")
                        .WithMany()
                        .HasForeignKey("SaturdayId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Shared.Models.Planners.FlexibleDayPlanner", "Sunday")
                        .WithMany()
                        .HasForeignKey("SundayId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Shared.Models.Planners.FlexibleDayPlanner", "Thursday")
                        .WithMany()
                        .HasForeignKey("ThursdayId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Shared.Models.Planners.FlexibleDayPlanner", "Tuesday")
                        .WithMany()
                        .HasForeignKey("TuesdayId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Shared.Models.Planners.FlexibleDayPlanner", "Wendsday")
                        .WithMany()
                        .HasForeignKey("WendsdayId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Friday");

                    b.Navigation("Monday");

                    b.Navigation("Saturday");

                    b.Navigation("Sunday");

                    b.Navigation("Thursday");

                    b.Navigation("Tuesday");

                    b.Navigation("Wendsday");
                });

            modelBuilder.Entity("Shared.Models.Planners.WeeklyPlanner", b =>
                {
                    b.HasOne("Shared.Models.PlannerBase", null)
                        .WithOne()
                        .HasForeignKey("Shared.Models.Planners.WeeklyPlanner", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
