# Deploy do VitalGest

Este documento descreve como fazer deploy do VitalGest em diferentes ambientes.

## Pré-requisitos

- .NET 10 SDK
- PostgreSQL 16
- Redis 7+ (opcional, recomendado para cache)
- Conta no [Render](https://render.com) (para deploy em nuvem)

---

## Execução Local (Desenvolvimento)

### 1. Configurar Banco de Dados

Certifique-se de que o PostgreSQL está rodando localmente.

```bash
# Criar banco de dados
psql -U postgres -c "CREATE DATABASE vitalgest_dev;"
```

### 2. Configurar Redis (opcional)

Se quiser usar cache Redis:

```bash
# Verificar se Redis está rodando
redis-cli ping
# Deve retornar: PONG
```

> O Redis é opcional. Sem ele, o cache é desabilitado graciosamente (CacheService trata falhas).

### 3. Configurar Variáveis de Ambiente

Edite o arquivo `src/VitalGest.Api/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=vitalgest_dev;Username=postgres;Password=SUA_SENHA"
  },
  "Redis": {
    "ConnectionString": "localhost:6379"
  },
  "Jwt": {
    "Secret": "Sua-Chave-Secreta-JWT-Com-32-Caracteres-No-Minimo!!"
  }
}
```

### 4. Executar a Aplicação

```bash
cd src/VitalGest.Api
dotnet run
```

A aplicação será iniciada em:

```
API:                    http://localhost:5000
Documentação Scalar:    http://localhost:5000/scalar/v1
Health Check:           http://localhost:5000/api/health
```

> A porta pode ser configurada via variável de ambiente `PORT`.

### 5. Verificar Migrations

As migrações DbUp são executadas automaticamente no startup.
Verifique os logs para confirmar:

```
[INF] Iniciando migrações do banco de dados...
[INF] 29 script(s) pendente(s) para execução.
[INF] Migrações concluídas com sucesso!
```

---

## Deploy com Docker

### Construir a Imagem

```bash
docker build -t vitalgest-api .
```

### Executar com Docker Compose

```bash
docker-compose up -d
```

O `docker-compose.yml` já inclui PostgreSQL e Redis como serviços dependentes.

---

## Deploy no Render

O projeto inclui `render.yaml` para deploy automatizado no Render.

### via Blueprint (recomendado)

1. Faça fork/clone do repositório no GitHub
2. Conecte o repositório ao Render
3. O Render detectará automaticamente o `render.yaml`
4. Configure as variáveis de ambiente no painel do Render

### Manualmente

1. Crie um Web Service no Render
2. Selecione o repositório
3. Configure:
   - **Runtime:** .NET 10
   - **Build Command:** `dotnet publish src/VitalGest.Api -c Release -o out`
   - **Start Command:** `./out/VitalGest.Api`
   - **Port:** `5000`

### Variáveis de Ambiente (Render)

| Variável | Descrição |
|----------|-----------|
| `ConnectionStrings__DefaultConnection` | String de conexão PostgreSQL |
| `Redis__ConnectionString` | String de conexão Redis (opcional) |
| `Jwt__Secret` | Chave secreta JWT |
| `ASPNETCORE_ENVIRONMENT` | `Production` ou `Development` |

---

## Variáveis de Ambiente

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=vitalgest_dev;Username=postgres;Password=postgres"
  },
  "Jwt": {
    "Secret": "Sua-Chave-Secreta-JWT-Com-32-Caracteres-No-Minimo!!",
    "Issuer": "VitalGest",
    "Audience": "vitalgest-api",
    "ExpireMinutes": 120,
    "RefreshTokenExpireDays": 7
  },
  "Redis": {
    "ConnectionString": "localhost:6379"
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.EntityFrameworkCore": "Warning"
      }
    }
  },
  "Cors": {
    "AllowedOrigins": ["http://localhost:3000", "http://localhost:5173"]
  },
  "RateLimiting": {
    "GlobalPermitLimit": 100,
    "GlobalWindowMinutes": 1,
    "AuthPermitLimit": 5,
    "AuthWindowMinutes": 1,
    "PublicPermitLimit": 30,
    "PublicWindowMinutes": 1
  },
  "Storage": {
    "Provider": "Local",
    "BasePath": "uploads",
    "MaxFileSize": 10485760
  }
}
```

---

## Estrutura de Logs

Os logs são armazenados em `logs/vitalgest-{yyyyMMdd}.log` com rotação diária e retenção de 30 dias.
