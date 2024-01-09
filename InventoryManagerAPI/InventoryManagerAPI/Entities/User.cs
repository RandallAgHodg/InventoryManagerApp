namespace InventoryManagerAPI.Entities;

public sealed class User
{
    private readonly HashSet<Purchase> _purchases = new();
    private readonly HashSet<Sale> _sales = new();

    public Guid Id { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
    public string Password { get; private set; }
    public string PictureUrl { get; private set; }
    public IReadOnlyCollection<Purchase> Purchases => _purchases;
    public IReadOnlyCollection<Sale> Sales => _sales;

    public User(Guid id, string firstName, string lastName, string email, string password, string pictureUrl)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Password = password;
        PictureUrl = pictureUrl;
    }
}
