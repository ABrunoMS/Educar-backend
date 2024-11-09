using Educar.Backend.Application.Commands;
using Educar.Backend.Application.Commands.School.DeleteSchool;
using Educar.Backend.Application.Commands.School.UpdateSchool;
using Educar.Backend.Application.Commands.SpawnPoint.CreateSpawnPoint;
using Educar.Backend.Application.Common.Models;
using Educar.Backend.Application.Queries.School;
using Educar.Backend.Domain.Enums;
using Microsoft.OpenApi.Extensions;

namespace Educar.Backend.Web.Endpoints;

public class SpawnPoints : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization(UserRole.Admin.GetDisplayName())
            .MapPost(CreateSpawnPoint);
        // .MapGet(GetAllSchools)
        // .MapGet(GetAllSchoolsByClient, "client/{clientId}")
        // .MapGet(GetSchool, "{id}")
        // .MapPut(UpdateSchool, "{id}")
        // .MapDelete(DeleteSchool, "{id}");
    }

    public async Task<SchoolDto> GetSchool(ISender sender, Guid id)
    {
        return await sender.Send(new GetSchoolQuery() { Id = id });
    }

    public Task<IdResponseDto> CreateSpawnPoint(ISender sender, CreateSpawnPointCommand command)
    {
        return sender.Send(command);
    }

    public Task<PaginatedList<SchoolDto>> GetAllSchools(ISender sender,
        [AsParameters] PaginatedQuery paginatedQuery)
    {
        var query = new GetSchoolsPaginatedQuery
        {
            PageNumber = paginatedQuery.PageNumber,
            PageSize = paginatedQuery.PageSize
        };

        return sender.Send(query);
    }

    public Task<PaginatedList<SchoolDto>> GetAllSchoolsByClient(ISender sender,
        Guid clientId, [AsParameters] PaginatedQuery paginatedQuery)
    {
        var query = new GetSchoolsByClientPaginatedQuery(clientId)
        {
            PageNumber = paginatedQuery.PageNumber,
            PageSize = paginatedQuery.PageSize
        };

        return sender.Send(query);
    }

    public async Task<IResult> DeleteSchool(ISender sender, Guid id)
    {
        await sender.Send(new DeleteSchoolCommand(id));
        return Results.NoContent();
    }

    public async Task<IResult> UpdateSchool(ISender sender, Guid id, UpdateSchoolCommand command)
    {
        command.Id = id;
        await sender.Send(command);
        return Results.NoContent();
    }
}