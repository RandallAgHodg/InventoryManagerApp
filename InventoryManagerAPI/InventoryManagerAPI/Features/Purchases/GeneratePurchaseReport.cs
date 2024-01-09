using FastEndpoints;
using InventoryManagerAPI.Abstractions.Reports;
using InventoryManagerAPI.Core;
using InventoryManagerAPI.Database;
using InventoryManagerAPI.Entities;
using InventoryManagerAPI.Extensions;
using InventoryManagerAPI.FileStorer;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagerAPI.Features.Purchases;

public sealed class GeneratePurchaseReport
{
    public sealed record GeneratePurchaseReportQuery(Guid Id) : Abstractions.Messaging.IQuery<string>;

    public sealed class GeneratePurchaseReportQueryHandler : Abstractions.Messaging.IQueryHandler<GeneratePurchaseReportQuery, string>
    {
        private readonly DBContext _context;
        private readonly IFileStorer _fileStorer;
        public GeneratePurchaseReportQueryHandler(DBContext context, IFileStorer fileStorer)
        {
            _context = context;
            _fileStorer = fileStorer;
        }

        public async Task<string> Handle(GeneratePurchaseReportQuery request, CancellationToken cancellationToken)
        {
            var purchase = await _context.Set<Purchase>()
                            .Include(x => x.User)
                            .Include(x => x.PurchaseDetails)
                            .ThenInclude(x => x.Product)
                            .FirstAsync(x => x.Id == request.Id);

            if (purchase is null)
                throw new NotFoundException($"Purchase with id ${request.Id} was not found");
                               
            var report = PurchaseReportCreator.PurchasePdfReport(purchase);

            if (report is null)
                throw new Exception("Error on creating report. Please try again later");
            
            var result = await _fileStorer.UploadDocumentAsync(report.ToRawUploadParams(purchase.Id));

            return result.ToString();
        }
    }


    public sealed class GeneratePurchaseReportEndpoint : Endpoint<GeneratePurchaseReportQuery, string>
    {
        private readonly IMediator _mediator;

        public GeneratePurchaseReportEndpoint(IMediator mediator) =>
            _mediator = mediator;


        public override void Configure() =>
            Get("purchase/{Id}/report");

        public override async Task HandleAsync(GeneratePurchaseReportQuery req, CancellationToken ct)
        {
            var result = await _mediator.Send(req);

            await SendOkAsync(result, ct);
        }
    }
}
