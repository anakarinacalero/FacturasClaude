# FacturasClaude — Contexto del Proyecto

> Documento único de referencia para desarrollo. Define arquitectura, reglas, objetivo y restricciones del sistema.

---

# 1. Identidad del proyecto

- **Nombre:** FacturasClaude
- **Tipo:** API empresarial REST
- **Dominio:** Document AI (extracción de datos desde facturas)
- **Tecnología base:** .NET 9 + ASP.NET Core Web API
- **Estado actual:** Setup inicial (sin lógica de negocio implementada)

---

# 2. Objetivo funcional

El sistema FacturasClaude tiene como objetivo procesar documentos de facturación utilizando inteligencia artificial (Claude de Anthropic) para extraer información estructurada desde archivos no estructurados.

## Flujo general del sistema:

1. Recibir un archivo de factura en formato PDF, imagen u otros formatos compatibles.
2. Enviar el contenido del documento a Claude (Anthropic API).
3. Extraer información estructurada del documento, incluyendo:
   - Fecha de emisión
   - Monto total
   - Descripción o concepto
   - Proveedor / emisor
   - Otros campos relevantes según el documento
4. Normalizar y estructurar la información extraída.
5. Validar los datos antes de persistirlos.
6. Guardar la información estructurada en SQL Server.
7. Mantener registro del archivo original y su resultado de extracción.

> ⚠️ Este documento es SOLO de contexto. No implementar lógica de negocio sin autorización explícita.

---

# 3. Stack tecnológico

| Capa | Tecnología |
|------|------------|
| Backend | .NET 9 |
| API | ASP.NET Core Web API |
| Acceso a datos | Dapper (único permitido) |
| Base de datos | SQL Server |
| IA | Anthropic API (Claude) |
| Configuración | appsettings + IOptions + User Secrets |
| Logging | Microsoft.Extensions.Logging |

---

# 4. Arquitectura del proyecto

## 4.1 Estructura de solución
Controllers/
Services/
Repositories/


### Interfaces

- Interfaces de Controllers → dentro de `Controllers/`
- Interfaces de Services → dentro de `Services/`
- Interfaces de Repositories → dentro de `Repositories/`

---

## 4.3 Flujo de ejecución
Controller → Service → Repository → SQL Server


---

# 5. Reglas de desarrollo

## 5.1 Principios generales

- No usar EF Core
- Solo Dapper para acceso a datos
- Todo I/O debe ser async
- Propagar CancellationToken en toda la cadena
- Usar DI (Dependency Injection) siempre
- No instanciar clases con `new` en lógica de negocio

---

## 5.2 Restricciones estrictas

- ❌ No generar lógica de negocio sin autorización
- ❌ No crear endpoints funcionales sin aprobación
- ❌ No instalar dependencias sin confirmación
- ❌ No asumir estructura de datos sin especificación
- ❌ No usar librerías no autorizadas

---

## 5.3 Estilo de código

- C# en inglés
- PascalCase para clases y métodos
- camelCase para variables y parámetros
- Prefijo `_` para campos privados
- Prefijo `p` para parámetros
- Una clase por archivo
- Archivos nombrados según clase

---

## 6. Base de datos (SQL Server)

## 6.1 Convención de nombres

Prefijo obligatorio:

- Tablas: `tDSVFAC`
- Vistas: `vDSVFAC`
- SPs: `pDSVFAC`
- Funciones: `fnDSVFAC`

## 6.2 Ejemplos
tDSVFACdocument
tDSVFACextractedData
tDSVFACfileStorage


---

## 6.3 Reglas SQL

- Prohibido `SELECT *`
- No usar nombres de clientes o personalizaciones
- Campos en `snake_case`
- Parámetros con prefijo `p`
- Objetos independientes (sin acoplamiento innecesario)

---

# 7. Integración con IA (Claude)

- Uso de Anthropic API (Claude)
- Comunicación vía HttpClient tipado
- Respuesta esperada: extracción estructurada de datos de factura
- No usar IA para lógica de negocio fuera de extracción documental

---

# 8. Seguridad y configuración

- Connection strings en User Secrets o variables de entorno
- API Keys nunca hardcodeadas
- Uso de `IOptions<T>` obligatorio para configuración
- No exponer secretos en logs

---

# 9. Reglas de colaboración con Claude

Claude debe:

- Preguntar antes de cualquier cambio
- No crear lógica de negocio sin autorización
- No instalar paquetes sin aprobación
- Mantener estructura limpia
- Seguir arquitectura definida estrictamente
- Generar solo scaffolding cuando no se indique lo contrario

---

# 10. Estado actual del sistema

- API base creada
- Estructura de carpetas definida
- Sin lógica de extracción implementada
- Sin endpoints funcionales de negocio
- Preparado para integración con Claude (fase futura)

---

# 11. Evolución futura (NO IMPLEMENTAR AÚN)

Posibles extensiones:

- OCR previo a Claude
- Validación humana de facturas
- Clasificación de tipos de factura
- Versionado de prompts
- Pipeline de procesamiento batch
- Auditoría de extracción

---

# Última actualización

Migración completa desde sistema de evaluación de competencias a sistema de extracción de datos de facturas mediante IA (Document AI con Claude).