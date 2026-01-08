using Educar.Backend.Application.Commands;
using Educar.Backend.Application.Commands.Account.CreateAccount;
using Educar.Backend.Application.Commands.Account.DeleteAccount;
using Educar.Backend.Application.Commands.Account.ResetPassword;
using Educar.Backend.Application.Commands.Account.UpdateAccount;
using Educar.Backend.Application.Commands.BulkImport.ImportAccounts;
using Educar.Backend.Application.Common.Models;
using Educar.Backend.Application.Queries.Account;
using Educar.Backend.Domain.Enums;
using Microsoft.OpenApi.Extensions;

namespace Educar.Backend.Web.Endpoints;

public class Accounts : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization(UserRole.Admin.GetDisplayName())
            .MapGet(GetAccount, "{id}")
            .MapGet(GetAccountsByRole, "role/{role}")
            .MapGet(GetAllAccountsBySchool, "school/{schoolId}")
            .MapGet(GetAllAccountsByClass, "class/{classId}")
            .MapGet(GetAllAccountsByClient, "client/{clientId}")
            .MapGet(GetAllAccounts)
            .MapPost(CreateAccount)
            .MapPut(UpdateAccount, "{id}")
            .MapDelete(DeleteAccount, "{id}");

        // Endpoint separado para upload de arquivo com DisableAntiforgery
        app.MapGroup(this)
            .RequireAuthorization(UserRole.Admin.GetDisplayName())
            .DisableAntiforgery()
            .MapPost(ImportAccounts, "import");

        app.MapGroup(this)
            .RequireAuthorization()
            .MapGet(GetMyAccount, "me");

        app.MapGroup(this)
       .AllowAnonymous() // <-- Permite acesso sem login
       .MapPut(ForgotPassword, "forgot-password/{email}");
    }

    public Task<IdResponseDto> CreateAccount(ISender sender, CreateAccountCommand command)
    {
        return sender.Send(command);
    }

    public async Task<AccountDto> GetAccount(ISender sender, Guid id)
    {
        return await sender.Send(new GetAccountQuery { Id = id });
    }

    public Task<PaginatedList<CleanAccountDto>> GetAccountsByRole(ISender sender, UserRole role,
        [AsParameters] PaginatedQuery paginatedQuery)
    {
        var query = new GetAccountsByRolePaginatedQuery
        {
            Role = role,
            PageNumber = paginatedQuery.PageNumber,
            PageSize = paginatedQuery.PageSize
        };
        return sender.Send(query);
    }


    public Task<PaginatedList<CleanAccountDto>> GetAllAccounts(ISender sender,
        [AsParameters] PaginatedQuery paginatedQuery)
    {
        var query = new GetAccountsPaginatedQuery
        {
            PageNumber = paginatedQuery.PageNumber,
            PageSize = paginatedQuery.PageSize
        };

        return sender.Send(query);
    }

    public Task<PaginatedList<CleanAccountDto>> GetAllAccountsByClient(ISender sender,
        Guid clientId, [AsParameters] PaginatedQuery paginatedQuery)
    {
        var query = new GetAccountsByClientPaginatedQuery(clientId)
        {
            PageNumber = paginatedQuery.PageNumber,
            PageSize = paginatedQuery.PageSize
        };

        return sender.Send(query);
    }

    public Task<PaginatedList<CleanAccountDto>> GetAllAccountsBySchool(ISender sender,
        Guid schoolId, [AsParameters] PaginatedQuery paginatedQuery)
    {
        var query = new GetAccountsBySchoolPaginatedQuery(schoolId)
        {
            PageNumber = paginatedQuery.PageNumber,
            PageSize = paginatedQuery.PageSize
        };

        return sender.Send(query);
    }

    public Task<PaginatedList<CleanAccountDto>> GetAllAccountsByClass(ISender sender,
        Guid classId, [AsParameters] PaginatedQuery paginatedQuery)
    {
        var query = new GetAccountsByClassPaginatedQuery(classId)
        {
            PageNumber = paginatedQuery.PageNumber,
            PageSize = paginatedQuery.PageSize
        };

        return sender.Send(query);
    }

    public async Task<IResult> DeleteAccount(ISender sender, Guid id)
    {
        await sender.Send(new DeleteAccountCommand(id));
        return Results.NoContent();
    }

    public async Task<IResult> UpdateAccount(ISender sender, Guid id, UpdateAccountCommand command)
    {
        command.Id = id;
        await sender.Send(command);
        return Results.NoContent();
    }

    public async Task<IResult> ForgotPassword(ISender sender, string email)
    {
        await sender.Send(new ForgotPasswordCommand(email));
        return Results.NoContent();
    }

    public async Task<AccountDto> GetMyAccount(ISender sender)
    {
        return await sender.Send(new GetMyAccountQuery());
    }

    public async Task<IResult> ImportAccounts(ISender sender, HttpRequest request)
    {
        if (!request.HasFormContentType)
        {
            return Results.BadRequest(new { Success = false, ErrorMessage = "O request deve ser multipart/form-data." });
        }

        var form = await request.ReadFormAsync();
        
        // Tenta pegar o primeiro arquivo, independente do nome do campo
        var file = form.Files.FirstOrDefault();

        if (file == null || file.Length == 0)
        {
            // Debug: mostra quantos arquivos foram enviados
            var fileCount = form.Files.Count;
            var fieldNames = string.Join(", ", form.Files.Select(f => f.Name));
            return Results.BadRequest(new 
            { 
                Success = false, 
                ErrorMessage = $"Arquivo não fornecido ou vazio. Arquivos recebidos: {fileCount}. Campos: [{fieldNames}]" 
            });
        }

        using var stream = file.OpenReadStream();
        var command = new ImportAccountsCommand(stream, file.FileName);
        var result = await sender.Send(command);

        if (result.Success)
        {
            return Results.Ok(result);
        }

        return Results.BadRequest(result);
    }
}
