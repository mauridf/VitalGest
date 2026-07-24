namespace VitalGest.Application.DTOs.Search;

public record SearchRequest(string Query, string? Type = null, int Page = 1, int PageSize = 20);
public record SearchResponse(IEnumerable<SearchResultItem> Results, int TotalResults, string Query);
public record SearchResultItem(string Type, int Id, string Title, string? Subtitle, string? Extra);