-- V026: Cria índices adicionais de performance
-- Índices para busca textual (requer extensão pg_trgm - instalada no V029)

-- Índices compostos para consultas frequentes
CREATE INDEX IF NOT EXISTS idx_appointments_clinic_date_status ON Appointments(ClinicId, AppointmentDate, Status);
CREATE INDEX IF NOT EXISTS idx_patients_clinic_active ON Patients(ClinicId, IsActive) WHERE IsActive = TRUE;
CREATE INDEX IF NOT EXISTS idx_payments_clinic_date_status ON Payments(ClinicId, PaymentDate, Status);
CREATE INDEX IF NOT EXISTS idx_exams_clinic_status ON Exams(ClinicId, Status);

-- Índices para foreign keys que podem faltar
CREATE INDEX IF NOT EXISTS idx_medical_records_clinic ON MedicalRecords(ClinicId);
CREATE INDEX IF NOT EXISTS idx_prescriptions_clinic ON Prescriptions(ClinicId);
CREATE INDEX IF NOT EXISTS idx_atests_clinic ON Atests(ClinicId);
CREATE INDEX IF NOT EXISTS idx_time_slots_clinic ON TimeSlots(ClinicId);
CREATE INDEX IF NOT EXISTS idx_schedule_exceptions_clinic ON ScheduleExceptions(ClinicId);
CREATE INDEX IF NOT EXISTS idx_waiting_room_patient ON WaitingRoomEntries(PatientId);