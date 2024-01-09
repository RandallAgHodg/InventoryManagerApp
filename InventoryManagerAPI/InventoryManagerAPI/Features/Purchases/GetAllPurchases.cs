using InventoryManagerAPI.Contracts;
using InventoryManagerAPI.Database;
using InventoryManagerAPI.Services.UserInformationProvider;
using Microsoft.EntityFrameworkCore;
using InventoryManagerAPI.Entities;
using InventoryManagerAPI.Extensions;
using FastEndpoints;
using MediatR;

namespace InventoryManagerAPI.Features.Purchases;

public sealed class GetAllPurchases
{
    public sealed record GetAllPurchasesQuery : Abstractions.Messaging.IQuery<IEnumerable<PurchaseResponse>>;

    public sealed class GetAllPurchasesQueryHandler : Abstractions.Messaging.IQueryHandler<GetAllPurchasesQuery, IEnumerable<PurchaseResponse>>
    {
        private readonly DBContext _context;
        private readonly IUserInformationProvider _userInformationProvider;

        public GetAllPurchasesQueryHandler(DBContext context, IUserInformationProvider userInformationProvider)
        {
            _context = context;
            _userInformationProvider = userInformationProvider;
        }

        public async Task<IEnumerable<PurchaseResponse>> Handle(GetAllPurchasesQuery request, CancellationToken cancellationToken)
        {
            var userId = _userInformationProvider.UserId;

            var purchases = await _context.Set<Purchase>()
                                .Include(x => x.User)
                                .Include(x => x.PurchaseDetails)
                                .ThenInclude(x => x.Product)
                                .ToListAsync();

            var response = purchases.Select(x => x.ToResponse()).OrderByDescending(x => x.createdAt);

            return response;
        }
    }

    public sealed class GetAllPurchasesEndpoint : Endpoint<GetAllPurchasesQuery, IEnumerable<PurchaseResponse>>
    {
        private readonly IMediator _mediator;

        public GetAllPurchasesEndpoint(IMediator mediator) =>
            _mediator = mediator;

        public override void Configure() =>
            Get("/purchase");

        public override async Task HandleAsync(GetAllPurchasesQuery req, CancellationToken ct)
        {
            var result = await _mediator.Send(req);

            await SendOkAsync(result, ct);
        }
    }
}
