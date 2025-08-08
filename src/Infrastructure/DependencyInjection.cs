using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Infrastructure.Data;
using Educar.Backend.Infrastructure.Data.Interceptors;
using Educar.Backend.Infrastructure.Options;
using Educar.Backend.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Npgsql;

namespace Educar.Backend.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration, IApplicationDbContext? context = null)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        Guard.Against.Null(connectionString, message: "Connection string 'DefaultConnection' not found.");

        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, SoftDeleteInterceptor>();
        services.AddScoped<IIdentityService, KeycloakService>();

        var useAzureFlag = configuration.GetValue<bool>("UseAzureCloud");
        if (useAzureFlag)
        {
            services.AddSingleton<IObjectStorage, AzureBlobStorageService>();
        }
        else
        {
            services.AddSingleton<IObjectStorage, OciObjectStorageService>();
        }


        if (context != null)
        {
            services.AddSingleton(context);
            services.AddSingleton(context);
        }
        else
        {
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());

                var connectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionString);
                var searchPaths = connectionStringBuilder.SearchPath?.Split(',');

                options.UseNpgsql(connectionString, sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                    sqlOptions.EnableRetryOnFailure(15, TimeSpan.FromSeconds(30),
                        null);

                    if (searchPaths is { Length: > 0 })
                    {
                        var mainSchema = searchPaths[0];
                        sqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, mainSchema);
                    }
                });
            });

            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        }

        services.AddScoped<ApplicationDbContextInitialiser>();

        var authOptions = configuration.GetAuthOptions();

        services.AddAuthentication().AddJwtBearer(options =>
        {
            options.Authority = authOptions.Issuer;
            options.RequireHttpsMetadata = authOptions.RequireHttpsMetadata;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = false,
                ValidIssuer = authOptions.Issuer,
                ClockSkew = TimeSpan.Zero
            };
        });
        services.AddAuthorization();

        services.AddSingleton(TimeProvider.System);

        return services;
    }
}