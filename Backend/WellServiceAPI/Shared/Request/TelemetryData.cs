namespace WellServiceAPI.Shared.Request
{
    public class TelemetryData
    {
        public int WellId { get; set; }

        public DateTime DateTime { get; set; }

        public float Depth { get; set; }
    }
}
