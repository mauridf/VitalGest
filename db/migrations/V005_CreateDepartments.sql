-- V005: Cria tabela de departamentos
CREATE TABLE IF NOT EXISTS Departments (
    Id SERIAL PRIMARY KEY,
    ClinicId INT NOT NULL REFERENCES Clinics(Id) ON DELETE CASCADE,
    Name VARCHAR(255) NOT NULL,
    Description TEXT,
    IsActive BOOLEAN DEFAULT TRUE,
    CreatedAt TIMESTAMPTZ DEFAULT NOW(),
    UpdatedAt TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_departments_clinic ON Departments(ClinicId);