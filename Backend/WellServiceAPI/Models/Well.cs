namespace WellServiceAPI.Models
{
    public class Well
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public int Active { get; set; }

        public int CompanyId { get; set; }

        public virtual Company Company { get; set; } = null!;

        public virtual ICollection<Telemetry> Telemetries { get; set; } = new List<Telemetry>();
    }
}
