using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ModuPOS.Api.Entities.Identity;
using ModuPOS.Shared.Constants;
using System.Security.Claims;

namespace ModuPOS.Api.Data
{
    public static class DbInitializer
    { 
        public static async Task SeedAsync(
            ModuPosDbContext context,
            UserManager<UsuarioAplicacion> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            await context.Database.MigrateAsync();

            //crear roles y claims
            //administrador
            if (!await roleManager.RoleExistsAsync(Roles.Administrador)) 
            { 
                var adminRole = new IdentityRole(Roles.Administrador);
                await roleManager.CreateAsync(adminRole);

                await roleManager.AddClaimAsync(adminRole, new Claim("Permission", Policies.SoloAdmin));
                await roleManager.AddClaimAsync(adminRole, new Claim("Permission", Policies.GestionarInventario));
                await roleManager.AddClaimAsync(adminRole, new Claim("Permission", Policies.RealizarVenta));
            }

            //cajero
            if (!await roleManager.RoleExistsAsync(Roles.Cajero))
            {
                var cajeroRole = new IdentityRole(Roles.Cajero);
                await roleManager.CreateAsync(cajeroRole);
                await roleManager.AddClaimAsync(cajeroRole, new Claim("Permission", Policies.RealizarVenta));
            }

            //crear usuario administrador
            var adminEmail = "admin@modupos.com";
            var adminUsuario = await userManager.FindByEmailAsync(adminEmail);

            if (adminUsuario == null) 
            {
                var adminUser = new UsuarioAplicacion
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    Nombres = "Administrador",
                    ApellidoPaterno = "Sistema",
                    ApellidoMaterno = "",
                    EmailConfirmed = true,
                    EstaActivo = true
                };

                var resultado = await userManager.CreateAsync(adminUser, "Admin123!");
                if (resultado.Succeeded) await userManager.AddToRoleAsync(adminUser, Roles.Administrador);
            }
        }
    }
}
