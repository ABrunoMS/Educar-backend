using Educar.Backend.Application.Commands;
using Educar.Backend.Application.Commands.BulkImport.ImportClasses;
using Educar.Backend.Application.Commands.Class.CreateClass;
using Educar.Backend.Application.Commands.Class.DeleteClass;
using Educar.Backend.Application.Commands.Class.UpdateClass;
using Educar.Backend.Application.Common.Models;
using Educar.Backend.Application.Queries.Class;
using Educar.Backend.Domain.Enums;
using Microsoft.OpenApi.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Educar.Backend.Web.Endpoints;

public class Classes : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {       
        app.MapGroup(this)
            .RequireAuthorization(UserRole.Teacher.ToString())
            .MapPut(UpdateClass, "{id}")
            .MapGet(GetClass, "{id}")
            .MapGet(GetAllClassesBySchool, "schools/{schoolIds}")
            .MapDelete(DeleteClass, "{id}")
            .MapPost(GetClassesBySchools, "by-schools")
            .MapPost(CreateClass);
        
        // Endpoint separado para upload de arquivo com DisableAntiforgery
        app.MapGroup(this)
            .RequireAuthorization(UserRole.Teacher.ToString())
            .DisableAntiforgery()
            .MapPost(ImportClasses, "import");
            
        app.MapGroup(this)
            .RequireAuthorization(UserRole.Teacher.ToString())
            .MapGet(GetAllClasses);            
    }

    public Task<PaginatedList<ClassDto>> GetAllClasses(ISender sender, [AsParameters] GetClassesPaginatedQuery query)
    {
        return sender.Send(query);
    }

    public Task<IdResponseDto> CreateClass(ISender sender, CreateClassCommand command)
    {
        return sender.Send(command);
    }

    public async Task<ClassDto> GetClass(ISender sender, Guid id)
    {
        return await sender.Send(new GetClassQuery { Id = id });
    }

    public Task<PaginatedList<ClassDto>> GetAllClassesBySchool(ISender sender,
        Guid schoolId, [AsParameters] PaginatedQuery paginatedQuery)
    {
        var query = new GetClassesBySchoolPaginatedQuery(schoolId)
        {
            PageNumber = paginatedQuery.PageNumber,
            PageSize = paginatedQuery.PageSize
        };

        return sender.Send(query);
    }

    public async Task<IResult> DeleteClass(ISender sender, Guid id)
    {
        await sender.Send(new DeleteClassCommand(id));
        return Results.NoContent();
    }

    public async Task<IResult> UpdateClass(ISender sender, Guid id, UpdateClassCommand command)
    {
        command.Id = id;
        await sender.Send(command);
        return Results.NoContent();
    }
     
    public async Task<List<ClassDto>> GetClassesBySchools(ISender sender, [FromBody] GetClassesBySchoolsQuery query)
    {
        return await sender.Send(query);
    }

    public async Task<IResult> ImportClasses(ISender sender, HttpRequest request)
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
        var command = new ImportClassesCommand(stream, file.FileName);
        var result = await sender.Send(command);

        if (result.Success)
        {
            return Results.Ok(result);
        }

        return Results.BadRequest(result);
    }
}
