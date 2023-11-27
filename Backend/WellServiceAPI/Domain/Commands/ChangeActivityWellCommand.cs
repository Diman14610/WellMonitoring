using MediatR;
using WellServiceAPI.Data;
using WellServiceAPI.Models;

namespace WellServiceAPI.Domain.Commands
{
    public class ChangeActivityWellCommand : IRequest
    {
        public ChangeActivityWellCommand(int id, int active)
        {
            Id = id;
            Active = active;
        }

        public int Id { get; }

        public int Active { get; }
    }

    public class ChangeActivityWellCommandHandler : IRequestHandler<ChangeActivityWellCommand>
    {
        private readonly WellDBContext _wellDBContext;

        public ChangeActivityWellCommandHandler(WellDBContext dbContext)
        {
            _wellDBContext = dbContext;
        }

        public async Task Handle(ChangeActivityWellCommand request, CancellationToken cancellationToken)
        {
            Well? foundWell = await _wellDBContext.Wells.FindAsync(request.Id, cancellationToken).ConfigureAwait(false);

            if (foundWell == null) return;

            foundWell.Active = request.Active;

            await _wellDBContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
