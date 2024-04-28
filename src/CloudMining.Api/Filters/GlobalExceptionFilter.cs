using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CloudMining.Api.Filters;

public sealed class GlobalExceptionFilter : IExceptionFilter
{
    private readonly IWebHostEnvironment _env;

    public GlobalExceptionFilter(IWebHostEnvironment env)
    {
        _env = env;
    }

    public void OnException(ExceptionContext context)
    {
        var statusCode = HttpStatusCode.InternalServerError;
        if (context.Exception is ArgumentNullException)
            statusCode = HttpStatusCode.BadRequest;

        var problemDetails = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = "An error occurred while processing your request.",
            Detail = _env.IsDevelopment() ? context.Exception.StackTrace : "A server error occurred.",
            Instance = context.HttpContext.Request.Path
        };

        context.Result = new ObjectResult(problemDetails)
        {
            StatusCode = (int)statusCode
        };

        context.ExceptionHandled = true;
    }
}