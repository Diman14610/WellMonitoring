using MediatR;
using Microsoft.EntityFrameworkCore;
using WellServiceAPI.Data;
using WellServiceAPI.Models;

namespace WellServiceAPI.Domain.Queries
{
    public class GetAllCompaniesQuery : IRequest<IEnumerable<Company>>
    {
    }

    public class GetAllCompaniesQueryHandler : IRequestHandler<GetAllCompaniesQuery, IEnumerable<Company>>
    {
        private readonly WellDBContext _wellDBContext;

        public GetAllCompaniesQueryHandler(WellDBContext dbContext)
        {
            _wellDBContext = dbContext;
        }

        public async Task<IEnumerable<Company>> Handle(GetAllCompaniesQuery request, CancellationToken cancellationToken)
        {
            return await _wellDBContext.Companys.ToListAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
