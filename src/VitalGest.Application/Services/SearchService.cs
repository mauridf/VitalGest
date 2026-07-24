using VitalGest.Application.DTOs.Search;
using VitalGest.Application.Interfaces;
using VitalGest.Core.Interfaces;

namespace VitalGest.Application.Services;

public class SearchService : ISearchService
{
    private readonly IUnitOfWork _uow;

    public SearchService(IUnitOfWork uow) => _uow = uow;

    public async Task<SearchResponse> SearchAsync(int clinicId, SearchRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Query) || request.Query.Length < 2)
            return new SearchResponse(Enumerable.Empty<SearchResultItem>(), 0, request.Query);

        var results = new List<SearchResultItem>();

        // Busca pacientes
        var patients = await _uow.Patients.SearchByNameAsync(request.Query, clinicId, ct);
        results.AddRange(patients.Select(p => new SearchResultItem("Paciente", p.Id, p.Name, p.CPF, p.Phone)));

        return new SearchResponse(results, results.Count, request.Query);
    }
}