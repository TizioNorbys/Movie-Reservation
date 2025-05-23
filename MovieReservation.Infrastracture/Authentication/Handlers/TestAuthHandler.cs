using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MovieReservation.Domain.Constants;

namespace MovieReservation.Infrastracture.Authentication.Handlers;

public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        List<Claim> claims = new()
        {
            new(ClaimTypes.NameIdentifier, "08dd9654-94ab-4817-8c3d-5e2b300b6929"),
            new(ClaimTypes.Email, "test@moviereservation.com"),
            new(ClaimTypes.Name, "test"),
            new(ClaimTypes.Surname, "test"),
            new(ClaimTypes.Role, RoleNames.User),
            new(ClaimTypes.Role, RoleNames.Admin)
        };

        ClaimsIdentity identity = new(claims, Scheme.Name);
        ClaimsPrincipal principal = new(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}