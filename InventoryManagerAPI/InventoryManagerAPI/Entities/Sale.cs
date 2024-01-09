namespace InventoryManagerAPI.Entities;

public class Sale
{
    private readonly HashSet<SaleDetail> _saleDetails = new();

    public Guid Id { get; private set; }
    public decimal Total { get; private set; }
    public Guid UserId { get; private set; }
    public User User { get; private set; }
    public DateTime CreatedAt { get; set; }
    public bool Deleted { get; private set; }
    public Guid ClientId { get; private set; }  
    public Client Client {  get; private set; }
    public IReadOnlyCollection<SaleDetail> SaleDetails => _saleDetails;

    public Sale(Guid id, decimal total, Guid clientId, Guid userId)
    {
        Id = id;
        Total = total;
        ClientId = clientId; 
        UserId = userId;
        Deleted =  false;
        CreatedAt = DateTime.UtcNow;
    }
    public void AssociateClient (Client client) => Client = client;
    public void UpdateTotal(decimal subtotal) => Total += subtotal;
    public void Delete() => Deleted = true; 
    public void AddDetail(SaleDetail detail) => _saleDetails.Add(detail);

}
