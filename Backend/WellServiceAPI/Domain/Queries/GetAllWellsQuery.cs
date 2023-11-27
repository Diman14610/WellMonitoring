using MediatR;
using Microsoft.EntityFrameworkCore;
using WellServiceAPI.Data;
using WellServiceAPI.Models;

namespace WellServiceAPI.Domain.Queries
{
    public class GetAllWellsQuery : IRequest<IEnumerable<Well>>
    {
    }

    public class GetAllWellsQueryHandler : IRequestHandler<GetAllWellsQuery, IEnumerable<Well>>
    {
        private readonly WellDBContext _wellDBContext;

        public GetAllWellsQueryHandler(WellDBContext dbContext)
        {
            _wellDBContext = dbContext;
        }

        public async Task<IEnumerable<Well>> Handle(GetAllWellsQuery request, CancellationToken cancellationToken)
        {
            return await _wellDBContext.Wells
                .Include(w => w.Company)
                .Include(w => w.Telemetries)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
