-- V004: Cria tabela de cargos
CREATE TABLE IF NOT EXISTS Positions (
    Id SERIAL PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    Description TEXT,
    IsActive BOOLEAN DEFAULT TRUE,
    CreatedAt TIMESTAMPTZ DEFAULT NOW()
);