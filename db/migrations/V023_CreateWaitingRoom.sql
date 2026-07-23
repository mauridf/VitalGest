-- V023: Cria tabela de sala de espera
CREATE TABLE IF NOT EXISTS WaitingRoomEntries (
    Id SERIAL PRIMARY KEY,
    ClinicId INT NOT NULL REFERENCES Clinics(Id) ON DELETE CASCADE,
    AppointmentId INT NOT NULL REFERENCES Appointments(Id),
    PatientId INT NOT NULL REFERENCES Patients(Id),
    ArrivalTime TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CalledAt TIMESTAMPTZ,
    AttendedAt TIMESTAMPTZ,
    Status INT DEFAULT 1, -- WaitingRoomStatus
    Priority INT DEFAULT 1,
    Notes TEXT,
    CreatedAt TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_waiting_room_clinic ON WaitingRoomEntries(ClinicId);
CREATE INDEX IF NOT EXISTS idx_waiting_room_status ON WaitingRoomEntries(Status);
CREATE INDEX IF NOT EXISTS idx_waiting_room_appointment ON WaitingRoomEntries(AppointmentId);