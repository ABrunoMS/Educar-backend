using Educar.Backend.Application.Queries.BnccQuest;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Educar.Backend.Web.Endpoints;

public class BnccQuest : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapGet(GetBnccsByQuestId);
    }

    public async Task<List<BnccQuestDto>> GetBnccsByQuestId(ISender sender, Guid questId)
    {
        return await sender.Send(new GetBnccsQuestQuery { QuestId = questId });
    }
}
