-- V015: Cria tabelas de exames e resultados
CREATE TABLE IF NOT EXISTS Exams (
    Id SERIAL PRIMARY KEY,
    ClinicId INT NOT NULL REFERENCES Clinics(Id) ON DELETE CASCADE,
    PatientId INT NOT NULL REFERENCES Patients(Id) ON DELETE CASCADE,
    DoctorUserId INT NOT NULL REFERENCES Users(Id),
    ExamTypeId INT NOT NULL REFERENCES ExamTypes(Id),
    AppointmentId INT REFERENCES Appointments(Id),
    RequestDate TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    Status INT DEFAULT 1, -- ExamStatus
    Notes TEXT,
    ClinicalHistory TEXT,
    CreatedAt TIMESTAMPTZ DEFAULT NOW(),
    UpdatedAt TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_exams_patient ON Exams(PatientId);
CREATE INDEX IF NOT EXISTS idx_exams_doctor ON Exams(DoctorUserId);
CREATE INDEX IF NOT EXISTS idx_exams_status ON Exams(Status);
CREATE INDEX IF NOT EXISTS idx_exams_type ON Exams(ExamTypeId);

CREATE TABLE IF NOT EXISTS ExamResults (
    Id SERIAL PRIMARY KEY,
    ExamId INT NOT NULL REFERENCES Exams(Id) ON DELETE CASCADE,
    ResultDate TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    Summary TEXT,
    ResultJson JSONB,
    FileUrl VARCHAR(500),
    PerformedById INT REFERENCES Users(Id),
    ReviewedById INT REFERENCES Users(Id),
    CreatedAt TIMESTAMPTZ DEFAULT NOW(),
    UpdatedAt TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_exam_results_exam ON ExamResults(ExamId);