using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Project.session;

namespace Project.auth.session;

public class SessionManager {
    private readonly SymmetricSecurityKey _secretKey;
    private readonly Dictionary<string, SessionData> _sessions = new();

    public SessionManager()
    {
        _secretKey = new SymmetricSecurityKey(Convert.FromBase64String("YourBase64SecretHere"));
    }

    public string AddSession(SessionData authData)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Audience = "Chat-app",
            IssuedAt = DateTime.UtcNow,
            Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, authData.username) }),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(_secretKey, SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var sessionId = tokenHandler.WriteToken(token);

        _sessions[sessionId] = authData;

        return sessionId;
    }

    public void RemoveSession(string sessionId)
    {
        _sessions.Remove(sessionId);
    }

    public SessionData GetSession(string sessionId)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            tokenHandler.ValidateToken(sessionId, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _secretKey,
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out var validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var username = jwtToken.Claims.First(claim => claim.Type == ClaimTypes.Name).Value;

            return new SessionData(username);
        }
        catch
        {
            return null;
        }
    }
}