namespace ClinicFlow.Api.Models;

public sealed class Doctor
{
    public Guid Id { get; private set; }

    public string FullName { get; private set; } = string.Empty;

    public string CrmNumber { get; private set; } = string.Empty;

    public string CrmState { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public string? Phone { get; private set; }

    public Guid SpecialtyId { get; private set; }

    public Specialty Specialty { get; private set; } = null!;
    
    public ICollection<DoctorAvailability> Availabilities { get; private set; }
        = new List<DoctorAvailability>();

    public bool IsActive { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    private Doctor()
    {
        // Construtor utilizado pelo Entity Framework.
    }

    public Doctor(
        string fullName,
        string crmNumber,
        string crmState,
        string email,
        string? phone,
        Guid specialtyId
    )
    {
        if (specialtyId == Guid.Empty)
        {
            throw new ArgumentException(
                "A especialidade do médico é obrigatória.",
                nameof(specialtyId)
            );
        }

        Id = Guid.NewGuid();
        FullName = ValidateFullName(fullName);
        CrmNumber = ValidateCrmNumber(crmNumber);
        CrmState = ValidateCrmState(crmState);
        Email = ValidateEmail(email);
        Phone = NormalizePhone(phone);
        SpecialtyId = specialtyId;
        IsActive = true;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void UpdateProfile(
        string fullName,
        string email,
        string? phone,
        Guid specialtyId
    )
    {
        if (specialtyId == Guid.Empty)
        {
            throw new ArgumentException(
                "A especialidade do médico é obrigatória.",
                nameof(specialtyId)
            );
        }

        FullName = ValidateFullName(fullName);
        Email = ValidateEmail(email);
        Phone = NormalizePhone(phone);
        SpecialtyId = specialtyId;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    private static string ValidateFullName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new ArgumentException(
                "O nome do médico é obrigatório.",
                nameof(fullName)
            );
        }

        var normalizedName = fullName.Trim();

        if (normalizedName.Length < 3)
        {
            throw new ArgumentException(
                "O nome do médico deve ter pelo menos 3 caracteres.",
                nameof(fullName)
            );
        }

        if (normalizedName.Length > 150)
        {
            throw new ArgumentException(
                "O nome do médico deve ter no máximo 150 caracteres.",
                nameof(fullName)
            );
        }

        return normalizedName;
    }

    private static string ValidateCrmNumber(string crmNumber)
    {
        if (string.IsNullOrWhiteSpace(crmNumber))
        {
            throw new ArgumentException(
                "O número do CRM é obrigatório.",
                nameof(crmNumber)
            );
        }

        var normalizedCrm = crmNumber.Trim();

        if (normalizedCrm.Length > 20)
        {
            throw new ArgumentException(
                "O CRM deve ter no máximo 20 caracteres.",
                nameof(crmNumber)
            );
        }

        return normalizedCrm;
    }

    private static string ValidateCrmState(string crmState)
    {
        if (string.IsNullOrWhiteSpace(crmState))
        {
            throw new ArgumentException(
                "O estado do CRM é obrigatório.",
                nameof(crmState)
            );
        }

        var normalizedState = crmState
            .Trim()
            .ToUpperInvariant();

        if (normalizedState.Length != 2)
        {
            throw new ArgumentException(
                "O estado do CRM deve possuir 2 caracteres.",
                nameof(crmState)
            );
        }

        return normalizedState;
    }

    private static string ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException(
                "O e-mail do médico é obrigatório.",
                nameof(email)
            );
        }

        var normalizedEmail = email
            .Trim()
            .ToLowerInvariant();

        if (normalizedEmail.Length > 150)
        {
            throw new ArgumentException(
                "O e-mail deve ter no máximo 150 caracteres.",
                nameof(email)
            );
        }

        return normalizedEmail;
    }

    private static string? NormalizePhone(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
        {
            return null;
        }

        var normalizedPhone = phone.Trim();

        if (normalizedPhone.Length > 20)
        {
            throw new ArgumentException(
                "O telefone deve ter no máximo 20 caracteres.",
                nameof(phone)
            );
        }

        return normalizedPhone;
    }
}