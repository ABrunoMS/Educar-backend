using Educar.Backend.Application.Commands;
using Educar.Backend.Application.Commands.Grade.CreateGradeCommand;
using Educar.Backend.Application.Commands.Grade.DeleteGrade;
using Educar.Backend.Application.Commands.Grade.UpdateGrade;
using Educar.Backend.Application.Common.Models;
using Educar.Backend.Application.Queries.Grade;
using Educar.Backend.Domain.Enums;
using Microsoft.OpenApi.Extensions;

namespace Educar.Backend.Web.Endpoints;

public class Grades : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            
            .MapPost(CreateGrade)
            .MapGet(GetGrade, "{id}")
            .MapGet(GetAllGrades)
            .MapPut(UpdateGrade, "{id}")
            .MapDelete(DeleteGrade, "{id}");
    }

    public Task<IdResponseDto> CreateGrade(ISender sender, CreateGradeCommand command)
    {
        return sender.Send(command);
    }

    public async Task<GradeDto> GetGrade(ISender sender, Guid id)
    {
        return await sender.Send(new GetGradeQuery { Id = id });
    }

    public Task<PaginatedList<GradeDto>> GetAllGrades(ISender sender, [AsParameters] PaginatedQuery paginatedQuery)
    {
        var query = new GetGradesPaginatedQuery
        {
            PageNumber = paginatedQuery.PageNumber,
            PageSize = paginatedQuery.PageSize
        };

        return sender.Send(query);
    }

    public async Task<IResult> DeleteGrade(ISender sender, Guid id)
    {
        await sender.Send(new DeleteGradeCommand(id));
        return Results.NoContent();
    }

    public async Task<IResult> UpdateGrade(ISender sender, Guid id, UpdateGradeCommand command)
    {
        command.Id = id;
        await sender.Send(command);
        return Results.NoContent();
    }
}