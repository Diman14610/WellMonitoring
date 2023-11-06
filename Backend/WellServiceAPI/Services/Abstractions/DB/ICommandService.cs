namespace WellServiceAPI.Services.Abstractions.DB
{
    public interface ICommandService<TCommand>
    {
        Task ExecuteAsync(TCommand command);
    }
}