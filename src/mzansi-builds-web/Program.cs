using mzansi_builds_web.Components;
using mzansi_builds_web.Services;
using Microsoft.AspNetCore.Components.Authorization;
using System;

var builder = WebApplication.CreateBuilder(args);

// 1. HttpClient Registration
// For Blazor Server, it is standard to use AddHttpClient to manage the lifecycle better
builder.Services.AddHttpClient("MzansiApi", client =>
{
    var config = builder.Configuration["ApiBaseUrl"];

    // Fallback logic: Use Azure Env Var if present, otherwise localhost
    var baseAddress = !string.IsNullOrWhiteSpace(config)
        ? new Uri(config)
        : new Uri("https://mzansi-builds-api-e2g9fhhmbjf8fgaq.southafricanorth-01.azurewebsites.net/");

    client.BaseAddress = baseAddress;
});

// We still provide a scoped HttpClient for easy injection in your Services
builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("MzansiApi"));

// 2. Authentication & Authorization Core
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();

// 3. Identity Services (The Handshake)
builder.Services.AddScoped<AuthService>();

// Register CustomAuthStateProvider and link it to the base provider
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<CustomAuthStateProvider>());

// 4. Application Services
builder.Services.AddScoped<ProjectService>();
builder.Services.AddScoped<CollaborationService>();

// 5. Razor Components Configuration
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();