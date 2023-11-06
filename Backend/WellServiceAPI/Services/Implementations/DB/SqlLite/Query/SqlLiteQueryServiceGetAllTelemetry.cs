using Microsoft.EntityFrameworkCore;
using WellServiceAPI.Data;
using WellServiceAPI.Services.Abstractions.DB;
using WellServiceAPI.Shared.Response.Telemetry;

namespace WellServiceAPI.Services.Implementations.DB.SqlLite.Query
{
    public class SqlLiteQueryServiceGetAllTelemetry : QueryServiceBase<IEnumerable<TelemetryInfo>>
    {
        public SqlLiteQueryServiceGetAllTelemetry(WellDBContext wellDBContext) : base(wellDBContext)
        {
        }

        public override async Task<IEnumerable<TelemetryInfo>> ExecuteAsync()
        {
            var telemetrs = await _wellDBContext.Telemetrys
                .Include(t => t.Well)
                .ThenInclude(t => t!.Company)
                .Select(telemetry => new TelemetryInfo
                {
                    TelemetryId = telemetry.Id,
                    DateTime = telemetry.DateTime,
                    Depth = telemetry.Depth,
                    WellId = telemetry.WellId ?? 0,
                    ContractorName = telemetry.Well!.Company.Name!,
                    WellName = telemetry.Well.Name,
                })
                .ToListAsync()
                .ConfigureAwait(false);

            return telemetrs;
        }
    }
}
