using DOCA.API.Constants;
using DOCA.API.Payload.Request.Account;
using DOCA.API.Payload.Response.Account;
using DOCA.API.Services.Implement;
using DOCA.API.Services.Interface;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using LoginRequest = DOCA.API.Payload.Request.Account.LoginRequest;

namespace DOCA.API.Controllers;

[ApiController]
[Route(ApiEndPointConstant.ApiEndpoint)]
public class AuthController : BaseController<AuthController>
{
    private readonly IUserService _userService;
    public AuthController(ILogger<AuthController> logger, IUserService userService) : base(logger)
    {
        _userService = userService;
    }

    [HttpPost(ApiEndPointConstant.Auth.Login)]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
    {
        var loginResponse = await _userService.LoginAsync(loginRequest);
        if (loginResponse == null)
        {
            _logger.LogError($"Login failed with {loginRequest.UsernameOrPhoneNumber}");
            return Problem(MessageConstant.User.LoginFail);
        }
        _logger.LogInformation($"Login successful with {loginRequest.UsernameOrPhoneNumber}");
        return Ok(loginResponse);
    }
    [HttpPost(ApiEndPointConstant.Auth.Signup)]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register([FromBody] SignUpRequest request)
    {
        var response = await _userService.RegisterAsync(request);
        if (response == null)
        {
            _logger.LogError($"Register failed with {request.Username}");
            return Problem(MessageConstant.User.RegisterFail);
        }
        _logger.LogInformation($"Register successful with {request.Username}");
        return CreatedAtAction(nameof(Register), response);
    }
    
    
    
    
}