using FastEndpoints;
using InventoryManagerAPI.Contracts;
using InventoryManagerAPI.Core;
using InventoryManagerAPI.Database;
using InventoryManagerAPI.Entities;
using InventoryManagerAPI.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagerAPI.Features.Products;

public sealed class UpdateProductInformation
{
    public sealed record UpdateProductInformationCommand(Guid Id, string name, decimal price, decimal cost) : Abstractions.Messaging.ICommand<ProductResponse>;

    public sealed class UpdateProductInformationCommandHandler : Abstractions.Messaging.ICommandHandler<UpdateProductInformationCommand, ProductResponse>
    {
        private readonly DBContext _context;

        public UpdateProductInformationCommandHandler(DBContext context) =>
            _context = context;

        public async Task<ProductResponse> Handle(UpdateProductInformationCommand request, CancellationToken cancellationToken)
        {
            var product = await _context.Set<Product>()
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (product is null)
                throw new NotFoundException($"The product with id {request.Id} was not found");

            product.UpdateInformation(request.name, request.price , request.cost);

            await _context.SaveChangesAsync();

            return product.ToResponse();
        }
    }   

    public sealed class UpdateProductInformationSummary : Summary<UpdateProductInformationEndpoint>
    {
        public UpdateProductInformationSummary()
        {
            Description = "Finds a product by its id and updates it information";
            Summary = "Finds a product by its id and updates it information";
            Response<ProductResponse>(200, "Product information updated");
            Response<Contracts.ErrorResponse>(400, "The request was not presented correctly");
            Response<Contracts.ErrorResponse>(404, "The product was not found");
        }
    }

    public sealed class UpdateProductInformationEndpoint : Endpoint<UpdateProductInformationCommand, ProductResponse>
    {
        private readonly IMediator _mediator;

        public UpdateProductInformationEndpoint(IMediator mediator) =>
            _mediator = mediator;

        public override void Configure() =>
            Put("/product/{Id}");

        public override async Task HandleAsync(UpdateProductInformationCommand req, CancellationToken ct)
        {
            var result = await _mediator.Send(req);

            await SendOkAsync(result, ct);
        }
    }

}
