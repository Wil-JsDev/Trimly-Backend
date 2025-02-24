using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trimly.Core.Domain.Models;

namespace Trimly.Infrastructure.Persistence.Context
{
    public class TrimlyContext : DbContext
    {
        public TrimlyContext(DbContextOptions<TrimlyContext> options) : base(options) { }

        #region Models

        public DbSet<RegisteredCompanies> RegisteredCompanies { get; set; }
        public DbSet<Services> Services { get; set; }
        public DbSet<Reviews> Reviews { get; set; }
        public DbSet<Schedules> Schedules { get; set; }
        public DbSet<Reservations> Reservations { get; set; }
        public DbSet<Appointments> Appointments { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Tables

            modelBuilder.Entity<RegisteredCompanies>()
                .ToTable("RegisteredCompany");
            modelBuilder.Entity<Services>()
                .ToTable("Service");
            modelBuilder.Entity<Reviews>()
                .ToTable("Review");
            modelBuilder.Entity<Schedules>()
                .ToTable("Schedule");
            modelBuilder.Entity<Reservations>()
                .ToTable("Reservation");
            modelBuilder.Entity<Appointments>()
                .ToTable("Appointment");

            #endregion

            #region Primary key

            modelBuilder.Entity<RegisteredCompanies>()
                .HasKey(b => b.RegisteredCompaniesId)
                .HasName("PkRegisteredCompany");

            modelBuilder.Entity<Services>()
                .HasKey(b => b.ServicesId)
                .HasName("PkService");

            modelBuilder.Entity<Reviews>()
                .HasKey(b => b.ReviewId)
                .HasName("PkReview");

            modelBuilder.Entity<Schedules>()
                .HasKey(b => b.SchedulesId)
                .HasName("PkSchedule");

            modelBuilder.Entity<Reservations>()
                .HasKey(b => b.ReservationsId)
                .HasName("PkReservation");

            modelBuilder.Entity<Appointments>()
                .HasKey(b => b.AppointmentId)
                .HasName("PkAppointment");

            #endregion

            #region Relationships

            modelBuilder.Entity<Services>()
                .HasOne(b => b.RegisteredCompanies)
                .WithMany(b => b.Services)
                .HasForeignKey(b => b.RegisteredCompanyId)
                .IsRequired()
                .HasConstraintName("FkSSRegisteredCompany");

            modelBuilder.Entity<Reviews>()
                .HasOne(b => b.RegisteredCompanies)
                .WithMany(b => b.Reviews)
                .HasForeignKey(b => b.RegisteredCompanyId)
                .IsRequired()
                .HasConstraintName("FkRRegisteredCompany");

            modelBuilder.Entity<Schedules>()
                .HasOne(b => b.RegisteredCompanies)
                .WithMany(b => b.Schedules)
                .HasForeignKey(b => b.RegisteredCompanyId)
                .IsRequired()
                .HasConstraintName("FkSRegisteredCompany");

            modelBuilder.Entity<Appointments>()
                .HasOne(b => b.Services)
                .WithMany(b => b.Appointments)
                .HasForeignKey(b => b.ServiceId)
                .IsRequired()
                .HasConstraintName("FkService");

            modelBuilder.Entity<Appointments>()
                .HasOne(b => b.Reservations)
                .WithMany(b => b.Appointments)
                .HasForeignKey(b => b.ReservationId)
                .IsRequired()
                .HasConstraintName("FkReservation");

            #endregion

            #region RegisteredCompanies Property

            modelBuilder.Entity<RegisteredCompanies>(entity =>
            {
                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsRequired();
                entity.Property(e => e.Description)
                    .HasMaxLength(250);
                entity.Property(e => e.RegisteredCompaniesId)
                    .ValueGeneratedOnAdd();
            });

            #endregion

            #region Services Property

            modelBuilder.Entity<Services>(entity =>
            {
                entity.Property(e => e.Name)
                    .HasMaxLength(50).IsRequired();
                entity.Property(e => e.Price)
                    .HasColumnType("decimal(10,2)")
                    .IsRequired();
                entity.Property(e => e.Description)
                    .HasMaxLength(100);
                entity.Property(e => e.DurationInMinutes)
                    .IsRequired();
                entity.Property(e => e.ServicesId)
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasConversion<string>();
            });

            #endregion

            #region Reviews Property

            modelBuilder.Entity<Reviews>(entity =>
            {
                entity.Property(e => e.Title)
                    .HasMaxLength(50)
                    .IsRequired();
                entity.Property(e => e.Comment)
                    .HasMaxLength(250)
                    .IsRequired();
                entity.Property(e => e.Rating)
                    .IsRequired();
                entity.Property(e => e.ReviewId)
                    .ValueGeneratedOnAdd();
            });

            #endregion

            #region Schedules Property

            modelBuilder.Entity<Schedules>(entity =>
            {
                entity.Property(e => e.OpeningTime)
                    .IsRequired();
                entity.Property(e => e.ClosingTime)
                    .IsRequired();
                entity.Property(e => e.Notes)
                    .HasMaxLength(150);
                entity.Property(e => e.IsHoliday)
                    .IsRequired();
                entity.Property(e => e.SchedulesId)
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.IsHoliday)
                    .IsRequired()
                    .HasConversion<string>();
                entity.Property(e => e.Week)
                    .IsRequired()
                    .HasConversion<string>();
            });

            #endregion

            #region Reservations Property

            modelBuilder.Entity<Reservations>(entity =>
            {
                entity.Property(e => e.StartDateTime)
                    .IsRequired();
                entity.Property(e => e.EndDateTime)
                    .IsRequired();
                entity.Property(e => e.Note)
                    .HasMaxLength(150);
                entity.Property(e => e.ReservationsId)
                    .ValueGeneratedOnAdd();
            });

            #endregion

            #region Appointments Property

            modelBuilder.Entity<Appointments>(entity =>
            {
                entity.Property(e => e.StartDateTime)
                    .IsRequired();
                entity.Property(e => e.EndDateTime)
                    .IsRequired();
                entity.Property(e => e.CancellationReason)
                    .HasMaxLength(150)
                    .IsRequired();
                entity.Property(e => e.AppointmentId)
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.AppointmentStatus)
                    .IsRequired()
                    .HasConversion<string>();
            });

            #endregion
        }
    }
}
