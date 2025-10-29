using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Educar.Backend.Infrastructure.Data.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyCustomConfigurationsFromAssembly(
        this ModelBuilder modelBuilder,
        Assembly assembly,
        DatabaseFacade database)
    {
        var configurations = assembly.GetTypes()
            .Where(t => t.GetInterfaces().Any(i =>
                            i.IsGenericType &&
                            i.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>)) &&
                        t is { IsAbstract: false, IsInterface: false })
            .ToList();

        foreach (var configurationType in configurations)
        {
            object? instance;

            // 🔍 tenta encontrar um construtor que receba DatabaseFacade
            var ctorWithDb = configurationType
                .GetConstructors()
                .FirstOrDefault(c =>
                {
                    var parameters = c.GetParameters();
                    return parameters.Length == 1 && parameters[0].ParameterType == typeof(DatabaseFacade);
                });

            // cria a instância de acordo com o construtor disponível
            instance = ctorWithDb != null
                ? Activator.CreateInstance(configurationType, database)
                : Activator.CreateInstance(configurationType);

            // aplica a configuração normalmente
            var entityType = configurationType.GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>))
                .GenericTypeArguments.First();

            var applyConfigMethod = typeof(ModelBuilder)
                .GetMethod(nameof(ModelBuilder.ApplyConfiguration))
                ?.MakeGenericMethod(entityType);

            applyConfigMethod?.Invoke(modelBuilder, new[] { instance! });
        }
    }
}
