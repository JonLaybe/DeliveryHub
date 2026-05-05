namespace Shared.RabbitMq.Interfaces
{
    public interface IClientRabbitMq : IDisposable
    {
        Task Connection();

        Task SendMessage(object obj, string? queue = default);

        Task SendMessage(string message, string? queue = default);
    }
}
