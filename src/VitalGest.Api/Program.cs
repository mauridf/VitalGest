using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Serilog;
using System.Text;
using System.Threading.RateLimiting;
using VitalGest.Api.Filters;
using VitalGest.Api.Middlewares;
using VitalGest.Application.DependencyInjection;
using VitalGest.Infrastructure.Data.Migrations;
using VitalGest.Infrastructure.DependencyInjection;

// ===== Configuração inicial do Serilog =====
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // ===== Serilog =====
    builder.Host.UseSerilog((context, services, configuration) =>
    {
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentName();
    });

    // ===== Configuração de porta =====
    var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

    // ===== Serviços =====

    // JWT Authentication
    var jwtSecret = builder.Configuration["Jwt:Secret"]
        ?? throw new InvalidOperationException("JWT Secret não configurado.");
    var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "VitalGest";
    var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "vitalgest-api";

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero, // Sem tolerância de expiração
            RoleClaimType = "role"
        };

        // Eventos para debugging
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Log.Warning("Falha na autenticação JWT: {Error}", context.Exception.Message);
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Log.Debug("Token JWT validado para: {Principal}",
                    context.Principal?.Identity?.Name);
                return Task.CompletedTask;
            }
        };
    });

    // Authorization Policies
    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("AdminOnly", policy =>
            policy.RequireRole("Admin", "SuperAdmin"));
        options.AddPolicy("DoctorOnly", policy =>
            policy.RequireRole("User", "Admin", "SuperAdmin"));
        options.AddPolicy("SuperAdminOnly", policy =>
            policy.RequireRole("SuperAdmin"));
    });

    // CORS
    builder.Services.AddCors(options =>
    {
        var allowedOrigins = builder.Configuration
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>() ?? ["http://localhost:3000"];

        options.AddPolicy("VitalGestCors", policy =>
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials()
                  .WithExposedHeaders("X-Correlation-Id");
        });
    });

    // Rate Limiting
    builder.Services.AddRateLimiter(options =>
    {
        options.RejectionStatusCode = 429; // Too Many Requests

        // Política Global (100 req/min)
        options.AddFixedWindowLimiter("Global", config =>
        {
            config.PermitLimit = builder.Configuration.GetValue<int>("RateLimiting:GlobalPermitLimit", 100);
            config.Window = TimeSpan.FromMinutes(
                builder.Configuration.GetValue<int>("RateLimiting:GlobalWindowMinutes", 1));
            config.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            config.QueueLimit = 0;
        });

        // Política de Auth (5 req/min) - para login/registro
        options.AddFixedWindowLimiter("Auth", config =>
        {
            config.PermitLimit = builder.Configuration.GetValue<int>("RateLimiting:AuthPermitLimit", 5);
            config.Window = TimeSpan.FromMinutes(
                builder.Configuration.GetValue<int>("RateLimiting:AuthWindowMinutes", 1));
            config.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            config.QueueLimit = 0;
        });

        // Política Pública (30 req/min)
        options.AddFixedWindowLimiter("Public", config =>
        {
            config.PermitLimit = builder.Configuration.GetValue<int>("RateLimiting:PublicPermitLimit", 30);
            config.Window = TimeSpan.FromMinutes(
                builder.Configuration.GetValue<int>("RateLimiting:PublicWindowMinutes", 1));
            config.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            config.QueueLimit = 0;
        });
    });

    // Controllers + Filters
    builder.Services.AddControllers(options =>
    {
        options.Filters.Add<ValidationFilter>();
    });

    // OpenAPI/Scalar
    builder.Services.AddOpenApi();

    // Camadas da aplicação
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);

    var app = builder.Build();

    // ===== Pipeline de Middleware (ordem importa!) =====

    // 1. Tratamento global de exceções (primeiro para capturar tudo)
    app.UseGlobalExceptionHandler();

    // 2. Response Time (mede tempo de resposta)
    app.UseResponseTime();

    // 3. Headers de segurança
    app.UseSecurityHeaders();

    // 4. Correlation ID
    app.UseCorrelationId();

    // 5. Request Logging
    app.UseRequestLogging();

    // 6. HTTPS (forçado em produção)
    if (!app.Environment.IsDevelopment())
    {
        app.UseHsts();
        app.UseHttpsRedirection();
    }

    // 7. CORS
    app.UseCors("VitalGestCors");

    // 8. Rate Limiting
    app.UseRateLimiter();

    // 9. Autenticação JWT
    app.UseAuthentication();

    // 10. Resolução de Tenant (após autenticação)
    app.UseTenantResolution();

    // 11. Autorização
    app.UseAuthorization();

    // 12. Mapeamento de Controllers
    app.MapControllers();

    // 13. Documentação Scalar (OpenAPI)
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "VitalGest API";
        options.Theme = ScalarTheme.BluePlanet;
        options.DefaultHttpClient = new(ScalarTarget.CSharp, ScalarClient.HttpClient);
        options.ShowSidebar = true;
        options.HideDownloadButton = false;
    });

    // ===== Inicialização do Banco de Dados (DbUp) =====
    using (var scope = app.Services.CreateScope())
    {
        var dbInitializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
        dbInitializer.RunMigrations();
    }

    // ===== Health Check simples =====
    app.MapGet("/", () => Results.Ok(new
    {
        Name = "VitalGest API",
        Version = "1.0.0",
        Status = "Running",
        Environment = app.Environment.EnvironmentName,
        Timestamp = DateTime.UtcNow
    }));

    Log.Information("VitalGest API iniciada em: {Url}", app.Urls.FirstOrDefault());

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Aplicação terminou inesperadamente");
}
finally
{
    Log.CloseAndFlush();
}