using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using WellServiceAPI.Data;
using WellServiceAPI.Services.Abstractions;
using WellServiceAPI.Shared.Response.Telemetry;

namespace WellServiceAPI.Domain.Commands
{
    public class SaveTelemetryAndNotifyHubCommand : IRequestPreProcessor<SaveTelemetryDataCommand>
    {
        private readonly IMessagesHub _messagesHub;
        private readonly WellDBContext _wellDBContext;

        public SaveTelemetryAndNotifyHubCommand(IMessagesHub messagesHub, WellDBContext wellDBContext)
        {
            _messagesHub = messagesHub;
            _wellDBContext = wellDBContext;
        }

        public async Task Process(SaveTelemetryDataCommand request, CancellationToken cancellationToken)
        {
            foreach (var TelemetryData in request.TelemetryData)
            {
                var telemetry = await _wellDBContext.Telemetrys
                    .Include(t => t.Well)
                    .ThenInclude(t => t.Company)
                    .OrderBy(t => t.Id)
                    .LastOrDefaultAsync(t =>
                        t.WellId == TelemetryData.WellId &&
                        t.Depth == TelemetryData.Depth &&
                        t.DateTime == TelemetryData.DateTime,
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

                if (telemetry == null) continue;

                await _messagesHub.SendNewTelemetryAsync(new TelemetryInfo
                {
                    TelemetryId = telemetry.Id,
                    DateTime = telemetry.DateTime,
                    Depth = telemetry.Depth,
                    WellId = telemetry.WellId,
                    ContractorName = telemetry.Well.Company.Name,
                    WellName = telemetry.Well.Name,
                }).ConfigureAwait(false);
            }
        }

        public async Task Process(SaveTelemetryDataCommand request, string response, CancellationToken cancellationToken)
        {
            foreach (var TelemetryData in request.TelemetryData)
            {
                var telemetry = await _wellDBContext.Telemetrys
                    .Include(t => t.Well)
                    .ThenInclude(t => t.Company)
                    .OrderBy(t => t.Id)
                    .LastOrDefaultAsync(t =>
                        t.WellId == TelemetryData.WellId &&
                        t.Depth == TelemetryData.Depth &&
                        t.DateTime == TelemetryData.DateTime,
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

                if (telemetry == null) continue;

                await _messagesHub.SendNewTelemetryAsync(new TelemetryInfo
                {
                    TelemetryId = telemetry.Id,
                    DateTime = telemetry.DateTime,
                    Depth = telemetry.Depth,
                    WellId = telemetry.WellId,
                    ContractorName = telemetry.Well.Company.Name,
                    WellName = telemetry.Well.Name,
                }).ConfigureAwait(false);
            }
        }
    }
}
