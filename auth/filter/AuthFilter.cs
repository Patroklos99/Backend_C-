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

/*using Project.auth.session;

namespace Project.auth.filter;

public class AuthFilter
{
private const string BEARER = "Bearer";
private readonly RequestDelegate _next;

private readonly SessionDataAccessor _sessionDataAccessor;
private readonly SessionManager _sessionManager;

public AuthFilter(RequestDelegate next, SessionDataAccessor sessionDataAccessor,
    SessionManager sessionManager)
{
    _next = next;
    _sessionDataAccessor = sessionDataAccessor;
    _sessionManager = sessionManager;
}

public async Task Invoke(HttpContext context)
{
    if (context.Request.Method.Equals("OPTIONS", StringComparison.OrdinalIgnoreCase))
    {
        // Allow pre-flight requests to pass
        await _next(context);
        return;
    }

    var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
    if (authHeader == null || !authHeader.StartsWith(BEARER))
    {
        // Request doesn't contain the header and bearer
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
    var data = _sessionManager.GetSession(token);

    if (data == null)
    {
        SendAuthErrorResponse(context);
        return;
    }

    _sessionDataAccessor.SetToken(token);
    _sessionDataAccessor.SetSessionData(data);

    // Continue processing
    await _next(context);
}

private bool IsValidToken(string token)
{
    // Currently accepting all tokens.
    return true;
}

private void SendAuthErrorResponse(HttpContext context)
{
    if (context.Request.Path.Value.Contains("auth/logout"))
        // For logout, just return 200 OK.
        context.Response.StatusCode = StatusCodes.Status200OK;
    else
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
}
}*/