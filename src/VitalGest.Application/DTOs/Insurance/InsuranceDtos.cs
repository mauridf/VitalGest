using VitalGest.Core.Enums;

namespace VitalGest.Application.DTOs.Insurance;

public record CreateInsurancePlanRequest(string Name, InsuranceContractType ContractType = InsuranceContractType.Private, string? CNPJ = null, string? Phone = null, string? Email = null);
public record UpdateInsurancePlanRequest(string Name, string? Phone = null, string? Email = null, bool IsActive = true);
public record InsurancePlanResponse(int Id, string Name, string? CNPJ, string? Phone, string? Email, string ContractType, bool IsActive);
public record InsurancePlanSimpleResponse(int Id, string Name);
public record CreateInsuranceCoverageRequest(int? ExamTypeId = null, int? SpecialtyId = null, int? ProcedureType = null, decimal CoveragePercent = 100, bool RequiresAuthorization = false, int? MaxSessions = null);
public record InsuranceCoverageResponse(int Id, string? ExamTypeName, string? SpecialtyName, decimal CoveragePercent, bool RequiresAuthorization, int? MaxSessions);