-- V011: Cria tabela de agendamentos
CREATE TABLE IF NOT EXISTS Appointments (
    Id SERIAL PRIMARY KEY,
    ClinicId INT NOT NULL REFERENCES Clinics(Id) ON DELETE CASCADE,
    PatientId INT NOT NULL REFERENCES Patients(Id) ON DELETE CASCADE,
    DoctorUserId INT NOT NULL REFERENCES Users(Id),
    DepartmentId INT REFERENCES Departments(Id),
    SpecialtyId INT REFERENCES Specialties(Id),
    AppointmentDate DATE NOT NULL,
    StartTime TIME NOT NULL,
    EndTime TIME NOT NULL,
    Status INT DEFAULT 1, -- AppointmentStatus
    Type INT DEFAULT 1, -- AppointmentType
    Notes TEXT,
    InternalNotes TEXT,
    IsConfirmed BOOLEAN DEFAULT FALSE,
    ConfirmedAt TIMESTAMPTZ,
    CancelledAt TIMESTAMPTZ,
    CancelReason TEXT,
    CreatedById INT NOT NULL REFERENCES Users(Id),
    CreatedAt TIMESTAMPTZ DEFAULT NOW(),
    UpdatedAt TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_appointments_clinic ON Appointments(ClinicId);
CREATE INDEX IF NOT EXISTS idx_appointments_patient ON Appointments(PatientId);
CREATE INDEX IF NOT EXISTS idx_appointments_doctor ON Appointments(DoctorUserId);
CREATE INDEX IF NOT EXISTS idx_appointments_date ON Appointments(AppointmentDate DESC);
CREATE INDEX IF NOT EXISTS idx_appointments_status ON Appointments(Status);
CREATE INDEX IF NOT EXISTS idx_appointments_doctor_date ON Appointments(DoctorUserId, AppointmentDate);