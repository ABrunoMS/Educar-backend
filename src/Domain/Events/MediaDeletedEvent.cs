using Educar.Backend.Domain.Entities;

namespace Educar.Backend.Domain.Events;

public class MediaDeletedEvent(Media media, Guid accountId) : BaseEvent
{
    public Media Media { get; } = media;
    public Guid AccountId { get; } = accountId;
}