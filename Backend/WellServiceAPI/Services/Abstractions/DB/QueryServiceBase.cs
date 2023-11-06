using WellServiceAPI.Data;

namespace WellServiceAPI.Services.Abstractions.DB
{
    public abstract class QueryServiceBase<TQuery, TResult> : IQueryService<TQuery, TResult>
    {
        protected readonly WellDBContext _wellDBContext;

        protected QueryServiceBase(WellDBContext wellDBContext)
        {
            _wellDBContext = wellDBContext ?? throw new ArgumentNullException(nameof(wellDBContext));
        }

        public abstract Task<TResult> ExecuteAsync(TQuery query);
    }

    public abstract class QueryServiceBase<TResult> : IQueryService<TResult>
    {
        protected readonly WellDBContext _wellDBContext;

        protected QueryServiceBase(WellDBContext wellDBContext)
        {
            _wellDBContext = wellDBContext ?? throw new ArgumentNullException(nameof(wellDBContext));
        }

        public abstract Task<TResult> ExecuteAsync();
    }
}
