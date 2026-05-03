# `.claude/` — Configuración de Claude Code para ExamenApi

Esta carpeta versiona la configuración compartida del equipo para trabajar con [Claude Code](https://docs.claude.com/en/docs/claude-code) sobre este repositorio.

## Estructura

```
.claude/
├── settings.json            # Permisos y configuración compartida (commiteable)
├── settings.local.json      # Configuración personal del desarrollador (gitignored)
├── commands/                # Slash commands del proyecto
│   ├── bootstrap-solution.md
│   ├── new-service.md
│   ├── new-repository.md
│   ├── new-controller.md
│   ├── check-conventions.md
│   └── commit-message.md
├── agents/                  # Subagentes especializados
│   └── convention-auditor.md
└── skills/                  # Skills de aitmpl adaptadas al proyecto
    ├── dotnet-backend.md
    ├── clean-code.md
    └── api-documentation-generator.md

# En la raíz del repositorio
.env.example                 # Template de variables de entorno (commiteable, sin valores reales)
.env                         # Variables reales (gitignored — NUNCA commitear)
```

## Slash commands disponibles

Se invocan en Claude Code con `/`:

| Comando | Para qué sirve |
|---|---|
| `/bootstrap-solution` | Crea la solución completa desde cero (proyectos, referencias, DI mínima). |
| `/new-service <Nombre>` | Genera interfaz + clase de un servicio en `ExamenApi.Services`, sin lógica. |
| `/new-repository <Nombre> <ModuloAcronimo>` | Genera repo Dapper en `ExamenApi.Data`, con SQL constante y stubs. |
| `/new-controller <Nombre>` | Genera controller stub en `ExamenApi.Api`. |
| `/check-conventions` | Audita el código actual contra CLAUDE.md y reporta violaciones. |
| `/commit-message` | Redacta un mensaje de commit en formato `CHANGE/FEATURE/FIX`. |

## Subagentes

| Agente | Propósito |
|---|---|
| `convention-auditor` | Revisa exhaustivamente cumplimiento de naming, capas, async, Dapper, SQL. Solo lectura. |

Se invocan con `@convention-auditor <petición>` o se delegan automáticamente cuando Claude Code detecta el caso de uso.

## Skills (.claude/skills/)

Skills de [aitmpl.com](https://www.aitmpl.com/skills) adaptadas al contexto del proyecto. **La versión local tiene precedencia sobre la skill genérica de aitmpl.** En cualquier conflicto con `CLAUDE.md`, gana `CLAUDE.md`.

| Skill | Fuente original | Cuándo aplica |
|---|---|---|
| `dotnet-backend.md` | [dotnet-backend](https://www.aitmpl.com/component/skill/development/dotnet-backend) | Controllers, servicios, repositorios, DI, patrones ASP.NET Core |
| `clean-code.md` | [clean-code](https://www.aitmpl.com/component/skill/development/clean-code) | Revisión de calidad de código, refactors, naming, anti-patrones |
| `api-documentation-generator.md` | [api-documentation-generator](https://www.aitmpl.com/component/skill/development/api-documentation-generator) | Documentar endpoints, XML docs, colección Postman, OpenAPI |

Para agregar nuevas skills: copiar el contenido de aitmpl, adaptarlo a convenciones locales, guardar en `.claude/skills/{nombre}.md`.

## Variables de entorno — `.env.example`

El archivo `.env.example` en la raíz del repo documenta **todas** las variables de entorno necesarias para el proyecto, con placeholders seguros. Es el único archivo de configuración de secretos que se commitea.

```bash
# Para desarrollo local:
cp .env.example .env
# Editar .env con los valores reales
# NUNCA commitear .env
```

Variables incluidas: `CONNECTIONSTRINGS__DEFAULT`, `ANTHROPIC__APIKEY`, `ANTHROPIC__MODEL`, `JWT__KEY`, logging y URLs.

## Permisos (`settings.json`)

Los permisos están **listados explícitamente** y son restrictivos por defecto:

- ✅ Permitido: comandos `dotnet`, lectura general, edición de `.cs` / `.csproj` / `.sln` / `.json` / `.md` / `.sql`.
- ❌ Denegado: lectura/edición de archivos sensibles (secrets, certs, .env, appsettings.Production), `git push`, `git commit`, `rm -rf`, `curl`, `wget`, mutaciones de user-secrets.

Las decisiones que se tomen interactivamente (por ejemplo "Always allow") se guardan en `settings.local.json`, que está en `.gitignore`.

## Convenciones del proyecto

Toda la configuración aquí es coherente con `CLAUDE.md` en la raíz del repo. **CLAUDE.md es la fuente de verdad.** Si algo en esta carpeta entra en contradicción con CLAUDE.md, gana CLAUDE.md y se debe abrir un PR para alinear.

## Cómo extender

- **Nuevo slash command:** crea `commands/{nombre}.md` con frontmatter (`description`, `argument-hint`, `allowed-tools`) y el cuerpo en Markdown. Se descubre automáticamente.
- **Nuevo subagente:** crea `agents/{nombre}.md` con frontmatter (`name`, `description`, `tools`, `model`) y un system prompt claro.
- **Cambio de permisos:** edita `settings.json` y abre PR. Los permisos personales van a `settings.local.json` (no commitear).
