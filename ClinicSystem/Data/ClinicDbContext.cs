using ClinicSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ClinicSystem.Data
{
    public class ClinicDbContext : IdentityDbContext<ApplicationUser>
    {
        public ClinicDbContext(DbContextOptions<ClinicDbContext> options)
            : base(options)
        {
        }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Specialization> Specializations { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Patient>()
                .HasIndex(p => p.PESEL)
                .IsUnique();

            modelBuilder.Entity<Patient>()
                .HasOne(p => p.ApplicationUser)
                .WithOne(u => u.Patient)
                .HasForeignKey<Patient>(p => p.ApplicationUserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Doctor>()
                .HasIndex(d => d.LicenseNumber)
                .IsUnique();

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Specialization>().HasData(
                new Specialization { Id = 1, Name = "Internista", Description = "Lekarz pierwszego kontaktu dla osób dorosłych." },
                new Specialization { Id = 2, Name = "Pediatra", Description = "Lekarz zajmujący się diagnostyką i leczeniem dzieci." },
                new Specialization { Id = 3, Name = "Kardiolog", Description = "Specjalista chorób serca i układu krążenia." },
                new Specialization { Id = 4, Name = "Dermatolog", Description = "Specjalista chorób skóry." },
                new Specialization { Id = 5, Name = "Ortopeda", Description = "Specjalista układu ruchu, kości i stawów." },
                new Specialization { Id = 6, Name = "Neurolog", Description = "Specjalista chorób układu nerwowego." }
            );
        }
    }
}