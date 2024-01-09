using FastEndpoints;
using InventoryManagerAPI.Contracts;
using InventoryManagerAPI.Core;
using InventoryManagerAPI.Database;
using InventoryManagerAPI.Entities;
using InventoryManagerAPI.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagerAPI.Features.Providers;

public sealed class DeleteProvider
{
    public sealed record DeleteProviderCommand(Guid Id) : Abstractions.Messaging.ICommand<Unit>;

    public sealed record DeleteProviderCommandHandler : Abstractions.Messaging.ICommandHandler<DeleteProviderCommand, Unit>
    {
        private readonly DBContext _context;

        public DeleteProviderCommandHandler(DBContext context) =>
            _context = context;

        public async Task<Unit> Handle(DeleteProviderCommand request, CancellationToken cancellationToken)
        {
            var provider = await _context.Set<Provider>()
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (provider is null)
                throw new NotFoundException($"A provider with id: {request.Id} was not found");

            provider.DeleteProvider();

            await _context.SaveChangesAsync();

            return Unit.Value;
        }

        public sealed class DeleteProviderSummary : Summary<DeleteProviderEndpoint>
        {
            public DeleteProviderSummary()
            {
                Description = "Finds a provider by id and sets their state to deleted";
                Summary = "Finds a provider by id and sets their state to deleted";
                Response<InformationResponse>(200, "Provider was deleted");
                Response<Contracts.ErrorResponse>(404, "Provider was not found");
            }
        }
        public sealed class DeleteProviderEndpoint: Endpoint<DeleteProviderCommand, InformationResponse>
        {
            private readonly IMediator _mediator;

            public DeleteProviderEndpoint(IMediator mediator) =>
                _mediator = mediator;

            public override void Configure() =>
                Delete("/provider/{Id}");

            public override async Task HandleAsync(DeleteProviderCommand req, CancellationToken ct)
            {
                await _mediator.Send(req);

                await SendOkAsync(new InformationResponse($"The user with id: {req.Id} was deleted successfully"), ct);
            }
        }
    }
}
