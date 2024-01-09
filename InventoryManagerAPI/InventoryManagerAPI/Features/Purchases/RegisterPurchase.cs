using FastEndpoints;
using InventoryManagerAPI.Contracts;
using InventoryManagerAPI.Database;
using InventoryManagerAPI.Entities;
using InventoryManagerAPI.Extensions;
using InventoryManagerAPI.Services.UserInformationProvider;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagerAPI.Features.Purchases;

public sealed class RegisterPurchase
{
    public sealed record RegisterPurchaseCommand(IEnumerable<PurchaseRequest> details) : Abstractions.Messaging.ICommand<PurchaseResponse>;

    public sealed class RegisterPurchaseCommandHandler : Abstractions.Messaging.ICommandHandler<RegisterPurchaseCommand, PurchaseResponse>
    {
        private readonly DBContext _context;
        private readonly IUserInformationProvider _userInformationProvider;

        public RegisterPurchaseCommandHandler(DBContext context, IUserInformationProvider userInformationProvider)
        {
            _context = context;
            _userInformationProvider = userInformationProvider;
        }

        public async Task<PurchaseResponse> Handle(RegisterPurchaseCommand request, CancellationToken cancellationToken)
        {
            var userId = _userInformationProvider.UserId;

            var purchase = new Purchase(Guid.NewGuid(), 0, userId);

            foreach (var detailRequest in request.details)
            {
                var product = await _context.Set<Product>()
                                    .FirstOrDefaultAsync(x => x.Id == detailRequest.productId);

                var detail = new PurchaseDetail(Guid.NewGuid(), detailRequest.productId, purchase.Id, detailRequest.amount, product.Cost);

                purchase.AddDetail(detail);

                purchase.UpdateTotal(detail.Subtotal);

                product.AddStock(detailRequest.amount);
            }

            await _context.AddAsync(purchase);

            await _context.SaveChangesAsync();

            var purchaseResponse = await _context.Set<Purchase>()
                .Include(x => x.User)
                .Include(x => x.PurchaseDetails)
                .ThenInclude(x => x.Product)
                .FirstOrDefaultAsync(x => x.Id == purchase.Id);

            return purchaseResponse.ToResponse();
        }
    }

    public sealed class RegisterPurchaseEndpoint : Endpoint<RegisterPurchaseCommand, PurchaseResponse>
    {
        private readonly IMediator _mediator;

        public RegisterPurchaseEndpoint(IMediator mediator) =>
            _mediator = mediator;

        public override void Configure() =>
            Post("/purchase");

        public override async Task HandleAsync(RegisterPurchaseCommand req, CancellationToken ct)
        {
            var result = await _mediator.Send(req);

            await SendOkAsync(result);
        }
    }
}


