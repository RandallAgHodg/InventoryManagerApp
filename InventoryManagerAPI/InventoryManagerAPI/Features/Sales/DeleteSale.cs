using FastEndpoints;
using InventoryManagerAPI.Contracts;
using InventoryManagerAPI.Core;
using InventoryManagerAPI.Database;
using InventoryManagerAPI.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagerAPI.Features.Sales;

public sealed class DeleteSale
{
    public sealed record DeleteSaleCommand(Guid Id) : Abstractions.Messaging.ICommand<Unit>;

    public sealed class DeleteSaleCommandHandler : Abstractions.Messaging.ICommandHandler<DeleteSaleCommand, Unit>
    {
        private readonly DBContext _context;

        public DeleteSaleCommandHandler(DBContext context) =>
            _context = context;

        public async Task<Unit> Handle(DeleteSaleCommand request, CancellationToken cancellationToken)
        {
            var sale = await _context.Set<Sale>()
                            .Include(x => x.SaleDetails)
                            .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (sale is null)
                throw new NotFoundException($"Sale with id {request.Id} was not found");

            foreach (var detail in sale.SaleDetails)
            {
                var product = await _context.Set<Product>()
                                    .FirstOrDefaultAsync(x => x.Id == detail.ProductId);

                product?.AddStock(detail.Amount);
            }

            sale.Delete();

            await _context.SaveChangesAsync();

            return Unit.Value;
        }
    }

    public sealed class DeleteSaleEndpoint : Endpoint<DeleteSaleCommand, InformationResponse>
    {
        private readonly IMediator _mediator;

        public DeleteSaleEndpoint(IMediator mediator) =>
            _mediator = mediator;
        public override void Configure() =>
            Delete("sale/{Id}");
        public override async Task HandleAsync(DeleteSaleCommand req, CancellationToken ct)
        {
            await _mediator.Send(req);

            await SendOkAsync(new InformationResponse("The sale was deleted successfully"), ct);
        }
    }
}
