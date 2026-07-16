namespace Socialize.Domain.Entities;

public class PostImage
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public Post? Post { get; set; }
    public string Url { get; set; } = default!;
    public int Order { get; set; }
}
