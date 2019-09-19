namespace MainProject.Common.Models.Rest
{
    public class ApiEnvironment
    {
        public enum HealthStatus
        {
            NotAvailable,
            Offline,
            Online
        }

        public string Name { get; set; }

        public HealthStatus Status { get; set; }
    }
}
