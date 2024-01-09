using InventoryManagerAPI.Contracts;
using InventoryManagerAPI.Entities;
using ProductsManagerAPI.Contracts;

namespace InventoryManagerAPI.Extensions;

public static class DomainToResponseMapping
{
    public static UserResponse ToResponse(this User user) =>
        new UserResponse(user.Id, user.FirstName, user.LastName, user.Email, user.PictureUrl);

    public static ProviderResponse ToResponse(this Provider provider) =>
        new ProviderResponse(provider.Id, provider.Name);

    public static ProviderWithProductsResponse ToProviderWithProductsResponse(this Provider provider) => 
        new ProviderWithProductsResponse(provider.Id, provider.Name, provider.Products.Select(x => x.ToResponse()));
    public static ProductResponse ToResponse(this Product product) =>
        new ProductResponse(product.Id, product.Name, product.Amount, product.Price, product.Cost, product.PictureUrl, product.CreatedAt);

    public static PurchaseDetailResponse ToResponse(this PurchaseDetail detail) =>
        new PurchaseDetailResponse(detail.Amount, detail.Product.Name, detail.Product.PictureUrl, detail.Subtotal);

    public static PurchaseResponse ToResponse(this Purchase purchase) =>
        new PurchaseResponse(purchase.Id, purchase.Total, purchase.CreatedAt, purchase.User.ToResponse(),
            purchase.PurchaseDetails.Select(x => x.ToResponse()));

    public static SaleDetailResponse ToResponse(this SaleDetail detail) =>
        new SaleDetailResponse(detail.Amount, detail.Product.Name, detail.Product.PictureUrl, detail.Subtotal);

    public static SaleResponse ToResponse(this Sale sale) =>
        new SaleResponse(sale.Id, sale.Total, sale.CreatedAt,
            sale.Client.Name, sale.User.ToResponse(),
            sale.SaleDetails.Select(x => x.ToResponse()));
}
