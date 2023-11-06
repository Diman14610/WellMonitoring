using WellServiceAPI.Data;

namespace WellServiceAPI.Services.Abstractions.DB
{
    public abstract class CommandServiceBase<TCommand> : ICommandService<TCommand>
    {
        protected readonly WellDBContext _wellDBContext;

        protected CommandServiceBase(WellDBContext wellDBContext)
        {
            _wellDBContext = wellDBContext ?? throw new ArgumentNullException(nameof(wellDBContext));
        }

        public abstract Task ExecuteAsync(TCommand command);
    }
}
