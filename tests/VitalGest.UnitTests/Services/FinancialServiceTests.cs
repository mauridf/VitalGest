using AutoMapper;
using Microsoft.Extensions.Logging;
using VitalGest.Application.DTOs.Financial;
using VitalGest.Application.Services;
using VitalGest.Core.Entities;
using VitalGest.Core.Enums;
using VitalGest.Core.Exceptions;
using VitalGest.Core.Interfaces;

namespace VitalGest.UnitTests.Services;

/// <summary>
/// Testes unitários para o FinancialService.
/// </summary>
public class FinancialServiceTests
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ILogger<FinancialService> _logger;
    private readonly FinancialService _sut;

    public FinancialServiceTests()
    {
        _uow = Substitute.For<IUnitOfWork>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<FinancialService>>();
        _sut = new FinancialService(_uow, _mapper, _logger);
    }

    [Fact]
    public async Task CreatePayment_WithZeroAmount_ShouldThrowBusinessRuleException()
    {
        var request = new CreatePaymentRequest(0, PaymentMethod.Cash);

        var act = () => _sut.CreatePaymentAsync(1, 1, request);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*Valor do pagamento deve ser maior que zero*");
    }

    [Fact]
    public async Task CreatePayment_WithDiscountExceedingAmount_ShouldThrowBusinessRuleException()
    {
        var request = new CreatePaymentRequest(100, PaymentMethod.Cash, Discount: 150);

        var act = () => _sut.CreatePaymentAsync(1, 1, request);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*Desconto não pode ser negativo nem exceder o valor total*");
    }

    [Fact]
    public async Task CreatePayment_WithNegativeDiscount_ShouldThrowBusinessRuleException()
    {
        var request = new CreatePaymentRequest(100, PaymentMethod.Cash, Discount: -10);

        var act = () => _sut.CreatePaymentAsync(1, 1, request);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*Desconto não pode ser negativo nem exceder o valor total*");
    }

    [Fact]
    public async Task CreatePayment_WithValidData_ShouldCalculateTotalAmount()
    {
        var request = new CreatePaymentRequest(200, PaymentMethod.CreditCard, Discount: 50);
        var payment = new Payment { Id = 1, Amount = 200, Discount = 50, TotalAmount = 150 };
        var response = new PaymentResponse(1, 200, 50, 150, DateTime.UtcNow, "CreditCard", "Paid", null, 1);

        _uow.Payments.AddAsync(Arg.Any<Payment>()).Returns(payment);
        _mapper.Map<PaymentResponse>(Arg.Any<Payment>()).Returns(response);

        var result = await _sut.CreatePaymentAsync(1, 1, request);

        result.Should().NotBeNull();
        result.TotalAmount.Should().Be(150);
        await _uow.Payments.Received(1).AddAsync(Arg.Any<Payment>());
        await _uow.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task CreateInvoice_WithZeroAmount_ShouldThrowBusinessRuleException()
    {
        var request = new CreateInvoiceRequest(1, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)), 0);

        var act = () => _sut.CreateInvoiceAsync(1, request);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*Valor da fatura deve ser maior que zero*");
    }

    [Fact]
    public async Task PayInvoice_WithAlreadyPaid_ShouldThrowBusinessRuleException()
    {
        var invoice = new Invoice { Id = 1, Status = InvoiceStatus.Paid };
        _uow.Invoices.GetByIdAsync(1).Returns(invoice);

        var act = () => _sut.PayInvoiceAsync(1, 1);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*Fatura já está paga*");
    }

    [Fact]
    public async Task PayInvoice_WithPending_ShouldMarkAsPaid()
    {
        var invoice = new Invoice { Id = 1, ClinicId = 1, Status = InvoiceStatus.Pending, TotalAmount = 500 };
        _uow.Invoices.GetByIdAsync(1).Returns(invoice);

        var result = await _sut.PayInvoiceAsync(1, 1);

        invoice.Status.Should().Be(InvoiceStatus.Paid);
        invoice.PaidAmount.Should().Be(500);
        await _uow.Invoices.Received(1).UpdateAsync(invoice);
        await _uow.Received(1).SaveChangesAsync();
    }
}
