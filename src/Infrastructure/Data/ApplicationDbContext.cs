using System.Linq.Expressions;
using System.Reflection;
using Educar.Backend.Application.Interfaces;
using Educar.Backend.Domain.Entities;
using Educar.Backend.Infrastructure.Extensions;
using Humanizer;
using Microsoft.EntityFrameworkCore;

namespace Educar.Backend.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options), IApplicationDbContext
{
    public DbSet<TodoList> TodoLists => Set<TodoList>();

    public DbSet<TodoItem> TodoItems => Set<TodoItem>();

    public DbSet<Contract> Contracts => Set<Contract>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);

        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            var tableName = entityType.DisplayName();
            entityType.SetTableName(tableName.ToSnakeCase().Singularize().ToLower());

            foreach (var property in entityType.GetProperties())
                property.SetColumnName(property.Name.ToSnakeCase().ToLower());
        }

        var softDeleteEntities = typeof(ISoftDelete).Assembly.GetTypes()
            .Where(type => typeof(ISoftDelete)
                               .IsAssignableFrom(type)
                           && type.IsClass
                           && !type.IsAbstract);

        foreach (var softDeleteEntity in softDeleteEntities)
        {
            builder.Entity(softDeleteEntity).HasQueryFilter(
                GenerateQueryFilterLambda(softDeleteEntity));
        }
    }

    private LambdaExpression GenerateQueryFilterLambda(Type type)
    {
        var parameter = Expression.Parameter(type, "w");
        var falseConstantValue = Expression.Constant(false);
        var propertyAccess = Expression.PropertyOrField(parameter, nameof(ISoftDelete.IsDeleted));
        var equalExpression = Expression.Equal(propertyAccess, falseConstantValue);
        var lambda = Expression.Lambda(equalExpression, parameter);

        return lambda;
    }
}