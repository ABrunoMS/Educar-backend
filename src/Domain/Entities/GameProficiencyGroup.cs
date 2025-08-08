using Educar.Backend.Infrastructure.Data;

namespace Educar.Backend.Domain.Entities;

    public class GameProficiencyGroup : ISoftDelete
    {
        public Guid GameId { get; set; }
        public Game Game { get; set; } = null!;

        public Guid ProficiencyGroupId { get; set; }
        public ProficiencyGroup ProficiencyGroup { get; set; } = null!;

        public bool IsDeleted { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
    }