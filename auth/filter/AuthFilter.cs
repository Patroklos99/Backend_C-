using System.Runtime.CompilerServices;
using Google.Api;
using Project.auth.session;

namespace Project.auth.filter;

public class AuthFilter {
    private const string BEARER = "Bearer";
    private readonly RequestDelegate _next;

    public AuthFilter(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, SessionDataAccessor sessionDataAccessor,
        SessionManager sessionManager)
    {
        if (context.Request.Method.Equals("OPTIONS", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        // var authHeader = httpContext.Request.Headers["Authorization"].FirstOrDefault();
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        if (authHeader == null || !authHeader.StartsWith(BEARER))
        {
            SendAuthErrorResponse(context);
            return;
        }

        var authParts = authHeader.Split(" ");
        if (authParts.Length != 2 || !IsValidToken(authParts[1]))
        {
            SendAuthErrorResponse(context);
            return;
        }

        var token = authParts[1];
        var data = sessionManager.GetSession(token);

        if (data == null)
        {
            SendAuthErrorResponse(context);
            return;
        }

        sessionDataAccessor.SetToken(token);
        sessionDataAccessor.SetSessionData(data);

        await _next(context);
    }

    private bool IsValidToken(string token)
    {
        // Add your token validation logic here.
        return true;
    }

    private void SendAuthErrorResponse(HttpContext context)
    {
        if (context.Request.Path.Value.Contains("auth/logout"))
            context.Response.StatusCode = StatusCodes.Status200OK;
        else
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
    }
}