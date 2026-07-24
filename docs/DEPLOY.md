# 🚀 Deploy do VitalGest

Este documento descreve como fazer deploy do VitalGest em diferentes ambientes.

## 📋 Pré-requisitos

- .NET 10 SDK
- PostgreSQL 16
- Redis 7+ (opcional, mas recomendado)
- Conta no [Render](https://render.com) (para deploy em nuvem)

---

## 💻 Execução Local (Desenvolvimento)

### 1. Configurar Banco de Dados

Certifique-se de que o PostgreSQL está rodando localmente.

```
# Criar banco de dados
psql -U postgres -c "CREATE DATABASE vitalgest_dev;"
```

### 2. Configurar Redis
Certifique-se de que o Redis está rodando na porta 6379.

```
# Verificar se Redis está rodando
redis-cli ping
# Deve retornar: PONG
```

### 3. Configurar Variáveis de Ambiente
Edite o arquivo src/VitalGest.Api/appsettings.Development.json:

```
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=vitalgest_dev;Username=postgres;Password=SUA_SENHA"
  },
  "Redis": {
    "ConnectionString": "localhost:6379"
  }
}
```

### 4. Executar a Aplicação
```
cd src/VitalGest.Api
dotnet run
A aplicação será iniciada em:

API: http://localhost:5000

Documentação Scalar: http://localhost:5000/scalar/v1

Health Check: http://localhost:5000/api/health
```

### 5. Verificar Migrations
As migrações DbUp são executadas automaticamente no startup.
Verifique os logs para confirmar:

```
[INF] Iniciando migrações do banco de dados...
[INF] 28 script(s) pendente(s) para execução.
[INF] Migrações concluídas com sucesso!
```