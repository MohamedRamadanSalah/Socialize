namespace Socialize.Domain.Entities;

public class Like
{
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public Guid PostId { get; set; }
    public Post? Post { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
