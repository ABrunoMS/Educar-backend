using Educar.Backend.Application.Commands;
using Educar.Backend.Application.Commands.Media.CreateMedia;
using Educar.Backend.Application.Commands.Media.UploadFileCommand;
using Educar.Backend.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Extensions;

namespace Educar.Backend.Web.Endpoints;

public class Medias : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization(UserRole.Teacher.GetDisplayName())
            .MapPostWithAccepts<IFormFile>(UploadMedia, "upload")
            .MapPost(CreateMedia);
        // .MapGet(GetAccount, "{id}")
        // .MapGet(GetAllAccountsBySchool, "school/{clientId}")
        // .MapGet(GetAllAccounts)
        // .MapPut(UpdateAccount, "{id}")
        // .MapDelete(DeleteAccount, "{id}");
    }

    public Task<UploadResponseDto> UploadMedia(ISender sender, [FromForm] IFormFile file)
    {
        var extension = file.FileName.Split('.').Last();
        var command = new UploadFileCommand(file.OpenReadStream(), extension);
        return sender.Send(command);
    }

    public Task<CreatedResponseDto> CreateMedia(ISender sender, CreateMediaCommand command)
    {
        return sender.Send(command);
    }

    // public async Task<AccountDto> GetAccount(ISender sender, Guid id)
    // {
    //     return await sender.Send(new GetAccountQuery { Id = id });
    // }
    //
    // public Task<PaginatedList<AccountDto>> GetAllAccounts(ISender sender, [AsParameters] PaginatedQuery paginatedQuery)
    // {
    //     var query = new GetAccountsPaginatedQuery
    //     {
    //         PageNumber = paginatedQuery.PageNumber,
    //         PageSize = paginatedQuery.PageSize
    //     };
    //
    //     return sender.Send(query);
    // }
    //
    // public Task<PaginatedList<AccountDto>> GetAllAccountsBySchool(ISender sender,
    //     Guid clientId, [AsParameters] PaginatedQuery paginatedQuery)
    // {
    //     var query = new GetAccountsBySchoolPaginatedQuery(clientId)
    //     {
    //         PageNumber = paginatedQuery.PageNumber,
    //         PageSize = paginatedQuery.PageSize
    //     };
    //
    //     return sender.Send(query);
    // }
    //
    // public async Task<IResult> DeleteAccount(ISender sender, Guid id)
    // {
    //     await sender.Send(new DeleteAccountCommand(id));
    //     return Results.NoContent();
    // }
    //
    // public async Task<IResult> UpdateAccount(ISender sender, Guid id, UpdateAccountCommand command)
    // {
    //     command.Id = id;
    //     await sender.Send(command);
    //     return Results.NoContent();
    // }
}