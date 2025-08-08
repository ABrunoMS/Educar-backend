using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Commands.Item.UpdateItem;

public record UpdateItemCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Lore { get; set; }
    public ItemType? ItemType { get; set; }
    public ItemRarity? ItemRarity { get; set; }
    public decimal? SellValue { get; set; }
    public Guid? DismantleId { get; set; }
    public string? Reference2D { get; set; }
    public string? Reference3D { get; set; }
    public decimal? DropRate { get; set; }
}

public class UpdateItemCommandHandler : IRequestHandler<UpdateItemCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public UpdateItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateItemCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Items.FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);
        Guard.Against.NotFound(request.Id, entity);
        
        entity.Name = request.Name ?? entity.Name;
        entity.Lore = request.Lore ?? entity.Lore;
        entity.ItemType = request.ItemType ?? entity.ItemType;
        entity.ItemRarity = request.ItemRarity ?? entity.ItemRarity;
        entity.SellValue = request.SellValue ?? entity.SellValue;
        entity.Reference2D = request.Reference2D ?? entity.Reference2D;
        entity.Reference3D = request.Reference3D ?? entity.Reference3D;
        entity.DropRate = request.DropRate ?? entity.DropRate;

        if (request.DismantleId != null)
        {
            var dismantleItem = await _context.Items.FirstOrDefaultAsync(i => i.Id == request.DismantleId, cancellationToken);
            if (dismantleItem == null) throw new NotFoundException(nameof(Item), request.DismantleId.ToString()!);

            entity.DismantleId = request.DismantleId;
            entity.Dismantle = dismantleItem;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}