# Copilot Instructions

## General Guidelines
- El usuario prefiere que la gestión de catálogos se defina explícitamente por entidad y evitar la reflexión, especialmente en el servidor y en la UI cuando sea posible.
- Los cambios deben priorizar seguridad, pruebas y alcance mínimo; comentarios solo para explicar motivos o decisiones, no repetir código.
- Mantener separación de responsabilidades y dependencias explícitas. Usar genéricos solo para infraestructura interna realmente compartida; no añadir repositorios genéricos, Unit of Work, CQRS, MediatR, microservicios o buses por defecto.
- No inventar requisitos ni decisiones de despliegue. Ante ambigüedades que cambien el diseño, preguntar; documentar supuestos, riesgos y tareas que requieran acceso externo.
- Tratar secretos, autenticación, autorización, transporte cifrado, validación de entradas y exposición de datos como requisitos de seguridad por defecto. Nunca añadir secretos reales, claves de respaldo conocidas ni credenciales a archivos versionados.
- En HSis, aplicar cambios mínimos, seguridad y pruebas; evitar abstracciones genéricas innecesarias, y antes de cambios inspeccionar implementaciones, referencias, configuración y pruebas afectadas.

## Modelos de Desarrollo
- Aplicar KISS, DRY, SOLID y YAGNI: implementar solo lo que el requisito actual necesita; no anticipar funcionalidades ni añadir abstracciones especulativas.
- En HSis, aplicar modelo híbrido y explícito por entidad en DTOs, contratos, validaciones, endpoints, clientes públicos y formularios; evitar reflexión y CRUD genérico en el flujo administrativo.
- Usar nombres significativos y coherentes con las convenciones C#: PascalCase para tipos y miembros públicos, camelCase para parámetros y locales, _camelCase para campos privados, interfaces con prefijo I y sufijo Async para operaciones asíncronas. Usar Id, Api, Dto y Http con capitalización consistente; evitar nuevos identificadores con tildes o ñ.

## Formularios
- Generar los formularios WinForms necesarios para crear registros de los catálogos.

## Pruebas y Documentación
- Añadir o actualizar pruebas para el comportamiento modificado; validar con build y pruebas disponibles antes de concluir e informar claramente de cualquier limitación.
- Antes de modificar código, inspeccionar las implementaciones, referencias, configuración y pruebas afectadas. Mantener el cambio mínimo y no hacer limpiezas no relacionadas.
- Para cada tarea, resumir el objetivo, el cambio mínimo propuesto y el TL;DR del resultado; no convertir una revisión en una refactorización global sin solicitud expresa.
- Mantener el idioma y vocabulario del área existentes, sin mezclar español e inglés dentro de una misma API sin motivo.
- Escribir comentarios solo para explicar por qué existe una decisión o una limitación no obvia; no comentar lo que el código ya expresa. Mantener espacios en blanco y formato coherentes con el archivo.
- No reescribir historial Git, borrar datos, ejecutar migraciones irreversibles ni realizar cambios remotos sin copia de seguridad y autorización explícita. Advertir sobre credenciales expuestas y separar su rotación externa de los cambios al código.
