# VitalGest - Gestao Inteligente para Clinicas e Laboratorios

**"Tudo o que sua clinica precisa para cuidar de quem precisa"**

Plataforma SaaS multi-tenant para gestao de clinicas medicas, laboratorios e consultorios odontologicos.

---

## Stack Tecnologica

- **.NET 10** + **C# 13**
- **PostgreSQL 16** + **Entity Framework Core 10**
- **Redis** (cache distribuido)
- **Quartz.NET** (background jobs)
- **DDD 5 camadas** + Multi-tenant
- **Repository Pattern** + **Unit of Work**
- **AutoMapper**, **FluentValidation**, **Serilog**, **BCrypt.Net**
- **JWT Bearer** com refresh tokens
- **Scalar** (documentacao OpenAPI interativa)
- **xUnit** + **NSubstitute** + **FluentAssertions** (testes)

---

## Arquitetura

```
VitalGest.Api           -> Controllers, Middlewares, Program.cs
VitalGest.Application   -> Services, DTOs, Validators, Mappings
VitalGest.Core          -> Entities, Enums, Interfaces, Exceptions
VitalGest.Infrastructure -> EF Core DbContext, Repositories, Migrations (DbUp)
VitalGest.CrossCutting  -> Extensions, Logging
```

### Middleware Pipeline

1. GlobalExceptionHandler  2. ResponseTime  3. SecurityHeaders
4. CorrelationId           5. RequestLogging  6. CORS
7. RateLimiting            8. Authentication  9. TenantResolution  10. Authorization

---

## Modulos Implementados

| Modulo | Entidades | Endpoints |
|--------|-----------|-----------|
| Core (Multi-tenant) | Clinic, Address, Position, Department, Specialty | 8 |
| Colaboradores | User, ClinicUser | 10 |
| Pacientes | Patient | 10 |
| Agendamento | Appointment, WaitingRoomEntry | 10 |
| Agenda | Schedule, ScheduleException, TimeSlot | 7 |
| Prontuario | MedicalRecord, MedicalRecordEntry | 6 |
| Exames | Exam, ExamResult, ExamType | 8 |
| Prescricoes | Prescription, PrescriptionItem | 5 |
| Atestados | Atest | 4 |
| Convenios | InsurancePlan, InsuranceCoverage | 6 |
| Financeiro | Payment, Invoice | 9 |
| Documentos | Document | 6 |
| Notificacoes | Notification | 4 |
| Relatorios | - | 6 |
| Dashboard | - | 5 |
| Busca | - | 1 |
| Auditoria | AuditLog | 3 |
| Admin (SuperAdmin) | - | 3 |

**Total: ~110+ endpoints**, 29 entidades, 17 enums, 29 migrations SQL, 89 testes unitarios.

---

## Pre-requisitos

- [.NET SDK 10.0](https://dotnet.microsoft.com/download/dotnet/10.0)
- [PostgreSQL 16](https://www.postgresql.org/download/)
- [Redis](https://redis.io/download/) (opcional - o sistema funciona sem)

## Configuracao Local

```bash
# Clone e configure
git clone <repo>
cd VitalGest

# Configure a connection string em:
# src/VitalGest.Api/appsettings.Development.json

# Execute (migracoes DbUp rodam automaticamente no startup)
cd src/VitalGest.Api
dotnet run

# API: http://localhost:5000
# Scalar: http://localhost:5000/scalar/v1
# Health: http://localhost:5000/api/health
```

## Estrutura do Projeto

```
src/
  VitalGest.Api/              # ASP.NET Core Web API
  VitalGest.Application/      # Servicos, DTOs, Validators, Mappings
  VitalGest.Core/             # Dominio (Entities, Enums, Interfaces)
  VitalGest.Infrastructure/   # EF Core, Repositories, Migrations, Cache
  VitalGest.CrossCutting/     # Extensions, Helpers
tests/
  VitalGest.UnitTests/        # 89 testes (14 arquivos)
  VitalGest.IntegrationTests/ # 4 arquivos de teste
db/migrations/                # 29 scripts SQL versionados (DbUp)
```

## Seguranca

- **Autenticacao**: JWT com tokens de acesso (2h) e refresh (7d)
- **Rate Limiting**: 3 politicas (Global 100/min, Auth 5/min, Public 30/min)
- **Multi-tenant**: Isolamento por ClinicId com Global Query Filter
- **Senhas**: BCrypt.Net com salt automatico
- **CORS**: Configurado por ambiente
- **Auditoria**: Todas as operacoes de escrita registradas em AuditLog

## Testes

```bash
# Unitarios
dotnet test tests/VitalGest.UnitTests

# Integracao (requer banco PostgreSQL)
dotnet test tests/VitalGest.IntegrationTests
```

## Deploy

Veja [docs/DEPLOY.md](docs/DEPLOY.md) para instrucoes detalhadas de deploy local, Docker e Render.

## Licenca

Proprietario - Todos os direitos reservados.
