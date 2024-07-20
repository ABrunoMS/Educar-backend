using System.Text.Json.Nodes;
using Educar.Backend.Application.Queries.Account;
using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Queries.MediaLog;

public class MediaLogDto
{
    public AccountDto? Account { get; set; }
    public AuditableAction? Action { get; set; }
    public JsonObject? CurrentState { get; set; }
    public JsonObject? PreviousState { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Domain.Entities.MediaLog, MediaLogDto>();
        }
    }
}