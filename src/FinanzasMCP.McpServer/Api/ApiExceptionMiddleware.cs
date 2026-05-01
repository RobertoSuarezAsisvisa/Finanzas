using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FinanzasMCP.McpServer.Api;

public sealed class ApiExceptionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (InvalidOperationException ex)
        {
            await WriteProblem(context, StatusCodes.Status400BadRequest, "Bad Request", ex.Message, "invalid-operation");
        }
        catch (ArgumentException ex)
        {
            await WriteProblem(context, StatusCodes.Status400BadRequest, "Bad Request", ex.Message, "argument");
        }
        catch (Exception ex)
        {
            await WriteProblem(context, StatusCodes.Status500InternalServerError, "Internal Server Error", ex.Message, "unhandled");
        }
    }

    private static async Task WriteProblem(HttpContext context, int statusCode, string title, string detail, string errorType)
    {
        if (context.Response.HasStarted)
        {
            throw new InvalidOperationException("The response has already started.");
        }

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Type = $"https://httpstatuses.com/{statusCode}"
        };
        problem.Extensions["errorType"] = errorType;
        problem.Extensions["traceId"] = context.TraceIdentifier;

        await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
    }
}
