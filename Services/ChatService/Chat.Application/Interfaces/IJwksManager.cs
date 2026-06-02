using Microsoft.IdentityModel.Tokens;

namespace Chat.Application.Interfaces
{
    public interface IJwksManager
    {
        IEnumerable<SecurityKey> GetKeys(string? kid);
    }
}
