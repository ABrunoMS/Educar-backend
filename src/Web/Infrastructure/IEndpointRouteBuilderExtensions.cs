using System.Diagnostics.CodeAnalysis;

namespace Educar.Backend.Web.Infrastructure;

public static class IEndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapGet(this IEndpointRouteBuilder builder, Delegate handler,
        [StringSyntax("Route")] string pattern = "")
    {
        Guard.Against.AnonymousMethod(handler);

        builder.MapGet(pattern, handler)
            .WithName(handler.Method.Name);

        return builder;
    }

    public static IEndpointRouteBuilder MapPost(this IEndpointRouteBuilder builder, Delegate handler,
        [StringSyntax("Route")] string pattern = "")
    {
        Guard.Against.AnonymousMethod(handler);

        builder.MapPost(pattern, handler)
            .WithName(handler.Method.Name);

        return builder;
    }

    public static IEndpointRouteBuilder MapPostWithAccepts<TContentType>(this IEndpointRouteBuilder builder,
        Delegate handler, [StringSyntax("Route")] string pattern = "", string mediaType = "multipart/form-data")
        where TContentType : notnull
    {
        Guard.Against.AnonymousMethod(handler);

        builder.MapPost(pattern, handler)
            .WithName(handler.Method.Name)
            .Accepts<TContentType>(mediaType)
            .DisableAntiforgery();

        return builder;
    }

    public static IEndpointRouteBuilder MapPut(this IEndpointRouteBuilder builder, Delegate handler,
        [StringSyntax("Route")] string pattern)
    {
        Guard.Against.AnonymousMethod(handler);

        builder.MapPut(pattern, handler)
            .WithName(handler.Method.Name);

        return builder;
    }

    public static IEndpointRouteBuilder MapDelete(this IEndpointRouteBuilder builder, Delegate handler,
        [StringSyntax("Route")] string pattern)
    {
        Guard.Against.AnonymousMethod(handler);

        builder.MapDelete(pattern, handler)
            .WithName(handler.Method.Name);

        return builder;
    }
}