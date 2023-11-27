using MediatR;
using WellServiceAPI.Data;
using WellServiceAPI.Models;

namespace WellServiceAPI.Domain.Queries
{
    public class GetWellByIdQuery : IRequest<Well?>
    {
        public GetWellByIdQuery(int id)
        {
            Id = id;
        }

        public int Id { get; }
    }

    public class GetWellByIdQueryHandler : IRequestHandler<GetWellByIdQuery, Well?>
    {
        private readonly WellDBContext _wellDBContext;

        public GetWellByIdQueryHandler(WellDBContext dbContext)
        {
            _wellDBContext = dbContext;
        }

        public async Task<Well?> Handle(GetWellByIdQuery request, CancellationToken cancellationToken)
        {
            return await _wellDBContext.Wells.FindAsync(request.Id, cancellationToken).ConfigureAwait(false);
        }
    }
}
