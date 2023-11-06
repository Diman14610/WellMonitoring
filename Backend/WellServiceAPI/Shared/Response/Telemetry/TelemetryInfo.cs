namespace WellServiceAPI.Shared.Response.Telemetry
{
    public class TelemetryInfo
    {
        public int TelemetryId { get; set; }

        public int WellId { get; set; }

        public string WellName { get; set; } = string.Empty;

        public DateTime DateTime { get; set; }

        public float Depth { get; set; }

        public string ContractorName { get; set; } = string.Empty;
    }
}
