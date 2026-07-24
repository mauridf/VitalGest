namespace VitalGest.Application.DTOs.Common;

// ===== Endereço =====

public record CreateAddressRequest(
    string Street,
    string City,
    string State,
    string? Number = null,
    string? Complement = null,
    string? Neighborhood = null,
    string? ZipCode = null,
    string Country = "Brasil",
    decimal? Latitude = null,
    decimal? Longitude = null
);

public record AddressResponse(
    int Id,
    string Street,
    string? Number,
    string? Complement,
    string? Neighborhood,
    string City,
    string State,
    string? ZipCode,
    string Country
);

// ===== Paginação =====

public record PagedRequest(
    int Page = 1,
    int PageSize = 20
);

public record PagedResponse<T>(
    IEnumerable<T> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages,
    bool HasNextPage,
    bool HasPreviousPage
);

public static class PagedResponse
{
    public static PagedResponse<T> Create<T>(
        IEnumerable<T> items,
        int page,
        int pageSize,
        int totalCount)
    {
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        return new PagedResponse<T>(
            items,
            page,
            pageSize,
            totalCount,
            totalPages,
            page < totalPages,
            page > 1
        );
    }
}