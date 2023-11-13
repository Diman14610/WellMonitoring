using Microsoft.EntityFrameworkCore;
using WellServiceAPI.Data;
using WellServiceAPI.Models;
using WellServiceAPI.Services.Abstractions.DB;

namespace WellServiceAPI.Services.Implementations.DB.Query
{
    public class GetAllCompaniesQueryService : QueryServiceBase<IEnumerable<Company>>
    {
        public GetAllCompaniesQueryService(WellDBContext wellDBContext) : base(wellDBContext)
        {
        }

        public override async Task<IEnumerable<Company>> ExecuteAsync()
        {
            return await _wellDBContext.Companys.ToListAsync();
        }
    }
}
