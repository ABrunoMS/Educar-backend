using Educar.Backend.Application.Commands;
using Educar.Backend.Application.Commands.Answer.CreateAnswer;
using Educar.Backend.Application.Commands.Answer.DeleteAnswer;
using Educar.Backend.Application.Commands.Answer.UpdateAnswer;
using Educar.Backend.Application.Common.Models;
using Educar.Backend.Application.Queries.Answer;
using Educar.Backend.Domain.Enums;
using Microsoft.OpenApi.Extensions;

namespace Educar.Backend.Web.Endpoints;

public class Answers : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization(UserRole.Student.GetDisplayName())
            .MapPost(CreateAnswer)
            .MapGet(GetAllAnswerByAccount, "account/{accountId}")
            .MapGet(GetAnswer, "{id}")
            .MapPut(UpdateAnswer, "{id}")
            .MapDelete(DeleteAnswer, "{id}");
    }

    public async Task<AnswerDto> GetAnswer(ISender sender, Guid id)
    {
        return await sender.Send(new GetAnswerQuery { Id = id });
    }

    public Task<IdResponseDto> CreateAnswer(ISender sender, CreateAnswerCommand command)
    {
        return sender.Send(command);
    }

    public Task<PaginatedList<AnswerDto>> GetAllAnswerByAccount(ISender sender,
        [AsParameters] PaginatedQuery paginatedQuery, Guid accountId)
    {
        var query = new GetAnswerByAccountPaginatedQuery(accountId)
        {
            PageNumber = paginatedQuery.PageNumber,
            PageSize = paginatedQuery.PageSize
        };

        return sender.Send(query);
    }

    public async Task<IResult> DeleteAnswer(ISender sender, Guid id)
    {
        await sender.Send(new DeleteAnswerCommand(id));
        return Results.NoContent();
    }

    public async Task<IResult> UpdateAnswer(ISender sender, Guid id, UpdateAnswerCommand command)
    {
        command.Id = id;
        await sender.Send(command);
        return Results.NoContent();
    }
}