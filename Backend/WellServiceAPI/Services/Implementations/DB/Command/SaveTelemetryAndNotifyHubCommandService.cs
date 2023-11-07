using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WellServiceAPI.Data;
using WellServiceAPI.Services.Abstractions.DB;
using WellServiceAPI.Services.Abstractions.SignalR;
using WellServiceAPI.Shared.Actions.Command;
using WellServiceAPI.Shared.Response.Telemetry;

namespace WellServiceAPI.Services.Implementations.DB.Command
{
    public class SaveTelemetryAndNotifyHubCommandService : CommandServiceBase<SaveTelemetryData>
    {
        private readonly ICommandService<SaveTelemetryData> _commandService;
        private readonly IMessagesHub _messagesHub;

        public SaveTelemetryAndNotifyHubCommandService(
            WellDBContext wellDBContext,
            ICommandService<SaveTelemetryData> commandService,
            IMessagesHub messagesHub) : base(wellDBContext)
        {
            _commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
            _messagesHub = messagesHub ?? throw new ArgumentNullException(nameof(messagesHub));
        }

        public override async Task ExecuteAsync(SaveTelemetryData command)
        {
            if (!command.TelemetryData.Any()) return;

            await _commandService.ExecuteAsync(command);

            foreach (var TelemetryData in command.TelemetryData)
            {
                var telemetry = await _wellDBContext.Telemetrys
                              .Include(t => t.Well)
                              .ThenInclude(t => t.Company)
                              .OrderBy(t => t.Id)
                              .LastOrDefaultAsync(t => t.WellId == TelemetryData.WellId && t.Depth == TelemetryData.Depth && t.DateTime == TelemetryData.DateTime);

                if (telemetry == null) continue;

                await _messagesHub.SendNewTelemetryAsync(new TelemetryInfo
                {
                    TelemetryId = telemetry.Id,
                    DateTime = telemetry.DateTime,
                    Depth = telemetry.Depth,
                    WellId = telemetry.WellId,
                    ContractorName = telemetry.Well.Company.Name,
                    WellName = telemetry.Well.Name,
                });
            }
        }
    }
}
