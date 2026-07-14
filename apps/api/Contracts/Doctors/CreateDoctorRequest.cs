using System.ComponentModel.DataAnnotations;

namespace ClinicFlow.Api.Contracts.Doctors;

public sealed class CreateDoctorRequest
{
    [Required(ErrorMessage = "O nome do médico é obrigatório.")]
    [MinLength(3, ErrorMessage = "O nome deve possuir pelo menos 3 caracteres.")]
    [MaxLength(150, ErrorMessage = "O nome deve possuir no máximo 150 caracteres.")]
    public string FullName { get; init; } = string.Empty;

    [Required(ErrorMessage = "O CRM é obrigatório.")]
    [MaxLength(20, ErrorMessage = "O CRM deve possuir no máximo 20 caracteres.")]
    public string CrmNumber { get; init; } = string.Empty;

    [Required(ErrorMessage = "O estado do CRM é obrigatório.")]
    [RegularExpression(
        "^[A-Za-z]{2}$",
        ErrorMessage = "O estado do CRM deve possuir exatamente 2 letras."
    )]
    public string CrmState { get; init; } = string.Empty;

    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "O e-mail informado é inválido.")]
    [MaxLength(150, ErrorMessage = "O e-mail deve possuir no máximo 150 caracteres.")]
    public string Email { get; init; } = string.Empty;

    [MaxLength(20, ErrorMessage = "O telefone deve possuir no máximo 20 caracteres.")]
    public string? Phone { get; init; }

    [Required(ErrorMessage = "A especialidade é obrigatória.")]
    public Guid SpecialtyId { get; init; }
}