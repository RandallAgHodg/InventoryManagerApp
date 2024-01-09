using FastEndpoints;
using InventoryManagerAPI.Abstractions.Messaging;
using InventoryManagerAPI.Contracts;
using InventoryManagerAPI.Database;
using InventoryManagerAPI.Entities;
using InventoryManagerAPI.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagerAPI.Features.Providers;

public sealed class FindProviderByName
{
    public sealed class FindProviderByNameQuery : IQuery<IEnumerable<ProviderResponse>>
    {
        [QueryParam]
        public string? Name { get; set; }
    }

    public sealed class FindProviderByNameQueryHandler: IQueryHandler<FindProviderByNameQuery, IEnumerable<ProviderResponse>>
    {
        private readonly DBContext _context;

        public FindProviderByNameQueryHandler(DBContext context) =>
            _context = context;

        public async Task<IEnumerable<ProviderResponse>> Handle(FindProviderByNameQuery request, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(request.Name))
            {
                var providers = await _context.Set<Provider>()
                    .Where(x => x.Name.Contains(request.Name))
                    .ToListAsync();

                return providers.Select(x => x.ToResponse());
            }

            return await _context.Set<Provider>()
                .Select(x => x.ToResponse())
                .ToListAsync();

        }
    }

    public sealed class FindProviderByNameSummary : Summary<FindProviderByNameEndpoint>
    {
        public FindProviderByNameSummary()
        {
            Summary = "Finds a provider by its name";
            Description = "Finds a provider by its name";
            Response<IEnumerable<ProviderResponse>>(200, "Providers were returned");
            Response<IEnumerable<Contracts.ErrorResponse>>(400, "The Request was not presented correctly");
        }
    }

    //public sealed class FindProviderByNameValidator : Validator<FindProviderByNameQuery>
    //{
    //    public FindProviderByNameValidator() =>
    //        RuleFor(x => x.Name)
    //            .NotNull()
    //            .WithMessage("The name query parameter can not be null");
    //}

    public sealed class FindProviderByNameEndpoint : Endpoint<FindProviderByNameQuery, IEnumerable<ProviderResponse>>
    {
        private readonly IMediator _mediator;

        public FindProviderByNameEndpoint(IMediator mediator) =>
            _mediator = mediator;

        public override void Configure() =>
            Get("/provider");

        public override async Task HandleAsync(FindProviderByNameQuery req, CancellationToken ct)
        {
            var result = await _mediator.Send(req);

            await SendOkAsync(result, ct);
        }
    }
}
