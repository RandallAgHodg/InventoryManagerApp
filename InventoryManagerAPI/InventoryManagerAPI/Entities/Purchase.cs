namespace InventoryManagerAPI.Entities;

public sealed class Purchase
{
    private readonly HashSet<PurchaseDetail> _purchaseDetails = new();

    public Guid Id { get; private set; }
    public decimal Total { get; private set; }
    public Guid UserId { get; private set; }
    public User User { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool Deleted { get; private set; }
    public IReadOnlyCollection<PurchaseDetail> PurchaseDetails => _purchaseDetails;
    public Purchase(Guid id, decimal total, Guid userId)
    {
        Id = id;
        Total = total;
        UserId = userId;
        Deleted = false;
        CreatedAt = DateTime.UtcNow; 
    }

    public void UpdateTotal(decimal subtotal) => Total += subtotal;
    public void Delete() => Deleted = true;
    public void AddDetail(PurchaseDetail detail) => _purchaseDetails.Add(detail);
}
