using System.ComponentModel.DataAnnotations;

namespace ClinicFlow.Api.Contracts.Patients;

public sealed class UpdatePatientRequest
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [MinLength(
        3,
        ErrorMessage = "O nome deve possuir pelo menos 3 caracteres."
    )]
    [MaxLength(
        150,
        ErrorMessage = "O nome deve possuir no máximo 150 caracteres."
    )]
    public string FullName { get; init; } = string.Empty;

    public DateOnly BirthDate { get; init; }

    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(
        ErrorMessage = "O e-mail informado é inválido."
    )]
    [MaxLength(
        150,
        ErrorMessage = "O e-mail deve possuir no máximo 150 caracteres."
    )]
    public string Email { get; init; } = string.Empty;

    [MaxLength(
        20,
        ErrorMessage = "O telefone informado é inválido."
    )]
    public string? Phone { get; init; }
}