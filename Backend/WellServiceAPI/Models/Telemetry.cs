namespace WellServiceAPI.Models
{
    public class Telemetry
    {
        public int Id { get; set; }

        public DateTime DateTime { get; set; }

        public float Depth { get; set; }

        public int? WellId { get; set; }

        public virtual Well? Well { get; set; }
    }
}
