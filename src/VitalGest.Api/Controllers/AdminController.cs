using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VitalGest.Core.Interfaces;

namespace VitalGest.Api.Controllers;

/// <summary>
/// Controller administrativo.
/// Acesso restrito a SuperAdmin.
/// </summary>
[Authorize(Policy = "SuperAdminOnly")]
[Route("api/admin")]
public class AdminController : BaseApiController
{
    private readonly IUnitOfWork _uow;

    public AdminController(IUnitOfWork uow) => _uow = uow;

    /// <summary>
    /// Lista todas as clínicas do sistema (ignora filtro multi-tenant).
    /// </summary>
    [HttpGet("clinics")]
    [ProducesResponseType(typeof(IEnumerable<object>), 200)]
    public async Task<IActionResult> GetAllClinics()
    {
        var clinics = await _uow.Clinics.FindAsync(c => true); // Ignora tenant filter
        return OkResponse(clinics);
    }

    /// <summary>
    /// Lista todos os usuários do sistema.
    /// </summary>
    [HttpGet("users")]
    [ProducesResponseType(typeof(IEnumerable<object>), 200)]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _uow.Users.GetAllAsync();
        return OkResponse(users);
    }

    /// <summary>
    /// Ativa/desativa uma clínica.
    /// </summary>
    [HttpPatch("clinics/{id:int}/toggle")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> ToggleClinic(int id)
    {
        var clinic = await _uow.Clinics.GetByIdAsync(id)
            ?? throw new VitalGest.Core.Exceptions.NotFoundException("Clínica", id);

        clinic.IsActive = !clinic.IsActive;
        clinic.UpdatedAt = DateTime.UtcNow;

        await _uow.Clinics.UpdateAsync(clinic);
        await _uow.SaveChangesAsync();

        return OkResponse(new { clinic.IsActive }, $"Clínica {(clinic.IsActive ? "ativada" : "desativada")}.");
    }
}