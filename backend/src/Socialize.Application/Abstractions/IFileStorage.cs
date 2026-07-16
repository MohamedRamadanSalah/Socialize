namespace Socialize.Application.Abstractions;

public interface IFileStorage
{
    /// <summary>Saves content under wwwroot/uploads/{subFolder} and returns a relative URL (e.g. /uploads/posts/{guid}.jpg).</summary>
    Task<string> SaveAsync(Stream content, string subFolder, string fileExtension, CancellationToken cancellationToken = default);

    void Delete(string relativeUrl);
}
