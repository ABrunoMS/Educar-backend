using Educar.Backend.Application.Commands;
using Educar.Backend.Application.Commands.Class.CreateClass;
using Educar.Backend.Application.Commands.Class.DeleteClass;
using Educar.Backend.Application.Commands.Class.UpdateClass;
using Educar.Backend.Application.Common.Models;
using Educar.Backend.Application.Queries.Class;
using Educar.Backend.Domain.Enums;
using Microsoft.OpenApi.Extensions;

namespace Educar.Backend.Web.Endpoints;

public class Classes : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization(UserRole.Admin.GetDisplayName())
            .MapPost(CreateClass)
            .MapPut(UpdateClass, "{id}")
            .MapDelete(DeleteClass, "{id}");

        app.MapGroup(this)
            .RequireAuthorization(UserRole.Student.GetDisplayName())
            .MapGet(GetClass, "{id}")
            .MapGet(GetAllClassesBySchool, "school/{schoolId}");
    }

    public Task<CreatedResponseDto> CreateClass(ISender sender, CreateClassCommand command)
    {
        return sender.Send(command);
    }

    public async Task<ClassDto> GetClass(ISender sender, Guid id)
    {
        return await sender.Send(new GetClassQuery { Id = id });
    }

    public Task<PaginatedList<ClassDto>> GetAllClassesBySchool(ISender sender,
        Guid schoolId, [AsParameters] PaginatedQuery paginatedQuery)
    {
        var query = new GetClassesBySchoolPaginatedQuery(schoolId)
        {
            PageNumber = paginatedQuery.PageNumber,
            PageSize = paginatedQuery.PageSize
        };

        return sender.Send(query);
    }

    public async Task<IResult> DeleteClass(ISender sender, Guid id)
    {
        await sender.Send(new DeleteClassCommand(id));
        return Results.NoContent();
    }

    public async Task<IResult> UpdateClass(ISender sender, Guid id, UpdateClassCommand command)
    {
        command.Id = id;
        await sender.Send(command);
        return Results.NoContent();
    }
}