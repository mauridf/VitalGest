-- V006: Cria tabela de especialidades
CREATE TABLE IF NOT EXISTS Specialties (
    Id SERIAL PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    Description TEXT,
    IsActive BOOLEAN DEFAULT TRUE,
    CreatedAt TIMESTAMPTZ DEFAULT NOW()
);