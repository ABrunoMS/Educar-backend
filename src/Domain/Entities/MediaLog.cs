using System.Text.Json;
using System.Text.Json.Nodes;
using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Domain.Entities;

public class MediaLog(AuditableAction action, JsonObject currentState) : BaseAuditableEntity
{
    public Guid AccountId { get; set; }
    public Account Account { get; set; } = null!;
    public Guid MediaId { get; set; }
    public Media Media { get; set; } = null!;
    public AuditableAction Action { get; set; } = action;
    public JsonObject CurrentState { get; set; } = currentState;
    public JsonObject? PreviousState { get; set; }
}