-- V016: Cria tabelas de prescrições
CREATE TABLE IF NOT EXISTS Prescriptions (
    Id SERIAL PRIMARY KEY,
    ClinicId INT NOT NULL REFERENCES Clinics(Id) ON DELETE CASCADE,
    PatientId INT NOT NULL REFERENCES Patients(Id) ON DELETE CASCADE,
    DoctorUserId INT NOT NULL REFERENCES Users(Id),
    AppointmentId INT REFERENCES Appointments(Id),
    IssueDate TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    ValidUntil DATE,
    Notes TEXT,
    IsDigitalSignature BOOLEAN DEFAULT FALSE,
    CreatedAt TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_prescriptions_patient ON Prescriptions(PatientId);
CREATE INDEX IF NOT EXISTS idx_prescriptions_doctor ON Prescriptions(DoctorUserId);

CREATE TABLE IF NOT EXISTS PrescriptionItems (
    Id SERIAL PRIMARY KEY,
    PrescriptionId INT NOT NULL REFERENCES Prescriptions(Id) ON DELETE CASCADE,
    MedicationName VARCHAR(255) NOT NULL,
    Dosage VARCHAR(100) NOT NULL,
    Frequency VARCHAR(100) NOT NULL,
    Duration VARCHAR(100),
    Notes TEXT,
    OrderNumber INT DEFAULT 1,
    CreatedAt TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_prescription_items_prescription ON PrescriptionItems(PrescriptionId);