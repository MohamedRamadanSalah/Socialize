using Socialize.Application.Abstractions;

namespace Socialize.Application.Tests.TestSupport;

public class FakeFileStorage : IFileStorage
{
    public List<string> Deleted { get; } = new();

    public Task<string> SaveAsync(Stream content, string subFolder, string fileExtension, CancellationToken cancellationToken = default)
    {
        return Task.FromResult($"/uploads/{subFolder}/{Guid.NewGuid():N}{fileExtension}");
    }

    public void Delete(string relativeUrl) => Deleted.Add(relativeUrl);
}
