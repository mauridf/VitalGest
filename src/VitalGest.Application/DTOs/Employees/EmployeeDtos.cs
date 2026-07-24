namespace VitalGest.Application.DTOs.Employees;

public record CreateEmployeeRequest(string Name, string Email, string? Phone, string? CPF, int? SpecialtyId);
public record UpdateEmployeeRequest(string Name, string Email, string? Phone, string? CPF, int? SpecialtyId, bool IsActive = true);
public record EmployeeResponse(int Id, string Name, string Email, string? Phone, string? CPF, string? SpecialtyName, bool IsActive, DateTime CreatedAt);
public record PositionResponse(int Id, string Name, string? Description);
