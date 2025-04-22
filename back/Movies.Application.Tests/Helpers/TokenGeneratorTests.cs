using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Movies.Application.Helpers;
using Movies.Domain.Entities;
using Movies.Domain.Enums;

namespace Movies.Application.Tests.Helpers;

public class TokenGeneratorTests
{
    private readonly ITokenGenerator _tokenGenerator;

    public TokenGeneratorTests()
    {
        var inMemorySettings = new Dictionary<string, string>()
        {
            { "JwtSettings:Secret", "rAB3WU0iPcNMLBVVpkH4FHEGI2E1AkSS" },
            { "JwtSettings:Issuer", "TestIssuer" },
            { "JwtSettings:Audience", "TestAudience" },
            { "JwtSettings:ExpiryInMinutes", "60" }
        };


        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        _tokenGenerator = new TokenGenerator(configuration);
    }

    [Fact]
    public void GenerateJwtToken_ReturnsValidTokenWithExpectedClaims()
    {
        // Arrange: Create a dummy user
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            Role = UserRoles.User // assuming UserRoles.User resolves to something like "User"
        };

        // Act: Generate the JWT token
        var tokenString = _tokenGenerator.GenerateJwtToken(user);

        // Parse the token using JwtSecurityTokenHandler
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(tokenString);

        // Assert: Check issuer, audience, claims, and expiry

        // Verify issuer
        Assert.Equal("TestIssuer", jwtToken.Issuer);
        // Verify audience (note: JwtSecurityToken.Audiences is an IEnumerable<string>)
        Assert.Contains("TestAudience", jwtToken.Audiences);

        // Verify claims contain user id, username, and role
        var claims = jwtToken.Claims.ToList();
        Assert.Contains(claims, c => c.Type == ClaimTypes.NameIdentifier && c.Value == user.Id.ToString());
        Assert.Contains(claims, c => c.Type == ClaimTypes.Name && c.Value == user.Username);
        Assert.Contains(claims, c => c.Type == ClaimTypes.Role && c.Value == user.Role.ToString());

        // Verify that the token hasn't expired
        Assert.True(jwtToken.ValidTo > DateTime.UtcNow, "Token expiry should be in the future");
    }

    // test: verifying that an expired token is handled
    // test: exceptions are thrown if configuration is missing.
}