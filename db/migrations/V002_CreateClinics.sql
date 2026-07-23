-- V002: Cria tabela de clínicas (tenant principal)
CREATE TABLE IF NOT EXISTS Clinics (
    Id SERIAL PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    CorporateName VARCHAR(255) NOT NULL,
    CNPJ VARCHAR(18) NOT NULL UNIQUE,
    Description TEXT,
    LogoUrl VARCHAR(500),
    Phone VARCHAR(20) NOT NULL,
    SecondaryPhone VARCHAR(20),
    Email VARCHAR(255) NOT NULL,
    Website VARCHAR(500),
    AddressId INT REFERENCES Addresses(Id),
    IsActive BOOLEAN DEFAULT TRUE,
    OpeningHours JSONB,
    CreatedAt TIMESTAMPTZ DEFAULT NOW(),
    UpdatedAt TIMESTAMPTZ DEFAULT NOW()
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_clinics_cnpj ON Clinics(CNPJ);