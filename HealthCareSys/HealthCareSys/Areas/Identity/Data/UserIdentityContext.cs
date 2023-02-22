using HealthCareSys.Areas.Identity.Data;
using HealthCareSys.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HealthCareSys.Areas.Identity.Data;

public class UserIdentityContext : IdentityDbContext<HealthCareSysUser>
{
    public UserIdentityContext(DbContextOptions<UserIdentityContext> options)
        : base(options)
    {
    }
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
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);

    }
}
