namespace Educar.Backend.Infrastructure.Data;

public interface ISoftDelete
{
    bool IsDeleted { get; set; }
}