using WellServiceAPI.Data;
using WellServiceAPI.Models;
using WellServiceAPI.Services.Abstractions.DB;
using WellServiceAPI.Shared.Actions.Command;

namespace WellServiceAPI.Services.Implementations.DB.Command
{
    public class SaveTelemetryCommandService : CommandServiceBase<SaveTelemetryData>
    {
        public SaveTelemetryCommandService(WellDBContext wellDBContext) : base(wellDBContext)
        {
        }

        public override async Task ExecuteAsync(SaveTelemetryData command)
        {
            if (!command.TelemetryData.Any()) return;

            foreach (var telemetry in command.TelemetryData)
            {
                if (telemetry == null || telemetry.WellId == 0) continue;

                _wellDBContext.Telemetrys.Add(new Telemetry
                {
                    Depth = telemetry.Depth,
                    DateTime = telemetry.DateTime,
                    WellId = telemetry.WellId,
                });
            }

            await _wellDBContext.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
