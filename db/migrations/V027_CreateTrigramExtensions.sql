-- V027: Instala extensão pg_trgm para busca textual fuzzy
CREATE EXTENSION IF NOT EXISTS pg_trgm;

-- Índices trigram para busca textual (case-insensitive)
CREATE INDEX IF NOT EXISTS idx_patients_name ON Patients USING gin (Name gin_trgm_ops);
CREATE INDEX IF NOT EXISTS idx_users_name ON Users USING gin (Name gin_trgm_ops);
CREATE INDEX IF NOT EXISTS idx_clinics_name ON Clinics USING gin (Name gin_trgm_ops);