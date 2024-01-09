using System.Security.Claims;

namespace InventoryManagerAPI.Services.UserInformationProvider;

public sealed class UserInformationProvider : IUserInformationProvider
{
    public UserInformationProvider(IHttpContextAccessor httpContextAccessor)
    {
        string? userIdClaim = httpContextAccessor
            .HttpContext?
            .User?
            .Claims
            .FirstOrDefault(x => x.Type.ToString().Equals("id"))?
            .Value;

        IsAuthenticated = Guid.TryParse(userIdClaim, out Guid userId);
        
        UserId = userId;
        

    }
    public Guid UserId { get; }

    public bool IsAuthenticated { get; }
}
