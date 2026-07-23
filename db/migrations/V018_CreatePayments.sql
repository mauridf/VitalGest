-- V018: Cria tabela de pagamentos
CREATE TABLE IF NOT EXISTS Payments (
    Id SERIAL PRIMARY KEY,
    ClinicId INT NOT NULL REFERENCES Clinics(Id) ON DELETE CASCADE,
    PatientId INT REFERENCES Patients(Id),
    AppointmentId INT REFERENCES Appointments(Id),
    Amount DECIMAL(12,2) NOT NULL,
    Discount DECIMAL(12,2) DEFAULT 0,
    TotalAmount DECIMAL(12,2) NOT NULL,
    PaymentDate TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    PaymentMethod INT NOT NULL, -- PaymentMethod enum
    Status INT DEFAULT 1, -- PaymentStatus enum
    Installments INT DEFAULT 1,
    Notes TEXT,
    ReceivedById INT REFERENCES Users(Id),
    CreatedAt TIMESTAMPTZ DEFAULT NOW(),
    UpdatedAt TIMESTAMPTZ DEFAULT NOW(),
    CONSTRAINT chk_payment_amount CHECK (Amount > 0),
    CONSTRAINT chk_payment_discount CHECK (Discount >= 0 AND Discount <= Amount)
);

CREATE INDEX IF NOT EXISTS idx_payments_clinic ON Payments(ClinicId);
CREATE INDEX IF NOT EXISTS idx_payments_patient ON Payments(PatientId);
CREATE INDEX IF NOT EXISTS idx_payments_appointment ON Payments(AppointmentId);
CREATE INDEX IF NOT EXISTS idx_payments_date ON Payments(PaymentDate DESC);
CREATE INDEX IF NOT EXISTS idx_payments_status ON Payments(Status);