using FastEndpoints;
using InventoryManagerAPI.Contracts;
using InventoryManagerAPI.Core;
using InventoryManagerAPI.Database;
using InventoryManagerAPI.Entities;
using InventoryManagerAPI.Services.UserInformationProvider;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagerAPI.Features.Purchases;

public sealed class DeletePurchase
{
    public sealed record DeletePurchaseCommand(Guid Id) : Abstractions.Messaging.ICommand<Unit>;

    public sealed class DeletePurchaseCommandHandler : Abstractions.Messaging.ICommandHandler<DeletePurchaseCommand, Unit>
    {
        private readonly DBContext _context;
        private readonly IUserInformationProvider _userInformationProvider;

        public DeletePurchaseCommandHandler(DBContext context, IUserInformationProvider userInformationProvider)
        {
            _context = context;
            _userInformationProvider = userInformationProvider;
        }

        public async Task<Unit> Handle(DeletePurchaseCommand request, CancellationToken cancellationToken)
        {
            var userId = _userInformationProvider.UserId;

            var purchase = await _context.Set<Purchase>()
                            .Include(x => x.PurchaseDetails)
                            .FirstOrDefaultAsync(x => x.UserId == userId && x.Id == request.Id);

            if (purchase is null)
                throw new NotFoundException($"Purchase with Id {request.Id} was not found");

            foreach (var detail in purchase.PurchaseDetails)
            {
                var product = await _context.Set<Product>()
                                .FirstOrDefaultAsync(x => x.Id == detail.ProductId);
                
                product?.ReduceStock(detail.Amount);
            }

            purchase.Delete();

            await _context.SaveChangesAsync();

            return Unit.Value;
        }
    }

    public sealed class DeletePurchaseEndpoint : Endpoint<DeletePurchaseCommand, InformationResponse>
    {
        private readonly IMediator _mediator;

        public DeletePurchaseEndpoint(IMediator mediator) =>
            _mediator = mediator;

        public override void Configure() =>
            Delete("/purchase/{Id}");

        public override async Task HandleAsync(DeletePurchaseCommand req, CancellationToken ct)
        {
            await _mediator.Send(req);

            await SendOkAsync(new InformationResponse("The purchase was deleted successfully"), ct);
        }
    }
}
