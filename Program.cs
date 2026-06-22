using MesaPartesDigital.Components;
// 👇 Agregamos los namespaces de tus nuevas carpetas
using MesaPartesDigital.Data;
using MesaPartesDigital.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// 🔌 CONEXIÓN A LA BASE DE DATOS (Usa la cadena de appsettings.json)
var connectionString = builder.Configuration.GetConnectionString("CadenaConexion");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// 🛠️ REGISTRO DE TU SERVICIO (Para poder usar el GET en los archivos .razor)
builder.Services.AddScoped<TipoPersonaService>();
builder.Services.AddScoped<TipoDocumentoService>();
builder.Services.AddScoped<TipoDocPerService>();
builder.Services.AddScoped<EstadoService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();