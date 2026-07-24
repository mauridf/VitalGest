using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VitalGest.Application.DTOs.Common;
using VitalGest.Application.DTOs.Financial;
using VitalGest.Application.Interfaces;

namespace VitalGest.Api.Controllers;

/// <summary>
/// Controller de gestão financeira (pagamentos e faturas).
/// </summary>
[Authorize]
public class FinancialController : BaseApiController
{
    private readonly IFinancialService _financialService;

    public FinancialController(IFinancialService financialService)
    {
        _financialService = financialService;
    }

    /// <summary>
    /// Lista pagamentos da clínica com paginação.
    /// </summary>
    [HttpGet("payments")]
    [ProducesResponseType(typeof(PagedResponse<PaymentResponse>), 200)]
    public async Task<IActionResult> GetPayments([FromQuery] PagedRequest request)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var result = await _financialService.GetPaymentsAsync(clinicId, request);
        return OkPagedResponse(result);
    }

    /// <summary>
    /// Registra um novo pagamento.
    /// </summary>
    [HttpPost("payments")]
    [ProducesResponseType(typeof(PaymentResponse), 201)]
    public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest request)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var userId = GetUserId();
        var result = await _financialService.CreatePaymentAsync(clinicId, userId, request);
        return CreatedAtAction(nameof(GetPaymentById), new { id = result.Id }, new
        {
            Success = true,
            Message = "Pagamento registrado com sucesso.",
            Data = result
        });
    }

    /// <summary>
    /// Obtém detalhes de um pagamento.
    /// </summary>
    [HttpGet("payments/{id:int}")]
    [ProducesResponseType(typeof(PaymentResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetPaymentById(int id)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var result = await _financialService.GetPaymentByIdAsync(id, clinicId);
        return OkResponse(result);
    }

    /// <summary>
    /// Lista faturas da clínica com paginação.
    /// </summary>
    [HttpGet("invoices")]
    [ProducesResponseType(typeof(PagedResponse<InvoiceResponse>), 200)]
    public async Task<IActionResult> GetInvoices([FromQuery] PagedRequest request)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var result = await _financialService.GetInvoicesAsync(clinicId, request);
        return OkPagedResponse(result);
    }

    /// <summary>
    /// Gera uma nova fatura.
    /// </summary>
    [HttpPost("invoices")]
    [ProducesResponseType(typeof(InvoiceResponse), 201)]
    public async Task<IActionResult> CreateInvoice([FromBody] CreateInvoiceRequest request)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var result = await _financialService.CreateInvoiceAsync(clinicId, request);
        return CreatedAtAction(nameof(GetInvoices), null, new
        {
            Success = true,
            Message = "Fatura gerada com sucesso.",
            Data = result
        });
    }

    /// <summary>
    /// Registra o pagamento de uma fatura.
    /// </summary>
    [HttpPatch("invoices/{id:int}/pay")]
    [ProducesResponseType(typeof(InvoiceResponse), 200)]
    public async Task<IActionResult> PayInvoice(int id)
    {
        var clinicId = GetClinicId() ?? throw new UnauthorizedAccessException("ClinicId não encontrado.");
        var result = await _financialService.PayInvoiceAsync(id, clinicId);
        return OkResponse(result, "Fatura paga com sucesso.");
    }
}