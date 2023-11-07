using Microsoft.EntityFrameworkCore;
using WellServiceAPI.Data;
using WellServiceAPI.Models;
using WellServiceAPI.Services.Abstractions.DB;

namespace WellServiceAPI.Services.Implementations.DB.Query
{
    public class GetAllWellsQueryService : QueryServiceBase<IEnumerable<Well>>
    {
        public GetAllWellsQueryService(WellDBContext wellDBContext) : base(wellDBContext)
        {
        }

        public override async Task<IEnumerable<Well>> ExecuteAsync()
        {
            return await _wellDBContext.Wells.Include(w => w.Company).Include(w => w.Telemetries).ToListAsync();
        }
    }
}
