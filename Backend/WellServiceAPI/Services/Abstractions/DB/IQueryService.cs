namespace WellServiceAPI.Services.Abstractions.DB
{
    public interface IQueryService<TQuery, TResult>
    {
        Task<TResult> ExecuteAsync(TQuery query);
    }

    public interface IQueryService<TResult>
    {
        Task<TResult> ExecuteAsync();
    }
}