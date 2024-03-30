using Microsoft.AspNetCore.Mvc;
using Project.auth.firestorerepository;
using Project.auth.model;
using Project.auth.session;
using Project.session;

namespace Project.auth;

/**
 * Contrôleur qui gère l'API de login et logout.
 *
 * Implémente ServletContextAware pour recevoir le contexte de la requête HTTP.
 */
[ApiController]
// [Route("auth/")]
// [EnableCors("AllowAllHeaders")] // Apply the CORS policy
public class AuthController : Controller {
    private readonly SessionDataAccessor _sessionDataAccessor;

    private readonly SessionManager _sessionManager;

    // private readonly PasswordHasher<IdentityUser> _passwordHasher;
    private readonly SimplePasswordHasher _simplePasswordHasher;
    private readonly UserAccountRepository _userAccountRepository;

    public AuthController(SessionManager sessionManager, SessionDataAccessor sessionDataAccessor,
        SimplePasswordHasher passwordHasher, UserAccountRepository userAccountRepository)
    {
        _sessionManager = sessionManager;
        _sessionDataAccessor = sessionDataAccessor;
        _simplePasswordHasher = passwordHasher;
        _userAccountRepository = userAccountRepository;
    }


    [HttpPost("auth/login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
    {
        try
        {
            var client = await _userAccountRepository.GetUserAccountAsync(loginRequest.username);
            // IdentityUser user = new IdentityUser { UserName = loginRequest.username };
            if (client != null)
            {
                // if (_passwordHasher.VerifyHashedPassword(user, client.getEncodedPassword(),
                if (_simplePasswordHasher.VerifyPassword(client.getEncodedPassword(), loginRequest.password))
                {
                    var sessionData = new SessionData(client.getUsername());
                    var token = _sessionManager.AddSession(sessionData);
                    return Ok(new LoginResponse(token));
                }
                return Forbid();
            }

            _userAccountRepository.SetUserAccountAsync(new FirestoreUserAccount(loginRequest.username,
                _simplePasswordHasher.HashPassword(loginRequest.password)));
            // _passwordHasher.HashPassword(user, loginRequest.password)));
            var newSessionData = new SessionData(loginRequest.username);
            var newToken = _sessionManager.AddSession(newSessionData);
            return Ok(new LoginResponse(newToken));
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred." });
        }
    }
}