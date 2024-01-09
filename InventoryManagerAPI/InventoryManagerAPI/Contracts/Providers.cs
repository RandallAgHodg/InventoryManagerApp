namespace InventoryManagerAPI.Contracts;

public sealed record ProviderResponse(Guid id, string name);

public sealed record ProviderWithProductsResponse(Guid id, string name, IEnumerable<ProductResponse> products);