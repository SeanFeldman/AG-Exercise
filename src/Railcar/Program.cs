using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Railcar.Components;
using Railcar.Data;
using Railcar.Data.Infrastructure;
using Railcar.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

var connectionString = builder.Configuration.GetConnectionString("RailcarDb")
                      ?? throw new InvalidOperationException("Connection string 'RailcarDb' not found.");

builder.Services.AddDbContext<RailcarDbContext>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();

// TODO: for higher environments would need to be more robust
if (Debugger.IsAttached)
{
    await DbInitializer.InitializeAsync(app.Services);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Railcar.Client._Imports).Assembly);

app.MapTripEndpoints();

app.Run();
