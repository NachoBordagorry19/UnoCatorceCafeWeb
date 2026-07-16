using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using UnoCatorceCafe.Web.Datos;

namespace UnoCatorceCafe.Tests.Controladores
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        private SqliteConnection? _conexion;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            _conexion = new SqliteConnection("Filename=:memory:");
            _conexion.Open();

            builder.ConfigureServices(services =>
            {
                // Buscar y remover la configuración de DbContext por defecto (la de producción)
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<CafeteriaDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Registrar CafeteriaDbContext utilizando la conexión compartida de SQLite en memoria
                services.AddDbContext<CafeteriaDbContext>(options =>
                {
                    options.UseSqlite(_conexion);
                });
            });
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                _conexion?.Dispose();
            }
        }
    }
}
