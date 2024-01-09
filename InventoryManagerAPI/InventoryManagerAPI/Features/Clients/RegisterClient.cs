using InventoryManagerAPI.Database;
using InventoryManagerAPI.Entities;
using InventoryManagerAPI.Services.UserInformationProvider;
using MediatR;

namespace InventoryManagerAPI.Features.Clients;

public sealed class RegisterClient
{
    public sealed record RegisterClientQuery(string? name): Abstractions.Messaging.IQuery<Unit>;

    public sealed class RegisterClientQueryHandler : Abstractions.Messaging.IQueryHandler<RegisterClientQuery, Unit>
    {
        private readonly DBContext _context;

        public RegisterClientQueryHandler(DBContext context) =>
            _context = context;

        public async Task<Unit> Handle(RegisterClientQuery request, CancellationToken cancellationToken)
        {
            Client client = string.IsNullOrEmpty(request.name) ? 
                Client.Default
                : new Client(Guid.NewGuid(), request.name);

            await _context.AddAsync(client);

            await _context.SaveChangesAsync();

            return Unit.Value;
        }


    }

}
