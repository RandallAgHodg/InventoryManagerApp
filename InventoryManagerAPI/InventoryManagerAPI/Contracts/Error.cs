namespace InventoryManagerAPI.Contracts;

public sealed record ErrorResponse(IEnumerable<string> errors);
