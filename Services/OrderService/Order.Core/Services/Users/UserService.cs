using Microsoft.AspNetCore.Http;
using OrderService.Core.Common.Exceptions;
using OrderService.Core.Services.Interfaces.Users;
using System.Security.Claims;

namespace OrderService.Core.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContext;

        public UserService(IHttpContextAccessor httpContextAccessor) =>
            this._httpContext = httpContextAccessor;

        public Guid GetCurrentUserId()
        {
            var userId = !this._httpContext.HttpContext.User.Identity.IsAuthenticated ?
                throw new NotAuthenticatedException() :
                this._httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (string.IsNullOrEmpty(userId))
                throw new NotAuthenticatedException();
            
            return new Guid(userId);
        }
    }
}
