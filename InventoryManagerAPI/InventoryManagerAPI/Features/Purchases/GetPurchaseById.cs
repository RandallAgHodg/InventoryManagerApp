using FastEndpoints;
using InventoryManagerAPI.Contracts;
using InventoryManagerAPI.Core;
using InventoryManagerAPI.Database;
using InventoryManagerAPI.Entities;
using InventoryManagerAPI.Extensions;
using InventoryManagerAPI.Services.UserInformationProvider;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagerAPI.Features.Purchases;

public class GetPurchaseById
{
    public sealed record GetPurchaseByIdQuery(Guid Id): Abstractions.Messaging.IQuery<PurchaseResponse>;

    public sealed class GetPurchaseByIdQueryHandler : Abstractions.Messaging.IQueryHandler<GetPurchaseByIdQuery, PurchaseResponse>
    {
        private readonly DBContext _context;
        private readonly IUserInformationProvider _userInformationProvider;

        public GetPurchaseByIdQueryHandler(DBContext context, IUserInformationProvider userInformationProvider)
        {
            _context = context;
            _userInformationProvider = userInformationProvider;
        }

        public async Task<PurchaseResponse> Handle(GetPurchaseByIdQuery request, CancellationToken cancellationToken)
        {
            var userId = _userInformationProvider.UserId;

            var purchase = await _context.Set<Purchase>()
                                 .Include(x => x.User)
                                 .Include(x => x.PurchaseDetails)
                                 .ThenInclude(x => x.Product)
                                 .FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == userId);

            if (purchase is null)
                throw new NotFoundException($"Purchase with id {request.Id} was not found");

            var response = purchase.ToResponse();

            return response;
        }

        public sealed class GetPurchaseByIdEndpintSummary: Summary<GetPurchaseByIdEndpoint>
        {
            public GetPurchaseByIdEndpintSummary()
            {
                Description = "Searches a purchase and its detail by its id and returns it if it was found";
                Summary = "Searches a purchase and its detail by its id and returns it if it was found";
                Response<PurchaseResponse>(200, "Purchase found");
                Response<Contracts.ErrorResponse>(400, "The request was not presented correctly");
                Response<Contracts.ErrorResponse>(404, "Purchase not found");

            }
        }
        public sealed class GetPurchaseByIdEndpoint: Endpoint<GetPurchaseByIdQuery, PurchaseResponse>
        {
            private readonly IMediator _mediator;

            public GetPurchaseByIdEndpoint(IMediator mediator) =>
                _mediator = mediator;

            public override void Configure() =>
                Get("purchase/{Id}");

            public override async Task HandleAsync(GetPurchaseByIdQuery req, CancellationToken ct)
            {
                var result = await _mediator.Send(req);

                await SendOkAsync(result, ct);
            }
        }

    }
}
