using ClinicFlow.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Api.Data;

public sealed class ClinicFlowDbContext : DbContext
{
    public ClinicFlowDbContext(
        DbContextOptions<ClinicFlowDbContext> options
    ) : base(options)
    {
    }

    public DbSet<Specialty> Specialties => Set<Specialty>();

    public DbSet<Doctor> Doctors => Set<Doctor>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureSpecialty(modelBuilder);
        ConfigureDoctor(modelBuilder);
    }

    private static void ConfigureSpecialty(ModelBuilder modelBuilder)
    {
        var specialty = modelBuilder.Entity<Specialty>();

        specialty.ToTable("specialties");

        specialty.HasKey(item => item.Id);

        specialty
            .Property(item => item.Id)
            .HasColumnName("id");

        specialty
            .Property(item => item.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();

        specialty
            .HasIndex(item => item.Name)
            .IsUnique();

        specialty
            .Property(item => item.IsActive)
            .HasColumnName("is_active")
            .IsRequired();

        specialty
            .Property(item => item.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();
    }

    private static void ConfigureDoctor(ModelBuilder modelBuilder)
    {
        var doctor = modelBuilder.Entity<Doctor>();

        doctor.ToTable("doctors");

        doctor.HasKey(item => item.Id);

        doctor
            .Property(item => item.Id)
            .HasColumnName("id");

        doctor
            .Property(item => item.FullName)
            .HasColumnName("full_name")
            .HasMaxLength(150)
            .IsRequired();

        doctor
            .Property(item => item.CrmNumber)
            .HasColumnName("crm_number")
            .HasMaxLength(20)
            .IsRequired();

        doctor
            .Property(item => item.CrmState)
            .HasColumnName("crm_state")
            .HasMaxLength(2)
            .IsRequired();

        doctor
            .HasIndex(item => new
            {
                item.CrmNumber,
                item.CrmState
            })
            .IsUnique();

        doctor
            .Property(item => item.Email)
            .HasColumnName("email")
            .HasMaxLength(150)
            .IsRequired();

        doctor
            .HasIndex(item => item.Email)
            .IsUnique();

        doctor
            .Property(item => item.Phone)
            .HasColumnName("phone")
            .HasMaxLength(20);

        doctor
            .Property(item => item.SpecialtyId)
            .HasColumnName("specialty_id")
            .IsRequired();

        doctor
            .Property(item => item.IsActive)
            .HasColumnName("is_active")
            .IsRequired();

        doctor
            .Property(item => item.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();

        doctor
            .HasOne(item => item.Specialty)
            .WithMany()
            .HasForeignKey(item => item.SpecialtyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}