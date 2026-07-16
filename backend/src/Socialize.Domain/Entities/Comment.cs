namespace Socialize.Domain.Entities;

public class Comment
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public Post? Post { get; set; }
    public Guid AuthorId { get; set; }
    public User? Author { get; set; }
    public string Content { get; set; } = default!;
    public DateTimeOffset CreatedAt { get; set; }
}
