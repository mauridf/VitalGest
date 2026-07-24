using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VitalGest.Core.Interfaces;

namespace VitalGest.Api.Controllers;

/// <summary>
/// Controller de departamentos.
/// </summary>
[Authorize]
public class DepartmentsController : BaseApiController
{
    private readonly IUnitOfWork _uow;

    public DepartmentsController(IUnitOfWork uow) => _uow = uow;

    /// <summary>
    /// Lista departamentos da clínica.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<object>), 200)]
    public async Task<IActionResult> GetAll()
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var departments = await _uow.Departments.FindAsync(d => d.ClinicId == clinicId && d.IsActive);
        return OkResponse(departments);
    }
}