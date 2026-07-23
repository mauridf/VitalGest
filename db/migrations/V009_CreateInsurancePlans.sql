-- V009: Cria tabela de planos de saúde
CREATE TABLE IF NOT EXISTS InsurancePlans (
    Id SERIAL PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    CNPJ VARCHAR(18),
    Phone VARCHAR(20),
    Email VARCHAR(255),
    ContractType INT DEFAULT 1, -- InsuranceContractType
    IsActive BOOLEAN DEFAULT TRUE,
    CreatedAt TIMESTAMPTZ DEFAULT NOW(),
    UpdatedAt TIMESTAMPTZ DEFAULT NOW()
);

-- Adiciona FK em Patients
ALTER TABLE Patients 
ADD CONSTRAINT fk_patients_insurance_plan 
FOREIGN KEY (InsurancePlanId) REFERENCES InsurancePlans(Id);