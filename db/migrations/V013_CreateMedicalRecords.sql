-- V013: Cria tabelas de prontuário eletrônico
CREATE TABLE IF NOT EXISTS MedicalRecords (
    Id SERIAL PRIMARY KEY,
    PatientId INT NOT NULL REFERENCES Patients(Id) ON DELETE CASCADE,
    ClinicId INT NOT NULL REFERENCES Clinics(Id) ON DELETE CASCADE,
    CreatedAt TIMESTAMPTZ DEFAULT NOW(),
    UpdatedAt TIMESTAMPTZ DEFAULT NOW(),
    UNIQUE(PatientId, ClinicId)
);

CREATE INDEX IF NOT EXISTS idx_medical_records_patient ON MedicalRecords(PatientId);

CREATE TABLE IF NOT EXISTS MedicalRecordEntries (
    Id SERIAL PRIMARY KEY,
    MedicalRecordId INT NOT NULL REFERENCES MedicalRecords(Id) ON DELETE CASCADE,
    AppointmentId INT REFERENCES Appointments(Id),
    DoctorUserId INT NOT NULL REFERENCES Users(Id),
    EntryType INT NOT NULL, -- MedicalRecordEntryType
    Description TEXT NOT NULL,
    IsConfidential BOOLEAN DEFAULT FALSE,
    CreatedAt TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_record_entries_record ON MedicalRecordEntries(MedicalRecordId);
CREATE INDEX IF NOT EXISTS idx_record_entries_doctor ON MedicalRecordEntries(DoctorUserId);
CREATE INDEX IF NOT EXISTS idx_record_entries_date ON MedicalRecordEntries(CreatedAt DESC);