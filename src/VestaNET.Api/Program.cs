using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using VestaNET.Api.Middleware;
using VestaNET.Application;
using VestaNET.Infrastructure;
using VestaNET.Infrastructure.Persistence;
using VestaNET.Infrastructure.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers();

var jwt = builder.Configuration.GetSection(JwtSettings.Section).Get<JwtSettings>()
    ?? throw new InvalidOperationException("Seção 'Jwt' não configurada.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o => o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer           = true,
        ValidateAudience         = true,
        ValidateLifetime         = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer              = jwt.Issuer,
        ValidAudience            = jwt.Audience,
        IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SecretKey)),
        ClockSkew                = TimeSpan.Zero
    });

builder.Services.AddAuthorization();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new OpenApiInfo
    {
        Title   = "Vesta .NET — Apoio à Decisão",
        Version = "v1",
        Description =
            "Serviço auxiliar da plataforma Vesta (Global Solution FIAP 2026). " +
            "Calcula criticidade, ranking e recomendações com base nos dados operacionais do backend Java."
    });
    var scheme = new OpenApiSecurityScheme
    {
        Name = "Authorization", Type = SecuritySchemeType.Http,
        Scheme = "bearer", BearerFormat = "JWT", In = ParameterLocation.Header,
        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
    };
    o.AddSecurityDefinition("Bearer", scheme);
    o.AddSecurityRequirement(new OpenApiSecurityRequirement { { scheme, [] } });
    var xml = Path.Combine(AppContext.BaseDirectory,
        $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml");
    if (File.Exists(xml)) o.IncludeXmlComments(xml);
});

builder.Services.AddHealthChecks()
    .AddCheck("self", () =>
        Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy(), tags: ["live"]);

var app = builder.Build();

if (builder.Configuration.GetValue<bool>("UseInMemoryDatabase"))
{
    using var scope = app.Services.CreateScope();
    DevDataSeeder.Seed(scope.ServiceProvider.GetRequiredService<AppDbContext>());
}

app.UseExceptionHandler();
app.UseCors();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseSwagger();
app.UseSwaggerUI(o =>
{
    o.SwaggerEndpoint("/swagger/v1/swagger.json", "Vesta .NET v1");
    o.DocumentTitle = "Vesta .NET — Apoio à Decisão";
    o.DefaultModelsExpandDepth(-1);
});
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health",    new HealthCheckOptions { Predicate = c => c.Tags.Contains("live") });
app.MapHealthChecks("/health/db", new HealthCheckOptions { Predicate = c => c.Tags.Contains("db")   });

app.Run();
public partial class Program { }
