using AutoMapper;
using Microsoft.Extensions.Logging;
using VitalGest.Application.DTOs.Prescriptions;
using VitalGest.Application.Interfaces;
using VitalGest.Core.Entities;
using VitalGest.Core.Exceptions;
using VitalGest.Core.Interfaces;

namespace VitalGest.Application.Services;

public class PrescriptionService : IPrescriptionService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ILogger<PrescriptionService> _logger;

    public PrescriptionService(IUnitOfWork uow, IMapper mapper, ILogger<PrescriptionService> logger)
    {
        _uow = uow;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<PrescriptionResponse>> GetByPatientAsync(int patientId, int clinicId, CancellationToken ct = default)
    {
        var prescriptions = await _uow.Prescriptions.GetByPatientIdAsync(patientId, clinicId, ct);
        return _mapper.Map<IEnumerable<PrescriptionResponse>>(prescriptions);
    }

    public async Task<PrescriptionResponse> GetByIdAsync(int id, int clinicId, CancellationToken ct = default)
    {
        var prescription = await _uow.Prescriptions.GetByIdWithItemsAsync(id, clinicId, ct)
            ?? throw new NotFoundException("Prescrição", id);
        return _mapper.Map<PrescriptionResponse>(prescription);
    }

    public async Task<PrescriptionResponse> CreateAsync(int clinicId, int doctorUserId, CreatePrescriptionRequest request, CancellationToken ct = default)
    {
        if (request.Items == null || request.Items.Count == 0)
            throw new BusinessRuleException("Prescrição deve ter no mínimo 1 item.", "NO_ITEMS");

        var prescription = new Prescription
        {
            ClinicId = clinicId,
            PatientId = request.PatientId,
            DoctorUserId = doctorUserId,
            AppointmentId = request.AppointmentId,
            IssueDate = DateTime.UtcNow,
            ValidUntil = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)),
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.Prescriptions.AddAsync(prescription, ct);
        await _uow.SaveChangesAsync(ct);

        // Adiciona itens
        foreach (var itemRequest in request.Items)
        {
            var item = new PrescriptionItem
            {
                PrescriptionId = prescription.Id,
                MedicationName = itemRequest.MedicationName,
                Dosage = itemRequest.Dosage,
                Frequency = itemRequest.Frequency,
                Duration = itemRequest.Duration,
                Notes = itemRequest.Notes,
                OrderNumber = itemRequest.OrderNumber,
                CreatedAt = DateTime.UtcNow
            };
            await _uow.Prescriptions.AddItemAsync(item, ct);
        }

        await _uow.SaveChangesAsync(ct);

        _logger.LogInformation("Prescrição criada: {PrescriptionId}, Itens: {ItemCount}", prescription.Id, request.Items.Count);

        return _mapper.Map<PrescriptionResponse>(prescription);
    }

    public async Task DeleteAsync(int id, int clinicId, int doctorUserId, CancellationToken ct = default)
    {
        var prescription = await _uow.Prescriptions.GetByIdAsync(id, ct)
            ?? throw new NotFoundException("Prescrição", id);

        if (prescription.DoctorUserId != doctorUserId)
            throw new BusinessRuleException("Apenas o médico que criou a prescrição pode excluí-la.", "NOT_OWNER");

        await _uow.Prescriptions.DeleteAsync(prescription, ct);
        await _uow.SaveChangesAsync(ct);
    }

    public async Task<PrescriptionResponse> UpdateAsync(int id, int clinicId, CreatePrescriptionRequest request, CancellationToken ct = default)
    {
        var prescription = await _uow.Prescriptions.GetByIdWithItemsAsync(id, clinicId, ct)
            ?? throw new NotFoundException("Prescrição", id);

        prescription.Notes = request.Notes;

        await _uow.Prescriptions.UpdateAsync(prescription, ct);
        await _uow.SaveChangesAsync(ct);

        _logger.LogInformation("Prescrição atualizada: {PrescriptionId}", id);
        return _mapper.Map<PrescriptionResponse>(prescription);
    }
}