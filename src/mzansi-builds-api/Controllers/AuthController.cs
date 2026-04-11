using Microsoft.AspNetCore.Mvc;
using mzansi_builds_api.Services;
using mzansi_builds_api.DTOs.User;

namespace mzansi_builds_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _registrationService;

    public AuthController(AuthService registrationService)
    {
        _registrationService = registrationService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegistrationDto dto)
    {
        var result = await _registrationService.RegisterUserAsync(dto);

        if (!result)
        {
            return BadRequest("Registration failed. Please check your details or try a different email.");
        }

        return Ok(new { message = "Registration successful!" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
    {
        // I am calling the updated service which now returns a JWT token string.
        var token = await _registrationService.LoginAsync(dto);

        // If the token is null, it means the email or password was incorrect.
        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized("Invalid email or password.");
        }

        // I return the token in a JSON object so the frontend can store it easily.
        return Ok(new { Token = token, Message = "Login successful!" });
    }
}