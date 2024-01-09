namespace InventoryManagerAPI.Entities;

public sealed class Client
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public bool Deleted { get; private set; }

    public Client(Guid id, string name)
    {
        Id = id;
        Name = name;
        Deleted = false;
    }

    public static Client Default => new(Guid.Empty, "Default");
    public void UpdateClientInformation(string name) => Name = name;
    public void DeleteClient() => Deleted = true;
}
