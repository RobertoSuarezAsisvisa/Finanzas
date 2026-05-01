# FinanzasMCP

`FinanzasMCP` es una solución en .NET 8 para gestionar finanzas personales con una arquitectura por capas y un servidor MCP para exponer herramientas relacionadas con presupuestos, metas de compra y transacciones.

## Estructura

- `src/FinanzasMCP.Domain`: entidades y reglas del dominio
- `src/FinanzasMCP.Application`: comandos, handlers y DTOs de la aplicación
- `src/FinanzasMCP.Infrastructure`: persistencia con Entity Framework Core y PostgreSQL
- `src/FinanzasMCP.McpServer`: host principal del servidor MCP
- `tests/FinanzasMCP.Domain.Tests`: pruebas del dominio
- `tests/FinanzasMCP.Application.Tests`: pruebas de aplicación
- `tests/FinanzasMCP.Integration.Tests`: pruebas de integración

## Requisitos

- .NET SDK 8.0
- PostgreSQL
- Acceso a las variables de configuración necesarias para la conexión a base de datos

## Ejecutar

Restaurar dependencias:

```bash
dotnet restore
```

Compilar la solución:

```bash
dotnet build FinanzasMCP.sln
```

Ejecutar el servidor MCP:

```bash
dotnet run --project src/FinanzasMCP.McpServer
```

Ejecutar pruebas:

```bash
dotnet test FinanzasMCP.sln
```

## Persistencia

El proyecto usa Entity Framework Core con migraciones para mantener el esquema de base de datos. Las configuraciones de persistencia están en `src/FinanzasMCP.Infrastructure`.

## Notas

- El proyecto principal expone herramientas MCP para operar sobre presupuestos, metas de compra y transacciones.
- Si vas a trabajar con la base de datos, revisa la configuración de `appsettings.json` en `src/FinanzasMCP.McpServer`.
