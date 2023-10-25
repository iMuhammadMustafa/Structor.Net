namespace Structor.Infrastructure.Entities;

public class IEntity
{
    public int Id { get; set; }
    public Guid Guid { get; set; } = Guid.NewGuid();

    public string CreatedBy { get; set; } = "System";
    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;

    public string? UpdatedBy { get; set; } = null;

    public DateTimeOffset? UpdatedDate { get; set; } = DateTimeOffset.UtcNow;
}