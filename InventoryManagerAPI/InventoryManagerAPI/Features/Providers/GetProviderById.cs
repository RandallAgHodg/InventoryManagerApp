using FastEndpoints;
using InventoryManagerAPI.Abstractions.Messaging;
using InventoryManagerAPI.Contracts;
using InventoryManagerAPI.Core;
using InventoryManagerAPI.Database;
using InventoryManagerAPI.Entities;
using InventoryManagerAPI.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagerAPI.Features.Providers;

public sealed class GetProviderById
{
    public sealed record GetProviderByIdQuery(Guid Id): IQuery<ProviderWithProductsResponse>;

    public sealed record GetProviderByIdQueryHandler : IQueryHandler<GetProviderByIdQuery, ProviderWithProductsResponse>
    {
        private readonly DBContext _context;

        public GetProviderByIdQueryHandler(DBContext context) =>
            _context = context;

        public async Task<ProviderWithProductsResponse> Handle(GetProviderByIdQuery request, CancellationToken cancellationToken)
        {
            var provider = await _context.Set<Provider>()
                .Include(x => x.Products)
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (provider is null)
                throw new NotFoundException($"The provider with id: {request.Id} was not found");

            return provider.ToProviderWithProductsResponse();
        }
    }

    public sealed class GetProviderByIdQuerySummary: Summary<GetProviderByIdEndpoint>
    {
        public GetProviderByIdQuerySummary()
        {
            Summary = "Finds a user by their id if exists";
            Description = "Finds a user by their id if exists";
            Response<ProviderResponse>(200, "The provider was found");
            Response<Contracts.ErrorResponse>(400, "The request was not presented correctly");
        }
    }

    public sealed class GetProviderByIdEndpoint : Endpoint<GetProviderByIdQuery, ProviderWithProductsResponse>
    {
        private readonly IMediator _mediator;

        public GetProviderByIdEndpoint(IMediator mediator) =>
            _mediator = mediator;

        public override void Configure()
        {
            Get("/provider/{id}");
        }

        public override async Task HandleAsync(GetProviderByIdQuery req, CancellationToken ct)
        {
            var result = await _mediator.Send(req);

            await SendOkAsync(result, ct);
        }
    }
}
