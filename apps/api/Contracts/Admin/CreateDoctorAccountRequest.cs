using System.ComponentModel.DataAnnotations;

namespace ClinicFlow.Api.Contracts.Admin;

public sealed class CreateDoctorAccountRequest
{
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