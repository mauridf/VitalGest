-- V008: Cria tabela de pacientes
CREATE TABLE IF NOT EXISTS Patients (
    Id SERIAL PRIMARY KEY,
    ClinicId INT NOT NULL REFERENCES Clinics(Id) ON DELETE CASCADE,
    Name VARCHAR(255) NOT NULL,
    CPF VARCHAR(14) UNIQUE,
    RG VARCHAR(20),
    BirthDate DATE,
    Gender INT, -- Gender enum
    Phone VARCHAR(20) NOT NULL,
    SecondaryPhone VARCHAR(20),
    Email VARCHAR(255),
    AddressId INT REFERENCES Addresses(Id),
    BloodType INT, -- BloodType enum
    Allergies TEXT,
    MedicalNotes TEXT,
    EmergencyContact VARCHAR(255),
    EmergencyPhone VARCHAR(20),
    InsurancePlanId INT, -- FK adicionada depois
    InsuranceCardNumber VARCHAR(50),
    InsuranceExpiryDate DATE,
    ProfilePhotoUrl VARCHAR(500),
    IsActive BOOLEAN DEFAULT TRUE,
    CreatedAt TIMESTAMPTZ DEFAULT NOW(),
    UpdatedAt TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_patients_clinic ON Patients(ClinicId);
CREATE UNIQUE INDEX IF NOT EXISTS idx_patients_cpf ON Patients(CPF) WHERE CPF IS NOT NULL;
CREATE INDEX IF NOT EXISTS idx_patients_phone ON Patients(Phone);