using Microsoft.EntityFrameworkCore;
using UnoCatorceCafe.Web.Datos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<CafeteriaDbContext>(opciones =>
    opciones.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=cafeteria.db"));

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Inicializar y sembrar base de datos
using (var alcance = app.Services.CreateScope())
{
    var servicios = alcance.ServiceProvider;
    try
    {
        var contexto = servicios.GetRequiredService<CafeteriaDbContext>();
        await DbInicializador.Inicializar(contexto);
    }
    catch (Exception ex)
    {
        var logger = servicios.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error sembrando base de datos inicial.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
