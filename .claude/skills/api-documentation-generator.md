# API Documentation Generator — ExamenApi Skill

> Skill tomada de [aitmpl.com/component/skill/development/api-documentation-generator](https://www.aitmpl.com/component/skill/development/api-documentation-generator) y adaptada al stack y convenciones de OneGoalDiscover / A·B Systems. En conflicto con `CLAUDE.md`, **gana `CLAUDE.md`**.

---

## Cuándo aplicar esta skill

- Documentar un endpoint nuevo o modificado.
- Generar la especificación OpenAPI/Swagger de un controller.
- Onboarding de desarrolladores a la API.
- Preparar la colección Postman (`OneGoalDiscover_Postman`) para testing.
- Mantener `OneGoalDiscover_Documentation` en sincronía con el código.

---

## Proceso de documentación

### Paso 1 — Analizar la estructura

Antes de generar documentación, leer:

1. El controller correspondiente (`ExamenApi.Api/Controllers/`).
2. La interfaz del servicio (`ExamenApi.Services/I*Service.cs`).
3. Los DTOs de entrada y salida en `ExamenApi.Models/`.
4. Los objetos de BD relacionados en `ExamenApi.Data/` (para entender qué columnas se exponen y cuáles no).

### Paso 2 — Generar XML Docs en el código fuente

Todo endpoint documentado lleva XML doc completo **en el controller**:

```csharp
/// <summary>
/// Retrieves the list of active questions for an evaluation session.
/// </summary>
/// <param name="pCancellationToken">Propagated cancellation token.</param>
/// <returns>
/// 200 OK — list of <see cref="QuestionDto"/>.<br/>
/// 500 Internal Server Error — unexpected server failure.
/// </returns>
[HttpGet("questions")]
[ProducesResponseType(typeof(IEnumerable<QuestionDto>), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public async Task<IActionResult> GetQuestionsAsync(CancellationToken pCancellationToken)
```

> **Regla:** `[ProducesResponseType]` por cada código de estado posible. Sin excepción.

### Paso 3 — Formato de documentación de endpoint

Para cada endpoint, generar un bloque Markdown con esta estructura exacta:

```markdown
## {Verbo} {ruta}

{Descripción una línea: qué hace, sobre qué recurso.}

**Autenticación:** Requerida / No requerida (Bearer token)

### Request

**Path params**
| Parámetro | Tipo | Requerido | Descripción |
|---|---|---|---|
| `id` | `int` | ✅ | Identificador de la pregunta. |

**Query params** *(si aplica)*
| Parámetro | Tipo | Requerido | Default | Descripción |
|---|---|---|---|---|

**Body** *(si aplica — referenciar el Request DTO)*
```json
{
  "questionId": 1,
  "userAnswer": "La respuesta del usuario."
}
```

### Responses

**200 OK**
```json
{
  "questionId": 1,
  "score": 8.5,
  "feedback": "Respuesta correcta con buena fundamentación."
}
```

**400 Bad Request**
```json
{ "error": "VALIDATION_ERROR", "message": "userAnswer is required." }
```

**500 Internal Server Error**
```json
{ "error": "INTERNAL_ERROR", "message": "Unexpected server error." }
```

### Ejemplo cURL
```bash
curl -X POST https://localhost:5001/api/exam/evaluate \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{ "questionId": 1, "userAnswer": "Mi respuesta aquí." }'
```
```

---

## Documentación de errores — referencia del proyecto

Todos los errores siguen este formato JSON estándar:

```json
{
  "error": "ERROR_CODE_IN_SCREAMING_SNAKE",
  "message": "Descripción legible para el desarrollador."
}
```

| Código HTTP | Cuándo usarlo |
|---|---|
| 200 | Operación exitosa con datos. |
| 201 | Recurso creado. Incluir `Location` header cuando aplique. |
| 204 | Operación exitosa sin cuerpo de respuesta. |
| 400 | Input inválido o validación fallida. |
| 401 | No autenticado (token faltante o inválido). |
| 403 | Autenticado pero sin permisos suficientes. |
| 404 | Recurso no encontrado. |
| 409 | Conflicto (ej. duplicado). |
| 422 | Entidad no procesable (validación de negocio). |
| 500 | Error interno inesperado. |

---

## Estructura de secciones en `OneGoalDiscover_Documentation`

Cada servicio/módulo tiene su propio `.md` con estas secciones:

1. **Introducción** — qué hace el módulo, URL base, versión.
2. **Autenticación** — cómo obtener el token, cómo usarlo.
3. **Quick Start** — ejemplo funcional mínimo de punta a punta.
4. **Endpoints** — un bloque por endpoint (formato §Paso 3).
5. **Modelos de datos** — schema de DTOs con descripción de cada campo.
6. **Manejo de errores** — tabla de códigos + ejemplos.
7. **Colección Postman** — enlace o instrucciones para importar desde `OneGoalDiscover_Postman`.
8. **Changelog** — cambios por versión, breaking changes, deprecaciones.

---

## Colección Postman — convenciones

La colección vive en `OneGoalDiscover_Postman` y sigue esta organización:

```
ExamenApi/
├── Auth/
│   ├── POST Login
│   └── POST Refresh Token
├── Exam/
│   ├── GET  Questions
│   ├── POST Submit Answer
│   └── GET  Results/{sessionId}
└── _Environments/
    ├── Local.json
    └── Staging.json
```

Variables de entorno Postman:

| Variable | Ejemplo | Descripción |
|---|---|---|
| `baseUrl` | `https://localhost:5001` | URL base de la API |
| `token` | `(se llena al hacer login)` | Bearer token activo |
| `questionId` | `1` | ID de pregunta para tests |

> **Regla:** Nunca hardcodear la API key de Anthropic en la colección Postman. Usar variable de entorno en el servidor.

---

## Pitfalls frecuentes — y cómo evitarlos

| Problema | Síntoma | Solución |
|---|---|---|
| Docs desincronizadas | Ejemplos que no compilan | Generar docs desde XML comments; review al hacer PR |
| Errores sin documentar | Support tickets innecesarios | `[ProducesResponseType]` por cada código posible |
| Ejemplos con datos falsos | Usuarios frustrados | Usar datos realistas que pasen validaciones reales |
| Parámetros ambiguos | Requests inválidos del cliente | Documentar tipo, formato, rango, ejemplo y si es requerido |
| Sin documentación de rate limiting | Clientes bloqueados inesperadamente | Agregar sección cuando se implemente |

---

## Buenas prácticas — lista rápida

- ✅ Consistencia de formato en todos los endpoints.
- ✅ Ejemplos que realmente funcionan (probarlos).
- ✅ Documentar todos los errores posibles, no solo el happy path.
- ✅ Datos realistas en los ejemplos.
- ✅ Mantener docs en el mismo PR que el código que documentan.
- ✅ Referenciar el DTO exacto por nombre y proyecto.
- ❌ "Gets data" como descripción — siempre ser específico.
- ❌ Olvidar documentar autenticación en cada endpoint que la requiera.
- ❌ Dejar ejemplos cURL con valores que no existen en la BD.
