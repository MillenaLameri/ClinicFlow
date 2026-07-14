using System.Net.Mail;

namespace ClinicFlow.Api.Models;

public sealed class Patient
{
    public Guid Id { get; private set; }

    public string FullName { get; private set; } = string.Empty;

    public string Cpf { get; private set; } = string.Empty;

    public DateOnly BirthDate { get; private set; }

    public string Email { get; private set; } = string.Empty;

    public string? Phone { get; private set; }
    
    public ICollection<Appointment> Appointments { get; private set; }
        = new List<Appointment>();

    public bool IsActive { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    private Patient()
    {
        // Utilizado pelo Entity Framework.
    }

    public Patient(
        string fullName,
        string cpf,
        DateOnly birthDate,
        string email,
        string? phone
    )
    {
        Id = Guid.NewGuid();
        FullName = ValidateFullName(fullName);
        Cpf = ValidateCpf(cpf);
        BirthDate = ValidateBirthDate(birthDate);
        Email = ValidateEmail(email);
        Phone = NormalizePhone(phone);
        IsActive = true;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void UpdateProfile(
        string fullName,
        DateOnly birthDate,
        string email,
        string? phone
    )
    {
        FullName = ValidateFullName(fullName);
        BirthDate = ValidateBirthDate(birthDate);
        Email = ValidateEmail(email);
        Phone = NormalizePhone(phone);
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
                "O nome do paciente é obrigatório.",
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

    private static string ValidateCpf(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
        {
            throw new ArgumentException(
                "O CPF é obrigatório.",
                nameof(cpf)
            );
        }

        var normalizedCpf = new string(
            cpf.Where(char.IsDigit).ToArray()
        );

        if (!IsValidCpf(normalizedCpf))
        {
            throw new ArgumentException(
                "O CPF informado é inválido.",
                nameof(cpf)
            );
        }

        return normalizedCpf;
    }

    private static bool IsValidCpf(string cpf)
    {
        if (cpf.Length != 11)
        {
            return false;
        }

        if (cpf.All(character => character == cpf[0]))
        {
            return false;
        }

        var firstSum = 0;

        for (var index = 0; index < 9; index++)
        {
            firstSum +=
                (cpf[index] - '0') * (10 - index);
        }

        var firstRemainder = firstSum % 11;

        var firstDigit =
            firstRemainder < 2
                ? 0
                : 11 - firstRemainder;

        if (firstDigit != cpf[9] - '0')
        {
            return false;
        }

        var secondSum = 0;

        for (var index = 0; index < 10; index++)
        {
            secondSum +=
                (cpf[index] - '0') * (11 - index);
        }

        var secondRemainder = secondSum % 11;

        var secondDigit =
            secondRemainder < 2
                ? 0
                : 11 - secondRemainder;

        return secondDigit == cpf[10] - '0';
    }

    private static DateOnly ValidateBirthDate(
        DateOnly birthDate
    )
    {
        if (birthDate == default)
        {
            throw new ArgumentException(
                "A data de nascimento é obrigatória.",
                nameof(birthDate)
            );
        }

        var today = DateOnly.FromDateTime(DateTime.Today);

        if (birthDate > today)
        {
            throw new ArgumentException(
                "A data de nascimento não pode estar no futuro.",
                nameof(birthDate)
            );
        }

        return birthDate;
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

        if (normalizedEmail.Length > 150)
        {
            throw new ArgumentException(
                "O e-mail deve possuir no máximo 150 caracteres.",
                nameof(email)
            );
        }

        if (!MailAddress.TryCreate(normalizedEmail, out _))
        {
            throw new ArgumentException(
                "O e-mail informado é inválido.",
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

        var normalizedPhone = new string(
            phone.Where(char.IsDigit).ToArray()
        );

        if (normalizedPhone.Length is < 10 or > 11)
        {
            throw new ArgumentException(
                "O telefone deve possuir 10 ou 11 dígitos.",
                nameof(phone)
            );
        }

        return normalizedPhone;
    }
}