using FluentAssertions;
using Xunit;

namespace Structor.Auth.Helpers;
public class AuthenticationUtilsTests
{

    [Theory]
    [InlineData(8, true, true, true)]
    [InlineData(10, true, false, false)]
    [InlineData(12, false, true, false)]
    [InlineData(16, false, false, true)]
    [InlineData(20, false, false, false)]
    public void GenerateRandomString_ShouldReturnStringWithExpectedLengthAndCharacters(int length, bool hasCapitalLetters, bool hasNumbers, bool hasSympols)
    {
        var generatedString = AuthenticationUtils.GenerateRandomRefresh(length, hasCapitalLetters, hasNumbers, hasSympols);

        generatedString.Length.Should().Be(length);

        if (hasCapitalLetters)
        {
            generatedString.Should().MatchRegex(@"[A-Z]");
        }
        if (hasNumbers)
        {
            generatedString.Should().MatchRegex(@"[0-9]");
        }
        if (hasSympols)
        {
            generatedString.Should().MatchRegex(@"[!@#$%^&*?_-]");
        }
    }


    [Fact]
    public void HashString_Returns_NonNull_Result()
    {
        // Arrange
        string password1 = "myPassword123";
        string password2 = "anotherPassword456";

        // Act
        string result1 = password1.HashString();
        string result2 = password2.HashString();
        // Assert
        result1.Should().NotBeNull();
        result2.Should().NotBeNull();

        result1.Should().NotBe(result2);
    }


    [Fact]
    public void VerifyHash_Returns_True_When_StringMatches()
    {
        // Arrange
        string password1 = "myPassword123";
        // Act
        string result1 = password1.HashString();
        // Assert
        var result = password1.DoesMatchHash(result1);

        result.Should().BeTrue();
    }
    [Fact]
    public void VerifyHash_Returns_False_WhenStringDoesNotMatch()
    {
        // Arrange
        string password1 = "myPassword123";
        string password2 = "myPassword";
        // Act
        string result1 = password1.HashString();
        // Assert
        var result = password2.DoesMatchHash(result1);

        result.Should().BeFalse();
    }




}
