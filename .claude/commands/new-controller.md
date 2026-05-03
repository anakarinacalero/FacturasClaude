---
description: Crea un Controller vacío en ExamenApi.Api siguiendo las convenciones REST y de naming.
argument-hint: <NombreController> [recursoREST]
allowed-tools: Read, Write, Edit, Bash(dotnet build:*)
---

# /new-controller

Vas a crear un controller para: **$ARGUMENTS**

## Reglas obligatorias

1. Archivo: `ExamenApi.Api/Controllers/{Nombre}Controller.cs`.
2. Hereda de `ControllerBase`, decorado con `[ApiController]` y `[Route("api/[controller]")]`.
3. Inyectar siempre la **interfaz** del servicio correspondiente (no la clase concreta). Guardar en `_camelCase` privado readonly.
4. Endpoints `async Task<IActionResult>`, todos con `CancellationToken pCancellationToken` como último parámetro.
5. Parámetros con prefijo `p` (camelCase).
6. Recibir DTOs específicos `*Request` desde `ExamenApi.Models`. Devolver DTOs `*Result` o `*Response`. **Nunca** entidades de DB directamente.
7. **Sin lógica funcional**. Cada acción retorna `StatusCode(501, "Not implemented")` con un comentario `// TODO:` claro.
8. Documentación XML en cada acción describiendo el verbo, el recurso y los códigos de estado esperados.

## Pasos

1. Pregunta:
   - Nombre del recurso REST (singular o plural según convenio).
   - Endpoints esperados (GET, POST, PUT, DELETE, etc.) con sus rutas.
2. Genera el controller stub.
3. `dotnet build`.
