using System.Text.Json.Serialization;
using Educar.Backend.Application.Commands.AnswerTypes;
using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.Common.NamingPolicies;
using Educar.Backend.Domain.Enums;
using Educar.Backend.Infrastructure.Data;
using Educar.Backend.Web.Extensions;
using Educar.Backend.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Extensions;
using NSwag;
using NSwag.Generation.Processors.Security;

namespace Educar.Backend.Web;

public static class DependencyInjection
{
    public static IServiceCollection AddWebServices(this IServiceCollection services)
    {
        services.AddScoped<IUser, CurrentUser>();

        services.AddHttpContextAccessor();

        services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>();

        services.AddEndpointsApiExplorer();

        //Nswag
        services.AddOpenApiDocument((configure, sp) =>
        {
            configure.Title = "Educar.Backend API";

            // Add JWT
            configure.AddSecurity("JWT", [], new OpenApiSecurityScheme
            {
                Type = OpenApiSecuritySchemeType.ApiKey,
                Name = "Authorization",
                In = OpenApiSecurityApiKeyLocation.Header,
                Description = "Type into the textbox: Bearer {your JWT token}."
            });

            configure.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
        });

        services.AddExceptionHandler<CustomExceptionHandler>();

        services.Configure<ApiBehaviorOptions>(options =>
            options.SuppressModelStateInvalidFilter = true);

        // services.AddRazorPages();
        services.AddControllers();


        services.AddHttpClient();

        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.SerializerOptions.Converters.Add(new ExpectedAnswerJsonConverter());
            options.SerializerOptions.PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy();
        });

        services.Configure<JsonOptions>(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.JsonSerializerOptions.Converters.Add(new ExpectedAnswerJsonConverter());
        });

        services.AddAuthorizationBuilder()
            .AddPolicy(UserRole.Admin.GetDisplayName(), policy =>
                policy.RequireAssertion(context => context.User.HasRoles([UserRole.Admin])));

        services.AddAuthorizationBuilder()
            .AddPolicy(UserRole.Teacher.GetDisplayName(), policy =>
                policy.RequireAssertion(context => context.User.HasRoles([UserRole.Teacher, UserRole.Admin])));

        services.AddAuthorizationBuilder()
            .AddPolicy(UserRole.Student.GetDisplayName(), policy =>
                policy.RequireAssertion(context =>
                    context.User.HasRoles([UserRole.Student, UserRole.Teacher, UserRole.Admin])));

        return services;
    }
}