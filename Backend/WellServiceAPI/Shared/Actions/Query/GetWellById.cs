namespace WellServiceAPI.Shared.Actions.Query
{
    public class GetWellById
    {
        public GetWellById(int id)
        {
            Id = id;
        }

        public int Id { get; }
    }
}
