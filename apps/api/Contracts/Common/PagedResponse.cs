namespace ClinicFlow.Api.Contracts.Common;

public sealed record PagedResponse<T>(
    IReadOnlyCollection<T> Items,
    int Page,
    int PageSize,
    int TotalItems,
    int TotalPages,
    bool HasPreviousPage,
    bool HasNextPage
)
{
    public static PagedResponse<T> Create(
        IReadOnlyCollection<T> items,
        int page,
        int pageSize,
        int totalItems
    )
    {
        var totalPages =
            totalItems == 0
                ? 0
                : (int)Math.Ceiling(
                    totalItems
                    / (double)pageSize
                );

        return new PagedResponse<T>(
            items,
            page,
            pageSize,
            totalItems,
            totalPages,
            page > 1,
            totalPages > 0
            && page < totalPages
        );
    }
}