using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ModuPOS.Api.Data;
using ModuPOS.Api.Entities.Identity;
using ModuPOS.Api.Middleware;
using ModuPOS.Api.Services;
using ModuPOS.Api.Services.Auth;
using ModuPOS.Api.Settings;
using ModuPOS.Shared.Constants;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ModuPosDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection(CloudinarySettings.Section));

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.Section));

builder.Services.AddIdentity<UsuarioAplicacion, IdentityRole>(options =>
{
    //requisitos de contraseña
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;

    //bloqueo por intentos fallidos
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);

    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ModuPosDbContext>()
.AddDefaultTokenProviders();

//jwt autenticacion
var jwtSettings = builder.Configuration.GetSection(JwtSettings.Section).Get<JwtSettings>()!;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true, //expiración
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
        ClockSkew = TimeSpan.Zero //margen de tolerancia para diferencias de reloj entre servidores, default era 5 minutos
    };
});

//politicas autorización a roles
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Policies.SoloAdmin, policy =>
        policy.RequireRole(Roles.Administrador));

    options.AddPolicy(Policies.GestionarInventario, policy =>
        policy.RequireRole(Roles.Administrador));

    options.AddPolicy(Policies.RealizarVenta, policy =>
        policy.RequireRole(Roles.Administrador, Roles.Cajero));
});

// Add business logic services
builder.Services.AddHttpContextAccessor(); //necesario para AuditService
builder.Services.AddScoped<IAuditService, AuditServiceImpl>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IVentasService, VentasServiceImpl>();
builder.Services.AddScoped<IProductosService, ProductosServiceImpl>();
builder.Services.AddScoped<IMetodosPagoService, MetodosPagoServiceImpl>();
builder.Services.AddScoped<ICategoriasService, CategoriasServiceImpl>();
builder.Services.AddScoped<IImagenService, ImagenServiceImpl>();

builder.Services.AddControllers(); 
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new() { Title = "ModuPOS API", Version = "v1" });
});

// Cors
builder.Services.AddCors(options => 
{
    options.AddPolicy("BlazorPolicy", policy => {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

//seeder de administrador
using var scope = app.Services.CreateScope();
var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UsuarioAplicacion>>();
var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

foreach (var rol in new[] { Roles.Administrador, Roles.Cajero })
    if (!await roleManager.RoleExistsAsync(rol))
        await roleManager.CreateAsync(new IdentityRole(rol));

if (!userManager.Users.Any())
{
    var admin = new UsuarioAplicacion
    {
        UserName = "admin@modupos.com",
        Email = "admin@modupos.com",
        Nombres = "Administrador",
        EstaActivo = true
    };
    await userManager.CreateAsync(admin, "Admin1234!");
    await userManager.AddToRoleAsync(admin, Roles.Administrador);
}

app.UseGlobalExceptionHandler(); //envuelve todo lo que venga después en el pipeline para manejar excepciones globalmente
app.UseCors("BlazorPolicy");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
