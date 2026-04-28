using CloudinaryDotNet.Actions;
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

            await AsegurarRolConClaims(roleManager, Roles.Administrador, new[]
            {
                Policies.SoloAdmin,
                Policies.GestionarInventario,
                Policies.RealizarVenta
            });

            await AsegurarRolConClaims(roleManager, Roles.Cajero, new[]
            {
                Policies.RealizarVenta
            });

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

        private static async Task AsegurarRolConClaims(RoleManager<IdentityRole> roleManager, string nombreRol, string[] permisos)
        {
            var rol = await roleManager.FindByNameAsync(nombreRol);

            if (rol == null) 
            {
                rol = new IdentityRole(nombreRol);
                await roleManager.CreateAsync(rol);
            }

            var claimsExistentes = await roleManager.GetClaimsAsync(rol);

            foreach (var permiso in permisos) 
            {
                if (!claimsExistentes.Any(c => c.Type == "Permission" && c.Value == permiso))
                    await roleManager.AddClaimAsync(rol, new Claim("Permission", permiso));
            }
        }
    }
}
