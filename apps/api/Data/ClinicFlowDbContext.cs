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

    public DbSet<Specialty> Specialties =>
        Set<Specialty>();

    public DbSet<Doctor> Doctors =>
        Set<Doctor>();

    public DbSet<DoctorAvailability> DoctorAvailabilities =>
        Set<DoctorAvailability>();
        
    public DbSet<Patient> Patients =>
        Set<Patient>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder
    )
    {
        base.OnModelCreating(modelBuilder);

        ConfigureSpecialty(modelBuilder);
        ConfigureDoctor(modelBuilder);
        ConfigureDoctorAvailability(modelBuilder);
        ConfigurePatient(modelBuilder);
    }

    private static void ConfigureSpecialty(
        ModelBuilder modelBuilder
    )
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

    private static void ConfigureDoctor(
        ModelBuilder modelBuilder
    )
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

    private static void ConfigureDoctorAvailability(
        ModelBuilder modelBuilder
    )
    {
        var availability =
            modelBuilder.Entity<DoctorAvailability>();

        availability.ToTable(
            "doctor_availabilities",
            tableBuilder =>
            {
                tableBuilder.HasCheckConstraint(
                    "ck_doctor_availabilities_day_of_week",
                    "day_of_week BETWEEN 1 AND 7"
                );

                tableBuilder.HasCheckConstraint(
                    "ck_doctor_availabilities_time_range",
                    "start_time < end_time"
                );

                tableBuilder.HasCheckConstraint(
                    "ck_doctor_availabilities_slot_duration",
                    "slot_duration_minutes BETWEEN 10 AND 240"
                );
            }
        );

        availability.HasKey(item => item.Id);

        availability
            .Property(item => item.Id)
            .HasColumnName("id");

        availability
            .Property(item => item.DoctorId)
            .HasColumnName("doctor_id")
            .IsRequired();

        availability
            .Property(item => item.DayOfWeek)
            .HasColumnName("day_of_week")
            .HasConversion<int>()
            .IsRequired();

        availability
            .Property(item => item.StartTime)
            .HasColumnName("start_time")
            .HasColumnType("time without time zone")
            .IsRequired();

        availability
            .Property(item => item.EndTime)
            .HasColumnName("end_time")
            .HasColumnType("time without time zone")
            .IsRequired();

        availability
            .Property(item => item.SlotDurationMinutes)
            .HasColumnName("slot_duration_minutes")
            .IsRequired();

        availability
            .Property(item => item.IsActive)
            .HasColumnName("is_active")
            .IsRequired();

        availability
            .Property(item => item.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();

        availability
            .HasIndex(item => new
            {
                item.DoctorId,
                item.DayOfWeek,
                item.StartTime,
                item.EndTime
            })
            .IsUnique()
            .HasFilter("is_active = TRUE");

        availability
            .HasOne(item => item.Doctor)
            .WithMany(doctor => doctor.Availabilities)
            .HasForeignKey(item => item.DoctorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
    private static void ConfigurePatient(
        ModelBuilder modelBuilder
    )
    {
        var patient = modelBuilder.Entity<Patient>();

        patient.ToTable("patients");

        patient.HasKey(item => item.Id);

        patient
            .Property(item => item.Id)
            .HasColumnName("id");

        patient
            .Property(item => item.FullName)
            .HasColumnName("full_name")
            .HasMaxLength(150)
            .IsRequired();

        patient
            .Property(item => item.Cpf)
            .HasColumnName("cpf")
            .HasMaxLength(11)
            .IsRequired();

        patient
            .HasIndex(item => item.Cpf)
            .IsUnique();

        patient
            .Property(item => item.BirthDate)
            .HasColumnName("birth_date")
            .HasColumnType("date")
            .IsRequired();

        patient
            .Property(item => item.Email)
            .HasColumnName("email")
            .HasMaxLength(150)
            .IsRequired();

        patient
            .HasIndex(item => item.Email)
            .IsUnique();

        patient
            .Property(item => item.Phone)
            .HasColumnName("phone")
            .HasMaxLength(11);

        patient
            .Property(item => item.IsActive)
            .HasColumnName("is_active")
            .IsRequired();

        patient
            .Property(item => item.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();
    }
}