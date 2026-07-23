-- V007: Cria tabela de vínculo usuário-clínica (colaboradores)
CREATE TABLE IF NOT EXISTS ClinicUsers (
    Id SERIAL PRIMARY KEY,
    UserId INT NOT NULL REFERENCES Users(Id) ON DELETE CASCADE,
    ClinicId INT NOT NULL REFERENCES Clinics(Id) ON DELETE CASCADE,
    PositionId INT NOT NULL REFERENCES Positions(Id),
    DepartmentId INT REFERENCES Departments(Id),
    ProfessionalDocument VARCHAR(50),
    ProfessionalDocumentType VARCHAR(20),
    ProfessionalDocumentUF VARCHAR(2),
    IsActive BOOLEAN DEFAULT TRUE,
    HireDate TIMESTAMPTZ,
    CreatedAt TIMESTAMPTZ DEFAULT NOW(),
    UpdatedAt TIMESTAMPTZ DEFAULT NOW(),
    UNIQUE(UserId, ClinicId)
);

CREATE INDEX IF NOT EXISTS idx_clinic_users_clinic ON ClinicUsers(ClinicId);
CREATE INDEX IF NOT EXISTS idx_clinic_users_position ON ClinicUsers(PositionId);
CREATE INDEX IF NOT EXISTS idx_clinic_users_department ON ClinicUsers(DepartmentId);