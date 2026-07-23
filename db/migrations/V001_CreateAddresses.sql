-- V001: Cria tabela de endereços (base para Clinics e Patients)
CREATE TABLE IF NOT EXISTS Addresses (
    Id SERIAL PRIMARY KEY,
    Street VARCHAR(255) NOT NULL,
    Number VARCHAR(20),
    Complement VARCHAR(255),
    Neighborhood VARCHAR(255),
    City VARCHAR(255) NOT NULL,
    State VARCHAR(100) NOT NULL,
    ZipCode VARCHAR(10),
    Country VARCHAR(100) DEFAULT 'Brasil',
    Latitude DECIMAL(10,7),
    Longitude DECIMAL(10,7),
    CreatedAt TIMESTAMPTZ DEFAULT NOW()
);