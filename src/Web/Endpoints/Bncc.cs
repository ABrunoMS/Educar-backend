using Educar.Backend.Application.Queries.Bncc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Educar.Backend.Web.Endpoints;

public class Bncc : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapGet(GetAll);
    }

    public async Task<List<BnccDto>> GetAll(ISender sender)
    {
        return await sender.Send(new GetAllBnccsQuery());
    }
}