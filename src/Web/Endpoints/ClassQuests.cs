                                                  using Educar.Backend.Application.Commands;
using Educar.Backend.Application.Commands.ClassQuest.CreateClassQuest;
using Educar.Backend.Application.Commands.ClassQuest.UpdateClassQuest;
using Educar.Backend.Application.Commands.ClassQuest.DeleteClassQuest;
using Educar.Backend.Application.Commands.BulkImport.LinkQuestsToClasses;
using Educar.Backend.Application.Queries.ClassQuest;
using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Web.Endpoints;

public record UpdateClassQuestRequest(string? StartDate, string ExpirationDate);

public class ClassQuests : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization(UserRole.Teacher.ToString())
            .MapPost(CreateClassQuest)
            .MapPut(UpdateClassQuest, "{id}")
            .MapDelete(DeleteClassQuest, "{id}")
            .MapGet(GetClassQuestById, "{id}")
            .MapGet(GetClassQuests);

        // Endpoint separado para upload de arquivo com DisableAntiforgery
        app.MapGroup(this)
            .RequireAuthorization(UserRole.Teacher.ToString())
            .DisableAntiforgery()
            .MapPost(LinkQuestsToClasses, "import");
    }

    public Task<IdResponseDto> CreateClassQuest(ISender sender, CreateClassQuestCommand command)
    {
        return sender.Send(command);
    }

    public async Task<IResult> UpdateClassQuest(ISender sender, Guid id, UpdateClassQuestRequest request)
    {
        var command = new UpdateClassQuestCommand(id, request.StartDate, request.ExpirationDate);
        await sender.Send(command);
        return Results.NoContent();
    }

    public async Task<IResult> DeleteClassQuest(ISender sender, Guid id)
    {
        await sender.Send(new DeleteClassQuestCommand(id));
        return Results.NoContent();
    }

    public async Task<ClassQuestDto> GetClassQuestById(ISender sender, Guid id)
    {
        return await sender.Send(new GetClassQuestByIdQuery { Id = id });
    }

    public Task<List<ClassQuestDto>> GetClassQuests(ISender sender, Guid? classId = null, Guid? questId = null)
    {
        return sender.Send(new GetClassQuestsQuery { ClassId = classId, QuestId = questId });
    }

    public async Task<IResult> LinkQuestsToClasses(ISender sender, HttpRequest request)
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
        var command = new LinkQuestsToClassesCommand(stream, file.FileName);
        var result = await sender.Send(command);

        if (result.Success)
        {
            return Results.Ok(result);
        }

        return Results.BadRequest(result);
    }
}
