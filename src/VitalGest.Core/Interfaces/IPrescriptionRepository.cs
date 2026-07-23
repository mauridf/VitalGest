using VitalGest.Core.Entities;

namespace VitalGest.Core.Interfaces;

/// <summary>
/// Repositório especializado para Prescrições.
/// </summary>
public interface IPrescriptionRepository : IRepository<Prescription>
{
    /// <summary>Busca prescrições do paciente</summary>
    Task<IEnumerable<Prescription>> GetByPatientIdAsync(int patientId, int clinicId, CancellationToken cancellationToken = default);

    /// <summary>Busca prescrição com itens</summary>
    Task<Prescription?> GetByIdWithItemsAsync(int prescriptionId, int clinicId, CancellationToken cancellationToken = default);

    /// <summary>Adiciona item à prescrição</summary>
    Task<PrescriptionItem> AddItemAsync(PrescriptionItem item, CancellationToken cancellationToken = default);

    /// <summary>Remove item da prescrição</summary>
    Task RemoveItemAsync(int itemId, CancellationToken cancellationToken = default);
}