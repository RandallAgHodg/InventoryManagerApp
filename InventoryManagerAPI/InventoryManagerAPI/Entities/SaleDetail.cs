namespace InventoryManagerAPI.Entities;

public sealed class SaleDetail
{
    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public Product Product { get; private set; }
    public Guid SaleId { get; private set; }
    public Sale Sale { get; private set; }
    public int Amount { get; private set; }
    public decimal Subtotal { get; private set; }

    public SaleDetail()
    {
        
    }

    public SaleDetail(Guid id, Guid productId, Guid saleId, int amount, decimal cost)
    {
        Id = id;
        ProductId = productId;
        SaleId = saleId;
        Amount = amount;
        Subtotal = cost * amount;
    }
}
