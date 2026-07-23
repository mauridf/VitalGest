-- V025: Cria tabela de slots de horário
CREATE TABLE IF NOT EXISTS TimeSlots (
    Id SERIAL PRIMARY KEY,
    ClinicId INT NOT NULL REFERENCES Clinics(Id) ON DELETE CASCADE,
    ScheduleId INT REFERENCES Schedules(Id),
    DoctorUserId INT NOT NULL REFERENCES Users(Id),
    Date DATE NOT NULL,
    StartTime TIME NOT NULL,
    EndTime TIME NOT NULL,
    IsAvailable BOOLEAN DEFAULT TRUE,
    AppointmentId INT REFERENCES Appointments(Id),
    CreatedAt TIMESTAMPTZ DEFAULT NOW(),
    CONSTRAINT chk_timeslot_times CHECK (StartTime < EndTime)
);

CREATE INDEX IF NOT EXISTS idx_timeslots_doctor_date ON TimeSlots(DoctorUserId, Date);
CREATE INDEX IF NOT EXISTS idx_timeslots_available ON TimeSlots(IsAvailable) WHERE IsAvailable = TRUE;
CREATE INDEX IF NOT EXISTS idx_timeslots_date ON TimeSlots(Date);