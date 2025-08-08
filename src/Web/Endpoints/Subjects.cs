using Educar.Backend.Application.Commands;
using Educar.Backend.Application.Commands.Subject;
using Educar.Backend.Application.Commands.Subject.CreateSubject;
using Educar.Backend.Application.Commands.Subject.DeleteSubject;
using Educar.Backend.Application.Commands.Subject.UpdateSubject;
using Educar.Backend.Application.Common.Models;
using Educar.Backend.Application.Queries.Subject;
using Educar.Backend.Domain.Enums;
using Microsoft.OpenApi.Extensions;

namespace Educar.Backend.Web.Endpoints;

public class Subjects : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization(UserRole.Teacher.GetDisplayName())
            .MapPost(CreateSubject)
            .MapGet(GetSubject, "{id}")
            .MapGet(GetAllSubjects)
            .MapPut(UpdateSubject, "{id}")
            .MapDelete(DeleteSubject, "{id}");
    }

    public Task<IdResponseDto> CreateSubject(ISender sender, CreateSubjectCommand command)
    {
        return sender.Send(command);
    }

    public async Task<SubjectDto> GetSubject(ISender sender, Guid id)
    {
        return await sender.Send(new GetSubjectQuery { Id = id });
    }

    public Task<PaginatedList<SubjectDto>> GetAllSubjects(ISender sender, [AsParameters] PaginatedQuery paginatedQuery)
    {
        var query = new GetSubjectsPaginatedQuery
        {
            PageNumber = paginatedQuery.PageNumber,
            PageSize = paginatedQuery.PageSize
        };

        return sender.Send(query);
    }

    public async Task<IResult> DeleteSubject(ISender sender, Guid id)
    {
        await sender.Send(new DeleteSubjectCommand(id));
        return Results.NoContent();
    }

    public async Task<IResult> UpdateSubject(ISender sender, Guid id, UpdateSubjectCommand command)
    {
        command.Id = id;
        await sender.Send(command);
        return Results.NoContent();
    }
}