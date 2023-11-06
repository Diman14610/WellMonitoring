namespace WellServiceAPI.Shared.Actions.Query
{
    public class GetCompanyByName
    {
        public GetCompanyByName(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
