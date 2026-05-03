---
description: Crea la estructura completa de la solución ExamenApi (proyectos vacíos, referencias, DI mínima). Solo andamiaje, sin lógica.
allowed-tools: Bash(dotnet new:*), Bash(dotnet sln:*), Bash(dotnet add:*), Bash(dotnet restore:*), Bash(dotnet build:*), Bash(mkdir:*), Read, Write, Edit
---

# /bootstrap-solution

Crea la solución `ExamenApi` desde cero con todos los proyectos según CLAUDE.md §3.

## Resultado esperado

```
ExamenApi/
├── ExamenApi.sln
├── CLAUDE.md
├── .editorconfig
├── .gitignore
├── Directory.Build.props
├── ExamenApi.Api/
│   ├── ExamenApi.Api.csproj
│   ├── Program.cs
│   ├── Controllers/
│   ├── appsettings.json
│   └── appsettings.Development.json   (con placeholders, NO secretos)
├── ExamenApi.Models/
│   ├── ExamenApi.Models.csproj
│   ├── Common/
│   ├── Dtos/
│   ├── Requests/
│   └── Results/
├── ExamenApi.Services/
│   ├── ExamenApi.Services.csproj
│   └── (vacío, listo para servicios)
├── ExamenApi.Data/
│   ├── ExamenApi.Data.csproj
│   └── (vacío, listo para repos)
└── ExamenApi.Utilities/
    ├── ExamenApi.Utilities.csproj
    └── (vacío)
```

## Pasos

1. Confirmar con el usuario el directorio destino.
2. `dotnet new sln -n ExamenApi`
3. `dotnet new webapi -n ExamenApi.Api -f net9.0 --use-controllers`
4. `dotnet new classlib -n ExamenApi.Models -f net9.0`
5. `dotnet new classlib -n ExamenApi.Services -f net9.0`
6. `dotnet new classlib -n ExamenApi.Data -f net9.0`
7. `dotnet new classlib -n ExamenApi.Utilities -f net9.0`
8. `dotnet sln add` para los 5 proyectos.
9. Referencias (CLAUDE.md §3):
   - `Api` → `Services`, `Models`, `Utilities`
   - `Services` → `Data`, `Models`, `Utilities`
   - `Data` → `Models`, `Utilities`
   - `Models` → ninguna
   - `Utilities` → ninguna
10. Eliminar `Class1.cs` que generan los `classlib`.
11. Crear `.editorconfig` y `Directory.Build.props` con `Nullable=enable`, `TreatWarningsAsErrors=true`, `LangVersion=latest`.
12. En `Api`, agregar paquetes NuGet base: `Dapper`, `Microsoft.Data.SqlClient`. Confirmar versiones con el usuario antes de instalar.
13. Stub de `Program.cs` con: controllers, OpenAPI/Swagger, DI vacía con comentario `// TODO: register services here`.
14. Crear `Result.cs` en `ExamenApi.Models/Common/`.
15. Compilar: `dotnet build`. Reportar.

## Reglas

- **No** añadas EF Core, MediatR, AutoMapper, FluentValidation, Serilog, etc., a menos que el usuario lo pida.
- **No** crees lógica de negocio, controllers funcionales ni endpoints reales.
- Mantén `appsettings.json` con placeholders vacíos para `ConnectionStrings:Default` y `Anthropic:ApiKey`.
- Confirma cada paso destructivo (sobreescribir archivos existentes, etc.).
