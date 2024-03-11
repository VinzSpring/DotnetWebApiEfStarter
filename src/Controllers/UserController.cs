using System.Security.Claims;
using backend.Entities;
using backend.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly SignInManager<User> _signInManager;

    public UserController(IUserService userService, SignInManager<User> signInManager)
    {
        _userService = userService;
        _signInManager = signInManager;
    }

    [HttpPost("login-google")]
    public async Task<IActionResult> LoginWithGoogle([FromBody] GoogleLoginDto dto)
    {
        var idToken = dto.TokenId;
        if(await _userService.HandleGoogleLogin(idToken))
        {
            return Ok();
        }

        return Unauthorized();
    }

    [HttpGet("self")]
    [Authorize]
    public async Task<User> getLoggedInUser()
    {
        var user = await _signInManager.UserManager.GetUserAsync(User);
        return user;
    }
}
