using FastEndpoints;
using InventoryManagerAPI.Contracts;
using InventoryManagerAPI.Database;
using InventoryManagerAPI.Entities;
using InventoryManagerAPI.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagerAPI.Features.Sales;

public sealed class GetAllSales
{
    public sealed record GetAllSalesQuery : Abstractions.Messaging.IQuery<IEnumerable<SaleResponse>>;

    public sealed class GetAllSalesQueryHandler : Abstractions.Messaging.IQueryHandler<GetAllSalesQuery, IEnumerable<SaleResponse>>
    {
        private readonly DBContext _context;

        public GetAllSalesQueryHandler(DBContext context) =>
            _context = context;

        public async Task<IEnumerable<SaleResponse>> Handle(GetAllSalesQuery request, CancellationToken cancellationToken)
        { 
            var sales = await _context.Set<Sale>()
                                .Include(x => x.User)
                                .Include(x => x.Client)
                                .Include(x => x.SaleDetails)
                                .ThenInclude(x => x.Product)
                                .ToListAsync();

            var response = sales.Select(x => x.ToResponse());

            return response;
        }
    }

    public sealed class GetAllSalesEndpoint : Endpoint<GetAllSalesQuery, IEnumerable<SaleResponse>>
    {
        private readonly IMediator _mediator;

        public GetAllSalesEndpoint(IMediator mediator) =>
            _mediator = mediator;

        public override void Configure() =>
            Get("sale");

        public override async Task HandleAsync(GetAllSalesQuery req, CancellationToken ct)
        {
            var result = await _mediator.Send(req);

            await SendOkAsync(result, ct);
        }
    }

}
