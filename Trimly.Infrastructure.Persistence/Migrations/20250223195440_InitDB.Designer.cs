﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Trimly.Infrastructure.Persistence.Context;

#nullable disable

namespace Trimly.Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(TrimlyContext))]
    [Migration("20250223195440_InitDB")]
    partial class InitDB
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Trimly.Core.Domain.Models.Appointments", b =>
                {
                    b.Property<Guid?>("AppointmentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int?>("AppointmentStatus")
                        .HasColumnType("int");

                    b.Property<string>("CancellationReason")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("ConfirmationCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("EndDateTime")
                        .IsRequired()
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("ReservationId")
                        .IsRequired()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ServiceId")
                        .IsRequired()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("StartDateTime")
                        .IsRequired()
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("UpdateAt")
                        .HasColumnType("datetime2");

                    b.HasKey("AppointmentId")
                        .HasName("PkAppointment");

                    b.HasIndex("ReservationId");

                    b.HasIndex("ServiceId");

                    b.ToTable("Appointment", (string)null);
                });

            modelBuilder.Entity("Trimly.Core.Domain.Models.RegisteredCompanies", b =>
                {
                    b.Property<Guid?>("RegisteredCompaniesId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LogoUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Phone")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RNC")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("RegistrationDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("Status")
                        .HasColumnType("int");

                    b.HasKey("RegisteredCompaniesId")
                        .HasName("PkRegisteredCompany");

                    b.ToTable("RegisteredCompany", (string)null);
                });

            modelBuilder.Entity("Trimly.Core.Domain.Models.Reservations", b =>
                {
                    b.Property<Guid?>("ReservationsId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("AppointmentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ConfirmationCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("EndDateTime")
                        .IsRequired()
                        .HasColumnType("datetime2");

                    b.Property<string>("Note")
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<DateTime?>("StartDateTime")
                        .IsRequired()
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("UpdateAt")
                        .HasColumnType("datetime2");

                    b.HasKey("ReservationsId")
                        .HasName("PkReservation");

                    b.ToTable("Reservation", (string)null);
                });

            modelBuilder.Entity("Trimly.Core.Domain.Models.Reviews", b =>
                {
                    b.Property<Guid?>("ReviewId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Comment")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("Rating")
                        .HasColumnType("int");

                    b.Property<Guid?>("RegisteredCompanyId")
                        .IsRequired()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime?>("UpdateAt")
                        .HasColumnType("datetime2");

                    b.HasKey("ReviewId")
                        .HasName("PkReview");

                    b.HasIndex("RegisteredCompanyId");

                    b.ToTable("Review", (string)null);
                });

            modelBuilder.Entity("Trimly.Core.Domain.Models.Schedules", b =>
                {
                    b.Property<Guid?>("SchedulesId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<TimeOnly>("ClosingTime")
                        .HasColumnType("time");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("IsHoliday")
                        .HasColumnType("int");

                    b.Property<string>("Notes")
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<TimeOnly>("OpeningTime")
                        .HasColumnType("time");

                    b.Property<Guid?>("RegisteredCompanyId")
                        .IsRequired()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("UpdateAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("Week")
                        .HasColumnType("int");

                    b.HasKey("SchedulesId")
                        .HasName("PkSchedule");

                    b.HasIndex("RegisteredCompanyId");

                    b.ToTable("Schedule", (string)null);
                });

            modelBuilder.Entity("Trimly.Core.Domain.Models.Services", b =>
                {
                    b.Property<Guid?>("ServicesId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("DurationInMinutes")
                        .HasColumnType("int");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(10,2)");

                    b.Property<Guid?>("RegisteredCompanyId")
                        .IsRequired()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int?>("Status")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdateAt")
                        .HasColumnType("datetime2");

                    b.HasKey("ServicesId")
                        .HasName("PkService");

                    b.HasIndex("RegisteredCompanyId");

                    b.ToTable("Service", (string)null);
                });

            modelBuilder.Entity("Trimly.Core.Domain.Models.Appointments", b =>
                {
                    b.HasOne("Trimly.Core.Domain.Models.Reservations", "Reservations")
                        .WithMany("Appointments")
                        .HasForeignKey("ReservationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FkAppointmentReservation");

                    b.HasOne("Trimly.Core.Domain.Models.Services", "Services")
                        .WithMany("Appointments")
                        .HasForeignKey("ServiceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FkAppointmentService");

                    b.Navigation("Reservations");

                    b.Navigation("Services");
                });

            modelBuilder.Entity("Trimly.Core.Domain.Models.Reviews", b =>
                {
                    b.HasOne("Trimly.Core.Domain.Models.RegisteredCompanies", "RegisteredCompanies")
                        .WithMany("Reviews")
                        .HasForeignKey("RegisteredCompanyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FkReviewsRegisteredCompany");

                    b.Navigation("RegisteredCompanies");
                });

            modelBuilder.Entity("Trimly.Core.Domain.Models.Schedules", b =>
                {
                    b.HasOne("Trimly.Core.Domain.Models.RegisteredCompanies", "RegisteredCompanies")
                        .WithMany("Schedules")
                        .HasForeignKey("RegisteredCompanyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FkSchedulesRegisteredCompany");

                    b.Navigation("RegisteredCompanies");
                });

            modelBuilder.Entity("Trimly.Core.Domain.Models.Services", b =>
                {
                    b.HasOne("Trimly.Core.Domain.Models.RegisteredCompanies", "RegisteredCompanies")
                        .WithMany("Services")
                        .HasForeignKey("RegisteredCompanyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FkServicesRegisteredCompany");

                    b.Navigation("RegisteredCompanies");
                });

            modelBuilder.Entity("Trimly.Core.Domain.Models.RegisteredCompanies", b =>
                {
                    b.Navigation("Reviews");

                    b.Navigation("Schedules");

                    b.Navigation("Services");
                });

            modelBuilder.Entity("Trimly.Core.Domain.Models.Reservations", b =>
                {
                    b.Navigation("Appointments");
                });

            modelBuilder.Entity("Trimly.Core.Domain.Models.Services", b =>
                {
                    b.Navigation("Appointments");
                });
#pragma warning restore 612, 618
        }
    }
}
