using Educar.Backend.Application.Common.Models;
using Educar.Backend.Application.Queries.MediaLog;
using Educar.Backend.Domain.Enums;
using Microsoft.OpenApi.Extensions;

namespace Educar.Backend.Web.Endpoints;

public class MediaLogs : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization(UserRole.Teacher.GetDisplayName())
            .MapGet(GetAllMediaLogsByMedia, "media/{mediaId}");
    }

    public Task<PaginatedList<MediaLogDto>> GetAllMediaLogsByMedia(ISender sender,
        Guid mediaId, [AsParameters] PaginatedQuery paginatedQuery)
    {
        var query = new GetMediaLogByMediaIdPaginatedQuery(mediaId)
        {
            PageNumber = paginatedQuery.PageNumber,
            PageSize = paginatedQuery.PageSize
        };

        return sender.Send(query);
    }
}