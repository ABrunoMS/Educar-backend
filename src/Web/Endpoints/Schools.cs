using Educar.Backend.Application.Commands;
using Educar.Backend.Application.Commands.School.CreateSchool;
using Educar.Backend.Application.Commands.School.DeleteSchool;
using Educar.Backend.Application.Commands.School.UpdateSchool;
using Educar.Backend.Application.Commands.School.AddAccountToSchool;
using Educar.Backend.Application.Commands.School.RemoveAccountFromSchool;
using Educar.Backend.Application.Common.Models;
using Educar.Backend.Application.Queries.School;
using Educar.Backend.Application.Queries.Account;
using Educar.Backend.Domain.Enums;
using Microsoft.OpenApi.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Educar.Backend.Web.Endpoints;

public class Schools : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization(UserRole.Admin.GetDisplayName())
            .MapPost(CreateSchool)
            .MapGet(GetAllSchools)
            .MapPut(UpdateSchool, "{id}")
            .MapDelete(DeleteSchool, "{id}")
            .MapGet(GetAccountsBySchool, "{schoolId}/accounts")
            .MapPost(AddAccountToSchool, "{schoolId}/accounts/{accountId}")
            .MapDelete(RemoveAccountFromSchool, "{schoolId}/accounts/{accountId}");

        app.MapGroup(this)
            .RequireAuthorization(UserRole.Teacher.GetDisplayName())       
            .MapGet(GetAllSchoolsByClient, "client/{clientId}")
            .MapGet(GetSchool, "{id}");      
    }

    public async Task<SchoolDto> GetSchool(ISender sender, Guid id)
    {
        return await sender.Send(new GetSchoolQuery() { Id = id });
    }

    public Task<IdResponseDto> CreateSchool(ISender sender, CreateSchoolCommand command)
    {
        return sender.Send(command);
    }

    public Task<PaginatedList<SchoolDto>> GetAllSchools(
        ISender sender,
        [FromQuery(Name = "PageNumber")] int PageNumber,
        [FromQuery(Name = "PageSize")] int PageSize)
    {
        var query = new GetSchoolsPaginatedQuery
        {
            PageNumber = PageNumber,
            PageSize = PageSize
        };

        return sender.Send(query);
    }

    public Task<PaginatedList<SchoolDto>> GetAllSchoolsByClient(
        ISender sender,
        Guid clientId,
        [FromQuery(Name = "PageNumber")] int PageNumber,
        [FromQuery(Name = "PageSize")] int PageSize)
    {
        var query = new GetSchoolsByClientPaginatedQuery(clientId)
        {
            PageNumber = PageNumber,
            PageSize = PageSize
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

    public async Task<IResult> AddAccountToSchool(ISender sender, Guid schoolId, Guid accountId)
    {
        var account = await sender.Send(new GetAccountQuery { Id = accountId });
        if (account == null)
        {
            return Results.NotFound($"Account with ID {accountId} not found");
        }

        var school = await sender.Send(new GetSchoolQuery { Id = schoolId });
        if (school == null)
        {
            return Results.NotFound($"School with ID {schoolId} not found");
        }

        // Adicionar o relacionamento AccountSchool
        var command = new AddAccountToSchoolCommand(schoolId, accountId);
        await sender.Send(command);
        
        return Results.Ok(new { message = "Account added to school successfully" });
    }

    public Task<PaginatedList<AccountDto>> GetAccountsBySchool(
        ISender sender,
        Guid schoolId,
        [FromQuery(Name = "PageNumber")] int PageNumber,
        [FromQuery(Name = "PageSize")] int PageSize,
        [FromQuery(Name = "SearchTerm")] string? SearchTerm)
    {
        var query = new GetAccountsBySchoolQuery(schoolId)
        {
            PageNumber = PageNumber,
            PageSize = PageSize,
            SearchTerm = SearchTerm ?? string.Empty
        };

        return sender.Send(query);
    }

    public async Task<IResult> RemoveAccountFromSchool(ISender sender, Guid schoolId, Guid accountId)
    {
        var command = new RemoveAccountFromSchoolCommand(schoolId, accountId);
        await sender.Send(command);
        return Results.NoContent();
    }
}