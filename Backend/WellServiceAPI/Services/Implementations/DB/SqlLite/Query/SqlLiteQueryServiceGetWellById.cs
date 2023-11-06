using WellServiceAPI.Data;
using WellServiceAPI.Models;
using WellServiceAPI.Services.Abstractions.DB;
using WellServiceAPI.Shared.Actions.Query;

namespace WellServiceAPI.Services.Implementations.DB.SqlLite.Query
{
    public class SQLLiteQueryServiceGetWellById : QueryServiceBase<GetWellById, Well?>
    {
        public SQLLiteQueryServiceGetWellById(WellDBContext wellDBContext) : base(wellDBContext)
        {
        }

        public override async Task<Well?> ExecuteAsync(GetWellById query)
        {
            return await _wellDBContext.Wells.FindAsync(query.Id).ConfigureAwait(false);
        }
    }
}
