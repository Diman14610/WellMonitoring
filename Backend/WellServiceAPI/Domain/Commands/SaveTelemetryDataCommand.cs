using MediatR;
using WellServiceAPI.Data;
using WellServiceAPI.Models;
using WellServiceAPI.Shared.Request;

namespace WellServiceAPI.Domain.Commands
{
    public class SaveTelemetryDataCommand : IRequest
    {
        public SaveTelemetryDataCommand(IEnumerable<TelemetryData> telemetryData)
        {
            TelemetryData = new List<TelemetryData>(telemetryData);
        }

        public IEnumerable<TelemetryData> TelemetryData { get; }
    }

    public class SaveTelemetryDataCommandHandler : IRequestHandler<SaveTelemetryDataCommand>
    {
        private readonly WellDBContext _wellDBContext;

        public SaveTelemetryDataCommandHandler(WellDBContext dbContext)
        {
            _wellDBContext = dbContext;
        }

        public async Task Handle(SaveTelemetryDataCommand request, CancellationToken cancellationToken)
        {
            foreach (var telemetry in request.TelemetryData)
            {
                if (telemetry == null || telemetry.WellId == 0) continue;

                _wellDBContext.Telemetrys.Add(new Telemetry
                {
                    Depth = telemetry.Depth,
                    DateTime = telemetry.DateTime,
                    WellId = telemetry.WellId,
                });
            }

            await _wellDBContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
