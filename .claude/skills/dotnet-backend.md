# .NET Backend — ExamenApi Skill

> Skill tomada de [aitmpl.com/component/skill/development/dotnet-backend](https://www.aitmpl.com/component/skill/development/dotnet-backend) y adaptada a las convenciones de OneGoalDiscover / A·B Systems. **Esta versión local tiene precedencia sobre la skill genérica.**

---

Eres un experto en .NET/C# backend con experiencia en APIs empresariales. Trabajas exclusivamente dentro de las reglas de `CLAUDE.md`. Cuando esta skill entre en conflicto con `CLAUDE.md`, **gana `CLAUDE.md`**.

## Cuándo aplicar esta skill

- Crear o refactorizar controllers, servicios o repositorios.
- Implementar autenticación/autorización.
- Diseñar acceso a datos con **Dapper** (no EF Core).
- Agregar background workers o integraciones externas en C#.
- Mejorar rendimiento o fiabilidad del backend.

---

## Expertise en este proyecto

| Área | Tecnología |
|---|---|
| Framework | ASP.NET Core 9 — controller-based |
| Acceso a datos | **Dapper** (no EF Core) |
| Base de datos | SQL Server |
| Auth (futuro) | JWT + Bearer |
| Background | `BackgroundService` / `IHostedService` |
| IA integration | Anthropic API vía `HttpClient` tipado |
| Testing (futuro) | xUnit + FluentAssertions + NSubstitute |

---

## Patrones de código del proyecto

### Controller stub (convención local)

```csharp
// ✅ ExamenApi.Api/Controllers/ExamController.cs

[ApiController]
[Route("api/[controller]")]
public class ExamController : ControllerBase
{
    private readonly IExamService _examService;
    private readonly ILogger<ExamController> _logger;

    public ExamController(IExamService pExamService, ILogger<ExamController> pLogger)
    {
        _examService = pExamService;
        _logger = pLogger;
    }

    /// <summary>
    /// Retrieves all active questions for an exam session.
    /// </summary>
    /// <param name="pCancellationToken">Cancellation token.</param>
    /// <returns>200 OK with list of questions, or 500 on error.</returns>
    [HttpGet("questions")]
    public async Task<IActionResult> GetQuestionsAsync(CancellationToken pCancellationToken)
    {
        // TODO: implement
        return StatusCode(501, "Not implemented");
    }
}
```

> **Diferencias clave vs. skill genérica:**
> - Prefijo `p` en todos los parámetros del constructor y métodos.
> - Campos privados con `_`.
> - `CancellationToken pCancellationToken` siempre presente.
> - Sin lógica funcional hasta que se pida.

---

### Repositorio Dapper (convención local)

```csharp
// ✅ ExamenApi.Data/QuestionRepository.cs

public class QuestionRepository : IQuestionRepository
{
    private readonly string _connectionString;
    private readonly ILogger<QuestionRepository> _logger;

    // SQL en constante privada — nunca inline
    private const string SqlGetActiveQuestions = @"
        SELECT question_id,
               question_text,
               weight
        FROM   tDSVEXAMpreguntasE
        WHERE  is_active = 1
        ORDER  BY question_order;";

    public QuestionRepository(IOptions<DatabaseSettings> pOptions, ILogger<QuestionRepository> pLogger)
    {
        _connectionString = pOptions.Value.Default;
        _logger = pLogger;
    }

    public async Task<IEnumerable<QuestionModel>> GetActiveQuestionsAsync(CancellationToken pCancellationToken)
    {
        await using var connection = new SqlConnection(_connectionString);

        return await connection.QueryAsync<QuestionModel>(
            new CommandDefinition(SqlGetActiveQuestions, cancellationToken: pCancellationToken));
    }
}
```

> **Reglas SQL que siempre aplican:**
> - Sin `SELECT *`.
> - Sin concatenación de strings en SQL.
> - Nombres de tablas: `{prefijo}{MODULO}{nombre}{E|D}` (ver CLAUDE.md §5).

---

### Servicio con Result pattern (convención local)

```csharp
// ✅ ExamenApi.Services/ExamService.cs

public class ExamService : IExamService
{
    private readonly IQuestionRepository _questionRepository;
    private readonly ILogger<ExamService> _logger;

    public ExamService(
        IQuestionRepository pQuestionRepository,
        ILogger<ExamService> pLogger)
    {
        _questionRepository = pQuestionRepository;
        _logger = pLogger;
    }

    public async Task<Result<IEnumerable<QuestionDto>>> GetActiveQuestionsAsync(
        CancellationToken pCancellationToken)
    {
        // TODO: implement
        return Result<IEnumerable<QuestionDto>>.Failure("Not implemented");
    }
}
```

---

### HttpClient tipado para Anthropic (patrón futuro)

```csharp
// ✅ ExamenApi.Services/AnthropicClient.cs

public class AnthropicClient : IAnthropicClient
{
    private readonly HttpClient _httpClient;
    private readonly IOptions<AnthropicSettings> _settings;
    private readonly ILogger<AnthropicClient> _logger;

    public AnthropicClient(
        HttpClient pHttpClient,
        IOptions<AnthropicSettings> pSettings,
        ILogger<AnthropicClient> pLogger)
    {
        _httpClient = pHttpClient;
        _settings = pSettings;
        _logger = pLogger;
    }

    public async Task<Result<string>> EvaluateAnswerAsync(
        string pQuestion,
        string pUserAnswer,
        CancellationToken pCancellationToken)
    {
        // TODO: implement
        return Result<string>.Failure("Not implemented");
    }
}

// Registro en Program.cs:
// builder.Services.AddHttpClient<IAnthropicClient, AnthropicClient>(client =>
// {
//     client.BaseAddress = new Uri(builder.Configuration["Anthropic:BaseUrl"]!);
//     client.DefaultRequestHeaders.Add("x-api-key", builder.Configuration["Anthropic:ApiKey"]);
//     client.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");
// });
```

---

## Registro de DI — Program.cs (patrón esperado)

```csharp
// Configuración fuertemente tipada (nunca hardcodear)
builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection("ConnectionStrings"));
builder.Services.Configure<AnthropicSettings>(
    builder.Configuration.GetSection("Anthropic"));

// Servicios — Scoped por default para operaciones por-request
builder.Services.AddScoped<IExamService, ExamService>();
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();

// HttpClient tipado para Anthropic
builder.Services.AddHttpClient<IAnthropicClient, AnthropicClient>(client =>
{
    // Configuración desde IOptions en el constructor del cliente
});
```

---

## Buenas prácticas — lista de verificación

- ✅ Async/await en toda operación I/O.
- ✅ DI por constructor, nunca `new` de dependencias internas.
- ✅ `CancellationToken` propagado hasta el repositorio.
- ✅ Configuración desde `IOptions<T>`, no `IConfiguration` directamente.
- ✅ `User Secrets` en desarrollo local (`dotnet user-secrets`).
- ✅ Variables de entorno en staging/producción (`.env` no va a producción).
- ✅ Logging estructurado, nunca loggear secretos ni PII.
- ✅ Manejo de errores con `Result<T>`, no excepciones de flujo esperado.
- ✅ Health checks (`AddHealthChecks`) — agregar cuando haya DB funcional.
- ✅ OpenAPI/Swagger habilitado en Development.

## Limitaciones

- No usar EF Core. Todo acceso a datos es vía Dapper.
- No agregar AutoMapper, MediatR, FluentValidation sin pedirlo.
- No implementar lógica funcional hasta que se indique explícitamente.
- Para .NET Framework legacy: no aplica — este proyecto es .NET 9 únicamente.
