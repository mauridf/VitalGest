using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using VitalGest.Application.DTOs.Documents;
using VitalGest.Application.Interfaces;
using VitalGest.Application.Services;
using VitalGest.Core.Entities;
using VitalGest.Core.Exceptions;
using VitalGest.Core.Interfaces;

namespace VitalGest.UnitTests.Services;

/// <summary>
/// Testes unitários para o DocumentService.
/// </summary>
public class DocumentServiceTests
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ILogger<DocumentService> _logger;
    private readonly IFileStorageService _fileStorage;
    private readonly DocumentService _sut;

    public DocumentServiceTests()
    {
        _uow = Substitute.For<IUnitOfWork>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<DocumentService>>();
        _fileStorage = Substitute.For<IFileStorageService>();
        _sut = new DocumentService(_uow, _mapper, _logger, _fileStorage);
    }

    [Fact]
    public async Task Upload_WithNullFile_ShouldThrowBusinessRuleException()
    {
        var act = () => _sut.UploadAsync(1, 1, null!);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*Arquivo é obrigatório*");
    }

    [Fact]
    public async Task Upload_WithEmptyFile_ShouldThrowBusinessRuleException()
    {
        var file = Substitute.For<IFormFile>();
        file.Length.Returns(0);

        var act = () => _sut.UploadAsync(1, 1, file);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*Arquivo é obrigatório*");
    }

    [Fact]
    public async Task Upload_WithFileExceedingMaxSize_ShouldThrowBusinessRuleException()
    {
        var file = Substitute.For<IFormFile>();
        file.Length.Returns(11 * 1024 * 1024);

        var act = () => _sut.UploadAsync(1, 1, file);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*Arquivo excede o tamanho máximo de 10MB*");
    }

    [Fact]
    public async Task Upload_WithValidFile_ShouldSaveAndReturnResponse()
    {
        var file = Substitute.For<IFormFile>();
        file.Length.Returns(1024);
        file.FileName.Returns("exam.pdf");
        file.ContentType.Returns("application/pdf");

        _fileStorage.SaveFileAsync(file, "1").Returns("/uploads/1/exam.pdf");
        var document = new Document { Id = 1, FileName = "exam.pdf", FileUrl = "/uploads/1/exam.pdf" };
        var response = new DocumentResponse(1, "exam.pdf", "/uploads/1/exam.pdf", 1024, "application/pdf", "Exam", DateTime.UtcNow, null, null);

        _mapper.Map<Document>(Arg.Any<Document>()).Returns(document);
        _mapper.Map<DocumentResponse>(Arg.Any<Document>()).Returns(response);

        var result = await _sut.UploadAsync(1, 1, file);

        result.Should().NotBeNull();
        result.FileName.Should().Be("exam.pdf");
        await _fileStorage.Received(1).SaveFileAsync(file, "1");
        await _uow.Documents.Received(1).AddAsync(Arg.Any<Document>());
        await _uow.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task Delete_ShouldRemovePhysicalFile()
    {
        var document = new Document { Id = 1, ClinicId = 1, FileUrl = "/uploads/1/exam.pdf" };
        _uow.Documents.GetByIdAsync(1).Returns(document);

        await _sut.DeleteAsync(1, 1);

        await _fileStorage.Received(1).DeleteFileAsync("/uploads/1/exam.pdf");
        await _uow.Documents.Received(1).DeleteAsync(document);
        await _uow.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task GetById_WithInvalidId_ShouldThrowNotFoundException()
    {
        _uow.Documents.GetByIdAsync(999).Returns((Document?)null);

        var act = () => _sut.GetByIdAsync(999, 1);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("*Documento*");
    }
}
