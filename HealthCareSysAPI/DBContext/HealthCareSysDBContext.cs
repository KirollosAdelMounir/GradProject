using HealthCareSys.Models;
using HealthCareSysAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthCareSysAPI.DBContext
{
    public class HealthCareSysDBContext : DbContext
    {
        public HealthCareSysDBContext(DbContextOptions<HealthCareSysDBContext> options)
      : base(options)
        {
        }
        public DbSet <HealthCareSysUser> Users { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<DoctorHospital> DoctorHospitals { get; set; }
        public DbSet<Forum> Forums { get; set; }
        public DbSet<Hospital> Hospitals { get; set; }
        public DbSet<LabTest> LabTests { get; set; }
        public DbSet<MedicalHistory> MedicalHistories { get; set; }
        public DbSet<Specialization> Specializations { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
        }

    }
}
