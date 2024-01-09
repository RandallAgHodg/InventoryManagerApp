using FastEndpoints;
using InventoryManagerAPI.Database;
using InventoryManagerAPI.Entities;
using InventoryManagerAPI.Extensions;
using InventoryManagerAPI.FileStorer;
using InventoryManagerAPI.Services.JWTProvider;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductsManagerAPI.Contracts;

namespace InventoryManagerAPI.Features.Users;

public sealed class RegisterUser
{

    public sealed record RegisterUserCommand(string firstname, string lastname, string email, string password, IFormFile picture) : Abstractions.Messaging.ICommand<AuthResponse>;


    public sealed class RegisterUserCommandHandler : Abstractions.Messaging.ICommandHandler<RegisterUserCommand, AuthResponse>
    {
        private readonly DBContext _context;
        private readonly IFileStorer _fileStorer;
        private readonly IJWTProvider _jWTProvider;

        public RegisterUserCommandHandler(DBContext context, IFileStorer fileStorer, IJWTProvider jWTProvider)
        {
            _context = context;
            _fileStorer = fileStorer;
            _jWTProvider = jWTProvider;
        }

        public async Task<AuthResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var pictureUrl = await _fileStorer.UploadImageAsync(request.picture.ToImageParams());

            var existsByEmail = await _context.Set<User>()
                .AnyAsync(x => x.Email == request.email);

            if (existsByEmail)
                throw new BadHttpRequestException("There is already a user with that email");

            var user = new User(new Guid(), request.firstname, request.lastname, 
                request.email, BC.HashPassword(request.password), pictureUrl);
        
            await _context.AddAsync(user);

            await _context.SaveChangesAsync();

            var token = _jWTProvider.Create(user.Id.ToString(), user.Email);

            var response = new AuthResponse(user.ToResponse(), token);

            return response;
        }
    }
    public sealed class RegisterUserSummary : Summary<RegisterUserEndpoint>
    {
        public RegisterUserSummary()
        {
            Description = "Register a new user in the system";
            Summary = "Registers a new user in the system";
            Response<AuthResponse>(201, "User was registered successfully");
            Response<ErrorResponse>(400, "The request was not presented correctly");
        }
    }

    public sealed class RegisterUserRequestValidator: Validator<RegisterUserRequest>
    {
        public RegisterUserRequestValidator()
        {
            RuleFor(x => x.firstname)
                .NotEmpty()
                .NotNull()
                .WithMessage("Firstname field is required");

            RuleFor(x => x.lastname)
                .NotEmpty()
                .NotNull()
                .WithMessage("Lastname field is required");

            RuleFor(x => x.email)
                .NotEmpty()
                .NotNull()
                .WithMessage("Email field is required")
                .EmailAddress()
                .WithMessage("Email was not given in a rigth format");

            RuleFor(x => x.password)
                .NotEmpty()
                .NotNull()
                .WithMessage("Password field is required")
                .MinimumLength(8)
                .WithMessage("Password must be 8 characters long");
        }
    }

    public class RegisterUserEndpoint : Endpoint<RegisterUserRequest, AuthResponse>
    {
        private readonly IMediator _mediator;

        public RegisterUserEndpoint(IMediator mediator) =>
            _mediator = mediator;

        public override void Configure()
        {
            Post("/user/register");
            AllowFileUploads();
            AllowAnonymous();
        }

        public override async Task HandleAsync(RegisterUserRequest req, CancellationToken ct)
        {
            var command = new RegisterUserCommand(req.firstname, req.lastname, req.email,
                req.password, req.picture);

            var response = await _mediator.Send(command);
            
            await SendOkAsync(response);
        }
    }
}
