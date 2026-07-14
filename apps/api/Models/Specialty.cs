namespace ClinicFlow.Api.Models;

public sealed class Specialty
{
    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public bool IsActive { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    private Specialty()
    {
        // Construtor utilizado pelo Entity Framework.
    }

    public Specialty(string name)
    {
        Id = Guid.NewGuid();
        Name = ValidateName(name);
        IsActive = true;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void UpdateName(string name)
    {
        Name = ValidateName(name);
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    private static string ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "O nome da especialidade é obrigatório.",
                nameof(name)
            );
        }

        var trimmedName = name.Trim();

        if (trimmedName.Length > 100)
        {
            throw new ArgumentException(
                "O nome da especialidade deve ter no máximo 100 caracteres.",
                nameof(name)
            );
        }

        return trimmedName;
    }
}