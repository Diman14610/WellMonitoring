namespace WellServiceAPI.Shared.Actions.Command
{
    public class ChangeActiveWell
    {
        public ChangeActiveWell(int id, int active)
        {
            Id = id;
            Active = active;
        }

        public int Id { get; }

        public int Active { get; }
    }
}
