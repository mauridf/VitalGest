-- V012: Cria tabela de regras de agenda
CREATE TABLE IF NOT EXISTS Schedules (
    Id SERIAL PRIMARY KEY,
    ClinicId INT NOT NULL REFERENCES Clinics(Id) ON DELETE CASCADE,
    DoctorUserId INT NOT NULL REFERENCES Users(Id),
    DayOfWeek INT NOT NULL CHECK (DayOfWeek BETWEEN 0 AND 6),
    StartTime TIME NOT NULL,
    EndTime TIME NOT NULL,
    SlotDuration INT DEFAULT 30,
    IsActive BOOLEAN DEFAULT TRUE,
    CreatedAt TIMESTAMPTZ DEFAULT NOW(),
    UpdatedAt TIMESTAMPTZ DEFAULT NOW(),
    CONSTRAINT chk_schedule_times CHECK (StartTime < EndTime)
);

CREATE INDEX IF NOT EXISTS idx_schedules_doctor ON Schedules(DoctorUserId);
CREATE INDEX IF NOT EXISTS idx_schedules_clinic ON Schedules(ClinicId);
CREATE INDEX IF NOT EXISTS idx_schedules_doctor_day ON Schedules(DoctorUserId, DayOfWeek);