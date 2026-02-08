using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

// TODO:
// 1. Security (authentication and authorization)
// 2. Use a UI library like MudBlazor or FluentUI
// 3. UI testing with BUnit

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped(_ => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});

await builder.Build().RunAsync();
