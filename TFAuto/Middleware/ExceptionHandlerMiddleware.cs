using SendGrid.Helpers.Errors.Model;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace TFAuto.WebApp.Middleware;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            await ProcessException(context, ex.GetBaseException().Message, ex.Message, (int)HttpStatusCode.BadRequest);
        }
        catch (NotFoundException ex)
        {
            await ProcessException(context, ex.GetBaseException().Message, ex.Message, (int)HttpStatusCode.NotFound);
        }
        catch (Exception ex)
        {
            await ProcessException(context, ex.GetBaseException().Message, ex.Message, (int)HttpStatusCode.InternalServerError);
        }
    }

    private static Task ProcessException(HttpContext httpContext, string message, string displayMessage, int statusCode)
    {
        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = statusCode;
        return httpContext.Response.WriteAsync(new ErrorDetails
        {
            StatusCode = httpContext.Response.StatusCode,
            Message = message,
            DisplayMessage = displayMessage
        }
        .ToString());
    }
}