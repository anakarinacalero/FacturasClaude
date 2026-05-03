# FacturasClaude

API REST para extracción automática de datos desde documentos de facturación utilizando inteligencia artificial (Claude de Anthropic).

---

## Stack tecnológico

| Capa | Tecnología |
|---|---|
| Backend | .NET 9 / ASP.NET Core Web API |
| Acceso a datos | Dapper |
| Base de datos | SQL Server |
| IA | Anthropic API (Claude) |
| Configuración | appsettings.json + User Secrets + IOptions\<T\> |
| Logging | Microsoft.Extensions.Logging |

---

## Estructura de la solución

```
FacturasClaude/
├── FacturasClaude.Api/
│   ├── Controllers/
│   │   ├── Interfaces/
│   │   │   └── IInvoiceController.cs
│   │   └── InvoiceController.cs
│   ├── Middleware/
│   │   └── ErrorHandlingMiddleware.cs
│   ├── Repositories/
│   │   ├── Interfaces/
│   │   │   ├── IClaudeUsageRepository.cs
│   │   │   ├── IDocumentRepository.cs
│   │   │   └── IInvoiceRepository.cs
│   │   ├── ClaudeUsageRepository.cs
│   │   ├── DocumentRepository.cs
│   │   └── InvoiceRepository.cs
│   ├── Services/
│   │   ├── Interfaces/
│   │   │   ├── IAnthropicHttpClient.cs
│   │   │   ├── IClaudeUsageService.cs
│   │   │   └── IInvoiceExtractionService.cs
│   │   ├── AnthropicHttpClient.cs
│   │   ├── ClaudeUsageService.cs
│   │   └── InvoiceExtractionService.cs
│   └── Program.cs
│
├── FacturasClaude.Models/
│   ├── DTOs/
│   │   ├── AnthropicContentBlock.cs
│   │   └── AnthropicUsage.cs
│   ├── Entities/
│   │   ├── ClaudeUsageRecord.cs
│   │   ├── DocumentRecord.cs
│   │   ├── InvoiceConcept.cs
│   │   └── InvoiceData.cs
│   ├── Responses/
│   │   └── AnthropicMessageResponse.cs
│   └── Settings/
│       └── AnthropicSettings.cs
│
└── FacturasClaude.Database/
    ├── dbo/
    │   └── Tables/
    │       ├── tDSVFACclaudeUsage.sql
    │       ├── tDSVFACdocument.sql
    │       ├── tDSVFACextractedData.sql
    │       └── tDSVFACinvoiceConcept.sql
    └── Scripts/
        └── CreateTable_tDSVFACclaudeUsage.sql
```

---

## Flujo de la aplicación

```
Cliente
  │
  │  POST /api/invoice/extract
  │  Content-Type: multipart/form-data
  │  Body: file (PDF, JPG, PNG, WEBP, GIF)
  │
  ▼
InvoiceController
  │  1. Valida que el archivo no esté vacío
  │  2. Valida que el Content-Type sea soportado
  │
  ▼
DocumentRepository.InsertAsync
  │  Registra el archivo original en tDSVFACdocument
  │  Retorna el ID generado (documentId)
  │
  ▼
InvoiceExtractionService.ExtractAsync
  │  1. Convierte el stream a Base64
  │  2. Construye el bloque de contenido:
  │     - PDF   → type: "document"
  │     - Imagen → type: "image"
  │  3. Envía a la API de Claude con prompt de extracción
  │
  ▼
AnthropicHttpClient.SendMessageAsync
  │  POST https://api.anthropic.com/v1/messages
  │  Mide el tiempo de respuesta con Stopwatch
  │  Captura request-id del header HTTP
  │  Retorna AnthropicMessageResponse completo:
  │    id, model, stop_reason, content, usage, requestId, elapsedMs
  │
  ▼
ClaudeUsageService.RecordAsync
  │  Persiste el uso de la API en tDSVFACclaudeUsage:
  │    - message_id / request_id
  │    - model / stop_reason
  │    - input_tokens / output_tokens
  │    - cache_creation_tokens / cache_read_tokens
  │    - elapsed_ms / created_at
  │  Vinculado al documento por document_id (FK)
  │
  ▼
InvoiceExtractionService (parseo de respuesta)
  │  Extrae el JSON del content de Claude
  │  Deserializa a InvoiceData
  │
  ▼
InvoiceRepository.InsertAsync
  │  Persiste los datos extraídos en tDSVFACextractedData
  │  Persiste los conceptos en tDSVFACinvoiceConcept
  │  Vinculado al documento por document_id (FK)
  │
  ▼
Cliente
     200 OK — InvoiceData (JSON)
```

