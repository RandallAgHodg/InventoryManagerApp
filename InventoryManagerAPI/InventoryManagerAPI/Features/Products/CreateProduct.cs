using FastEndpoints;
using InventoryManagerAPI.Contracts;
using InventoryManagerAPI.Core;
using InventoryManagerAPI.Database;
using InventoryManagerAPI.Entities;
using InventoryManagerAPI.Extensions;
using InventoryManagerAPI.FileStorer;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagerAPI.Features.Products;

public sealed class CreateProduct
{
    public sealed record CreateProductCommand (string name, string price, string cost,
        IFormFile picture, Guid providerId) : Abstractions.Messaging.ICommand<ProductResponse>;

    public sealed class CreateProductCommandHandler : Abstractions.Messaging.ICommandHandler<CreateProductCommand, ProductResponse>
    {
        private readonly DBContext _context;
        private readonly IFileStorer _fileStorer;
        public CreateProductCommandHandler(DBContext context, IFileStorer fileStorer)
        {
            _context = context;
            _fileStorer = fileStorer;
        }

        public async Task<ProductResponse> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var pictureUrl = await _fileStorer.UploadImageAsync(request.picture.ToImageParams());

            var provider = await _context.Set<Provider>()
                .FirstOrDefaultAsync(x => x.Id ==  request.providerId);

            if (provider is null)
                throw new NotFoundException($"Provider with id:{request.providerId} was not found");

            var product = new Product(Guid.NewGuid(), request.name, 0, 
                decimal.Parse(request.price), decimal.Parse(request.cost), pictureUrl, provider.Id);

            await _context.AddAsync(product);

            await _context.SaveChangesAsync();

            return product.ToResponse();
        }

        public sealed class CreateProductEndpointSummary : Summary<CreateProductEndpoint>
        {
            public CreateProductEndpointSummary()
            {
                Description = "Registers a new product which is associated to a provider";
                Summary = "Registers a new product which is associated to a provider";
                Response<ProductResponse>(200, "Product created");
                Response<Contracts.ErrorResponse>(400, "The request was not presented correctly");
            }
        }
        public sealed class CreateProductEndpoint : Endpoint<CreateProductCommand, ProductResponse>
        {
            private readonly IMediator _mediator;

            public CreateProductEndpoint(IMediator mediator) =>
                _mediator = mediator;

            public override void Configure()
            {
                Post("/provider/{providerId}/product");
                AllowFileUploads();
            }

            public override async Task HandleAsync(CreateProductCommand req, CancellationToken ct)
            {
                var result = await _mediator.Send(req);

                await SendOkAsync(result, ct);
            }
        }
    }
}
