using Microsoft.Extensions.DependencyInjection;
using VestaNET.Application.Services;
using VestaNET.Domain.Services;
namespace VestaNET.Application;
public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<CalculadoraCriticidade>();
        services.AddScoped<AnaliseService>();
        return services;
    }
}
