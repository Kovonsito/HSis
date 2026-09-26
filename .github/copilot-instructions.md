# Copilot Instructions

## Directrices del proyecto
- El usuario prefiere que la gestión de catálogos se defina explícitamente por entidad y evitar la reflexión, especialmente en el servidor y en la UI cuando sea posible.
- En HSis se debe usar un modelo híbrido: definir explícitamente por entidad los DTOs, contratos, validaciones, endpoints, clientes públicos y formularios; reutilizar genéricos solo para infraestructura común interna, como llamadas HTTP, respuestas, paginación y controles compartidos, evitando reflexión en el flujo administrativo.
- El usuario aprueba reemplazar la gestión genérica/reflexiva de catálogos por definiciones y operaciones explícitas por entidad.

## Formularios
- Generar los formularios WinForms necesarios para crear registros de los catálogos.