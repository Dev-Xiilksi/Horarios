using CurrieTechnologies.Razor.SweetAlert2;
using SistemaHorario.Application;
using SistemaHorario.Application.Interfaces;
using SistemaHorario.Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Radzen;
using SistemaHorarioApp.Components;
using SistemaHorarioApp.Seguridad;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

//seguro por defecto: si la clave falta, se exige HTTPS. Solo appsettings.Development.json la desactiva
var requiereHttps = builder.Configuration.GetValue("Seguridad:RequiereHttps", true);
var politicaCookie = requiereHttps ? CookieSecurePolicy.Always : CookieSecurePolicy.SameAsRequest;

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddApplication(builder.Configuration["MediatR:LicenseKey"]);
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddAuthorization();

//login: el componente deja la sesion validada en memoria y /Account/Login la canjea por la cookie
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<LoginTicketStore>();

//usuario de la sesion para las reglas de acceso de Application
builder.Services.AddScoped<IUsuarioActual, UsuarioActualBlazor>();
//sweetalert
builder.Services.AddSweetAlert2();
builder.Services.AddHttpClient();

//dialog
builder.Services.AddScoped<DialogService>();
builder.Services.AddHttpContextAccessor();

//added
builder.Services.AddRazorPages();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/";
        options.AccessDeniedPath = "/unauthorized";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.SlidingExpiration = true;
        options.Cookie.Name = "SistemaHorario.Sesion";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.SecurePolicy = politicaCookie;
    });
builder.Services.AddAntiforgery(options => options.Cookie.SecurePolicy = politicaCookie);

//limite HTTP para /Account/Login y /Account/Logout: 10 peticiones por minuto por IP.
//El login con usuario y contrasena corre dentro de Blazor (no es una peticion HTTP): lo limita LimitadorIntentosLogin
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy(PoliticasLimite.Cuenta, http =>
        RateLimitPartition.GetFixedWindowLimiter(
            http.Connection.RemoteIpAddress?.ToString() ?? "desconocida",
            _ => new FixedWindowRateLimiterOptions { PermitLimit = 10, Window = TimeSpan.FromMinutes(1), QueueLimit = 0 }));
});
//notificaciton
builder.Services.AddScoped<NotificationService>();


//Theme
builder.Services.AddRadzenComponents();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

if (requiereHttps)
{
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

app.MapRazorPages();

//estado de la base de datos y del directorio activo para monitoreo: solo responde Healthy, Degraded o Unhealthy, sin detalles
app.MapHealthChecks("/salud");

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
