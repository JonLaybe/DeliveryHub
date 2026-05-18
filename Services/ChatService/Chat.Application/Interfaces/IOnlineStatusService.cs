namespace Chat.Application.Interfaces
{
    public interface IOnlineStatusService
    {
        Task SetOnlineAsync(Guid userId);
        Task SetOfflineAsync(Guid userId);
        Task<bool> IsOnlineAsync(Guid userId);
    }
}
