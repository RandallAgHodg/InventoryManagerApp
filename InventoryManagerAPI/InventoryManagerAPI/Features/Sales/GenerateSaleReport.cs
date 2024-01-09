using FastEndpoints;
using InventoryManagerAPI.Abstractions.Reports;
using InventoryManagerAPI.Core;
using InventoryManagerAPI.Database;
using InventoryManagerAPI.Entities;
using InventoryManagerAPI.Extensions;
using InventoryManagerAPI.FileStorer;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagerAPI.Features.Sales;

public sealed class GenerateSaleReport
{
    public sealed record GenerateSaleReportQuery(Guid Id) : Abstractions.Messaging.IQuery<string>;

    public sealed class GenerateSaleReportQueryHandler : Abstractions.Messaging.IQueryHandler<GenerateSaleReportQuery, string>
    {
        private readonly DBContext _context;
        private readonly IFileStorer _fileStorer;

        public GenerateSaleReportQueryHandler(DBContext context, IFileStorer fileStorer)
        {
            _context = context;
            _fileStorer = fileStorer;
        }

        public async Task<string> Handle(GenerateSaleReportQuery request, CancellationToken cancellationToken)
        {
            var sale = await _context.Set<Sale>()
                        .Include(x => x.User)
                        .Include(x => x.Client)
                        .Include(x => x.SaleDetails)
                        .ThenInclude(x => x.Product)
                        .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (sale is null)
                throw new NotFoundException($"Sale with id ${request.Id} was not found");

            var report = SaleReportCreator.SalePdfReport(sale);

            if (report is null)
                throw new Exception("Error on creating report. Please try again later");

            var result = await _fileStorer.UploadDocumentAsync(report.ToRawUploadParams(sale.Id));

            return result.ToString();
        }
    }

    public sealed class GenerateSaleReportEndpoint : Endpoint<GenerateSaleReportQuery, string>
    {
        private readonly IMediator _mediator;

        public GenerateSaleReportEndpoint(IMediator mediator) =>
            _mediator = mediator;
        public override void Configure() =>
            Get("sale/{Id}/report");
        public override async Task HandleAsync(GenerateSaleReportQuery req, CancellationToken ct)
        {
            var result = await _mediator.Send(req);

            await SendOkAsync(result, ct);
        }
    } 
}
