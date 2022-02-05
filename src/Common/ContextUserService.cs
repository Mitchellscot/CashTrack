using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace CashTrack.Common
{
    public class ContextUserService : ICurrentUserService
    {
        public ContextUserService(IHttpContextAccessor httpContextAccessor)
        {
            var user = httpContextAccessor.HttpContext?.User;
            Id = user == null ? "Guest" : user.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public string Id { get; }
    }
}
