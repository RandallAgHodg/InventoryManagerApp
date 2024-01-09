using FastEndpoints.Security;

namespace InventoryManagerAPI.Services.JWTProvider;

public interface IJWTProvider
{
    public string Create(string id, string email);
}
