using ProductsManagerAPI.Contracts;

namespace InventoryManagerAPI.Contracts;

public sealed record SaleRequest(Guid productId, int amount);
public sealed record SaleDetailResponse(int amount, string productName, string productImageUrl, decimal subtotal);
public sealed record SaleResponse(Guid Id, decimal total, DateTime createdAt, string client, UserResponse user, IEnumerable<SaleDetailResponse> details);