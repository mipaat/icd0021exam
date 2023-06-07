using Microsoft.AspNetCore.Http;

namespace Base.WebHelpers.Middleware;

public class RequestDebugMiddleware
{
    private readonly RequestDelegate _next;

    public RequestDebugMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        // Add a breakpoint here or any other debugging logic
        Console.WriteLine("Breakpoint triggered at the start of the request pipeline.");

        // Call the next middleware in the pipeline
        await _next(context);
    }
}