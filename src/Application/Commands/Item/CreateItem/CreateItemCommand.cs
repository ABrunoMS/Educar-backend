using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Commands.Item.CreateItem;

public record CreateItemCommand(
    string Name,
    string Lore,
    ItemType ItemType,
    ItemRarity ItemRarity,
    decimal SellValue,
    string Reference2D,
    string Reference3D,
    decimal DropRate)
    : IRequest<CreatedResponseDto>
{
    public string Name { get; set; } = Name;
    public string Lore { get; set; } = Lore;
    public ItemType ItemType { get; set; } = ItemType;
    public ItemRarity ItemRarity { get; set; } = ItemRarity;
    public decimal SellValue { get; set; } = SellValue;
    public Guid? DismantleId { get; set; }
    public string Reference2D { get; set; } = Reference2D;
    public string Reference3D { get; set; } = Reference3D;
    public decimal DropRate { get; set; } = DropRate;
}

public class CreateItemCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CreateItemCommand, CreatedResponseDto>
{
    public async Task<CreatedResponseDto> Handle(CreateItemCommand request, CancellationToken cancellationToken)
    {
        Domain.Entities.Item? dismantleItem = null;
        if (request.DismantleId != null && request.DismantleId != Guid.Empty)
        {
            dismantleItem = await context.Items.FindAsync([request.DismantleId], cancellationToken: cancellationToken);
            if (dismantleItem == null)
                throw new NotFoundException(nameof(Domain.Entities.Item), request.DismantleId.ToString()!);
        }

        var entity = new Domain.Entities.Item(request.Name, request.Lore, request.ItemType, request.ItemRarity,
            request.SellValue, request.Reference2D, request.Reference3D, request.DropRate)
        {
            Dismantle = dismantleItem
        };

        context.Items.Add(entity);
        await context.SaveChangesAsync(cancellationToken);

        return new CreatedResponseDto(entity.Id);
    }
}