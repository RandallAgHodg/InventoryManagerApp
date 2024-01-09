using FastEndpoints;
using InventoryManagerAPI.Abstractions.Messaging;
using InventoryManagerAPI.Contracts;
using InventoryManagerAPI.Database;
using InventoryManagerAPI.Entities;
using InventoryManagerAPI.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagerAPI.Features.Providers;

public sealed class CreateProvider
{
    public sealed record CreateProviderCommand(string name) : Abstractions.Messaging.ICommand<ProviderResponse>;

    public sealed class CreateProviderCommandHandler : Abstractions.Messaging.ICommandHandler<CreateProviderCommand, ProviderResponse>
    {
        private readonly DBContext _context;

        public CreateProviderCommandHandler(DBContext context) =>
            _context = context;

        public async Task<ProviderResponse> Handle(CreateProviderCommand request, CancellationToken cancellationToken)
        {
            var existsByName = await _context.Set<Provider>()
                .AnyAsync(x => x.Name == request.name);

            if (existsByName)
                throw new BadHttpRequestException("There is already a provider registered with that name");

            var provider = new Provider(Guid.NewGuid(), request.name);

            await _context.AddAsync(provider);

            await _context.SaveChangesAsync();

            var response = provider.ToResponse();

            return response;
        }
    }

    public sealed class CreateProviderRequestValidator : Validator<CreateProviderCommand>
    {
        public CreateProviderRequestValidator()
        {
            RuleFor(x => x.name)
                .NotEmpty()
                .NotNull()
                .WithMessage("Name field is required");
        }
    }

    public sealed class CreateProviderSummary : Summary<CreateProviderEndpoint>
    {
        public CreateProviderSummary()
        {
            Description = "Resgisters a new provider";
            Summary = "Resgisters a new provider";
            Response<ProviderResponse>(200, "User was registered successfully");
            Response<Contracts.ErrorResponse>(400, "Request was not presented correctly");
        }
    }

    public sealed class CreateProviderEndpoint : Endpoint<CreateProviderCommand, ProviderResponse>
    {
        private readonly IMediator _mediator;

        public CreateProviderEndpoint(IMediator mediator) =>
            _mediator = mediator;

        public override void Configure() =>
            Post("/provider");


        public override async Task HandleAsync(CreateProviderCommand req, CancellationToken ct)
        {
            var result = await _mediator.Send(req);

            await SendOkAsync(result, ct);
        }
    }
}
