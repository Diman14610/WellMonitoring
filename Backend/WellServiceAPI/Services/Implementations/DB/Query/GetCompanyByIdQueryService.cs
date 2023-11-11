using Microsoft.EntityFrameworkCore;
using WellServiceAPI.Data;
using WellServiceAPI.Models;
using WellServiceAPI.Services.Abstractions.DB;
using WellServiceAPI.Shared.Actions.Query;

namespace WellServiceAPI.Services.Implementations.DB.Query
{
    public class GetCompanyByIdQueryService : QueryServiceBase<GetCompanyById, Company?>
    {
        public GetCompanyByIdQueryService(WellDBContext wellDBContext) : base(wellDBContext)
        {
        }

        public override async Task<Company?> ExecuteAsync(GetCompanyById query)
        {
            return await _wellDBContext.Companys.FirstOrDefaultAsync(c => c.Id == query.Id).ConfigureAwait(false);
        }
    }
}
