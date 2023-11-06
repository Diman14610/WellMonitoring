using WellServiceAPI.Shared.Request;

namespace WellServiceAPI.Shared.Actions.Command
{
    public class SaveTelemetryData
    {
        public SaveTelemetryData(IEnumerable<TelemetryData> telemetryData)
        {
            TelemetryData = telemetryData;
        }

        public IEnumerable<TelemetryData> TelemetryData { get; }
    }
}
