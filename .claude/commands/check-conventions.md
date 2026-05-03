---
description: Revisa el archivo o cambio actual contra todas las convenciones definidas en CLAUDE.md y reporta violaciones.
allowed-tools: Read, Bash(git diff:*), Bash(git status:*), Bash(rg:*)
---

# /check-conventions

Audita el código contra las reglas de **CLAUDE.md**. Reporta violaciones de forma estructurada.

## Checklist de auditoría

### Naming (CLAUDE.md §4)
- [ ] Archivos en PascalCase con sufijo correcto (`*Service.cs`, `*Controller.cs`, `*Repository.cs`, `*Dto.cs`, `*Model.cs`, `*Helper.cs`, `*Utilities.cs`).
- [ ] Clases, métodos, propiedades, constantes en PascalCase.
- [ ] Parámetros, variables locales, campos privados en camelCase.
- [ ] Interfaces empiezan con `I` mayúscula.
- [ ] Campos privados con prefijo `_`.
- [ ] **Todos los parámetros con prefijo `p`** (regla específica de la organización).
- [ ] Identificadores en inglés (excepto propiedades mapeadas a columnas DB en snake_case).

### Arquitectura (CLAUDE.md §3)
- [ ] No hay `new` de servicios, repositorios o dependencias internas. Todo inyectado.
- [ ] La capa `Api` no es referenciada por `Services` ni `Data`.
- [ ] Los DTOs vienen de `ExamenApi.Models`, no se duplican por capa.

### Async / I/O (CLAUDE.md §6.1)
- [ ] Todo método I/O es `async Task<T>`.
- [ ] No hay `.Result`, `.Wait()`, `.GetAwaiter().GetResult()`, ni `async void`.
- [ ] `CancellationToken` se propaga end-to-end.

### Dapper (CLAUDE.md §6.2)
- [ ] SQL en `private const string` al tope del archivo, no inline.
- [ ] **Cero `SELECT *`** en cualquier query.
- [ ] Parámetros vía objetos anónimos, nunca concatenación de strings.
- [ ] `using` o `await using` en la conexión.
- [ ] `CommandDefinition` con `cancellationToken`.

### Convenciones SQL (CLAUDE.md §5)
- [ ] Objetos referenciados respetan `{prefijo}{MODULO}{nombre}{E|D}`.
- [ ] Columnas en `snake_case`.
- [ ] Parámetros SP con prefijo `p` (`@pUserId`, etc.).

### Seguridad (CLAUDE.md §8)
- [ ] No hay connection strings, API keys, ni tokens hardcodeados.
- [ ] No se modificaron archivos sensibles.

### Limpieza (CLAUDE.md §6.1)
- [ ] Sin warnings del compilador.
- [ ] Sin comentarios obvios ni código muerto.
- [ ] Métodos cortos (idealmente <30 líneas).

## Salida esperada

Genera un reporte con tres secciones:

1. **✅ Cumple** — qué reglas pasan.
2. **⚠️ Riesgos** — cosas que cumplen al borde o requieren juicio.
3. **❌ Violaciones** — cada una con:
   - Archivo y línea.
   - Regla violada (con referencia a sección de CLAUDE.md).
   - Sugerencia concreta de fix.

Si hay violaciones, ofrece corregirlas pero **no las apliques sin confirmación**.
