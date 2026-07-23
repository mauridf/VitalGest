# VitalGest - Gestão Inteligente para Clínicas e Laboratórios

**"Tudo o que sua clínica precisa para cuidar de quem precisa"**

## Stack Tecnológica

- **.NET 10** + **C# 13**
- **PostgreSQL 16**
- **Entity Framework Core 10**
- **Redis** (cache)
- **Quartz.NET** (background jobs)
- **DDD** 5 camadas + Multi-tenant

## Pré-requisitos

- [.NET SDK 10.0](https://dotnet.microsoft.com/download/dotnet/10.0)
- [PostgreSQL 16](https://www.postgresql.org/download/)
- [Redis](https://redis.io/download/) (ou via Docker)

## Configuração Local

1. Clone o repositório
2. Configure a connection string em `src/VitalGest.Api/appsettings.Development.json`
3. Execute as migrations:

```bash
cd src/VitalGest.Api
dotnet run
```

## Estrutura do Projeto

VitalGest/
├── src/
│   ├── VitalGest.Api/              # ASP.NET Core Web API
│   ├── VitalGest.Core/             # Domínio (Entities, Enums, Interfaces)
│   ├── VitalGest.Application/      # Serviços, DTOs, Validators
│   ├── VitalGest.Infrastructure/   # EF Core, Repositories, Redis
│   └── VitalGest.CrossCutting/     # Extensões, Helpers
└── tests/
    ├── VitalGest.UnitTests/
    └── VitalGest.IntegrationTests/
	
## Licença
Proprietário - Todos os direitos reservados.

---