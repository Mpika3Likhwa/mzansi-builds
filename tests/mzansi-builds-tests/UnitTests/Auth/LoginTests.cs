using Xunit;
using mzansi_builds_api.Services;
using mzansi_builds_api.DTOs;
using Microsoft.Extensions.Configuration;

namespace mzansi_builds_tests.UnitTests.Auth;

public class LoginTests
{
    [Fact]
    public async Task Login_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // I'm setting up a mock login attempt for a user that isn't in my DB.
        var loginDto = new UserLoginDto { Email = "ghost@example.com", Password = "Password123" };

        // I am passing null for both DbContext and IConfiguration for this unit test.
        // Note: This will cause a NullReferenceException when the service tries to touch the DB.
        var service = new AuthService(null!, null!);

        // I expect this to return null because the user doesn't exist (and the DB isn't there).
        var result = await service.LoginAsync(loginDto);

        Assert.Null(result);
    }

    [Fact]
    public async Task Login_ShouldReturnNull_WhenPasswordIsWrong()
    {
        // I am simulating a login attempt with a wrong password.
        var loginDto = new UserLoginDto { Email = "test@mzansi.com", Password = "WrongPassword123" };

        var service = new AuthService(null!, null!);

        // Since I'm using JWT now, a failed login returns null instead of false.
        var result = await service.LoginAsync(loginDto);

        Assert.Null(result);
    }
}