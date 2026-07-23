-- V028: Dados iniciais (seed)

-- Cargos padrão
INSERT INTO Positions (Name, Description) VALUES
('Médico', 'Profissional de medicina com CRM ativo'),
('Enfermeiro', 'Profissional de enfermagem com COREN ativo'),
('Técnico de Enfermagem', 'Técnico de enfermagem'),
('Atendente/Recepcionista', 'Responsável pelo atendimento e agendamentos'),
('Administrador', 'Administrador da clínica'),
('Laboratorista', 'Profissional de laboratório de análises clínicas'),
('Dentista', 'Profissional de odontologia com CRO ativo'),
('Fisioterapeuta', 'Profissional de fisioterapia'),
('Nutricionista', 'Profissional de nutrição'),
('Psicólogo', 'Profissional de psicologia')
ON CONFLICT DO NOTHING;

-- Especialidades médicas comuns
INSERT INTO Specialties (Name, Description) VALUES
('Clínica Geral', 'Medicina geral e preventiva'),
('Cardiologia', 'Especialidade do coração e sistema circulatório'),
('Dermatologia', 'Especialidade da pele, cabelos e unhas'),
('Pediatria', 'Especialidade infantil e adolescente'),
('Ginecologia', 'Saúde da mulher'),
('Ortopedia', 'Especialidade do sistema musculoesquelético'),
('Oftalmologia', 'Especialidade dos olhos e visão'),
('Otorrinolaringologia', 'Especialidade de ouvido, nariz e garganta'),
('Psiquiatria', 'Saúde mental'),
('Neurologia', 'Especialidade do sistema nervoso'),
('Endocrinologia', 'Especialidade de hormônios e metabolismo'),
('Urologia', 'Especialidade do trato urinário'),
('Odontologia Geral', 'Saúde bucal geral'),
('Ortodontia', 'Correção dental e maxilar'),
('Fisioterapia', 'Reabilitação física'),
('Nutrição', 'Especialidade nutricional'),
('Psicologia', 'Saúde mental e comportamental')
ON CONFLICT DO NOTHING;

-- Tipos de exame comuns
INSERT INTO ExamTypes (Name, Description, Category, IsLaboratory, IsImage, RequiresPreparation) VALUES
('Hemograma Completo', 'Análise completa do sangue', 1, TRUE, FALSE, FALSE),
('Glicemia em Jejum', 'Taxa de glicose no sangue', 1, TRUE, FALSE, TRUE),
('Colesterol Total', 'Perfil lipídico', 1, TRUE, FALSE, TRUE),
('TSH', 'Hormônio tireoestimulante', 1, TRUE, FALSE, FALSE),
('Raio-X Tórax', 'Radiografia da região torácica', 2, FALSE, TRUE, FALSE),
('Ultrassonografia Abdominal', 'Imagem da região abdominal', 2, FALSE, TRUE, TRUE),
('Eletrocardiograma', 'Registro da atividade elétrica do coração', 3, FALSE, FALSE, FALSE),
('Tomografia Computadorizada', 'Imagem por tomografia', 2, FALSE, TRUE, TRUE),
('Ressonância Magnética', 'Imagem por ressonância', 2, FALSE, TRUE, TRUE),
('Teste Ergométrico', 'Teste de esforço cardíaco', 3, FALSE, FALSE, TRUE)
ON CONFLICT DO NOTHING;