using ClinicFlow.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Api.Data;

public sealed class ClinicFlowDbContext
    : IdentityDbContext<
        ApplicationUser,
        IdentityRole<Guid>,
        Guid
    >
{
    private static readonly Guid AdminRoleId =
        Guid.Parse(
            "11111111-1111-1111-1111-111111111111"
        );

    private static readonly Guid DoctorRoleId =
        Guid.Parse(
            "22222222-2222-2222-2222-222222222222"
        );

    private static readonly Guid PatientRoleId =
        Guid.Parse(
            "33333333-3333-3333-3333-333333333333"
        );

    public ClinicFlowDbContext(
        DbContextOptions<ClinicFlowDbContext> options
    ) : base(options)
    {
    }

    public DbSet<Specialty> Specialties =>
        Set<Specialty>();

    public DbSet<Doctor> Doctors =>
        Set<Doctor>();

    public DbSet<DoctorAvailability>
        DoctorAvailabilities =>
            Set<DoctorAvailability>();

    public DbSet<Patient> Patients =>
        Set<Patient>();

    public DbSet<Appointment> Appointments =>
        Set<Appointment>();

    public DbSet<RefreshToken> RefreshTokens =>
        Set<RefreshToken>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder
    )
    {
        base.OnModelCreating(modelBuilder);

        ConfigureIdentity(modelBuilder);
        ConfigureApplicationUser(modelBuilder);
        ConfigureRefreshToken(modelBuilder);

        ConfigureSpecialty(modelBuilder);
        ConfigureDoctor(modelBuilder);
        ConfigureDoctorAvailability(modelBuilder);
        ConfigurePatient(modelBuilder);
        ConfigureAppointment(modelBuilder);
    }

    private static void ConfigureIdentity(
        ModelBuilder modelBuilder
    )
    {
        var user =
            modelBuilder.Entity<ApplicationUser>();

        user.ToTable("users");

        user
            .Property(item => item.Id)
            .HasColumnName("id");

        user
            .Property(item => item.UserName)
            .HasColumnName("user_name")
            .HasMaxLength(256);

        user
            .Property(item => item.NormalizedUserName)
            .HasColumnName("normalized_user_name")
            .HasMaxLength(256);

        user
            .Property(item => item.Email)
            .HasColumnName("email")
            .HasMaxLength(256);

        user
            .Property(item => item.NormalizedEmail)
            .HasColumnName("normalized_email")
            .HasMaxLength(256);

        user
            .Property(item => item.EmailConfirmed)
            .HasColumnName("email_confirmed");

        user
            .Property(item => item.PasswordHash)
            .HasColumnName("password_hash");

        user
            .Property(item => item.SecurityStamp)
            .HasColumnName("security_stamp");

        user
            .Property(item => item.ConcurrencyStamp)
            .HasColumnName("concurrency_stamp");

        user
            .Property(item => item.PhoneNumber)
            .HasColumnName("phone_number");

        user
            .Property(item => item.PhoneNumberConfirmed)
            .HasColumnName(
                "phone_number_confirmed"
            );

        user
            .Property(item => item.TwoFactorEnabled)
            .HasColumnName("two_factor_enabled");

        user
            .Property(item => item.LockoutEnd)
            .HasColumnName("lockout_end");

        user
            .Property(item => item.LockoutEnabled)
            .HasColumnName("lockout_enabled");

        user
            .Property(item => item.AccessFailedCount)
            .HasColumnName("access_failed_count");

        user
            .HasIndex(item => item.NormalizedUserName)
            .IsUnique()
            .HasDatabaseName(
                "ux_users_normalized_user_name"
            );

        user
            .HasIndex(item => item.NormalizedEmail)
            .IsUnique()
            .HasDatabaseName(
                "ux_users_normalized_email"
            );

        var role =
            modelBuilder.Entity<
                IdentityRole<Guid>
            >();

        role.ToTable("roles");

        role
            .Property(item => item.Id)
            .HasColumnName("id");

        role
            .Property(item => item.Name)
            .HasColumnName("name")
            .HasMaxLength(256);

        role
            .Property(item => item.NormalizedName)
            .HasColumnName("normalized_name")
            .HasMaxLength(256);

        role
            .Property(item => item.ConcurrencyStamp)
            .HasColumnName("concurrency_stamp");

        role
            .HasIndex(item => item.NormalizedName)
            .IsUnique()
            .HasDatabaseName(
                "ux_roles_normalized_name"
            );

        role.HasData(
            new IdentityRole<Guid>
            {
                Id = AdminRoleId,
                Name = UserRoleNames.Admin,
                NormalizedName = "ADMIN",
                ConcurrencyStamp =
                    "admin-role-concurrency-stamp"
            },
            new IdentityRole<Guid>
            {
                Id = DoctorRoleId,
                Name = UserRoleNames.Doctor,
                NormalizedName = "DOCTOR",
                ConcurrencyStamp =
                    "doctor-role-concurrency-stamp"
            },
            new IdentityRole<Guid>
            {
                Id = PatientRoleId,
                Name = UserRoleNames.Patient,
                NormalizedName = "PATIENT",
                ConcurrencyStamp =
                    "patient-role-concurrency-stamp"
            }
        );

        var userRole =
            modelBuilder.Entity<
                IdentityUserRole<Guid>
            >();

        userRole.ToTable("user_roles");

        userRole
            .Property(item => item.UserId)
            .HasColumnName("user_id");

        userRole
            .Property(item => item.RoleId)
            .HasColumnName("role_id");

        var userClaim =
            modelBuilder.Entity<
                IdentityUserClaim<Guid>
            >();

        userClaim.ToTable("user_claims");

        userClaim
            .Property(item => item.Id)
            .HasColumnName("id");

        userClaim
            .Property(item => item.UserId)
            .HasColumnName("user_id");

        userClaim
            .Property(item => item.ClaimType)
            .HasColumnName("claim_type");

        userClaim
            .Property(item => item.ClaimValue)
            .HasColumnName("claim_value");

        var userLogin =
            modelBuilder.Entity<
                IdentityUserLogin<Guid>
            >();

        userLogin.ToTable("user_logins");

        userLogin
            .Property(item => item.LoginProvider)
            .HasColumnName("login_provider");

        userLogin
            .Property(item => item.ProviderKey)
            .HasColumnName("provider_key");

        userLogin
            .Property(item => item.ProviderDisplayName)
            .HasColumnName(
                "provider_display_name"
            );

        userLogin
            .Property(item => item.UserId)
            .HasColumnName("user_id");

        var roleClaim =
            modelBuilder.Entity<
                IdentityRoleClaim<Guid>
            >();

        roleClaim.ToTable("role_claims");

        roleClaim
            .Property(item => item.Id)
            .HasColumnName("id");

        roleClaim
            .Property(item => item.RoleId)
            .HasColumnName("role_id");

        roleClaim
            .Property(item => item.ClaimType)
            .HasColumnName("claim_type");

        roleClaim
            .Property(item => item.ClaimValue)
            .HasColumnName("claim_value");

        var userToken =
            modelBuilder.Entity<
                IdentityUserToken<Guid>
            >();

        userToken.ToTable("user_tokens");

        userToken
            .Property(item => item.UserId)
            .HasColumnName("user_id");

        userToken
            .Property(item => item.LoginProvider)
            .HasColumnName("login_provider");

        userToken
            .Property(item => item.Name)
            .HasColumnName("name");

        userToken
            .Property(item => item.Value)
            .HasColumnName("value");
    }

    private static void ConfigureApplicationUser(
        ModelBuilder modelBuilder
    )
    {
        var user =
            modelBuilder.Entity<ApplicationUser>();

        user.ToTable(
            "users",
            tableBuilder =>
            {
                tableBuilder.HasCheckConstraint(
                    "ck_users_single_profile",
                    "NOT (" +
                    "patient_id IS NOT NULL " +
                    "AND doctor_id IS NOT NULL" +
                    ")"
                );
            }
        );

        user
            .Property(item => item.FullName)
            .HasColumnName("full_name")
            .HasMaxLength(150)
            .IsRequired();

        user
            .Property(item => item.IsActive)
            .HasColumnName("is_active")
            .IsRequired();

        user
            .Property(item => item.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();

        user
            .Property(item => item.PatientId)
            .HasColumnName("patient_id");

        user
            .Property(item => item.DoctorId)
            .HasColumnName("doctor_id");

        user
            .HasIndex(item => item.PatientId)
            .IsUnique()
            .HasFilter("patient_id IS NOT NULL")
            .HasDatabaseName(
                "ux_users_patient_id"
            );

        user
            .HasIndex(item => item.DoctorId)
            .IsUnique()
            .HasFilter("doctor_id IS NOT NULL")
            .HasDatabaseName(
                "ux_users_doctor_id"
            );

        user
            .HasOne(item => item.Patient)
            .WithOne()
            .HasForeignKey<ApplicationUser>(
                item => item.PatientId
            )
            .OnDelete(DeleteBehavior.Restrict);

        user
            .HasOne(item => item.Doctor)
            .WithOne()
            .HasForeignKey<ApplicationUser>(
                item => item.DoctorId
            )
            .OnDelete(DeleteBehavior.Restrict);
    }

    private static void ConfigureRefreshToken(
        ModelBuilder modelBuilder
    )
    {
        var refreshToken =
            modelBuilder.Entity<RefreshToken>();

        refreshToken.ToTable("refresh_tokens");

        refreshToken.HasKey(item => item.Id);

        refreshToken
            .Property(item => item.Id)
            .HasColumnName("id");

        refreshToken
            .Property(item => item.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        refreshToken
            .Property(item => item.TokenHash)
            .HasColumnName("token_hash")
            .HasMaxLength(128)
            .IsRequired();

        refreshToken
            .Property(item => item.ExpiresAtUtc)
            .HasColumnName("expires_at_utc")
            .IsRequired();

        refreshToken
            .Property(item => item.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();

        refreshToken
            .Property(item => item.RevokedAtUtc)
            .HasColumnName("revoked_at_utc");

        refreshToken
            .Property(item =>
                item.ReplacedByTokenHash
            )
            .HasColumnName(
                "replaced_by_token_hash"
            )
            .HasMaxLength(128);

        refreshToken
            .HasIndex(item => item.TokenHash)
            .IsUnique()
            .HasDatabaseName(
                "ux_refresh_tokens_token_hash"
            );

        refreshToken
            .HasIndex(item => new
            {
                item.UserId,
                item.ExpiresAtUtc
            })
            .HasDatabaseName(
                "ix_refresh_tokens_user_expiration"
            );

        refreshToken
            .HasOne(item => item.User)
            .WithMany(user => user.RefreshTokens)
            .HasForeignKey(item => item.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private static void ConfigureSpecialty(
        ModelBuilder modelBuilder
    )
    {
        var specialty =
            modelBuilder.Entity<Specialty>();

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
        var doctor =
            modelBuilder.Entity<Doctor>();

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
            modelBuilder.Entity<
                DoctorAvailability
            >();

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
            .HasColumnType(
                "time without time zone"
            )
            .IsRequired();

        availability
            .Property(item => item.EndTime)
            .HasColumnName("end_time")
            .HasColumnType(
                "time without time zone"
            )
            .IsRequired();

        availability
            .Property(item =>
                item.SlotDurationMinutes
            )
            .HasColumnName(
                "slot_duration_minutes"
            )
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
            .WithMany(
                doctor => doctor.Availabilities
            )
            .HasForeignKey(item => item.DoctorId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private static void ConfigurePatient(
        ModelBuilder modelBuilder
    )
    {
        var patient =
            modelBuilder.Entity<Patient>();

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

    private static void ConfigureAppointment(
        ModelBuilder modelBuilder
    )
    {
        var appointment =
            modelBuilder.Entity<Appointment>();

        appointment.ToTable(
            "appointments",
            tableBuilder =>
            {
                tableBuilder.HasCheckConstraint(
                    "ck_appointments_time_range",
                    "start_time < end_time"
                );

                tableBuilder.HasCheckConstraint(
                    "ck_appointments_status",
                    "status BETWEEN 1 AND 4"
                );
            }
        );

        appointment.HasKey(item => item.Id);

        appointment
            .Property(item => item.Id)
            .HasColumnName("id");

        appointment
            .Property(item => item.DoctorId)
            .HasColumnName("doctor_id")
            .IsRequired();

        appointment
            .Property(item => item.PatientId)
            .HasColumnName("patient_id")
            .IsRequired();

        appointment
            .Property(item =>
                item.DoctorAvailabilityId
            )
            .HasColumnName(
                "doctor_availability_id"
            )
            .IsRequired();

        appointment
            .Property(item =>
                item.AppointmentDate
            )
            .HasColumnName(
                "appointment_date"
            )
            .HasColumnType("date")
            .IsRequired();

        appointment
            .Property(item => item.StartTime)
            .HasColumnName("start_time")
            .HasColumnType(
                "time without time zone"
            )
            .IsRequired();

        appointment
            .Property(item => item.EndTime)
            .HasColumnName("end_time")
            .HasColumnType(
                "time without time zone"
            )
            .IsRequired();

        appointment
            .Property(item => item.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .IsRequired();

        appointment
            .Property(item => item.Notes)
            .HasColumnName("notes")
            .HasMaxLength(500);

        appointment
            .Property(item =>
                item.CancellationReason
            )
            .HasColumnName(
                "cancellation_reason"
            )
            .HasMaxLength(300);

        appointment
            .Property(item => item.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();

        appointment
            .Property(item =>
                item.CancelledAtUtc
            )
            .HasColumnName(
                "cancelled_at_utc"
            );

        appointment
            .Property(item =>
                item.CompletedAtUtc
            )
            .HasColumnName(
                "completed_at_utc"
            );

        appointment
            .HasIndex(item => new
            {
                item.DoctorId,
                item.AppointmentDate,
                item.StartTime
            })
            .IsUnique()
            .HasFilter("status = 1")
            .HasDatabaseName(
                "ux_appointments_doctor_date_start_scheduled"
            );

        appointment
            .HasIndex(item => new
            {
                item.PatientId,
                item.AppointmentDate,
                item.StartTime
            })
            .IsUnique()
            .HasFilter("status = 1")
            .HasDatabaseName(
                "ux_appointments_patient_date_start_scheduled"
            );

        appointment
            .HasIndex(item => new
            {
                item.DoctorId,
                item.AppointmentDate,
                item.Status
            })
            .HasDatabaseName(
                "ix_appointments_doctor_date_status"
            );

        appointment
            .HasOne(item => item.Doctor)
            .WithMany(
                doctor => doctor.Appointments
            )
            .HasForeignKey(item => item.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        appointment
            .HasOne(item => item.Patient)
            .WithMany(
                patient => patient.Appointments
            )
            .HasForeignKey(item => item.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        appointment
            .HasOne(item =>
                item.DoctorAvailability
            )
            .WithMany(
                availability =>
                    availability.Appointments
            )
            .HasForeignKey(item =>
                item.DoctorAvailabilityId
            )
            .OnDelete(DeleteBehavior.Restrict);
    }
}