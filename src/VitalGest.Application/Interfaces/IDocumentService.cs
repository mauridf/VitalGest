using VitalGest.Application.DTOs.Documents;
using Microsoft.AspNetCore.Http;

namespace VitalGest.Application.Interfaces;

public interface IDocumentService
{
    Task<IEnumerable<DocumentResponse>> GetByPatientAsync(int patientId, int clinicId, CancellationToken ct = default);
    Task<DocumentResponse> GetByIdAsync(int id, int clinicId, CancellationToken ct = default);
    Task<DocumentResponse> UploadAsync(int clinicId, int uploadedById, IFormFile file, int? patientId = null, int? appointmentId = null, int? examId = null, int documentType = 5, CancellationToken ct = default);
    Task DeleteAsync(int id, int clinicId, CancellationToken ct = default);
    Task<IEnumerable<DocumentResponse>> GetByAppointmentAsync(int appointmentId, int clinicId, CancellationToken ct = default);
}