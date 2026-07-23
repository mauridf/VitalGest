-- V021: Cria tabela de auditoria
CREATE TABLE IF NOT EXISTS AuditLogs (
    Id SERIAL PRIMARY KEY,
    ClinicId INT REFERENCES Clinics(Id),
    UserId INT REFERENCES Users(Id),
    EntityType VARCHAR(100) NOT NULL,
    EntityId INT NOT NULL,
    Action VARCHAR(50) NOT NULL,
    OldValues JSONB,
    NewValues JSONB,
    IpAddress VARCHAR(50),
    UserAgent VARCHAR(500),
    CreatedAt TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_audit_clinic ON AuditLogs(ClinicId);
CREATE INDEX IF NOT EXISTS idx_audit_entity ON AuditLogs(EntityType, EntityId);
CREATE INDEX IF NOT EXISTS idx_audit_date ON AuditLogs(CreatedAt DESC);
CREATE INDEX IF NOT EXISTS idx_audit_user ON AuditLogs(UserId);
CREATE INDEX IF NOT EXISTS idx_audit_action ON AuditLogs(Action);