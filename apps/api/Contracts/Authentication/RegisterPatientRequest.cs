using System.ComponentModel.DataAnnotations;

namespace ClinicFlow.Api.Contracts.Authentication;

public sealed class RegisterPatientRequest
{
    [Required(
        ErrorMessage = "O nome é obrigatório."
    )]
    [MinLength(
        3,
        ErrorMessage =
            "O nome deve possuir pelo menos 3 caracteres."
    )]
    [MaxLength(
        150,
        ErrorMessage =
            "O nome deve possuir no máximo 150 caracteres."
    )]
    public string FullName { get; init; } =
        string.Empty;

    [Required(
        ErrorMessage = "O CPF é obrigatório."
    )]
    [MaxLength(
        14,
        ErrorMessage = "O CPF informado é inválido."
    )]
    public string Cpf { get; init; } =
        string.Empty;

    public DateOnly BirthDate { get; init; }

    [Required(
        ErrorMessage = "O e-mail é obrigatório."
    )]
    [EmailAddress(
        ErrorMessage = "O e-mail informado é inválido."
    )]
    [MaxLength(
        150,
        ErrorMessage =
            "O e-mail deve possuir no máximo 150 caracteres."
    )]
    public string Email { get; init; } =
        string.Empty;

    [MaxLength(
        20,
        ErrorMessage = "O telefone informado é inválido."
    )]
    public string? Phone { get; init; }

    [Required(
        ErrorMessage = "A senha é obrigatória."
    )]
    [MinLength(
        8,
        ErrorMessage =
            "A senha deve possuir pelo menos 8 caracteres."
    )]
    [MaxLength(
        128,
        ErrorMessage =
            "A senha deve possuir no máximo 128 caracteres."
    )]
    public string Password { get; init; } =
        string.Empty;

    [Required(
        ErrorMessage =
            "A confirmação da senha é obrigatória."
    )]
    [Compare(
        nameof(Password),
        ErrorMessage =
            "A confirmação da senha não corresponde à senha informada."
    )]
    public string ConfirmPassword { get; init; } =
        string.Empty;
}