using System.Net.Mail;
using Microsoft.AspNetCore.Identity;

namespace ClinicFlow.Api.Models;

public sealed class ApplicationUser : IdentityUser<Guid>
{
    public string FullName { get; private set; } =
        string.Empty;

    public bool IsActive { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public Guid? PatientId { get; private set; }

    public Patient? Patient { get; private set; }

    public Guid? DoctorId { get; private set; }

    public Doctor? Doctor { get; private set; }

    public ICollection<RefreshToken> RefreshTokens
    {
        get;
        private set;
    } = new List<RefreshToken>();

    private ApplicationUser()
    {
        // Construtor utilizado pelo Entity Framework.
    }

    public ApplicationUser(
        string fullName,
        string email
    )
    {
        Id = Guid.NewGuid();

        FullName = ValidateFullName(fullName);

        var normalizedEmail = ValidateEmail(email);

        Email = normalizedEmail;
        UserName = normalizedEmail;

        IsActive = true;
        CreatedAtUtc = DateTime.UtcNow;

        SecurityStamp = Guid.NewGuid().ToString();
        ConcurrencyStamp = Guid.NewGuid().ToString();
    }

    public void LinkPatient(Guid patientId)
    {
        if (patientId == Guid.Empty)
        {
            throw new ArgumentException(
                "O paciente é obrigatório.",
                nameof(patientId)
            );
        }

        if (DoctorId.HasValue)
        {
            throw new InvalidOperationException(
                "Um usuário vinculado a um médico não pode ser vinculado a um paciente."
            );
        }

        PatientId = patientId;
    }

    public void LinkDoctor(Guid doctorId)
    {
        if (doctorId == Guid.Empty)
        {
            throw new ArgumentException(
                "O médico é obrigatório.",
                nameof(doctorId)
            );
        }

        if (PatientId.HasValue)
        {
            throw new InvalidOperationException(
                "Um usuário vinculado a um paciente não pode ser vinculado a um médico."
            );
        }

        DoctorId = doctorId;
    }

    public void UpdateFullName(string fullName)
    {
        FullName = ValidateFullName(fullName);
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Reactivate()
    {
        IsActive = true;
    }

    private static string ValidateFullName(
        string fullName
    )
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new ArgumentException(
                "O nome do usuário é obrigatório.",
                nameof(fullName)
            );
        }

        var normalizedName = fullName.Trim();

        if (normalizedName.Length < 3)
        {
            throw new ArgumentException(
                "O nome deve possuir pelo menos 3 caracteres.",
                nameof(fullName)
            );
        }

        if (normalizedName.Length > 150)
        {
            throw new ArgumentException(
                "O nome deve possuir no máximo 150 caracteres.",
                nameof(fullName)
            );
        }

        return normalizedName;
    }

    private static string ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException(
                "O e-mail é obrigatório.",
                nameof(email)
            );
        }

        var normalizedEmail =
            email.Trim().ToLowerInvariant();

        if (normalizedEmail.Length > 256)
        {
            throw new ArgumentException(
                "O e-mail deve possuir no máximo 256 caracteres.",
                nameof(email)
            );
        }

        if (!MailAddress.TryCreate(
                normalizedEmail,
                out _
            ))
        {
            throw new ArgumentException(
                "O e-mail informado é inválido.",
                nameof(email)
            );
        }

        return normalizedEmail;
    }
}