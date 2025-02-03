using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace MyFinanceAPI.Application.Services;

public class UserContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContextService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int? GetUserIdFromClaims()
    {
        var userIdString = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdString == null || !int.TryParse(userIdString, out var userId))
            return null;

        return userId;
    }
}
