using Educar.Backend.Application.Commands;
using Educar.Backend.Application.Commands.Item.CreateItem;
using Educar.Backend.Application.Commands.Item.DeleteItem;
using Educar.Backend.Application.Commands.Item.UpdateItem;
using Educar.Backend.Application.Common.Models;
using Educar.Backend.Application.Queries.Item;
using Educar.Backend.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Extensions;

namespace Educar.Backend.Web.Endpoints;

public class Items : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization(UserRole.Teacher.GetDisplayName())
            .MapPost(CreateItem)
            .MapGet(GetItem, "{id}")
            .MapGet(GetAllItemsByName)
            .MapPut(UpdateItem, "{id}")
            .MapDelete(DeleteItem, "{id}");
    }

    public Task<IdResponseDto> CreateItem(ISender sender, CreateItemCommand command)
    {
        return sender.Send(command);
    }

    public async Task<ItemDto> GetItem(ISender sender, Guid id)
    {
        return await sender.Send(new GetItemQuery { Id = id });
    }

    public Task<PaginatedList<ItemDto>> GetAllItemsByName(ISender sender, [FromQuery(Name = "name")] string name,
        [AsParameters] PaginatedQuery paginatedQuery)
    {
        var query = new GetItemsByNamePaginatedQuery(name)
        {
            PageNumber = paginatedQuery.PageNumber,
            PageSize = paginatedQuery.PageSize
        };

        return sender.Send(query);
    }

    public async Task<IResult> DeleteItem(ISender sender, Guid id)
    {
        await sender.Send(new DeleteItemCommand(id));
        return Results.NoContent();
    }

    public async Task<IResult> UpdateItem(ISender sender, Guid id, UpdateItemCommand command)
    {
        command.Id = id;
        await sender.Send(command);
        return Results.NoContent();
    }
}