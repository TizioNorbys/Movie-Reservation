using System.Security.Claims;
using Microsoft.Extensions.Options;
using System.Text;
using MovieReservation.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using MovieReservation.Application.Interfaces.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace MovieReservation.Infrastracture.Authentication;

public class JwtProvider : IJwtProvider
{
    private readonly UserManager<User> _userManager;
    private readonly JwtOptions jwtOptions;
    private readonly ILogger<JwtProvider> _logger;

    public JwtProvider(UserManager<User> userManager, IOptions<JwtOptions> options, ILogger<JwtProvider> logger)
    {
        _userManager = userManager;
        jwtOptions = options.Value;
        _logger = logger;
    }

    public async Task<string> GenerateAsync(User user)
    {
        var claims = new List<Claim>()
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Name, user.FirstName!),
            new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName!)
        };

        _logger.LogInformation("Fetching roles of user with {Id} id", user.Id);
        var roleNames = await _userManager.GetRolesAsync(user);
        foreach (var roleName in roleNames)
        {
            claims.Add(new Claim(ClaimTypes.Role, roleName));
        }

        var signingCredentials = new SigningCredentials(new SymmetricSecurityKey
            (Encoding.UTF8.GetBytes(jwtOptions.SecretKey)), SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtOptions.Issuer,
            audience: jwtOptions.Audience,
            claims,
            null,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials);

        _logger.LogInformation("Generating jwt token for user with {Id} id", user.Id);
        return new JwtSecurityTokenHandler().WriteToken(token);
    } 
}