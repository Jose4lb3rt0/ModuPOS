using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ModuPOS.Client;
using ModuPOS.Client.Auth;
using ModuPOS.Client.Services.Categoria;
using ModuPOS.Client.Services.Producto;
using System.Net.Http;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// --- COMPONENTES RAÍZ ---
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// --- CONFIGURACIÓN DE RUTAS Y API ---
var apiUrl = builder.Configuration["ApiUrl"] 
    ?? throw new InvalidOperationException("Falta 'ApiUrl' en appsettings.json");
var baseAddress = apiUrl.EndsWith("/") ? apiUrl : apiUrl + "/";

// --- SERVICIOS DE NEGOCIO (CLIENTES API) ---
builder.Services.AddScoped<IProductoClientService, ProductoClientService>();
builder.Services.AddScoped<ICategoriaClientService, CategoriaClientService>();

// --- INFRAESTRUCTURA DE SEGURIDAD (AUTH) ---
builder.Services.AddAuthorizationCore();

// TokenService y AuthDelegatingHandler deben ser Scoped debido a IJSRuntime
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<AuthDelegatingHandler>();

// Configuración del Estado de Autenticación (Patrón estándar Blazor)
builder.Services.AddScoped<JwtAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<JwtAuthStateProvider>());

// --- CONFIGURACIÓN DE HTTP CLIENT CON SEGURIDAD ---

// 1. Configuramos el cliente con nombre "ApiClient" incluyendo el manejador de tokens
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri(baseAddress);
})
.AddHttpMessageHandler<AuthDelegatingHandler>();

// 2. Registramos la instancia de HttpClient por defecto. 
// Esto asegura que cualquier servicio que inyecte 'HttpClient' reciba la versión 
// configurada con la URL del API y el envío automático del JWT.
builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("ApiClient"));

await builder.Build().RunAsync();