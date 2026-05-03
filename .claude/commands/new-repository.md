---
description: Crea el andamiaje de un nuevo repositorio Dapper (interfaz + clase) en ExamenApi.Data.
argument-hint: <NombreRepositorio> <ModuloAcronimo>
allowed-tools: Read, Write, Edit, Bash(dotnet build:*)
---

# /new-repository

Vas a crear un repositorio Dapper para: **$ARGUMENTS**

## Reglas obligatorias (ver CLAUDE.md §4, §5, §6.2)

1. Crear:
   - `ExamenApi.Data/I{Nombre}Repository.cs`
   - `ExamenApi.Data/{Nombre}Repository.cs`
2. La clase recibe el connection string vía `IOptions<DatabaseSettings>` o `IConfiguration` (preguntar si ya existe el patrón).
3. Métodos `async`, con `CancellationToken pCancellationToken` propagado a Dapper vía `CommandDefinition`.
4. Parámetros con prefijo `p`.
5. Campos privados con `_`.
6. SQL en **constantes `private const string`** al tope del archivo. **Nunca** SQL inline en medio del método. **Nunca** `SELECT *`.
7. Nombres de objetos SQL siguen `{prefijo}{MODULO}{nombreEnCamelCase}{E|D}` — ver CLAUDE.md §5.2. Confirmar el acrónimo del módulo (3-5 letras MAYÚSCULAS) antes de generar SQL.
8. Las entidades retornadas viven en `ExamenApi.Models` y mapean columnas `snake_case` a propiedades C# usando el nombre exacto de la columna (excepción a la regla de inglés, ver CLAUDE.md §4.3).
9. **Sin lógica funcional**: stubs que devuelven `default`, `null` o lanzan `NotImplementedException` con TODO claro.
10. Registrar en DI en `Program.cs` como `Scoped`.

## Pasos

1. Pregunta al usuario:
   - Acrónimo del módulo (ej. `EXAM`, `QST`, `SCR`).
   - Operaciones esperadas (Get, GetById, Insert, Update, Delete, etc.).
   - Si la tabla es encabezado/detalle (sufijo `E` o `D`).
2. Genera el código siguiendo todas las reglas anteriores.
3. Registra en DI.
4. Corre `dotnet build`.
