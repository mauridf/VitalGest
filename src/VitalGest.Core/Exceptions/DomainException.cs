namespace VitalGest.Core.Exceptions;

/// <summary>
/// Exceção base para erros de domínio.
/// Mapeada para HTTP 422 Unprocessable Entity.
/// </summary>
public class DomainException : Exception
{
    /// <summary>
    /// Código de erro para identificação no frontend.
    /// </summary>
    public string ErrorCode { get; }

    public DomainException(string message, string errorCode = "DOMAIN_ERROR")
        : base(message)
    {
        ErrorCode = errorCode;
    }

    public DomainException(string message, Exception innerException, string errorCode = "DOMAIN_ERROR")
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}