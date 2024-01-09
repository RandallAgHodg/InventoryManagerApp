namespace InventoryManagerAPI.Services.UserInformationProvider;

public interface IUserInformationProvider
{
    Guid UserId { get; }
    bool IsAuthenticated { get; }
}
