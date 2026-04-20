using Microsoft.AspNetCore.Identity;

namespace PortalAcademico.Data
{
    public static class SeedData
    {
        public static async Task InicializarAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

            foreach (var rol in new[] { "Coordinador", "Estudiante" })
                if (!await roleManager.RoleExistsAsync(rol))
                    await roleManager.CreateAsync(new IdentityRole(rol));

            const string email = "coordinador@usmp.pe";
            const string pass  = "Coord@1234";

            if (await userManager.FindByEmailAsync(email) == null)
            {
                var user = new IdentityUser
                {
                    UserName       = email,
                    Email          = email,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(user, pass);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(user, "Coordinador");
            }
        }
    }
}