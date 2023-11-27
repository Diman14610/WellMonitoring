using MediatR;
using Microsoft.EntityFrameworkCore;
using WellServiceAPI.Data;
using WellServiceAPI.Shared.Response.Telemetry;

namespace WellServiceAPI.Domain.Queries
{
    public class GetAllTelemetryQuery : IRequest<IEnumerable<TelemetryInfo>>
    {
    }

    public class GetAllTelemetryQueryHandler : IRequestHandler<GetAllTelemetryQuery, IEnumerable<TelemetryInfo>>
    {
        private readonly WellDBContext _wellDBContext;

        public GetAllTelemetryQueryHandler(WellDBContext dbContext)
        {
            _wellDBContext = dbContext;
        }

        public async Task<IEnumerable<TelemetryInfo>> Handle(GetAllTelemetryQuery request, CancellationToken cancellationToken)
        {
            var telemetrs = await _wellDBContext.Telemetrys
                .Include(t => t.Well)
                .ThenInclude(t => t.Company)
                .Select(telemetry => new TelemetryInfo
                {
                    TelemetryId = telemetry.Id,
                    DateTime = telemetry.DateTime,
                    Depth = telemetry.Depth,
                    WellId = telemetry.WellId,
                    ContractorName = telemetry.Well.Company.Name,
                    WellName = telemetry.Well.Name,
                })
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return telemetrs;
        }
    }
}
