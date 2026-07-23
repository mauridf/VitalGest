namespace VitalGest.Core.Entities;

/// <summary>
/// Especialidade médica/odontológica.
/// Ex: Cardiologia, Dermatologia, Ortodontia, etc.
/// </summary>
public class Specialty
{
    public int Id { get; set; }

    /// <summary>Nome da especialidade</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Descrição da especialidade</summary>
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<InsuranceCoverage> InsuranceCoverages { get; set; } = new List<InsuranceCoverage>();
}