namespace VitalGest.Core.Entities;

/// <summary>
/// Cargo/função do colaborador na clínica.
/// Ex: Médico, Enfermeiro, Atendente, Administrador, etc.
/// </summary>
public class Position
{
    public int Id { get; set; }

    /// <summary>Nome do cargo</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Descrição das responsabilidades</summary>
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public ICollection<ClinicUser> ClinicUsers { get; set; } = new List<ClinicUser>();
}