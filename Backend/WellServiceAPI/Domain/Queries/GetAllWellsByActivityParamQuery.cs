using MediatR;
using Microsoft.EntityFrameworkCore;
using WellServiceAPI.Data;
using WellServiceAPI.Models;

namespace WellServiceAPI.Domain.Queries
{
    public class GetAllWellsByActivityParamQuery : IRequest<IEnumerable<Well>>
    {
        public GetAllWellsByActivityParamQuery(int active)
        {
            Active = active;
        }

        public int Active { get; }
    }

    public class GetAllWellsByActivityParamQueryHandler : IRequestHandler<GetAllWellsByActivityParamQuery, IEnumerable<Well>>
    {
        private readonly WellDBContext _wellDBContext;

        public GetAllWellsByActivityParamQueryHandler(WellDBContext dbContext)
        {
            _wellDBContext = dbContext;
        }

        public async Task<IEnumerable<Well>> Handle(GetAllWellsByActivityParamQuery request, CancellationToken cancellationToken)
        {
            return await _wellDBContext.Wells
                .Where(w => w.Active == request.Active)
                .Include(w => w.Company)
                .Include(w => w.Telemetries)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
