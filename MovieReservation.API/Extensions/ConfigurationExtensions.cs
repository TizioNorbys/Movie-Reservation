using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MovieReservation.Infrastracture.Authentication;

namespace MovieReservation.API.Extensions;

public static class ConfigurationExtensions
{
    public static AuthenticationBuilder AddJwtBearerConfiguration(this AuthenticationBuilder authBuilder, IConfiguration configuration)
    {
        authBuilder.AddJwtBearer(options =>
        {
            JwtOptions jwtOptions = new();
            configuration.GetSection("Jwt").Bind(jwtOptions);

            options.TokenValidationParameters = new()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidAudience = jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey
                    (Encoding.UTF8.GetBytes(jwtOptions.SecretKey))
            };
        });

        return authBuilder;
    }
}