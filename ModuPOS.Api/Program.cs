using Microsoft.EntityFrameworkCore;
using ModuPOS.Api.Data;
using ModuPOS.Api.Middleware;
using ModuPOS.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ModuPosDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add business logic services
builder.Services.AddScoped<IVentasService, VentasServiceImpl>();
builder.Services.AddScoped<IProductosService, ProductosServiceImpl>();
builder.Services.AddScoped<IMetodosPagoService, MetodosPagoServiceImpl>();

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

app.UseCors("BlazorPolicy");

app.UseGlobalExceptionHandler(); //envuelve todo lo que venga después en el pipeline para manejar excepciones globalmente

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
