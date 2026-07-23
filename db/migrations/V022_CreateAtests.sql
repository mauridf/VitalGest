-- V022: Cria tabela de atestados
CREATE TABLE IF NOT EXISTS Atests (
    Id SERIAL PRIMARY KEY,
    ClinicId INT NOT NULL REFERENCES Clinics(Id) ON DELETE CASCADE,
    PatientId INT NOT NULL REFERENCES Patients(Id) ON DELETE CASCADE,
    DoctorUserId INT NOT NULL REFERENCES Users(Id),
    AppointmentId INT REFERENCES Appointments(Id),
    IssueDate TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    CID VARCHAR(10),
    Description TEXT NOT NULL,
    RestDays INT NOT NULL,
    IsDigitalSignature BOOLEAN DEFAULT FALSE,
    CreatedAt TIMESTAMPTZ DEFAULT NOW(),
    CONSTRAINT chk_atest_dates CHECK (EndDate >= StartDate),
    CONSTRAINT chk_atest_rest_days CHECK (RestDays > 0)
);

CREATE INDEX IF NOT EXISTS idx_atests_patient ON Atests(PatientId);
CREATE INDEX IF NOT EXISTS idx_atests_doctor ON Atests(DoctorUserId);