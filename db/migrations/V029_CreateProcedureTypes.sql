CREATE TABLE IF NOT EXISTS ProcedureTypes (
    Id SERIAL PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    Description TEXT,
    Category INT NOT NULL DEFAULT 1,
    TussCode VARCHAR(20),
    DefaultPrice DECIMAL(12,2),
    DefaultDuration INT,
    RequiresAuthorization BOOLEAN DEFAULT FALSE,
    IsActive BOOLEAN DEFAULT TRUE,
    CreatedAt TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX idx_procedure_types_name ON ProcedureTypes USING gin(Name gin_trgm_ops);
CREATE INDEX idx_procedure_types_category ON ProcedureTypes(Category);
