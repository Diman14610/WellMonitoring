using WellServiceAPI.Data;
using WellServiceAPI.Models;
using WellServiceAPI.Services.Abstractions.DB;
using WellServiceAPI.Shared.Actions.Query;

namespace WellServiceAPI.Services.Implementations.DB.Query
{
    public class GetWellByIdQueryService : QueryServiceBase<GetWellById, Well?>
    {
        public GetWellByIdQueryService(WellDBContext wellDBContext) : base(wellDBContext)
        {
        }

        public override async Task<Well?> ExecuteAsync(GetWellById query)
        {
            return await _wellDBContext.Wells.FindAsync(query.Id).ConfigureAwait(false);
        }
    }
}
