using MesaPartesDigital.Components;
using MesaPartesDigital.Data;
using MesaPartesDigital.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// 🔌 CONEXIÓN A LA BASE DE DATOS
var connectionString = builder.Configuration.GetConnectionString("CadenaConexion");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Registrar HttpClient apuntando a la API de la intranet
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://intranet.agrorural.gob.pe/")
});

// 🛠️ REGISTRO DE SERVICIOS (Uno de cada uno, limpio y ordenado)
builder.Services.AddScoped<TipoPersonaService>();
builder.Services.AddScoped<TipoDocumentoService>();
builder.Services.AddScoped<TipoDocPerService>(); // 👈 Deja solo este (ya incluye el namespace gracias al using)
builder.Services.AddScoped<EstadoService>();
builder.Services.AddScoped<UbigeoService>();
 builder.Services.AddScoped<MesaPartesDigital.Services.DocumentoService>();
builder.Services.AddScoped<IEmailService, EmailService>(); // Registrar el servicio de correos electrónico


builder.Services.AddMemoryCache(); // <- Activa el soporte de caché en memoria
var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

// En las últimas versiones, MapStaticAssets() optimiza el manejo de archivos estáticos (wwwroot)
app.MapStaticAssets();

app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();