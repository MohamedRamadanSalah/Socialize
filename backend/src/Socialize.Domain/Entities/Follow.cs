namespace Socialize.Domain.Entities;

public class Follow
{
    public Guid FollowerId { get; set; }
    public User? FollowerUser { get; set; }
    public Guid FolloweeId { get; set; }
    public User? FolloweeUser { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
