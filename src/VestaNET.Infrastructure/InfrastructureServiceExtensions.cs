using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VestaNET.Application.Interfaces;
using VestaNET.Application.Settings;
using VestaNET.Infrastructure.HttpClients;
using VestaNET.Infrastructure.Persistence;
using VestaNET.Infrastructure.Persistence.Repositories;
using VestaNET.Infrastructure.Security;

namespace VestaNET.Infrastructure;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        if (config.GetValue<bool>("UseInMemoryDatabase"))
            services.AddDbContext<AppDbContext>(o => o.UseInMemoryDatabase("VestaDev"));
        else
        {
            var cs = config.GetConnectionString("Oracle")
                ?? throw new InvalidOperationException("Connection string 'Oracle' não configurada.");
            services.AddDbContext<AppDbContext>(o => o.UseOracle(cs));
        }

        var useJavaApi = config.GetValue<bool>("UseJavaApi");
        if (useJavaApi)
        {
            services.Configure<JavaApiSettings>(config.GetSection("JavaApi"));
            services.AddSingleton<JavaAuthHandler>();
            services.AddHttpClient<IJavaApiClient, JavaApiClient>(client =>
            {
                var baseUrl = config["JavaApi:BaseUrl"] ?? "http://localhost:8080";
                client.BaseAddress = new Uri(baseUrl);
            }).AddHttpMessageHandler<JavaAuthHandler>();

            services.AddScoped<IAbrigoRepository>(sp => sp.GetRequiredService<IJavaApiClient>());
        }
        else
        {
            services.AddScoped<DevAbrigoStub>();
            services.AddScoped<IAbrigoRepository>(sp => sp.GetRequiredService<DevAbrigoStub>());
            services.AddScoped<IJavaApiClient>(sp => sp.GetRequiredService<DevAbrigoStub>());
        }

        services.AddScoped<IAnaliseRepository, AnaliseRepository>();
        services.Configure<JwtSettings>(config.GetSection(JwtSettings.Section));
        services.AddScoped<JwtTokenGenerator>();

        services.AddHealthChecks()
            .AddDbContextCheck<AppDbContext>(name: "database", tags: ["db"]);

        return services;
    }
}
