namespace VitalGest.Core.Exceptions;

/// <summary>
/// Exceção lançada quando uma regra de negócio é violada.
/// Mapeada para HTTP 409 Conflict.
/// </summary>
public class BusinessRuleException : DomainException
{
    public BusinessRuleException(string message, string errorCode = "BUSINESS_RULE_VIOLATION")
        : base(message, errorCode)
    {
    }
}