using VitalGest.Core.Enums;

namespace VitalGest.Application.DTOs.Financial;

public record CreatePaymentRequest(decimal Amount, PaymentMethod PaymentMethod, int? PatientId = null, int? AppointmentId = null, decimal Discount = 0, int Installments = 1, string? Notes = null);
public record PaymentResponse(int Id, decimal Amount, decimal Discount, decimal TotalAmount, DateTime PaymentDate, string PaymentMethod, string Status, int? PatientName, int Installments);
public record CreateInvoiceRequest(int PatientId, DateOnly DueDate, decimal TotalAmount, string? Notes = null);
public record InvoiceResponse(int Id, string InvoiceNumber, int PatientId, string PatientName, decimal TotalAmount, decimal PaidAmount, string Status, DateOnly DueDate, DateTime IssueDate);