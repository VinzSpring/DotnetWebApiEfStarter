using backend.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;

namespace backend.Service;

public interface IUserService
{
    Task<bool> HandleGoogleLogin(String idToken);
    Task<User?> GetLoggedInUserAsync(ClaimsPrincipal user);
    Task SignOutAsync();
    Task<User?> GetByEmail(string email);
}


public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly AppDbContext _dbContext;
    private readonly ILogger<UserService> _logger;

    public UserService(UserManager<User> userManager, SignInManager<User> signInManager, AppDbContext dbContext, ILogger<UserService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _dbContext = dbContext;
        _logger = logger;
    }

    private string GeneratePassword()
    {
        var securePassword = new byte[128];
        using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
        {
            rng.GetBytes(securePassword);
            return Convert.ToBase64String(securePassword);
        }
    }

    public async Task SignOutAsync()
    {
        await _signInManager.SignOutAsync();
    }

    public async Task<bool> HandleGoogleLogin(String idToken)
    {
        GoogleJsonWebSignature.Payload payload;
        try
        {
            payload = await GoogleJsonWebSignature.ValidateAsync(idToken);
        }
        catch
        {
            _logger.LogError("Invalid Google token");
            return false;
        }

        var existingUser = await _userManager.FindByEmailAsync(payload.Email);
        if (existingUser == null)
        {
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Email = payload.Email,
                UserName = payload.Email,
                EmailConfirmed = true,
            };
            var result = await _userManager.CreateAsync(user, GeneratePassword());

            await _userManager.AddToRoleAsync(user, Roles.User);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    _logger.LogError(error.Description);
                }
                return false;
            }
            existingUser = user;
        }

        await _signInManager.SignInAsync(existingUser, isPersistent: true);
        return true;
    }

    public async Task<User?> GetLoggedInUserAsync(ClaimsPrincipal user)
    {
        return await _signInManager.UserManager.GetUserAsync(user);
    }

    public async Task<User?> GetByEmail(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }
}
