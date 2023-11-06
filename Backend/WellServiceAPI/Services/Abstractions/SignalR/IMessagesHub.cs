using WellServiceAPI.Shared.Response.Telemetry;

namespace WellServiceAPI.Services.Abstractions.SignalR
{
    public interface IMessagesHub
    {
        Task SendNewTelemetryAsync(TelemetryInfo telemetryInfo);
    }
}