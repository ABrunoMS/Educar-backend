using System.Linq.Expressions;
using System.Reflection;
using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Application.Extensions;
using Educar.Backend.Domain.Entities;
using Educar.Backend.Infrastructure.Data.Extensions;
using Humanizer;
using Microsoft.EntityFrameworkCore;

namespace Educar.Backend.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options), IApplicationDbContext
{
    public DbSet<Contract> Contracts => Set<Contract>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Game> Games => Set<Game>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<School> Schools => Set<School>();
    public DbSet<Media> Medias => Set<Media>();
    public DbSet<MediaLog> MediaLogs => Set<MediaLog>();
    public DbSet<Class> Classes => Set<Class>();
    public DbSet<AccountClass> AccountClasses => Set<AccountClass>();
    public DbSet<Grade> Grades => Set<Grade>();
    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<GameSubject> GameSubjects => Set<GameSubject>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyCustomConfigurationsFromAssembly(Assembly.GetExecutingAssembly(), Database);

        // builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);

        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            var tableName = entityType.DisplayName();
            entityType.SetTableName(tableName.ToSnakeCase().Pluralize().ToLower());

            foreach (var property in entityType.GetProperties())
                property.SetColumnName(property.Name.ToSnakeCase().ToLower());
        }

        var softDeleteEntities = typeof(ISoftDelete).Assembly.GetTypes()
            .Where(type => typeof(ISoftDelete).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract);

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