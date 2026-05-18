namespace Shared.Domain
{
    public class RabbitMqClientSettings
    {
        public string HostName { get; set; } = string.Empty;

        public int Port { get; set; }

        public TimeSpan RetryDelay { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public bool EnableTls { get; set; }

        public string Queue { get; set; } = string.Empty;
    }
}
