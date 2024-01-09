namespace ProductsManagerAPI.Contracts;

public sealed record RegisterUserRequest(string firstname, string lastname, string email, string password, IFormFile picture);
public sealed record LoginUserRequest(string email, string password);
public sealed record UserResponse(Guid id, string firstname, string lastname, string email, string pictureUrl);
public sealed record TokenResponse(string token);
public sealed record AuthResponse(UserResponse user, string token);