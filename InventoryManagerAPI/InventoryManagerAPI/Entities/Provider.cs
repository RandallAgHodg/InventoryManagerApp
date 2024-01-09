namespace InventoryManagerAPI.Entities;

public sealed class Provider
{
    private readonly HashSet<Product> _products = new();
 
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public IReadOnlyCollection<Product> Products => _products;
    public bool Deleted { get; private set; }

    public Provider(Guid id, string name)
    {
        Id = id;
        Name = name;
        Deleted = false;
    }

    public void UpdateProviderInformation(string name) => Name = name;
    public void DeleteProvider() => Deleted = true;
    public void AddProduct(Product product) => _products.Add(product);
}
