# Clean Code — ExamenApi Skill

> Skill tomada de [aitmpl.com/component/skill/development/clean-code](https://www.aitmpl.com/component/skill/development/clean-code) y adaptada a las convenciones de OneGoalDiscover / A·B Systems. En conflicto con `CLAUDE.md`, **gana `CLAUDE.md`**.

---

## Principios base

| Principio | Regla |
|---|---|
| **SRP** | Cada clase/método hace UNA cosa |
| **DRY** | Extraer duplicados; reutilizar desde `Utilities` y `Models` |
| **KISS** | La solución más simple que funcione |
| **YAGNI** | No construir lo que no se ha pedido todavía |
| **Boy Scout** | Dejar el código más limpio de lo que se encontró |

---

## Naming — reglas locales (sobre las de CLAUDE.md)

| Elemento | Convención | Ejemplo |
|---|---|---|
| Parámetros | `camelCase` con **prefijo `p`** | `pUserId`, `pQuestionId` |
| Variables locales | `camelCase` sin prefijo | `trimmedAnswer`, `userScore` |
| Campos privados | `camelCase` con **prefijo `_`** | `_logger`, `_repository` |
| Booleanos | Forma de pregunta | `isActive`, `hasScore`, `canEvaluate` |
| Constantes privadas | SCREAMING_SNAKE o PascalCase (C# convención) | `private const string SqlGetQuestions` |

> **Regla:** Si necesitas un comentario para explicar el nombre, renómbralo.

---

## Reglas de funciones/métodos

| Regla | Detalle |
|---|---|
| **Pequeños** | Máx 30 líneas (CLAUDE.md §6.1). Idealmente 5-15. |
| **Una cosa** | Un método, una responsabilidad |
| **Pocos argumentos** | Máx 3-4. Si hay más, agrupar en un objeto Request |
| **Sin efectos secundarios** | No mutar inputs inesperadamente |
| **Early return** | Guard clauses para casos borde |

```csharp
// ✅ Correcto — guard clause, un nivel de anidación
public async Task<Result<ScoreDto>> EvaluateAsync(
    int pQuestionId,
    string pUserAnswer,
    CancellationToken pCancellationToken)
{
    if (pQuestionId <= 0)
        return Result<ScoreDto>.Failure("Invalid question ID");

    if (string.IsNullOrWhiteSpace(pUserAnswer))
        return Result<ScoreDto>.Failure("Answer cannot be empty");

    var question = await _repository.GetByIdAsync(pQuestionId, pCancellationToken);
    if (question is null)
        return Result<ScoreDto>.Failure($"Question {pQuestionId} not found");

    // lógica principal aquí — sin anidación profunda
    return Result<ScoreDto>.Success(new ScoreDto());
}

// ❌ Incorrecto — anidación profunda, sin guard clauses
public async Task<ScoreDto?> EvaluateAsync(int questionId, string answer)
{
    if (questionId > 0)
    {
        if (!string.IsNullOrWhiteSpace(answer))
        {
            var q = await _repo.GetByIdAsync(questionId);
            if (q != null)
            {
                // lógica enterrada en 3 niveles
            }
        }
    }
    return null;
}
```

---

## Anti-patrones — NO hacer

| ❌ Anti-patrón | ✅ Fix |
|---|---|
| Comentar cada línea obvia | Eliminar el comentario; renombrar si hace falta |
| `new ExamService()` en otra clase | Inyectar `IExamService` por constructor |
| SQL inline en medio del método | Mover a `private const string` al tope del archivo |
| `SELECT *` en cualquier query | Listar columnas explícitamente |
| Concatenar strings para SQL | Usar parámetros Dapper con objeto anónimo |
| `.Result` o `.Wait()` en async | Usar `await` siempre |
| Duplicar helpers entre capas | Centralizar en `ExamenApi.Utilities` o `ExamenApi.Models` |
| Hardcodear connection string | Leer de `IOptions<DatabaseSettings>` |
| Hardcodear API key de Anthropic | Leer de `IOptions<AnthropicSettings>` / variable de entorno |
| Números mágicos inline | Definir constante nombrada con su significado |
| Método de 100+ líneas | Extraer en métodos privados con nombre descriptivo |

---

## Antes de editar cualquier archivo — PENSAR PRIMERO

Antes de modificar un archivo, preguntarse:

| Pregunta | Por qué |
|---|---|
| ¿Quién referencia este archivo? | Puede romper otras capas |
| ¿Cambia la firma de una interfaz? | Todos los que la implementan deben actualizarse |
| ¿Hay tests que cubren esto? | Los tests pueden fallar |
| ¿Es un tipo compartido en `Models`? | Afecta a `Api`, `Services` y `Data` simultáneamente |

> **Regla:** editar el archivo **y** todos sus dependientes en la misma tarea. Nunca dejar imports rotos ni implementaciones desactualizadas.

---

## Self-check antes de declarar tarea completa

| Check | Pregunta |
|---|---|
| ✅ Objetivo cumplido | ¿Hice exactamente lo que se pidió? |
| ✅ Archivos completos | ¿Modifiqué todos los archivos necesarios? |
| ✅ Compila | ¿`dotnet build` pasa sin errores ni warnings? |
| ✅ Convenciones | ¿Cumple todas las reglas de CLAUDE.md §4 y §5? |
| ✅ Prefijo `p` | ¿Todos los parámetros llevan `p`? |
| ✅ Sin secretos | ¿Nada hardcodeado? |
| ✅ Sin `SELECT *` | ¿Ninguna query usa asterisco? |

> Si algún check falla → corregir antes de responder "listo".

---

## Verificación post-cambio (obligatoria para Claude Code)

Después de cada cambio significativo, ejecutar:

```bash
dotnet build
```

Reportar resultado en este formato:

```
## Build result

### ✅ Éxito / ❌ Errores (N)
- [Archivo:Línea] Descripción del error

### ⚠️ Warnings (N)
- [Archivo:Línea] Descripción del warning

**¿Corrijo los errores?** (esperar confirmación antes de actuar)
```

> Nunca auto-corregir sin confirmación del desarrollador.
