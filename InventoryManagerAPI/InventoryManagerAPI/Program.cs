global using FluentValidation;
using CloudinaryDotNet;
using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using FluentValidation.Results;
using InventoryManagerAPI.Database;
using InventoryManagerAPI.FileStorer;
using InventoryManagerAPI.Middleware;
using InventoryManagerAPI.Services.JWTProvider;
using InventoryManagerAPI.Services.UserInformationProvider;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;
using System.Linq;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

var config = builder.Configuration;

services.AddEndpointsApiExplorer();

services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

services.AddFastEndpoints();

QuestPDF.Settings.License = LicenseType.Community;

var cloudSecret = config.GetValue<string>("cloudinary:secret");
var cloudApiKey = config.GetValue<string>("cloudinary:key");
var cloudName = config.GetValue<string>("cloudinary:name");

services.AddSingleton(new Cloudinary(
    new Account(cloudName, cloudApiKey, cloudSecret)));

services.AddSingleton<IFileStorer, FileStorer>();

services.AddSingleton<IJWTProvider, JWTProvider>();

services.AddSingleton<IUserInformationProvider, UserInformationProvider>();

services.AddMediatR(configuration =>
configuration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

services.AddSwaggerDoc();
    
services.AddJWTBearerAuth(config.GetValue<string>("jwt:secret")).AddAuthorization();

services.AddDbContext<DBContext>(options =>
    options.UseSqlServer(config.GetConnectionString("Database")
));

services.AddScoped<DBContext>();

var app = builder.Build();

app.UseFastEndpoints();

app.UseCors(options => options
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseOpenApi();

app.UseSwaggerUi3(s => s.ConfigureDefaults());

app.UseAuthentication();

app.UseAuthorization();

app.Run();
