using MediatR;
using Microsoft.EntityFrameworkCore;
using WellServiceAPI.Data;
using WellServiceAPI.Models;

namespace WellServiceAPI.Domain.Queries
{
    public class GetTelemetryByWellIdQuery : IRequest<IEnumerable<Telemetry>>
    {
        public GetTelemetryByWellIdQuery(int id)
        {
            Id = id;
        }

        public int Id { get; }
    }

    public class GetTelemetryByWellIdQueryHandler : IRequestHandler<GetTelemetryByWellIdQuery, IEnumerable<Telemetry>>
    {
        private readonly WellDBContext _wellDBContext;

        public GetTelemetryByWellIdQueryHandler(WellDBContext dbContext)
        {
            _wellDBContext = dbContext;
        }

        public async Task<IEnumerable<Telemetry>> Handle(GetTelemetryByWellIdQuery request, CancellationToken cancellationToken)
        {
            return await _wellDBContext.Telemetrys
                .Where(t => t.WellId == request.Id)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
