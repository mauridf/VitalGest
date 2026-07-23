-- V024: Cria tabela de exceções de agenda
CREATE TABLE IF NOT EXISTS ScheduleExceptions (
    Id SERIAL PRIMARY KEY,
    ClinicId INT NOT NULL REFERENCES Clinics(Id) ON DELETE CASCADE,
    DoctorUserId INT NOT NULL REFERENCES Users(Id),
    ExceptionDate DATE NOT NULL,
    StartTime TIME,
    EndTime TIME,
    Reason VARCHAR(500) NOT NULL,
    IsAvailable BOOLEAN DEFAULT FALSE,
    CreatedAt TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_schedule_exceptions_doctor ON ScheduleExceptions(DoctorUserId);
CREATE INDEX IF NOT EXISTS idx_schedule_exceptions_date ON ScheduleExceptions(ExceptionDate);
CREATE INDEX IF NOT EXISTS idx_schedule_exceptions_doctor_date ON ScheduleExceptions(DoctorUserId, ExceptionDate);