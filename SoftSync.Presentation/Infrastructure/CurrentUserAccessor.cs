using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace SoftSync.Presentation.Infrastructure;

public class CurrentUserAccessor
{
    private readonly AuthenticationStateProvider _authProvider;
    public CurrentUserAccessor(AuthenticationStateProvider authProvider) => _authProvider = authProvider;

    public async Task<int?> GetUserIdAsync()
    {
        var state = await _authProvider.GetAuthenticationStateAsync();
        var claim = state.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(claim, out var id) ? id : null;
    }
}