using VitalGest.Application.DTOs.Search;

namespace VitalGest.Application.Interfaces;

public interface ISearchService
{
    Task<SearchResponse> SearchAsync(int clinicId, SearchRequest request, CancellationToken ct = default);
}