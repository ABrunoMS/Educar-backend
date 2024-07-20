using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Educar.Backend.Infrastructure.Data.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyCustomConfigurationsFromAssembly(this ModelBuilder modelBuilder, Assembly assembly,
        DatabaseFacade database)
    {
        // Get all types in the assembly that implement IEntityTypeConfiguration<> and are not abstract or interfaces
        var configurations = assembly.GetTypes()
            .Where(t => t.GetInterfaces().Any(i =>
                            i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>)) &&
                        t is { IsAbstract: false, IsInterface: false })
            .ToList();

        // Iterate through the configurations and apply each one to the modelBuilder
        foreach (var configurationType in configurations)
        {
            // Create an instance of the configuration type, passing the database facade to the constructor
            var instance = Activator.CreateInstance(configurationType, database);

            // Get the ApplyConfiguration method from ModelBuilder
            var applyConfigurationMethod = typeof(ModelBuilder)
                .GetMethod("ApplyConfiguration", BindingFlags.Instance | BindingFlags.Public)
                ?.MakeGenericMethod(configurationType.GetInterfaces().First().GenericTypeArguments.First());

            // Invoke the ApplyConfiguration method with the instance of the configuration
            applyConfigurationMethod?.Invoke(modelBuilder, [instance]);
        }
    }
}