### Manejo de errores

Todos los errores no controlados son capturados por `ErrorHandlingMiddleware`. Las excepciones se mapean a códigos HTTP:

| Excepción | HTTP |
|---|---|
| `ArgumentException` / `ArgumentNullException` | 400 Bad Request |
| `KeyNotFoundException` | 404 Not Found |
| `UnauthorizedAccessException` | 401 Unauthorized |
| Cualquier otra | 500 Internal Server Error |

---

## Configuración

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=FacturasClaude.Database;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Anthropic": {
    "ApiKey": "",
    "Model": "claude-sonnet-4-6",
    "MaxTokens": 1024
  }
}
```

> La `ApiKey` debe configurarse en User Secrets, nunca en `appsettings.json`.

### Cambiar el modelo

Edita el campo `Model` en `appsettings.json`. No requiere recompilar.

| Modelo | ID |
|---|---|
| Opus 4.7 (más capaz) | `claude-opus-4-7` |
| Sonnet 4.6 (balance) | `claude-sonnet-4-6` |
| Haiku 4.5 (más rápido) | `claude-haiku-4-5-20251001` |

### User Secrets

```bash
dotnet user-secrets set "Anthropic:ApiKey" "sk-ant-..."
```

### Base de datos

Ejecutar los scripts de `FacturasClaude.Database/dbo/Tables/` en SQL Server antes de iniciar la API. Para `tDSVFACclaudeUsage` también está disponible el script idempotente en `Scripts/CreateTable_tDSVFACclaudeUsage.sql`.

### Tipos de archivo soportados

| Tipo | MIME |
|---|---|
| PDF | `application/pdf` |
| JPEG | `image/jpeg` |
| PNG | `image/png` |
| WebP | `image/webp` |
| GIF | `image/gif` |

---

## Convenciones de base de datos

| Objeto | Prefijo | Ejemplo |
|---|---|---|
| Tablas | `tDSVFAC` | `tDSVFACdocument` |
| Vistas | `vDSVFAC` | `vDSVFACfacturas` |
| Stored Procedures | `pDSVFAC` | `pDSVFACinsertDocument` |
| Funciones | `fnDSVFAC` | `fnDSVFACformatDate` |

---

## Próximos pasos

### Validación de datos extraídos
Agregar un `IInvoiceValidator` / `InvoiceValidator` que se ejecute entre la extracción de Claude y el INSERT a base de datos. Retorna `422 Unprocessable Entity` si los datos no son coherentes.

Reglas propuestas:
- `Supplier` no puede ser nulo ni vacío
- `TotalAmount` debe ser mayor que cero si está presente
- `IssueDate` debe tener formato `YYYY-MM-DD` válido si está presente
- `Currency` debe tener exactamente 3 caracteres si está presente

### Stored Procedures
Mover las queries SQL inline de los repositorios a stored procedures en `FacturasClaude.Database`:
- `pDSVFACinsertDocument`
- `pDSVFACgetDocumentById`
- `pDSVFACinsertExtractedData`
- `pDSVFACgetExtractedDataByDocumentId`
- `pDSVFACinsertClaudeUsage`

### Endpoint de consulta
Agregar `GET /api/invoice/{documentId}` para recuperar los datos extraídos de una factura previamente procesada.

### OCR previo a Claude
Para imágenes de baja calidad, agregar una etapa de OCR antes de enviar a Claude para mejorar la precisión de la extracción.

### Procesamiento batch
Endpoint `POST /api/invoice/batch` para procesar múltiples facturas en una sola llamada, con resultados por archivo.

### Versionado de prompts
Externalizar el prompt de extracción a configuración o base de datos para permitir ajustes sin redespliegue.

### Validación humana
Flujo opcional de revisión manual para facturas donde la extracción tenga campos críticos nulos.
