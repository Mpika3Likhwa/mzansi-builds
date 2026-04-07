using mzansi_builds_api.Services;
using Xunit;

namespace mzansi_builds_tests.UnitTests.Auth;

public class RegistrationTests
{
    [Fact]
    public void IsPasswordSecure_ShouldReturnFalse_WhenPasswordIsShort()
    {
        // 1. Arrange 
        // I am passing null for DbContext and IConfiguration because 
        // IsPasswordSecure doesn't use them. This is a common TDD shortcut.
        var service = new AuthService(null!, null!);
        var shortPassword = "123";

        // 2. Act 
        bool isValid = service.IsPasswordSecure(shortPassword);

        // 3. Assert 
        Assert.False(isValid);
    }

    [Fact]
    public void IsPasswordSecure_ShouldReturnTrue_WhenPasswordIsLongEnough()
    {
        // Arrange
        var service = new AuthService(null!, null!);
        var strongPassword = "SuperSecurePassword123!";

        // Act
        var result = service.IsPasswordSecure(strongPassword);

        // Assert
        Assert.True(result);
    }
}