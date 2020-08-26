using Microsoft.EntityFrameworkCore;
using P01_HospitalDatabase.Data.Models;
using System.Security.Cryptography.X509Certificates;

namespace P01_HospitalDatabase.Data
{
    public class HospitalContext : DbContext
    {
        public HospitalContext() { }
        public HospitalContext(DbContextOptions options) : base(options)
        {
        }
         protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server = .; Database = HospitalDatabase; Integrated Security = true");
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient>().Property(x => x.Email)
                .IsUnicode(false);
           //
           modelBuilder.Entity<Patient>().HasMany(x => x.Diagnoses);
           modelBuilder.Entity<Patient>().HasMany(x => x.Visitations);
           modelBuilder.Entity<Patient>().HasMany(x => x.Prescriptions);
            modelBuilder.Entity<PatientMedicament>().HasKey(x => new { x.MedicamentId, x.PatientId });
        }

        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Diagnose> Diagnoses { get; set; }
        public DbSet<Medicament> Medicaments { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<PatientMedicament> PatientMedicaments { get; set; }
        public DbSet<Visitation> Visitations { get; set; }
    }
}
