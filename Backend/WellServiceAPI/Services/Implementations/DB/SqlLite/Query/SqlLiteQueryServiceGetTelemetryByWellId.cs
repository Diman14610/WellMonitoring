using Microsoft.EntityFrameworkCore;
using WellServiceAPI.Data;
using WellServiceAPI.Models;
using WellServiceAPI.Services.Abstractions.DB;
using WellServiceAPI.Shared.Actions.Query;

namespace WellServiceAPI.Services.Implementations.DB.SqlLite.Query
{
    public class SQLLiteQueryServiceGetTelemetryByWellId : QueryServiceBase<GetTelemetryByWellId, IEnumerable<Telemetry>>
    {
        public SQLLiteQueryServiceGetTelemetryByWellId(WellDBContext wellDBContext) : base(wellDBContext)
        {
        }

        public override async Task<IEnumerable<Telemetry>> ExecuteAsync(GetTelemetryByWellId query)
        {
            return await _wellDBContext.Telemetrys.Where(t => t.WellId == query.Id).ToListAsync().ConfigureAwait(false);
        }
    }
}
