using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using VitalGest.Application.DTOs.Documents;
using VitalGest.Application.Interfaces;
using VitalGest.Core.Entities;
using VitalGest.Core.Enums;
using VitalGest.Core.Exceptions;
using VitalGest.Core.Interfaces;

namespace VitalGest.Application.Services;

public class DocumentService : IDocumentService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ILogger<DocumentService> _logger;
    private readonly IFileStorageService _fileStorage;

    public DocumentService(IUnitOfWork uow, IMapper mapper, ILogger<DocumentService> logger, IFileStorageService fileStorage)
    {
        _uow = uow;
        _mapper = mapper;
        _logger = logger;
        _fileStorage = fileStorage;
    }

    public async Task<IEnumerable<DocumentResponse>> GetByPatientAsync(int patientId, int clinicId, CancellationToken ct = default)
    {
        var docs = await _uow.Documents.FindAsync(d => d.PatientId == patientId && d.ClinicId == clinicId, ct);
        return _mapper.Map<IEnumerable<DocumentResponse>>(docs);
    }

    public async Task<DocumentResponse> GetByIdAsync(int id, int clinicId, CancellationToken ct = default)
    {
        var doc = await _uow.Documents.GetByIdAsync(id, ct)
            ?? throw new NotFoundException("Documento", id);
        return _mapper.Map<DocumentResponse>(doc);
    }

    public async Task<DocumentResponse> UploadAsync(int clinicId, int uploadedById, IFormFile file, int? patientId = null, int? appointmentId = null, int? examId = null, int documentType = 5, CancellationToken ct = default)
    {
        if (file == null || file.Length == 0)
            throw new BusinessRuleException("Arquivo é obrigatório.", "FILE_REQUIRED");

        // Valida tamanho máximo (10MB)
        const long maxFileSize = 10 * 1024 * 1024;
        if (file.Length > maxFileSize)
            throw new BusinessRuleException("Arquivo excede o tamanho máximo de 10MB.", "FILE_TOO_LARGE");

        // Salva arquivo
        var fileUrl = await _fileStorage.SaveFileAsync(file, clinicId.ToString(), ct);

        var document = new Document
        {
            ClinicId = clinicId,
            PatientId = patientId,
            AppointmentId = appointmentId,
            ExamId = examId,
            FileName = file.FileName,
            FileUrl = fileUrl,
            FileSize = file.Length,
            ContentType = file.ContentType,
            DocumentType = (DocumentType)documentType,
            UploadedById = uploadedById,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.Documents.AddAsync(document, ct);
        await _uow.SaveChangesAsync(ct);

        return _mapper.Map<DocumentResponse>(document);
    }

    public async Task DeleteAsync(int id, int clinicId, CancellationToken ct = default)
    {
        var doc = await _uow.Documents.GetByIdAsync(id, ct)
            ?? throw new NotFoundException("Documento", id);

        // Remove arquivo físico
        await _fileStorage.DeleteFileAsync(doc.FileUrl, ct);

        await _uow.Documents.DeleteAsync(doc, ct);
        await _uow.SaveChangesAsync(ct);
    }
}