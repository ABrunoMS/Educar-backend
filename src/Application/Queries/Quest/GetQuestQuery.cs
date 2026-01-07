using Educar.Backend.Application.Common.Interfaces;
using System.Text.Json.Nodes;

namespace Educar.Backend.Application.Queries.Quest;

public record GetQuestQuery : IRequest<QuestDto>
{
    public Guid Id { get; init; }
}

public class GetQuestQueryHandler(IApplicationDbContext context, IMapper mapper, IUser currentUser)
    : IRequestHandler<GetQuestQuery, QuestDto>
{
    public async Task<QuestDto> Handle(GetQuestQuery request, CancellationToken cancellationToken)
    {
        // Obter o ClientId do usuário atual
        var userId = currentUser.Id;
        Guid? clientId = null;
        
        if (!string.IsNullOrEmpty(userId))
        {
            var account = await context.Accounts
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id.ToString() == userId, cancellationToken);
            clientId = account?.ClientId;
        }

        var entity = await context.Quests
            .AsNoTracking()
            .Include(q => q.Subject) 
            .Include(q => q.Grade)
            .Include(q => q.Content)
            .Include(q => q.Product)
            .Include(q => q.BnccQuests)
            .ThenInclude(bq => bq.Bncc)
            .Include(q => q.QuestSteps)
            .ThenInclude(qs => qs.Contents) // Carrega os conteúdos (questões) de cada etapa
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

        Guard.Against.NotFound(request.Id, entity, nameof(Domain.Entities.Quest));

        // DEBUG: Log dos QuestSteps
        Console.WriteLine($"[DEBUG] Quest {entity.Name} tem {entity.QuestSteps?.Count ?? 0} QuestSteps");
        if (entity.QuestSteps != null)
        {
            foreach (var step in entity.QuestSteps)
            {
                Console.WriteLine($"[DEBUG]   - Step: {step.Name} (ID: {step.Id}), Contents: {step.Contents?.Count ?? 0}");
            }
        }

        // Filtrar Content e Product: só retorna se o cliente possui acesso
        if (clientId.HasValue)
        {
            // Verificar se o cliente tem acesso ao Content
            var hasContent = await context.ClientContents
                .AsNoTracking()
                .AnyAsync(cc => cc.ClientId == clientId.Value && cc.ContentId == entity.ContentId, cancellationToken);
            
            if (!hasContent)
                entity.Content = null; // Remove o Content do resultado

            // Verificar se o cliente tem acesso ao Product
            var hasProduct = await context.ClientProducts
                .AsNoTracking()
                .AnyAsync(cp => cp.ClientId == clientId.Value && cp.ProductId == entity.ProductId, cancellationToken);
            
            if (!hasProduct)
                entity.Product = null; // Remove o Product do resultado
        }
        else
        {
            // Se não há cliente, remove Content e Product
            entity.Content = null;
            entity.Product = null;
        }

        //order queststeps by order
        entity.QuestSteps = entity.QuestSteps?.OrderBy(qs => qs.Order).ToList() ?? new List<Domain.Entities.QuestStep>();
    
    var dto = mapper.Map<QuestDto>(entity);
    
    // Fix: Mapear ExpectedAnswers manualmente para evitar erro "node already has a parent"
    if (dto.QuestSteps != null)
        {
            foreach (var step in dto.QuestSteps)
            {
                if (step.Contents != null)
                {
                    foreach (var content in step.Contents)
                    {
                        var originalContent = entity.QuestSteps?
                            .FirstOrDefault(qs => qs.Id == step.Id)?
                            .Contents
                            .FirstOrDefault(c => c.Id == content.Id);
                            
                        if (originalContent?.ExpectedAnswers != null)
                        {
                            var jsonString = originalContent.ExpectedAnswers.ToJsonString();
                            content.ExpectedAnswers = JsonNode.Parse(jsonString)?.AsObject();
                        }
                    }
                }
            }
        }
        
        return dto;
    }
}
