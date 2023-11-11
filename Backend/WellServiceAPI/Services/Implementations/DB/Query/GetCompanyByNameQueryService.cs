using Microsoft.EntityFrameworkCore;
using WellServiceAPI.Data;
using WellServiceAPI.Models;
using WellServiceAPI.Services.Abstractions.DB;
using WellServiceAPI.Shared.Actions.Query;

namespace WellServiceAPI.Services.Implementations.DB.Query
{
    public class GetCompanyByNameQueryService : QueryServiceBase<GetCompanyByName, Company?>
    {
        public GetCompanyByNameQueryService(WellDBContext wellDBContext) : base(wellDBContext)
        {
        }

        public override async Task<Company?> ExecuteAsync(GetCompanyByName query)
        {
            return await _wellDBContext.Companys
                .FirstOrDefaultAsync(c => c.Name.ToUpper() == query.Name.ToUpper())
                .ConfigureAwait(false);
        }
    }
}
