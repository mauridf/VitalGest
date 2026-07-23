namespace VitalGest.Core.Entities;

/// <summary>
/// Departamento/setor da clínica.
/// Ex: Cardiologia, Pediatria, Laboratório, Recepção, etc.
/// </summary>
public class Department
{
    public int Id { get; set; }

    /// <summary>Clínica a que pertence (tenant)</summary>
    public int ClinicId { get; set; }
    public Clinic Clinic { get; set; } = null!;

    /// <summary>Nome do departamento</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Descrição do departamento</summary>
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Relacionamentos
    public ICollection<ClinicUser> ClinicUsers { get; set; } = new List<ClinicUser>();
}