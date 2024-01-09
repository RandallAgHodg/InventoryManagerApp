using FastEndpoints;
using InventoryManagerAPI.Abstractions.Messaging;
using InventoryManagerAPI.Core;
using InventoryManagerAPI.Database;
using InventoryManagerAPI.Entities;
using InventoryManagerAPI.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductsManagerAPI.Contracts;

namespace InventoryManagerAPI.Features.Users;

public sealed class GetUserById
{
    public sealed record GetUserByIdQuery(Guid Id) : IQuery<UserResponse>;

    public sealed record GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, UserResponse>
    {
        private readonly DBContext _context;

        public GetUserByIdQueryHandler(DBContext context) =>
            _context = context;
        

        public async Task<UserResponse> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _context.Set<User>()
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (user is null)
                throw new NotFoundException($"User with id: {request.Id} was not found");

            return user.ToResponse();
        }

        public sealed class GetUserByIdSummary: Summary<GetUserByIdEndpoint>
        {
            public GetUserByIdSummary()
            {
                Description = "Finds a user by their id and returns it";
                Summary = "Finds a user by their id and returns it";
                Response<UserResponse>(200, "User found");
                Response<ErrorResponse>(404, "User was not found");
            }
        }

        public sealed class GetUserByIdEndpoint : Endpoint<GetUserByIdQuery, UserResponse>
        {
            private readonly IMediator _mediator;

            public GetUserByIdEndpoint(IMediator mediator) =>
                _mediator = mediator;

            public override void Configure()
            {
                Get("/user/{Id}");
            }

            public override async Task HandleAsync(GetUserByIdQuery request, CancellationToken ct)
            {
                var result = await _mediator.Send(request);

                await SendOkAsync(result, ct);
            }
        }
    }
}
