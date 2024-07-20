using System.Text.Json;
using System.Text.Json.Nodes;
using Educar.Backend.Domain.Entities;

namespace Educar.Backend.Domain.Events;

public class MediaUpdatedEvent(Media media, Guid accountId, JsonObject previousState) : BaseEvent
{
    public Media Media { get; } = media;
    public Guid AccountId { get; } = accountId;
    public JsonObject PreviousState { get; } = previousState;
}