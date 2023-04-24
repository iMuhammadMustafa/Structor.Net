using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
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
        var (accessToken, refreshToken) = _jwtService.GenerateJWTokens(claims);

        // Assert
        accessToken.Should().NotBeNullOrWhiteSpace();
        refreshToken.Should().NotBeNullOrWhiteSpace();
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
        var (accessToken, _) = _jwtService.GenerateJWTokens(claims);

        // Act
        var result = _jwtService.ValidateToken(accessToken, JWTEnum.Access);

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

        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(c => c["JWT:Issuer"]).Returns("exampleIssuer");
        mockConfig.Setup(c => c["JWT:Audience"]).Returns("exampleAudience");
        mockConfig.Setup(c => c["JWT:Keys:Access"]).Returns("accessKey+SupersecretKey");
        mockConfig.Setup(c => c["JWT:Expiry:Access"]).Returns("30");
        mockConfig.Setup(c => c["JWT:Keys:Refresh"]).Returns("refreshKey+SupersecretKey");
        mockConfig.Setup(c => c["JWT:Expiry:Refresh"]).Returns("365");


        //var authSettings = new AuthenticationSettings(mockConfig.Object);
        AuthenticationSettings.Initialize(mockConfig.Object);
        var jwtService = new JWTService();
        var (accessToken, refreshToken) = jwtService.GenerateJWTokens(claims, refreshClaims);

        // Act
        var accessResults = jwtService.ValidateToken(accessToken, JWTEnum.Access);
        var refreshResults = jwtService.ValidateToken(refreshToken, JWTEnum.Refresh);

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
