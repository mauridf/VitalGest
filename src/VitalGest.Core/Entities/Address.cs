namespace VitalGest.Core.Entities;

/// <summary>
/// Endereço (utilizado por Clínicas e Pacientes).
/// </summary>
public class Address
{
    public int Id { get; set; }

    /// <summary>Logradouro (rua, avenida, etc.)</summary>
    public string Street { get; set; } = string.Empty;

    /// <summary>Número do imóvel</summary>
    public string? Number { get; set; }

    /// <summary>Complemento (apartamento, bloco, sala, etc.)</summary>
    public string? Complement { get; set; }

    /// <summary>Bairro</summary>
    public string? Neighborhood { get; set; }

    /// <summary>Cidade</summary>
    public string City { get; set; } = string.Empty;

    /// <summary>Estado/UF</summary>
    public string State { get; set; } = string.Empty;

    /// <summary>CEP</summary>
    public string? ZipCode { get; set; }

    /// <summary>País (padrão: Brasil)</summary>
    public string Country { get; set; } = "Brasil";

    /// <summary>Latitude para geolocalização</summary>
    public decimal? Latitude { get; set; }

    /// <summary>Longitude para geolocalização</summary>
    public decimal? Longitude { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}