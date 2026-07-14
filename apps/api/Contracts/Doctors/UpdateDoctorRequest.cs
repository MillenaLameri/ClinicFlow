using System.ComponentModel.DataAnnotations;

namespace ClinicFlow.Api.Contracts.Doctors;

public sealed class UpdateDoctorRequest
{
    [Required(ErrorMessage = "O nome do médico é obrigatório.")]
    [MinLength(3, ErrorMessage = "O nome deve possuir pelo menos 3 caracteres.")]
    [MaxLength(150, ErrorMessage = "O nome deve possuir no máximo 150 caracteres.")]
    public string FullName { get; init; } = string.Empty;

    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "O e-mail informado é inválido.")]
    [MaxLength(150, ErrorMessage = "O e-mail deve possuir no máximo 150 caracteres.")]
    public string Email { get; init; } = string.Empty;

    [MaxLength(20, ErrorMessage = "O telefone deve possuir no máximo 20 caracteres.")]
    public string? Phone { get; init; }

    [Required(ErrorMessage = "A especialidade é obrigatória.")]
    public Guid SpecialtyId { get; init; }
}