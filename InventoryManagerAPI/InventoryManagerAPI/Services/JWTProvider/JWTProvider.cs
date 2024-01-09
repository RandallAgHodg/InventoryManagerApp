using FastEndpoints.Security;
using System.Security.Claims;

namespace InventoryManagerAPI.Services.JWTProvider;

public sealed class JWTProvider : IJWTProvider
{
    private readonly IConfiguration _configuration;

    public JWTProvider(IConfiguration configuration) =>
        _configuration = configuration;


    public string Create(string id, string email) =>
        JWTBearer.CreateToken(
            signingKey: _configuration.GetValue<string>("jwt:secret"),
            expireAt: DateTime.UtcNow.AddDays(1),
            claims: new List<Claim>() { new("email", email), new("id", id) }
            );
}
