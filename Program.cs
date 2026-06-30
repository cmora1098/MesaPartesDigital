using MesaPartesDigital.Components;
using MesaPartesDigital.Data;
using MesaPartesDigital.Services;
using MesaPartesDigital.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

var builder = WebApplication.CreateBuilder(args);

// 1. Añadir componentes de Razor y modo interactivo
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// 2. Necesario para el funcionamiento de ProtectedSessionStorage
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ProtectedSessionStorage>();

// 3. Conexión a la Base de Datos
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// 4. Registro de modelos y servicios de sesión
builder.Services.AddScoped<UserSession>();
builder.Services.AddMemoryCache();

// 5. Registro de Servicios del Sistema
builder.Services.AddScoped<TipoPersonaService>();
builder.Services.AddScoped<TipoDocumentoService>();
builder.Services.AddScoped<TipoDocPerService>();
builder.Services.AddScoped<EstadoService>();
builder.Services.AddScoped<UbigeoService>();
builder.Services.AddScoped<DocumentoService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ILoginService, LoginService>();

// 6. Registro de Cliente HTTP (para llamadas a la intranet)
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://intranet.agrorural.gob.pe/")
});

var app = builder.Build();

// 7. Configuración del pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();