namespace Chat.Application.Interfaces
{
    public interface IUserProfileService
    {
        Task<Dictionary<Guid, (string SellerName, string SellerPhoto)>> GetUserInfosByIdsAsync(IEnumerable<Guid> userIds, CancellationToken cancellationToken = default);
    }
}
