﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Trimly.Infrastructure.Persistence.Context;

#nullable disable

namespace Trimly.Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(TrimlyContext))]
    partial class TrimlyContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Trimly.Core.Domain.Models.Appointments", b =>
                {
                    b.Property<Guid?>("AppointmentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("AppointmentStatus")
                        .HasColumnType("integer");

                    b.Property<string>("CancellationReason")
                        .HasColumnType("text");

                    b.Property<string>("ConfirmationCode")
                        .HasColumnType("text");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("EndDateTime")
                        .IsRequired()
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("ServiceId")
                        .IsRequired()
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("StartDateTime")
                        .IsRequired()
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("UpdateAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("AppointmentId")
                        .HasName("PkAppointment");

                    b.HasIndex("ServiceId");

                    b.ToTable("Appointment", (string)null);
                });

            modelBuilder.Entity("Trimly.Core.Domain.Models.RegisteredCompanies", b =>
                {
                    b.Property<Guid?>("RegisteredCompaniesId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Address")
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<string>("LogoUrl")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("Phone")
                        .HasColumnType("text");

                    b.Property<string>("RNC")
                        .HasColumnType("text");

                    b.Property<DateTime?>("RegistrationDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int?>("Status")
                        .HasColumnType("integer");

                    b.HasKey("RegisteredCompaniesId")
                        .HasName("PkRegisteredCompany");

                    b.ToTable("RegisteredCompany", (string)null);
                });

            modelBuilder.Entity("Trimly.Core.Domain.Models.Reviews", b =>
                {
                    b.Property<Guid?>("ReviewId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Comment")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Rating")
                        .HasColumnType("integer");

                    b.Property<Guid?>("RegisteredCompanyId")
                        .IsRequired()
                        .HasColumnType("uuid");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<DateTime?>("UpdateAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("ReviewId")
                        .HasName("PkReview");

                    b.HasIndex("RegisteredCompanyId");

                    b.ToTable("Review", (string)null);
                });

            modelBuilder.Entity("Trimly.Core.Domain.Models.Schedules", b =>
                {
                    b.Property<Guid?>("SchedulesId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<TimeOnly>("ClosingTime")
                        .HasColumnType("time without time zone");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("IsHoliday")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Notes")
                        .HasMaxLength(150)
                        .HasColumnType("character varying(150)");

                    b.Property<TimeOnly>("OpeningTime")
                        .HasColumnType("time without time zone");

                    b.Property<Guid?>("RegisteredCompanyId")
                        .IsRequired()
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("UpdateAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Week")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("SchedulesId")
                        .HasName("PkSchedule");

                    b.HasIndex("RegisteredCompanyId");

                    b.ToTable("Schedule", (string)null);
                });

            modelBuilder.Entity("Trimly.Core.Domain.Models.Services", b =>
                {
                    b.Property<Guid?>("ServicesId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<int>("DurationInMinutes")
                        .HasColumnType("integer");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<decimal>("PenaltyAmount")
                        .HasColumnType("decimal(10,2)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(10,2)");

                    b.Property<Guid?>("RegisteredCompanyId")
                        .IsRequired()
                        .HasColumnType("uuid");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("UpdateAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("ServicesId")
                        .HasName("PkService");

                    b.HasIndex("RegisteredCompanyId");

                    b.ToTable("Service", (string)null);
                });

            modelBuilder.Entity("Trimly.Core.Domain.Models.Appointments", b =>
                {
                    b.HasOne("Trimly.Core.Domain.Models.Services", "Services")
                        .WithMany("Appointments")
                        .HasForeignKey("ServiceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FkService");

                    b.Navigation("Services");
                });

            modelBuilder.Entity("Trimly.Core.Domain.Models.Reviews", b =>
                {
                    b.HasOne("Trimly.Core.Domain.Models.RegisteredCompanies", "RegisteredCompanies")
                        .WithMany("Reviews")
                        .HasForeignKey("RegisteredCompanyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FkRRegisteredCompany");

                    b.Navigation("RegisteredCompanies");
                });

            modelBuilder.Entity("Trimly.Core.Domain.Models.Schedules", b =>
                {
                    b.HasOne("Trimly.Core.Domain.Models.RegisteredCompanies", "RegisteredCompanies")
                        .WithMany("Schedules")
                        .HasForeignKey("RegisteredCompanyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FkSRegisteredCompany");

                    b.Navigation("RegisteredCompanies");
                });

            modelBuilder.Entity("Trimly.Core.Domain.Models.Services", b =>
                {
                    b.HasOne("Trimly.Core.Domain.Models.RegisteredCompanies", "RegisteredCompanies")
                        .WithMany("Services")
                        .HasForeignKey("RegisteredCompanyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FkSSRegisteredCompany");

                    b.Navigation("RegisteredCompanies");
                });

            modelBuilder.Entity("Trimly.Core.Domain.Models.RegisteredCompanies", b =>
                {
                    b.Navigation("Reviews");

                    b.Navigation("Schedules");

                    b.Navigation("Services");
                });

            modelBuilder.Entity("Trimly.Core.Domain.Models.Services", b =>
                {
                    b.Navigation("Appointments");
                });
#pragma warning restore 612, 618
        }
    }
}
