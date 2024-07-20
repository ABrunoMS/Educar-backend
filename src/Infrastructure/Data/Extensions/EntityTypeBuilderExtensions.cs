using System.Text.Json.Nodes;
using Educar.Backend.Application.Common.Extensions;
using Educar.Backend.Infrastructure.Data.Comparers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Educar.Backend.Infrastructure.Data.Extensions;

public static class EntityTypeBuilderExtensions
{
    private const string NpgsqlEntityframeworkcorePostgresql = "Npgsql.EntityFrameworkCore.PostgreSQL";

    public static PropertyBuilder<JsonObject> ConfigureJsonProperty(
        this EntityTypeBuilder builder,
        string propertyName,
        DatabaseFacade database)
    {
        var propertyBuilder = builder.Property<JsonObject>(propertyName);

        propertyBuilder
            .HasConversion(
                v => JsonObjectExtensions.JsonObjectToString(v),
                v => JsonObjectExtensions.StringToJsonObject(v))
            .Metadata.SetValueComparer(JsonObjectValueComparer.Instance);

        if (database.ProviderName == NpgsqlEntityframeworkcorePostgresql)
        {
            // Use jsonb for PostgreSQL
            propertyBuilder.HasColumnType("jsonb");
        }

        return propertyBuilder;
    }
}