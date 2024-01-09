using InventoryManagerAPI.Contracts;
using InventoryManagerAPI.Core;

namespace InventoryManagerAPI.Middleware;


public sealed class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _request;

    public ExceptionHandlerMiddleware(RequestDelegate request) =>
        _request = request;
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _request(context);
        }
        catch (ValidationException exception)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            if (exception.Errors.Any())
            {
                var messages = exception
                    .Errors
                    .Select(x => x.ErrorMessage)
                    .ToList();
                await context.Response.WriteAsJsonAsync(new ErrorResponse(messages));
            }
            else
            {
                var messages = new[] { exception.Message };

                await context
                    .Response
                    .WriteAsJsonAsync(
                    new ErrorResponse(messages));
            }

        }
        catch (NotFoundException exception) 
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;

            var messages = new[] { exception.Message }; 

            await context
                .Response
                .WriteAsJsonAsync(new ErrorResponse(messages));
        }
        catch (BadHttpRequestException exception)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            var messages = new[] { exception.Message };

            await context
                .Response
                .WriteAsJsonAsync(
                new ErrorResponse(messages));
        }
        catch (Exception exception)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var messages = new[] { exception.Message };

            await context
                .Response
                .WriteAsJsonAsync(
                new ErrorResponse(messages));
        }
    }
}
