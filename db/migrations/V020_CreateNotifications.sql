-- V020: Cria tabela de notificações
CREATE TABLE IF NOT EXISTS Notifications (
    Id SERIAL PRIMARY KEY,
    ClinicId INT NOT NULL REFERENCES Clinics(Id) ON DELETE CASCADE,
    UserId INT REFERENCES Users(Id),
    PatientId INT REFERENCES Patients(Id),
    Title VARCHAR(255) NOT NULL,
    Message TEXT NOT NULL,
    Type INT NOT NULL, -- NotificationType enum
    Channel VARCHAR(20),
    SentAt TIMESTAMPTZ,
    IsRead BOOLEAN DEFAULT FALSE,
    ReadAt TIMESTAMPTZ,
    CreatedAt TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_notifications_user ON Notifications(UserId);
CREATE INDEX IF NOT EXISTS idx_notifications_clinic ON Notifications(ClinicId);
CREATE INDEX IF NOT EXISTS idx_notifications_read ON Notifications(IsRead) WHERE IsRead = FALSE;
CREATE INDEX IF NOT EXISTS idx_notifications_date ON Notifications(CreatedAt DESC);