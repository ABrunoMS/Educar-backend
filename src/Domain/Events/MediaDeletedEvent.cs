using Educar.Backend.Domain.Entities;

namespace Educar.Backend.Domain.Events;

public class MediaDeletedEvent(Media media) : BaseEvent
{
    public Media Media { get; } = media;
}