using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

public static class SeedData
{
    public static async Task Initialize( RoleManager<IdentityRole> roleManager)
    {
        // Define roles
        string[] roleNames = { "Manager", "Employee", "Customer" };


        // Create roles if they don't already exist
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }


        // Seed a Manager user
        
    }
}
