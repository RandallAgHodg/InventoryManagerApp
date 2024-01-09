using FastEndpoints;
using InventoryManagerAPI.Abstractions.Messaging;
using InventoryManagerAPI.Contracts;
using InventoryManagerAPI.Database;
using InventoryManagerAPI.Entities;
using InventoryManagerAPI.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagerAPI.Features.Products;

public sealed class FindProducts
{
    public sealed record FindProductsQuery(string? name) : IQuery<IEnumerable<ProductResponse>>;

    public sealed class FindProductsQueryHandler : IQueryHandler<FindProductsQuery, IEnumerable<ProductResponse>>
    {
        private readonly DBContext _context;

        public FindProductsQueryHandler(DBContext context) =>
            _context = context;

        public async Task<IEnumerable<ProductResponse>> Handle(FindProductsQuery request, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(request.name))
                return await _context.Set<Product>()
                    .Where(x => x.Name.Contains(request.name))
                    .Select(x => x.ToResponse())
                    .ToListAsync();

            return await _context.Set<Product>()
                .Select(x => x.ToResponse())
                .ToListAsync();
        }
    }

    public sealed class FindProductsEndpointSummary : Summary<FindProductsEndpoint>
    {
        public FindProductsEndpointSummary()
        {
            Description = "If provided a filter, returns all the products that matches the given criteria, if not. Returns all the products registered";
            Summary = "If provided a filter, returns all the products that matches the given criteria, if not. Returns all the products registered";
            Response<IEnumerable<ProductResponse>>(200, "The products were returned");
            Response<Contracts.ErrorResponse>(400, "The request was not presented correctly");
        }
    }

    public sealed class FindProductsEndpoint : Endpoint<FindProductsQuery, IEnumerable<ProductResponse>>
    {
        private readonly IMediator _mediator;

        public FindProductsEndpoint(IMediator mediator) =>
            _mediator = mediator;

        public override void Configure() =>
            Get("/product");

        public override async Task HandleAsync(FindProductsQuery req, CancellationToken ct)
        {
            var result = await _mediator.Send(req);

            await SendOkAsync(result, ct);
        }
    }
}
