namespace WellServiceAPI.Models
{
    public class Company
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public virtual ICollection<Well> Wells { get; set; } = new List<Well>();
    }
}
