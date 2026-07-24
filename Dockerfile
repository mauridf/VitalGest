# Estágio de build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copia arquivos de projeto
COPY VitalGest.slnx ./
COPY src/VitalGest.Core/*.csproj ./src/VitalGest.Core/
COPY src/VitalGest.Application/*.csproj ./src/VitalGest.Application/
COPY src/VitalGest.Infrastructure/*.csproj ./src/VitalGest.Infrastructure/
COPY src/VitalGest.CrossCutting/*.csproj ./src/VitalGest.CrossCutting/
COPY src/VitalGest.Api/*.csproj ./src/VitalGest.Api/
COPY tests/VitalGest.UnitTests/*.csproj ./tests/VitalGest.UnitTests/
COPY tests/VitalGest.IntegrationTests/*.csproj ./tests/VitalGest.IntegrationTests/

# Restaura dependências
RUN dotnet restore

# Copia todo o código fonte
COPY . .

# Publica a aplicação
WORKDIR /src/src/VitalGest.Api
RUN dotnet publish -c Release -o /app --no-restore

# Estágio final
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

# Copia os scripts de migração
COPY db/migrations/ ./db/migrations/

# Copia a aplicação publicada
COPY --from=build /app ./

# Expõe a porta
EXPOSE 5000
ENV PORT=5000
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=40s --retries=3 \
    CMD curl -f http://localhost:5000/api/health || exit 1

# Entry point
ENTRYPOINT ["dotnet", "VitalGest.Api.dll"]