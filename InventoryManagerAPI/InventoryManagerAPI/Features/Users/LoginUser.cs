using FastEndpoints;
using InventoryManagerAPI.Database;
using InventoryManagerAPI.Entities;
using InventoryManagerAPI.Extensions;
using InventoryManagerAPI.Services.JWTProvider;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductsManagerAPI.Contracts;

namespace InventoryManagerAPI.Features.Users;

public sealed class LoginUser
{
    public sealed record LoginUserCommand(string email, string password) : Abstractions.Messaging.ICommand<AuthResponse>;

    public sealed class LoginUserCommandHandler : Abstractions.Messaging.ICommandHandler<LoginUserCommand, AuthResponse>
    {
        private readonly DBContext _context;
        private readonly IJWTProvider _jwtProvider;

        public LoginUserCommandHandler(DBContext context, IJWTProvider jwtProvider)
        {
            _context = context;
            _jwtProvider = jwtProvider;
        }

        public async Task<AuthResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Set<User>()
                .FirstOrDefaultAsync(x => x.Email == request.email);

            if (user is null)
                throw new BadHttpRequestException("Invalid credentials");

            var passwordMatches = BC.Verify(request.password, user.Password);

            if (!passwordMatches)
                throw new BadHttpRequestException("Invalid credentials");

            var token = _jwtProvider.Create(user.Id.ToString(), user.Email);

            var response = new AuthResponse(user.ToResponse(), token);

            return response;
        }
    }

    public sealed class LoginUserSummary : Summary<LoginUserEndpoint>
    {
        public LoginUserSummary()
        {
            Description = "Logs in a user in the system";
            Summary = "Logs in a user in the system";
            Response<AuthResponse>(200, "User was logged in successfully");
            Response<Contracts.ErrorResponse>(400, "The credentials were not valid");
        }
    }

    public class LoginUserEndpoint : Endpoint<LoginUserCommand, AuthResponse>
    {
        private readonly IMediator _mediator;

        public LoginUserEndpoint(IMediator mediator) =>
            _mediator = mediator;

        public override void Configure()
        {
            Post("/user/login");
            AllowAnonymous();
        }

        public override async Task HandleAsync(LoginUserCommand req, CancellationToken ct)
        {
            var response = await _mediator.Send(req);

            await SendOkAsync(response, ct);
        }
    }

}
