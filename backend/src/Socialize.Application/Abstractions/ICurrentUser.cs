namespace Socialize.Application.Abstractions;

public interface ICurrentUser
{
    Guid UserId { get; }
    string UserName { get; }
}
