using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using StructorAuth.Config;
using Xunit;

namespace StructorAuth.Services;

public class JWTServiceTests : IClassFixture<DependencySetupFixture>
{
    private readonly IJWTService _jwtService;

    public JWTServiceTests(DependencySetupFixture fixture)
    {
        _jwtService = fixture.ScopeServices.GetService<IJWTService>() ?? throw new NullReferenceException("IJWTService is not defined");
    }


    [Fact]
    public void GenerateJWTokens_Returns_Tokens()
    {
        // Arrange
        var claims = new Dictionary<string, string>()
        {
            { "sub", "1234" },
            { "name", "John Doe" },
            { "role", "admin" }
        };

        // Act
        var jwt = _jwtService.GenerateJWTokens(claims);

        // Assert
        jwt.AccessToken.Should().NotBeNullOrWhiteSpace();
        jwt.RefreshToken.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void ValidateToken_ValidAccessToken_ReturnsClaims()
    {
        // Arrange
        var claims = new Dictionary<string, string>
        {
            { "sub", "12345" },
            { "email", "test@example.com" }
        };
        var jwt = _jwtService.GenerateJWTokens(claims);

        // Act
        var result = _jwtService.ValidateToken(jwt.AccessToken, JWTEnum.Access);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().NotBeEmpty();
        result.Should().HaveCount(claims.Count + 3);
        result.First().Type.Should().Be(claims.First().Key);
        result.First().Value.Should().Be(claims.First().Value);
    }

    [Fact]
    public void ValidateToken_InvalidAccessToken_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var invalidToken = "invalid.token.here";

        // Act and Assert
        Action act = () => _jwtService.ValidateToken(invalidToken);

        act.Should().Throw<UnauthorizedAccessException>();
    }

    [Fact]
    public void ValidateToken_WithValidAccessToken_ReturnsClaims()
    {
        // Arrange
        var claims = new Dictionary<string, string>
        {
            { "userId", "123" },
            { "role", "Admin" }
        };

        var refreshClaims = new Dictionary<string, string>
        {
            { "userId", "123456" },
            { "role", "User" }
        };

        var mockConfig = Substitute.For<IConfiguration>();
        
        mockConfig["JWT:Issuer"].Returns("exampleIssuer");
        mockConfig["JWT:Audience"].Returns("exampleAudience");
        mockConfig["JWT:Keys:Access"].Returns("accessKey+SupersecretKey");
        mockConfig["JWT:Expiry:Access"].Returns("30");
        mockConfig["JWT:Keys:Refresh"].Returns("refreshKey+SupersecretKey");
        mockConfig["JWT:Expiry:Refresh"].Returns("365");


        //var authSettings = new JWTSettings(mockConfig.Object);
        JWTSettings.Initialize(mockConfig);
        var jwtService = new JWTService();
        var jwt = jwtService.GenerateJWTokens(claims, refreshClaims);

        // Act
        var accessResults = jwtService.ValidateToken(jwt.AccessToken, JWTEnum.Access);
        var refreshResults = jwtService.ValidateToken(jwt.RefreshToken, JWTEnum.Refresh);

        // Assert
        accessResults.Should().NotBeNull();
        accessResults.Should().HaveCount(claims.Count + 3);
        accessResults.FirstOrDefault(c => c.Type == "userId")?.Value.Should().Be("123");
        accessResults.FirstOrDefault(c => c.Type == "role")?.Value.Should().Be("Admin");

        refreshResults.Should().NotBeNull();
        refreshResults.Should().HaveCount(claims.Count + 3);
        refreshResults.FirstOrDefault(c => c.Type == "userId")?.Value.Should().Be("123456");
        refreshResults.FirstOrDefault(c => c.Type == "role")?.Value.Should().Be("User");
    }


}
