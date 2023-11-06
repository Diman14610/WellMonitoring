namespace WellServiceAPI.Shared.Actions.Query
{
    public class GetCompanyById
    {
        public GetCompanyById(int id)
        {
            Id = id;
        }

        public int Id { get; }
    }
}
