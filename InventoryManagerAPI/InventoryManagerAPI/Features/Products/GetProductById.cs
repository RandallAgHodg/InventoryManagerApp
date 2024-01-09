using FastEndpoints;
using InventoryManagerAPI.Abstractions.Messaging;
using InventoryManagerAPI.Contracts;
using InventoryManagerAPI.Core;
using InventoryManagerAPI.Database;
using InventoryManagerAPI.Entities;
using InventoryManagerAPI.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagerAPI.Features.Products;

public sealed class GetProductById
{
    public sealed record GetProductByIdQuery(Guid? Id) : IQuery<ProductResponse>;

    public sealed class GetProductByIdQueryHandler : IQueryHandler<GetProductByIdQuery, ProductResponse>
    {
        private readonly DBContext _context;

        public GetProductByIdQueryHandler(DBContext context) =>
            _context = context;

        public async Task<ProductResponse> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _context.Set<Product>()
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (product is null)
                throw new NotFoundException($"The product with id {request.Id} was not found");

            return product.ToResponse();
        }

        public sealed class GetProductByEndpointSummary : Summary<GetProductByIdEndpoint>
        {
            public GetProductByEndpointSummary()
            {
                Description = "Searches a product by its id and returns it if it was found";
                Summary = "Searches a product by its id and returns it if it was found";
                Response<ProductResponse>(200, "Product found");
                Response<Contracts.ErrorResponse>(400, "The request was not presented correctly");
                Response<Contracts.ErrorResponse>(404, "Product not found");
            }
        }
        public sealed class GetProductByIdEndpoint : Endpoint<GetProductByIdQuery, ProductResponse>
        {
            private readonly IMediator _mediator;

            public GetProductByIdEndpoint(IMediator mediator) =>
                _mediator = mediator;

            public override void Configure() =>
                Get("/product/{Id}");
            

            public override async Task HandleAsync(GetProductByIdQuery req, CancellationToken ct)
            {
                var result = await _mediator.Send(req);

                await SendOkAsync(result, ct);
            }
        }
    }
}
