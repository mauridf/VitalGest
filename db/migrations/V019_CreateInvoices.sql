-- V019: Cria tabela de faturas
CREATE TABLE IF NOT EXISTS Invoices (
    Id SERIAL PRIMARY KEY,
    ClinicId INT NOT NULL REFERENCES Clinics(Id) ON DELETE CASCADE,
    PatientId INT REFERENCES Patients(Id),
    InvoiceNumber VARCHAR(50) NOT NULL,
    IssueDate TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    DueDate DATE NOT NULL,
    TotalAmount DECIMAL(12,2) NOT NULL,
    PaidAmount DECIMAL(12,2) DEFAULT 0,
    Status INT DEFAULT 1, -- InvoiceStatus enum
    Notes TEXT,
    CreatedAt TIMESTAMPTZ DEFAULT NOW(),
    UpdatedAt TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_invoices_clinic ON Invoices(ClinicId);
CREATE INDEX IF NOT EXISTS idx_invoices_patient ON Invoices(PatientId);
CREATE UNIQUE INDEX IF NOT EXISTS idx_invoices_number ON Invoices(ClinicId, InvoiceNumber);
CREATE INDEX IF NOT EXISTS idx_invoices_status ON Invoices(Status);