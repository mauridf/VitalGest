-- V010: Cria tabela de coberturas de planos
CREATE TABLE IF NOT EXISTS InsuranceCoverages (
    Id SERIAL PRIMARY KEY,
    InsurancePlanId INT NOT NULL REFERENCES InsurancePlans(Id) ON DELETE CASCADE,
    ExamTypeId INT, -- FK adicionada depois
    SpecialtyId INT REFERENCES Specialties(Id),
    ProcedureType INT,
    CoveragePercent DECIMAL(5,2) DEFAULT 100.00,
    RequiresAuthorization BOOLEAN DEFAULT FALSE,
    MaxSessions INT,
    CreatedAt TIMESTAMPTZ DEFAULT NOW(),
    UpdatedAt TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_insurance_coverages_plan ON InsuranceCoverages(InsurancePlanId);