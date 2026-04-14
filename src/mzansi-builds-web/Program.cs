using mzansi_builds_web.Components;
using mzansi_builds_web.Services;
using Microsoft.AspNetCore.Components.Authorization;
using System;

var builder = WebApplication.CreateBuilder(args);

// 1. HttpClient Registration
builder.Services.AddScoped(sp =>
{
    var config = builder.Configuration["ApiBaseUrl"];
    var baseAddress = !string.IsNullOrWhiteSpace(config)
        ? new Uri(config)
        : new Uri("https://localhost:7018/");

    return new HttpClient { BaseAddress = baseAddress };
});

// 2. Authentication & Authorization Core
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();

// 3. Identity Services (The Handshake)
// We register AuthService first so CustomAuthStateProvider can inject it.
builder.Services.AddScoped<AuthService>();

// CRITICAL: We register CustomAuthStateProvider, then tell Blazor 
// that whenever it asks for 'AuthenticationStateProvider', it must use the Custom one.
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