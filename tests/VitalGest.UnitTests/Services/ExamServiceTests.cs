using AutoMapper;
using Microsoft.Extensions.Logging;
using VitalGest.Application.DTOs.Exams;
using VitalGest.Application.Services;
using VitalGest.Core.Entities;
using VitalGest.Core.Enums;
using VitalGest.Core.Exceptions;
using VitalGest.Core.Interfaces;

namespace VitalGest.UnitTests.Services;

/// <summary>
/// Testes unitários para o ExamService.
/// </summary>
public class ExamServiceTests
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ILogger<ExamService> _logger;
    private readonly ExamService _sut;

    public ExamServiceTests()
    {
        _uow = Substitute.For<IUnitOfWork>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<ExamService>>();
        _sut = new ExamService(_uow, _mapper, _logger);
    }

    [Fact]
    public async Task Create_ShouldSetStatusRequested()
    {
        var request = new CreateExamRequest(1, 1);
        var exam = new Exam { Id = 1, ClinicId = 1, DoctorUserId = 1 };
        var response = new ExamResponse(1, 1, "John", 1, "Blood Test", ExamStatus.Requested, "Requested", DateTime.UtcNow, null);

        _mapper.Map<Exam>(request).Returns(exam);
        _mapper.Map<ExamResponse>(Arg.Any<Exam>()).Returns(response);

        var result = await _sut.CreateAsync(1, 1, request);

        result.Should().NotBeNull();
        result.Status.Should().Be(ExamStatus.Requested);
        await _uow.Exams.Received(1).AddAsync(exam);
        await _uow.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task GetById_WithInvalidId_ShouldThrowNotFoundException()
    {
        _uow.Exams.GetByIdWithResultAsync(999, 1).Returns((Exam?)null);

        var act = () => _sut.GetByIdAsync(999, 1);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("*Exame*");
    }

    [Fact]
    public async Task UpdateStatus_WithWrongClinic_ShouldThrowBusinessRuleException()
    {
        var exam = new Exam { Id = 1, ClinicId = 2 };
        _uow.Exams.GetByIdAsync(1).Returns(exam);
        var request = new UpdateExamStatusRequest(ExamStatus.InAnalysis);

        var act = () => _sut.UpdateStatusAsync(1, 1, request);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*Exame não pertence a esta clínica*");
    }

    [Fact]
    public async Task AddResult_WithStatusRequested_ShouldThrowBusinessRuleException()
    {
        var exam = new Exam { Id = 1, ClinicId = 1, Status = ExamStatus.Requested };
        _uow.Exams.GetByIdAsync(1).Returns(exam);
        var request = new CreateExamResultRequest("Resultado normal");

        var act = () => _sut.AddResultAsync(1, 1, 1, request);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*Exame deve estar em análise para receber resultado*");
    }

    [Fact]
    public async Task AddResult_WithInAnalysis_ShouldSetStatusReady()
    {
        var exam = new Exam { Id = 1, ClinicId = 1, Status = ExamStatus.InAnalysis };
        _uow.Exams.GetByIdAsync(1).Returns(exam);
        var request = new CreateExamResultRequest("Resultado: normal");
        var result = new ExamResult { Id = 1, Summary = "Resultado: normal" };
        var response = new ExamResultResponse(1, "Resultado: normal", null, null, DateTime.UtcNow, "Dr. House", null);

        _uow.Exams.AddResultAsync(Arg.Any<ExamResult>()).Returns(result);
        _mapper.Map<ExamResultResponse>(Arg.Any<ExamResult>()).Returns(response);

        var resultResponse = await _sut.AddResultAsync(1, 1, 1, request);

        resultResponse.Should().NotBeNull();
        resultResponse.Summary.Should().Be("Resultado: normal");
        exam.Status.Should().Be(ExamStatus.Ready);
        await _uow.Exams.Received(1).AddResultAsync(Arg.Any<ExamResult>());
        await _uow.Received(1).SaveChangesAsync();
    }
}
