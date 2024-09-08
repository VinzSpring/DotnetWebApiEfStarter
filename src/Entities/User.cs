
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace backend.Entities;
public class User: IdentityUser {
}
public static class Roles {
    public static string Admin = "Admin";
    public static string User = "User";

    public static async Task EnsureRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        // Check if the roles exist; if not, create them
        if (!await roleManager.RoleExistsAsync(Roles.Admin))
        {
            await roleManager.CreateAsync(new IdentityRole(Roles.Admin));
        }
        if (!await roleManager.RoleExistsAsync(Roles.User))
        {
            await roleManager.CreateAsync(new IdentityRole(Roles.User));
        }
    }
}
