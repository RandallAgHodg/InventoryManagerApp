namespace InventoryManagerAPI.Entities;

public sealed class PurchaseDetail
{
    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public Product Product { get; private set; }
    public Guid PurchaseId { get; private set; }
    public Purchase Purchase { get; private set; }
    public int Amount { get; private set; }
    public decimal Subtotal { get; private set; }
    public PurchaseDetail()
    {
        
    }
    public PurchaseDetail(Guid id, Guid productId, Guid purchaseId, int amount, decimal cost)
    {
        Id = id;
        ProductId = productId;
        PurchaseId = purchaseId;
        Amount = amount;
        Subtotal = cost * amount;
    }
}
