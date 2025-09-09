using LinkDev.Talabat.Core.Application.Abstraction;
using System.Security.Claims;

namespace LinkDev.Talabat.APIs.Services
{
    public class LoggedInUserService : ILoggedInUserService
    {
        private readonly IHttpContextAccessor _httpContext;

        public string? UserId { get ; set; }
        public LoggedInUserService(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
            UserId = _httpContext.HttpContext?.User.FindFirstValue(ClaimTypes.PrimarySid);
        }
    }
}
