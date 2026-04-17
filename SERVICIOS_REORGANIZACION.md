# Reorganización de Servicios - HSis.Logic

## Estructura Organizada por Modelo

### 1. UsuarioService (consolidado)
**Archivo:** `HSis.Logic\UsuarioService.cs`

**Métodos - Autenticación:**
- `Autenticar(string nombreUsuario, string contraseña)` → `Usuario?`

**Métodos - Obtener Listados Complementarios:**
- `ObtenerDepartamentos()` → `List<Departamento>`
- `ObtenerPuestos()` → `List<Puesto>`
- `ObtenerSucursales()` → `List<Sucursal>`

**Métodos - CRUD Usuario:**
- `CrearUsuario(Usuario usuario)` → `void`
- `ObtenerUsuarioPorId(int idUsuario)` → `Usuario?`
- `ObtenerUsuarios()` → `List<Usuario>`
- `ActualizarUsuario(Usuario usuario)` → `void`
- `EliminarUsuario(int idUsuario)` → `void`

---

### 2. TicketService
**Archivo:** `HSis.Logic\TicketService.cs`

**Métodos - Obtención:**
- `ObtenerTickets()` → `List<Ticket>` (todos los tickets)
- `ObtenerTicketPorId(int id)` → `Ticket` (ticket específico)
- `ObtenerTicketsPorSLA(bool esUrgente)` → `List<Ticket>` (filtrados por urgencia)
- `ObtenerTicketsPorEstatus(string estatus)` → `List<Ticket>` (filtrados por estado)

---

### 3. MaterialService (nuevo)
**Archivo:** `HSis.Logic\MaterialService.cs`

**Métodos - Obtención:**
- `ObtenerMateriales()` → `List<Material>`
- `ObtenerMaterialPorId(int idMaterial)` → `Material?`

**Métodos - CRUD:**
- `CrearMaterial(Material material)` → `void`
- `ActualizarMaterial(Material material)` → `void`
- `EliminarMaterial(int idMaterial)` → `void`

**Métodos - Gestión de Costos:**
- `ActualizarCostoMaterial(int idMaterial, decimal nuevoCosto)` → `void`

---

### 4. IngresoService (nuevo)
**Archivo:** `HSis.Logic\IngresoService.cs`

**Métodos - Obtención:**
- `ObtenerIngresos()` → `List<Ingreso>`
- `ObtenerIngresoPorId(int idIngreso)` → `Ingreso?`
- `ObtenerIngresoPorMaterial(int idMaterial)` → `Ingreso?`
- `ObtenerIngresosPorRangoFechas(DateTime inicio, DateTime fin)` → `List<Ingreso>`

**Métodos - Registro:**
- `RegistrarIngreso(Ingreso ingreso)` → `void`
- `ActualizarIngreso(Ingreso ingreso)` → `void`

---

### 5. DetTicketService (nuevo)
**Archivo:** `HSis.Logic\DetTicketService.cs`

**Métodos - Obtención:**
- `ObtenerDetallesTicket(int idTicket)` → `List<DetTicket>`
- `ObtenerDetallePorId(int idTicket, int idMaterial)` → `DetTicket?`
- `ObtenerTodosDetalles()` → `List<DetTicket>`

**Métodos - CRUD:**
- `AgregarMaterialATicket(DetTicket detTicket)` → `void`
- `ActualizarDetalleTicket(DetTicket detTicket)` → `void`
- `EliminarMaterialDeTicket(int idTicket, int idMaterial)` → `void`

**Métodos - Cálculos:**
- `ObtenerCostoTotalMaterialesTicket(int idTicket)` → `decimal`

---

## Archivos Removidos
- `LoginService.cs` → Consolidado en `UsuarioService.cs`
- `CrearUsuarioService.cs` → Consolidado en `UsuarioService.cs`

## Archivos Actualizados en HSis.UI
- `frmIniciarSesion.cs` → Usa `UsuarioService` (antes `LoginService`)
- `frmCrearUsuario.cs` → Usa `UsuarioService` (antes `CrearUsuarioService`)

## Beneficios de la Reorganización
✓ Mejor cohesión: todos los métodos relacionados a un modelo están juntos
✓ Mantenibilidad mejorada: fácil encontrar métodos por tipo de entidad
✓ Escalabilidad: estructura clara para nuevos servicios
✓ Standardización: nomenclatura consistente con verbos en español (Obtener, Crear, Actualizar, Eliminar)
