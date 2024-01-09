namespace InventoryManagerAPI.Entities;

public sealed class Product
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public int Amount { get; private set; }
    public decimal Price { get; private set; }
    public decimal Cost { get; private set; }
    public string PictureUrl { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool Deleted { get; private set; }
    public Guid ProviderId { get; private set; }
    public Provider Provider { get; private set; }


    public Product(Guid id, string name, int amount, decimal price, decimal cost, string pictureUrl, Guid providerId)
    {
        Id = id;
        Name = name;
        Amount = amount;
        Price = price;
        Cost = cost;
        PictureUrl = pictureUrl;
        ProviderId = providerId;
        CreatedAt = DateTime.Now;
        Deleted = false;
    }

    public void UpdateInformation(string name, decimal price, decimal cost)
    {
        Name = name;
        Price = price;
        Cost = cost;
    }

    public bool CheckStockForSaleAmount(int amount) => Amount > amount;
    public void DeleteProduct() => Deleted = true;
    public void ReduceStock(int amount) => Amount -= amount;
    public void AddStock(int amount) => Amount += amount;
}
