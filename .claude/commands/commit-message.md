---
description: Genera un mensaje de commit en el formato de OneGoalDiscover (CHANGE/FEATURE/FIX) basado en los cambios actuales.
allowed-tools: Bash(git status:*), Bash(git diff:*), Bash(git log:*)
---

# /commit-message

Genera un mensaje de commit siguiendo el estándar de la organización (CLAUDE.md §7).

## Formato obligatorio

```
{ETIQUETA}: {Asunto breve, en imperativo, en inglés}

{Descripción con contexto: qué cambió, por qué, y qué impacto tiene.
Líneas <= 72 caracteres. Una línea en blanco entre asunto y descripción.}
```

### Etiquetas válidas

- `CHANGE` — refactors, cambios en código existente, mejoras técnicas que no añaden funcionalidad nueva.
- `FEATURE` — nueva funcionalidad o capacidad.
- `FIX` — corrección de un bug.

## Pasos

1. Corre `git status` y `git diff --stat` para entender el alcance.
2. Corre `git diff` para ver el contenido del cambio.
3. Decide la etiqueta correcta (si hay duda entre dos, prefiere la más conservadora).
4. Redacta:
   - **Asunto:** una línea, modo imperativo, ≤ 72 caracteres, sin punto final.
   - **Descripción:** 2 a 5 líneas que expliquen *por qué* y *qué impacto* tiene el cambio.
5. **NO ejecutes el commit.** Solo presenta el mensaje al usuario para que lo copie o confirme.

## Ejemplo de salida

```
FEATURE: Add initial scaffolding for ExamenApi solution

Creates ExamenApi.Api, .Models, .Services, .Data and .Utilities
projects with base folder layout and DI wiring. No business logic
yet — all service and repository methods return NotImplemented stubs.
Establishes the foundation for upcoming question/answer evaluation
flow integrated with the Anthropic API.
```
