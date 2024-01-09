using FastEndpoints;
using InventoryManagerAPI.Contracts;
using InventoryManagerAPI.Database;
using InventoryManagerAPI.Entities;
using InventoryManagerAPI.Extensions;
using InventoryManagerAPI.Services.UserInformationProvider;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagerAPI.Features.Sales;

public sealed class RegisterSale
{
    public sealed record RegisterSaleCommand(IEnumerable<PurchaseRequest> details, string? client) : Abstractions.Messaging.ICommand<SaleResponse>;

    public sealed record RegisterSaleCommandHandler : Abstractions.Messaging.ICommandHandler<RegisterSaleCommand, SaleResponse>
    {
        private readonly IUserInformationProvider _userInformationProvider;
        private readonly DBContext _context;

        public RegisterSaleCommandHandler(IUserInformationProvider userInformationProvider, DBContext context)
        {
            _userInformationProvider = userInformationProvider;
            _context = context;
        }

        public async Task<SaleResponse> Handle(RegisterSaleCommand request, CancellationToken cancellationToken)
        {
            var userId = _userInformationProvider.UserId;

            var client = await _context.Set<Client>()
                                .FirstOrDefaultAsync(x => x.Name == request.client);

            if (client is null)
                client = await _context.Set<Client>()
                                .FirstOrDefaultAsync(x => x.Id == Client.Default.Id);

            var sale = new Sale(Guid.NewGuid(), 0, client.Id, userId);
            
            sale.AssociateClient(client);

            foreach (var saleRequest in request.details)
            {
                var product = await _context.Set<Product>()
                                    .FirstOrDefaultAsync(x => x.Id == saleRequest.productId);

                var detail = new SaleDetail(Guid.NewGuid(), saleRequest.productId, 
                    sale.Id, saleRequest.amount, product.Price);

                var isValidProductAmount = product.CheckStockForSaleAmount(detail.Amount);

                if(!isValidProductAmount)
                    throw new BadHttpRequestException($"There is not enough stock to satisfy sale for product {product.Name}");

                sale.AddDetail(detail);

                sale.UpdateTotal(detail.Subtotal);

                product.ReduceStock(saleRequest.amount);
            }

            await _context.AddAsync(sale);

            await _context.SaveChangesAsync();

            var saleResponse = await _context.Set<Sale>()
                .Include(x => x.User)
                .Include(x => x.SaleDetails)
                .ThenInclude(x => x.Product)
                .FirstOrDefaultAsync(x => x.Id == sale.Id);

            return saleResponse.ToResponse();
        }
    }

    public sealed class RegisterSaleValidator : Validator<RegisterSaleCommand>
    {
        public RegisterSaleValidator()
        {
            RuleFor(x => x.details)
                .Must((details) => details.All(x => x.amount > 0))
                .WithMessage("All products must be sold with a prositive amount");
        }

        public bool AmountIsPositive(IEnumerable<PurchaseRequest> details) =>
            details.Any(x => x.amount < 0);

    }
    public sealed class RegisterSaleEndpoint: Endpoint<RegisterSaleCommand, SaleResponse>
    {
        private readonly IMediator _mediator;

        public RegisterSaleEndpoint(IMediator mediator) =>
            _mediator = mediator;

        public override void Configure() =>
            Post("/sale");

        public override async Task HandleAsync(RegisterSaleCommand req, CancellationToken ct)
        {
            var result = await _mediator.Send(req);

            await SendOkAsync(result, ct);
        }
    }
}
