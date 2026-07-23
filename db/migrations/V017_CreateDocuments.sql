-- V017: Cria tabela de documentos
CREATE TABLE IF NOT EXISTS Documents (
    Id SERIAL PRIMARY KEY,
    ClinicId INT NOT NULL REFERENCES Clinics(Id) ON DELETE CASCADE,
    PatientId INT REFERENCES Patients(Id),
    AppointmentId INT REFERENCES Appointments(Id),
    ExamId INT REFERENCES Exams(Id),
    FileName VARCHAR(255) NOT NULL,
    FileUrl VARCHAR(500) NOT NULL,
    FileSize BIGINT,
    ContentType VARCHAR(100),
    DocumentType INT NOT NULL, -- DocumentType enum
    UploadedById INT REFERENCES Users(Id),
    CreatedAt TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_documents_patient ON Documents(PatientId);
CREATE INDEX IF NOT EXISTS idx_documents_clinic ON Documents(ClinicId);
CREATE INDEX IF NOT EXISTS idx_documents_appointment ON Documents(AppointmentId);