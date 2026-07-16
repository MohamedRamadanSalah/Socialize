using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Socialize.Application.Abstractions;
using Socialize.Infrastructure.Auth;
using Socialize.Infrastructure.Persistence;
using Socialize.Infrastructure.Realtime;
using Socialize.Infrastructure.Search;
using Socialize.Infrastructure.Storage;

namespace Socialize.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Default")));

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<AppDbContext>());

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.AddSingleton<IJwtService, JwtService>();
        services.AddSingleton<ITokenService, TokenService>();

        services.AddScoped<IFileStorage, LocalFileStorage>();
        services.AddScoped<IUserNotificationPublisher, SignalRNotificationPublisher>();
        services.AddScoped<ISearchService, PostgresSearchService>();

        services.AddSignalR();

        return services;
    }
}
