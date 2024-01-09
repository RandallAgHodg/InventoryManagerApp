using FastEndpoints;
using InventoryManagerAPI.Abstractions.Messaging;
using InventoryManagerAPI.Core;
using InventoryManagerAPI.Database;
using InventoryManagerAPI.Entities;
using InventoryManagerAPI.Services.JWTProvider;
using InventoryManagerAPI.Services.UserInformationProvider;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductsManagerAPI.Contracts;

namespace InventoryManagerAPI.Features.Users;

public sealed class RefreshToken
{
    public sealed record RefreshTokenQuery: IQuery<TokenResponse>;

    public sealed class RefreshTokenQueryHandler : IQueryHandler<RefreshTokenQuery, TokenResponse>
    {
        private readonly IUserInformationProvider _userInformationProvider;
        private readonly IJWTProvider _jwtProvider;
        private readonly DBContext _context;

        public RefreshTokenQueryHandler(IUserInformationProvider userInformationProvider, IJWTProvider jwtProvider, DBContext context)
        {
            _userInformationProvider = userInformationProvider;
            _jwtProvider = jwtProvider;
            _context = context;
        }

        public async Task<TokenResponse> Handle(RefreshTokenQuery request, CancellationToken cancellationToken)
        {
            var userId = _userInformationProvider.UserId;

            var user = await _context.Set<User>()
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (user is null)
                throw new NotFoundException($"User with id {userId} was not found");

            var token = _jwtProvider.Create(user.Id.ToString(), user.Email);

            return new TokenResponse(token);
        }
    }

    public sealed class RefreshTokenQuerySummary : Summary<RefreshTokenEndpoint>
    {
        public RefreshTokenQuerySummary()
        {
            Description = "Refreshes the current logged in user`s token";
            Summary = "Refreshes the current logged in user`s token";
            Response<TokenResponse>(200, "Token refreshed");
        }
    }

    public sealed class RefreshTokenEndpoint : Endpoint<RefreshTokenQuery, TokenResponse>
    {
        private readonly IMediator _mediator;

        public RefreshTokenEndpoint(IMediator mediator) =>
            _mediator = mediator;

        public override void Configure() =>
            Get("/user/refresh-token");

        public override async Task HandleAsync(RefreshTokenQuery req, CancellationToken ct)
        {
            var result = await _mediator.Send(req);

            await SendOkAsync(result, ct);
        }
    }
}
