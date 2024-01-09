using ProductsManagerAPI.Contracts;

namespace InventoryManagerAPI.Contracts;

public sealed record PurchaseRequest(Guid productId, int amount);
public sealed record PurchaseDetailResponse(int amount, string productName, string productImageUrl, decimal subtotal);
public sealed record PurchaseResponse(Guid Id, decimal total, DateTime createdAt, UserResponse user, IEnumerable<PurchaseDetailResponse> details);
