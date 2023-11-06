using Microsoft.EntityFrameworkCore;
using WellServiceAPI.Data;
using WellServiceAPI.Models;
using WellServiceAPI.Services.Abstractions.DB;
using WellServiceAPI.Shared.Actions.Query;

namespace WellServiceAPI.Services.Implementations.DB.SqlLite.Query
{
    public class SQLLiteQueryServiceGetCompanyByName : QueryServiceBase<GetCompanyByName, Company?>
    {
        public SQLLiteQueryServiceGetCompanyByName(WellDBContext wellDBContext) : base(wellDBContext)
        {
        }

        public override async Task<Company?> ExecuteAsync(GetCompanyByName query)
        {
            return await _wellDBContext.Companys.FirstOrDefaultAsync(c => c.Name.ToLower() == query.Name.ToLower()).ConfigureAwait(false);
        }
    }
}
