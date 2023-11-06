namespace WellServiceAPI.Shared.Actions.Query
{
    public class GetTelemetryByWellId
    {
        public GetTelemetryByWellId(int id)
        {
            Id = id;
        }

        public int Id { get; }
    }
}
