using RabbitMQ.Client;
using Shared.Domain;
using Shared.RabbitMq.Interfaces;
using System.Text;
using System.Text.Json;

namespace Shared.RabbitMq
{
    public class RabbitMqClient : IClientRabbitMq
    {
        private readonly RabbitMqClientSettings _settings;
        private IChannel _channel;
        private IConnection _connection;

        public RabbitMqClient(RabbitMqClientSettings settings) =>
            this._settings = settings;

        public async Task Connection()
        {
            var factory = new ConnectionFactory()
            {
                HostName = this._settings.HostName,
                Port = this._settings.Port,
                UserName = this._settings.UserName,
                Password = this._settings.Password,
                Ssl = new()
                {
                    Enabled = this._settings.EnableTls,
                    ServerName = _settings.HostName,
                }
            };

            this._connection = await factory.CreateConnectionAsync();
            this._channel = await this._connection.CreateChannelAsync();
        }

        public async Task SendMessage(object obj, string? queue = default) =>
            await this.SendMessage(JsonSerializer.Serialize(obj), queue);

        public async Task SendMessage(string message, string? queue = default)
        {
            var body = Encoding.UTF8.GetBytes(message);

            await this._channel.BasicPublishAsync(exchange: "",
                routingKey: queue != default ? queue : _settings.Queue,
                body: body);
        }

        public void Dispose()
        {
            this._channel?.Dispose();
            this._connection?.Dispose();
        }
    }
}
