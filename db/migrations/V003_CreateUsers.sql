-- V003: Cria tabela de usuários base
CREATE TABLE IF NOT EXISTS Users (
    Id SERIAL PRIMARY KEY,
    Username VARCHAR(100) NOT NULL UNIQUE,
    Email VARCHAR(255) NOT NULL UNIQUE,
    PasswordHash VARCHAR(500) NOT NULL,
    Name VARCHAR(255) NOT NULL,
    CPF VARCHAR(14) UNIQUE,
    Phone VARCHAR(20),
    AvatarUrl VARCHAR(500),
    Role INT DEFAULT 1, -- UserRole: 1=User, 2=Admin, 3=SuperAdmin
    RefreshToken VARCHAR(500),
    RefreshTokenExpiryTime TIMESTAMPTZ,
    IsActive BOOLEAN DEFAULT TRUE,
    CreatedAt TIMESTAMPTZ DEFAULT NOW(),
    UpdatedAt TIMESTAMPTZ DEFAULT NOW()
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_users_username ON Users(Username);
CREATE UNIQUE INDEX IF NOT EXISTS idx_users_email ON Users(Email);
CREATE UNIQUE INDEX IF NOT EXISTS idx_users_cpf ON Users(CPF) WHERE CPF IS NOT NULL;