namespace InventoryManagerAPI.Contracts;

public sealed record ProductResponse(Guid Id, string name, int amount, decimal price, decimal cost, string pictureUrl, DateTime createdAt);
