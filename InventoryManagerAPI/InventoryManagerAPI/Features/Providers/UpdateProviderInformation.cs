using FastEndpoints;
using InventoryManagerAPI.Contracts;
using InventoryManagerAPI.Core;
using InventoryManagerAPI.Database;
using InventoryManagerAPI.Entities;
using InventoryManagerAPI.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagerAPI.Features.Providers;

public sealed class UpdateProviderInformation
{
    public sealed record UpdateProviderInformationCommand(Guid Id, string name) : Abstractions.Messaging.ICommand<ProviderResponse>;

    public sealed class UpdateProviderInformationCommandHandler : Abstractions.Messaging.ICommandHandler<UpdateProviderInformationCommand, ProviderResponse>
    {
        private readonly DBContext _context;

        public UpdateProviderInformationCommandHandler(DBContext context) =>
            _context = context;

        public async Task<ProviderResponse> Handle(UpdateProviderInformationCommand request, CancellationToken cancellationToken)
        {
            var provider = await _context.Set<Provider>()
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (provider is null)
                throw new NotFoundException($"A provider with id: {request.Id} was not found");

            provider.UpdateProviderInformation(request.name);

            await _context.SaveChangesAsync();

            return provider.ToResponse();
        }

        public sealed class UpdateProviderInformationValidator: Validator<UpdateProviderInformationCommand>
        {
            public UpdateProviderInformationValidator()
            {
                RuleFor(x => x.name)
                    .NotEmpty()
                    .NotNull()
                    .WithMessage("Name field is required");
            }
        }

        public sealed class UpdateProviderInformationProviderSummary : Summary<UpdateProviderInformationEndpoint>
        {
            public UpdateProviderInformationProviderSummary()
            {
                Description = "Finds a user by id and updates their information";
                Summary = "Finds a user by id and updates their information";
                Response<ProviderResponse>(200, "Provider`s information was updated successfully");
                Response<Contracts.ErrorResponse>(400, "Request was not presented correctly");
                Response<Contracts.ErrorResponse>(404, "Provider was not found");
            }
        } 

        public sealed class UpdateProviderInformationEndpoint: Endpoint<UpdateProviderInformationCommand, ProviderResponse>
        {
            private readonly IMediator _mediator;

            public UpdateProviderInformationEndpoint(IMediator mediator) =>
                _mediator = mediator;

            public override void Configure() =>
                Put("provider/{Id}");

            public override async Task HandleAsync(UpdateProviderInformationCommand req, CancellationToken ct)
            {
                var result = await _mediator.Send(req);

                await SendOkAsync(result);
            }
        }
    }
}
