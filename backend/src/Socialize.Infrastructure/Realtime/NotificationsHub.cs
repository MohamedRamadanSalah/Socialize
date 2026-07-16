using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Socialize.Infrastructure.Realtime;

[Authorize]
public class NotificationsHub : Hub
{
    // Server -> client only ("ReceiveNotification"); no client-invokable methods needed.
}
