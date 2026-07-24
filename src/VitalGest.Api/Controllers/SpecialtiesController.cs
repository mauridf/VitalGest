using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VitalGest.Core.Interfaces;

namespace VitalGest.Api.Controllers;

/// <summary>
/// Controller de especialidades médicas.
/// </summary>
[Authorize]
public class SpecialtiesController : BaseApiController
{
    private readonly IUnitOfWork _uow;

    public SpecialtiesController(IUnitOfWork uow) => _uow = uow;

    /// <summary>
    /// Lista todas as especialidades ativas.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<object>), 200)]
    public async Task<IActionResult> GetAll()
    {
        var specialties = await _uow.Specialties.FindAsync(s => s.IsActive);
        return OkResponse(specialties);
    }
}