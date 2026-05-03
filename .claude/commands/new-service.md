---
description: Crea el andamiaje de un nuevo servicio (interfaz + clase) en ExamenApi.Services siguiendo las convenciones del proyecto.
argument-hint: <NombreServicio>
allowed-tools: Read, Write, Edit, Bash(dotnet build:*)
---

# /new-service

Vas a crear un nuevo servicio en `ExamenApi.Services` para: **$ARGUMENTS**

## Reglas obligatorias (ver CLAUDE.md §4)

1. Crear **dos archivos**:
   - `ExamenApi.Services/I{Nombre}Service.cs` — interfaz pública con `I` al inicio.
   - `ExamenApi.Services/{Nombre}Service.cs` — implementación.
2. La clase recibe sus dependencias **solo por constructor** y las guarda en campos `_camelCase` privados readonly.
3. Todos los métodos públicos del servicio son `async Task<Result<T>>` y reciben `CancellationToken pCancellationToken` como último parámetro.
4. Todos los parámetros llevan prefijo `p` (camelCase). Sin excepciones.
5. Usar `Result<T>` desde `ExamenApi.Models` (si no existe, créalo en `Models/Common/Result.cs`).
6. **NO implementar lógica todavía**. Cada método del servicio retorna `Result<T>.Failure("Not implemented");` o equivalente.
7. Registrar el servicio en `ExamenApi.Api/Program.cs` en la sección de DI: `builder.Services.AddScoped<I{Nombre}Service, {Nombre}Service>();`
8. Idioma: **inglés**. Comentarios XML doc en cada método público.

## Pasos

1. Confirma con el usuario los métodos que debe exponer la interfaz (firma + propósito), antes de generar código.
2. Genera la interfaz con XML docs.
3. Genera la implementación con campos privados, constructor y stubs `NotImplemented`.
4. Registra en DI.
5. Corre `dotnet build` y reporta resultado.

Si algo no encaja con CLAUDE.md, **pregunta antes de continuar**.
