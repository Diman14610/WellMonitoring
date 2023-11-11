using Microsoft.AspNetCore.SignalR;
using WellServiceAPI.Services.Abstractions.SignalR;
using WellServiceAPI.Shared.Response.Telemetry;

namespace WellServiceAPI.Services.Implementations.SignalR
{
    public class MessagesHubService : IMessagesHub
    {
        private readonly IHubContext<MessagesHub> _messagesHub;

        public MessagesHubService(IHubContext<MessagesHub> messagesHub)
        {
            _messagesHub = messagesHub ?? throw new ArgumentNullException(nameof(messagesHub));
        }

        public async Task SendNewTelemetryAsync(TelemetryInfo telemetryInfo)
        {
            if (telemetryInfo == null) return;

            await _messagesHub.Clients.All.SendAsync("newTelemetry", telemetryInfo).ConfigureAwait(false);
        }
    }
}
