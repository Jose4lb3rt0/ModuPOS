using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ModuPOS.Client;
using ModuPOS.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<IProductoClientService, ProductoClientService>();

var apiUrl = builder.Configuration["ApiUrl"] ?? throw new InvalidOperationException("Falta 'ApiUrl' en appsettings.json");

builder.Services.AddScoped(sp =>
    //antiguo método
    //new HttpClient { 
    //este mismo frontend, por eso se debe cambiar al api
    //BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) }
{
    var baseAddress = apiUrl.EndsWith("/") ? apiUrl : apiUrl + "/";
    return new HttpClient { BaseAddress = new Uri(baseAddress) };
});

await builder.Build().RunAsync();
