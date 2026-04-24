using System.Security.Claims;
using Duende.IdentityModel;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdentityService.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Services;

public class CustomProfileService(UserManager<ApplicationUser> userManager) : IProfileService
{
    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var user = await userManager.GetUserAsync(context.Subject);

        if (user == null) return;

        var existingClaims = await userManager.GetClaimsAsync(user);
        
        var claims = new List<Claim>
        {
            new Claim("username", user.UserName!),
        };
        
        context.IssuedClaims.AddRange(claims);

        var nameClaim = existingClaims.FirstOrDefault(z => z.Type == JwtClaimTypes.Name);
        if (nameClaim != null)
            context.IssuedClaims.Add(nameClaim);
    }

    public Task IsActiveAsync(IsActiveContext context)
    {
        return Task.CompletedTask;
    }
}