using MediatR;
using Microsoft.EntityFrameworkCore;
using WellServiceAPI.Data;
using WellServiceAPI.Models;

namespace WellServiceAPI.Domain.Queries
{
    public class GetCompanyByIdQuery : IRequest<Company?>
    {
        public GetCompanyByIdQuery(int id)
        {
            Id = id;
        }

        public int Id { get; }
    }

    public class GetCompanyByIdQueryHandler : IRequestHandler<GetCompanyByIdQuery, Company?>
    {
        private readonly WellDBContext _wellDBContext;

        public GetCompanyByIdQueryHandler(WellDBContext dbContext)
        {
            _wellDBContext = dbContext;
        }

        public async Task<Company?> Handle(GetCompanyByIdQuery request, CancellationToken cancellationToken)
        {
            return await _wellDBContext.Companys.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken).ConfigureAwait(false);
        }
    }
}
