using AutoMapper;
using Microsoft.Extensions.Logging;
using VitalGest.Application.DTOs.Common;
using VitalGest.Application.DTOs.Financial;
using VitalGest.Application.Interfaces;
using VitalGest.Core.Entities;
using VitalGest.Core.Enums;
using VitalGest.Core.Exceptions;
using VitalGest.Core.Interfaces;

namespace VitalGest.Application.Services;

public class FinancialService : IFinancialService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ILogger<FinancialService> _logger;

    public FinancialService(IUnitOfWork uow, IMapper mapper, ILogger<FinancialService> logger)
    {
        _uow = uow;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PagedResponse<PaymentResponse>> GetPaymentsAsync(int clinicId, PagedRequest request, CancellationToken ct = default)
    {
        var payments = await _uow.Payments.GetPagedAsync(request.Page, request.PageSize, p => p.ClinicId == clinicId, ct);
        var count = await _uow.Payments.CountAsync(p => p.ClinicId == clinicId, ct);
        return PagedResponse.Create(_mapper.Map<IEnumerable<PaymentResponse>>(payments), request.Page, request.PageSize, count);
    }

    public async Task<PaymentResponse> GetPaymentByIdAsync(int id, int clinicId, CancellationToken ct = default)
    {
        var payment = await _uow.Payments.GetByIdAsync(id, ct)
            ?? throw new NotFoundException("Pagamento", id);
        return _mapper.Map<PaymentResponse>(payment);
    }

    public async Task<PaymentResponse> CreatePaymentAsync(int clinicId, int receivedById, CreatePaymentRequest request, CancellationToken ct = default)
    {
        if (request.Amount <= 0)
            throw new BusinessRuleException("Valor do pagamento deve ser maior que zero.", "INVALID_AMOUNT");
        if (request.Discount < 0 || request.Discount > request.Amount)
            throw new BusinessRuleException("Desconto não pode ser negativo nem exceder o valor total.", "INVALID_DISCOUNT");

        var totalAmount = request.Amount - request.Discount;

        var payment = new Payment
        {
            ClinicId = clinicId,
            PatientId = request.PatientId,
            AppointmentId = request.AppointmentId,
            Amount = request.Amount,
            Discount = request.Discount,
            TotalAmount = totalAmount,
            PaymentDate = DateTime.UtcNow,
            PaymentMethod = request.PaymentMethod,
            Status = PaymentStatus.Paid,
            Installments = request.Installments,
            Notes = request.Notes,
            ReceivedById = receivedById,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.Payments.AddAsync(payment, ct);
        await _uow.SaveChangesAsync(ct);

        _logger.LogInformation("Pagamento registrado: {PaymentId}, Valor: {Amount}", payment.Id, totalAmount);

        return _mapper.Map<PaymentResponse>(payment);
    }

    public async Task<PagedResponse<InvoiceResponse>> GetInvoicesAsync(int clinicId, PagedRequest request, CancellationToken ct = default)
    {
        var invoices = await _uow.Invoices.GetPagedAsync(request.Page, request.PageSize, i => i.ClinicId == clinicId, ct);
        var count = await _uow.Invoices.CountAsync(i => i.ClinicId == clinicId, ct);
        return PagedResponse.Create(_mapper.Map<IEnumerable<InvoiceResponse>>(invoices), request.Page, request.PageSize, count);
    }

    public async Task<InvoiceResponse> CreateInvoiceAsync(int clinicId, CreateInvoiceRequest request, CancellationToken ct = default)
    {
        if (request.TotalAmount <= 0)
            throw new BusinessRuleException("Valor da fatura deve ser maior que zero.", "INVALID_AMOUNT");

        // Gera número da fatura (YYYYMM-XXXXX)
        var invoiceNumber = $"{DateTime.UtcNow:yyyyMM}-{Guid.NewGuid().ToString()[..5].ToUpper()}";

        var invoice = new Invoice
        {
            ClinicId = clinicId,
            PatientId = request.PatientId,
            InvoiceNumber = invoiceNumber,
            IssueDate = DateTime.UtcNow,
            DueDate = request.DueDate,
            TotalAmount = request.TotalAmount,
            PaidAmount = 0,
            Status = InvoiceStatus.Pending,
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.Invoices.AddAsync(invoice, ct);
        await _uow.SaveChangesAsync(ct);

        return _mapper.Map<InvoiceResponse>(invoice);
    }

    public async Task<InvoiceResponse> PayInvoiceAsync(int id, int clinicId, CancellationToken ct = default)
    {
        var invoice = await _uow.Invoices.GetByIdAsync(id, ct)
            ?? throw new NotFoundException("Fatura", id);

        if (invoice.Status == InvoiceStatus.Paid)
            throw new BusinessRuleException("Fatura já está paga.", "ALREADY_PAID");

        invoice.PaidAmount = invoice.TotalAmount;
        invoice.Status = InvoiceStatus.Paid;
        invoice.UpdatedAt = DateTime.UtcNow;

        await _uow.Invoices.UpdateAsync(invoice, ct);
        await _uow.SaveChangesAsync(ct);

        return _mapper.Map<InvoiceResponse>(invoice);
    }
}