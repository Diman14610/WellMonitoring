using MediatR;
using Microsoft.EntityFrameworkCore;
using WellServiceAPI.Data;
using WellServiceAPI.Models;

namespace WellServiceAPI.Domain.Queries
{
    public class GetCompanyByNameQuery : IRequest<Company?>
    {
        public GetCompanyByNameQuery(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }

    public class GetCompanyByNameQueryHandler : IRequestHandler<GetCompanyByNameQuery, Company?>
    {
        private readonly WellDBContext _wellDBContext;

        public GetCompanyByNameQueryHandler(WellDBContext dbContext)
        {
            _wellDBContext = dbContext;
        }

        public async Task<Company?> Handle(GetCompanyByNameQuery request, CancellationToken cancellationToken)
        {
            return await _wellDBContext.Companys
                .FirstOrDefaultAsync(c => c.Name.ToUpper() == request.Name.ToUpper(), cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
