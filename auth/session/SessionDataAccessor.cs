using Project.session;

public class SessionDataAccessor {
    private const string TOKEN_KEY = "TOKEN_KEY";
    private const string SESSION_DATA_KEY = "SESSION_DATA_KEY";

    private readonly IHttpContextAccessor _contextAccessor;

    public SessionDataAccessor(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    public void SetSessionData(SessionData sessionData)
    {
        _contextAccessor.HttpContext.Items[SESSION_DATA_KEY] = sessionData;
    }

    public void SetToken(string token)
    {
        _contextAccessor.HttpContext.Items[TOKEN_KEY] = token;
    }

    public SessionData GetSessionData()
    {
        return _contextAccessor.HttpContext.Items[SESSION_DATA_KEY] as SessionData;
    }

    public string GetToken()
    {
        return _contextAccessor.HttpContext.Items[TOKEN_KEY] as string;
    }
}