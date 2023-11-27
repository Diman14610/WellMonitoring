using WellServiceAPI.Shared.Response.Telemetry;

namespace WellServiceAPI.Services.Abstractions
{
    public interface IMessagesHub
    {
        Task SendNewTelemetryAsync(TelemetryInfo telemetryInfo);
    }
}