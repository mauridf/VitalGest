using VitalGest.Application.DTOs.Financial;
using VitalGest.Application.DTOs.Common;

namespace VitalGest.Application.Interfaces;

public interface IFinancialService
{
    Task<PagedResponse<PaymentResponse>> GetPaymentsAsync(int clinicId, PagedRequest request, CancellationToken ct = default);
    Task<PaymentResponse> GetPaymentByIdAsync(int id, int clinicId, CancellationToken ct = default);
    Task<PaymentResponse> CreatePaymentAsync(int clinicId, int receivedById, CreatePaymentRequest request, CancellationToken ct = default);
    Task<PagedResponse<InvoiceResponse>> GetInvoicesAsync(int clinicId, PagedRequest request, CancellationToken ct = default);
    Task<InvoiceResponse> CreateInvoiceAsync(int clinicId, CreateInvoiceRequest request, CancellationToken ct = default);
    Task<InvoiceResponse> PayInvoiceAsync(int id, int clinicId, CancellationToken ct = default);
}