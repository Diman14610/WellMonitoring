namespace WellServiceAPI.Shared.Actions.Query
{
    public class GetAllWellsByActivityParam
    {
        public GetAllWellsByActivityParam(int active)
        {
            Active = active;
        }

        public int Active { get; }
    }
}
