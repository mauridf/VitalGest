-- V014: Cria tabela de tipos de exame
CREATE TABLE IF NOT EXISTS ExamTypes (
    Id SERIAL PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    Description TEXT,
    Category INT NOT NULL,
    IsLaboratory BOOLEAN DEFAULT FALSE,
    IsImage BOOLEAN DEFAULT FALSE,
    RequiresPreparation BOOLEAN DEFAULT FALSE,
    PreparationInstructions TEXT,
    IsActive BOOLEAN DEFAULT TRUE,
    CreatedAt TIMESTAMPTZ DEFAULT NOW()
);

-- Adiciona FK em InsuranceCoverages
ALTER TABLE InsuranceCoverages 
ADD CONSTRAINT fk_insurance_coverages_exam_type 
FOREIGN KEY (ExamTypeId) REFERENCES ExamTypes(Id);