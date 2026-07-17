# Uno Catorce Café

MVP web para presentar el menú de una cafetería y convertir la selección del cliente en un pedido por WhatsApp.

El proyecto combina una interfaz pública simple con una API en ASP.NET Core, persistencia local mediante SQLite y pruebas automatizadas. Está pensado para validar rápidamente una experiencia de catálogo digital sin incorporar todavía pagos ni un panel administrativo completo.

## Funcionalidades

- Menú organizado por categorías.
- Productos con nombre, descripción, precio e imagen.
- Filtrado de categorías y productos activos.
- Interfaz adaptable para consultar el menú desde el celular.
- Armado del pedido en el navegador.
- Envío del resumen del pedido mediante WhatsApp.
- API para obtener el catálogo.
- Datos iniciales para probar el sistema sin configuración manual.

## Tecnologías

- .NET 9 y ASP.NET Core Web API
- Entity Framework Core
- SQLite
- HTML, CSS y JavaScript
- xUnit
- Pruebas de integración con `Microsoft.AspNetCore.Mvc.Testing`

## Arquitectura

```text
UnoCatorceCafeWeb/
├── src/
│   └── UnoCatorceCafe.Web/
│       ├── Controllers/   # Endpoints HTTP
│       ├── Datos/         # DbContext e inicialización
│       ├── Dominio/       # Categorías y productos
│       ├── Modelos/       # DTO de salida
│       └── wwwroot/       # Interfaz pública
└── tests/
    └── UnoCatorceCafe.Tests/
```

## Ejecución local

### Requisitos

- SDK de .NET 9

### Restaurar, probar y ejecutar

```bash
dotnet restore UnoCatorceCafe.sln
dotnet test UnoCatorceCafe.sln
dotnet run --project src/UnoCatorceCafe.Web/UnoCatorceCafe.Web.csproj
```

La aplicación crea automáticamente una base SQLite local llamada `cafeteria.db` y carga datos iniciales. La URL se muestra en la terminal al iniciar.

## API

```http
GET /api/menu
```

Devuelve las categorías activas y sus productos disponibles, ordenados para la visualización del menú.

## Alcance del MVP

El objetivo actual es validar el catálogo digital y el flujo de pedidos por WhatsApp. No incluye pagos en línea, autenticación ni administración remota del menú, y no debe presentarse como un sistema listo para producción.

## Autoría

Proyecto personal de **Nacho Bordagorry**, desarrollado como MVP para demostrar integración entre frontend, API, persistencia y pruebas automatizadas con .NET.

## Próximos pasos

- Panel de administración para categorías y productos.
- Autenticación y autorización.
- Despliegue público con configuración persistente.
- Capturas o demo en video del flujo de compra.

