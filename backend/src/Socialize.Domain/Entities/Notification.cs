using Socialize.Domain.Enums;

namespace Socialize.Domain.Entities;

public class Notification
{
    public Guid Id { get; set; }
    public Guid RecipientId { get; set; }
    public User? Recipient { get; set; }
    public Guid ActorId { get; set; }
    public User? Actor { get; set; }
    public NotificationType Type { get; set; }
    public Guid? EntityId { get; set; }
    public bool IsRead { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
