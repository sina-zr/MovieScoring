using System.Security.Claims;

namespace Movies.Api.ExtensionMethods;

public static class ClaimsPrincipal
{
    public static ulong GetUserId(this System.Security.Claims.ClaimsPrincipal user)
    {
        var idString = user.FindFirstValue(ClaimTypes.NameIdentifier);
        ulong id = 0;
        id = ulong.Parse(idString);
        return id;
    }
}