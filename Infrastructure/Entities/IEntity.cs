namespace Structor.Net.Infrastructure.Entities;

public class IEntity
{
    public int Id { get; set; }
    public Guid Guid { get; set; } = Guid.NewGuid();

    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? UpdatedDate { get; set; } = DateTimeOffset.UtcNow;
}