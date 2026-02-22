using System.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using ModuloNet.Application.Abstractions;
using ModuloNet.Infrastructure.Dapper;
using ModuloNet.Infrastructure.Identity;
using ModuloNet.Infrastructure.Persistence;
using ModuloNet.Infrastructure.Services;
using ModuloNet.Infrastructure.Auth;
using ModuloNet.Infrastructure.Auth.ExternalAuth;
using ModuloNet.Infrastructure.Persistence.Seed;

namespace ModuloNet.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Host=localhost;Database=ModuloNet;Username=postgres;Password=postgres";

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.Configure<BootstrapAdminOptions>(configuration.GetSection(BootstrapAdminOptions.SectionName));
        services.Configure<ExternalAuthOptions>(configuration.GetSection(ExternalAuthOptions.SectionName));

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IDateTimeProvider, DateTimeProvider>();
        services.AddScoped<ICurrentUser, CurrentUserService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IRefreshTokenStore, RefreshTokenStore>();
        services.AddScoped<IAuthService, AuthService>();

        services.AddHttpClient<GoogleAuthProvider>();
        services.AddHttpClient<FacebookAuthProvider>();
        services.AddTransient<IExternalAuthProvider, GoogleAuthProvider>();
        services.AddTransient<IExternalAuthProvider, FacebookAuthProvider>();


        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 6;
        })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<IDbConnection>(_ => new NpgsqlConnection(connectionString));
        services.AddScoped<ICourseReadRepository, CourseReadRepository>();
        services.AddScoped<IdentitySeeder>();

        return services;
    }
}
