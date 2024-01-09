using FastEndpoints;
using InventoryManagerAPI.Contracts;
using InventoryManagerAPI.Core;
using InventoryManagerAPI.Database;
using InventoryManagerAPI.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagerAPI.Features.Products;

public sealed class DeleteProduct
{
    public sealed record DeleteProductCommand(Guid Id) : Abstractions.Messaging.ICommand<Unit>;

    public sealed class DeleteProductCommandHandler : Abstractions.Messaging.ICommandHandler<DeleteProductCommand, Unit>
    {
        private readonly DBContext _context;

        public DeleteProductCommandHandler(DBContext context) =>
            _context = context;

        public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _context.Set<Product>()
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (product is null)
                throw new NotFoundException($"The product with id {request.Id} was not found");

            product.DeleteProduct();

            await _context.SaveChangesAsync();

            return Unit.Value; 
        }
    }

    public sealed class DeleteProductEndpointSummaty : Summary<DeleteProductEndpoint>
    {
        public DeleteProductEndpointSummaty()
        {
            Description = "Deletes a product";
            Summary = "Deletes a product";
            Response<InformationResponse>(200, "The  product was deleted successfully");
            Response<Contracts.ErrorResponse>(400, "The request was not presented correctly");
            Response<Contracts.ErrorResponse>(404, "The product was not found");
        }
    }

    public sealed class DeleteProductEndpoint : Endpoint<DeleteProductCommand, InformationResponse>
    {
        private readonly IMediator _mediator;

        public DeleteProductEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override void Configure() =>
            Delete("/product/{Id}");

        public override async Task HandleAsync(DeleteProductCommand req, CancellationToken ct)
        {
            await _mediator.Send(req);

            await SendOkAsync(new InformationResponse("Product was deleted successfully"), ct);
        }
    }
}
