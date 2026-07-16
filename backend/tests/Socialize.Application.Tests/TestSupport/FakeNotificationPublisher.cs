using Socialize.Application.Abstractions;
using Socialize.Application.Common.Dtos;

namespace Socialize.Application.Tests.TestSupport;

public class FakeNotificationPublisher : INotificationPublisher
{
    public List<(Guid RecipientId, NotificationDto Notification)> Published { get; } = new();

    public Task PublishAsync(Guid recipientId, NotificationDto notification, CancellationToken cancellationToken = default)
    {
        Published.Add((recipientId, notification));
        return Task.CompletedTask;
    }
}
