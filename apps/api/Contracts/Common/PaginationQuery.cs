using System.ComponentModel.DataAnnotations;

namespace ClinicFlow.Api.Contracts.Common;

public sealed class PaginationQuery
{
    [Range(
        1,
        int.MaxValue,
        ErrorMessage =
            "A página deve ser maior ou igual a 1."
    )]
    public int Page { get; init; } = 1;

    [Range(
        1,
        100,
        ErrorMessage =
            "O tamanho da página deve estar entre 1 e 100."
    )]
    public int PageSize { get; init; } = 20;
}