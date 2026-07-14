using System.ComponentModel.DataAnnotations;

namespace ClinicFlow.Api.Contracts.Specialties;

public sealed class UpdateSpecialtyRequest
{
    [Required(ErrorMessage = "O nome da especialidade é obrigatório.")]
    [MinLength(
        2,
        ErrorMessage = "O nome da especialidade deve ter pelo menos 2 caracteres."
    )]
    [MaxLength(
        100,
        ErrorMessage = "O nome da especialidade deve ter no máximo 100 caracteres."
    )]
    public string Name { get; init; } = string.Empty;
}