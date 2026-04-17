# 📐 Especificación Técnica: Sistema de Tickets HSis - Arquitectura Multi-Interfaz

## 1. Introducción

Este documento describe la arquitectura e implementación de un sistema de separación de interfaces para la gestión de tickets, aplicando el **Principio de Responsabilidad Única (SRP)** y **Control de Acceso Basado en Roles (RBAC)**.

---

## 2. Requisitos

### Funcionales (FR)
- FR1: Usuario puede crear ticket con descripción
- FR2: Cliente puede ver progreso de ticket en modo lectura
- FR3: Admin/Técnico pueden editar tickets con RBAC
- FR4: Sistema registra automáticamente fechas de Atención y Cierre
- FR5: Historial de cambios se mantiene de forma auditable

### No Funcionales (NFR)
- NFR1: Separación clara de responsabilidades (SRP)
- NFR2: Control de acceso por rol (RBAC)
- NFR3: UI responsiva y accesible
- NFR4: Performance: carga < 2 segundos

---

## 3. Arquitectura General

### 3.1 Componentes Principales

```
┌─────────────────────────────────────────────────────────┐
│                    PRESENTACIÓN (UI)                    │
├─────────────────────────────────────────────────────────┤
│  frmNuevoTicket    │  frmDetalleCliente  │  frmTicket   │
│  (Crear)           │  (Lectura)          │  (Editar)    │
└──────────┬──────────────┬───────────────────┬────────────┘
           │              │                   │
           └──────────────┼───────────────────┘
                          ↓
┌─────────────────────────────────────────────────────────┐
│                     LÓGICA NEGOCIO                       │
├─────────────────────────────────────────────────────────┤
│              TicketService (HSis.Logic)                  │
│  - CrearTicketAsync()                                    │
│  - ObtenerTicketPorIdAsync()                             │
│  - ActualizarTicketConHistorialAsync()                   │
│  - ObtenerTicketsPorUsuarioAsync()                       │
│  - ObtenerTicketsAsignadosATecnicoAsync()               │
│  - ObtenerHistorialPorTicketAsync()                      │
└───────────────────────┬─────────────────────────────────┘
                        ↓
┌─────────────────────────────────────────────────────────┐
│                 ACCESO A DATOS (EF Core)                │
├─────────────────────────────────────────────────────────┤
│  HSisDbContext                                           │
│  ├─ Tickets                                              │
│  ├─ Usuarios                                             │
│  ├─ HistorialCambiosTickets                             │
│  └─ DetTickets                                           │
└─────────────────────────────────────────────────────────┘
```

### 3.2 Flujo de Datos

```
CREAR TICKET:
Usuario → frmNuevoTicket → Ticket {
    IdUsuario = SesionSistema.IdUsuario,
    Status = "Abierto",
    Alta = DateTime.Now,
    Descripción = input
} → TicketService.CrearTicketAsync() → BD

LEER TICKET:
Usuario → frmDetalleCliente → TicketService.ObtenerTicketPorIdAsync()
                    ↓
        Mostrar datos (ReadOnly=true)

EDITAR TICKET:
Usuario → frmTicket → RBAC Check → TicketService.ActualizarTicketConHistorialAsync()
                         ↓
                 HistorialCambiosTicket INSERT
```

---

## 4. Formularios: Especificación Detallada

### 4.1 frmNuevoTicket.cs

#### Propósito
Formulario para crear nuevos tickets. Responsabilidad única: capturar descripción y guardar.

#### Ubicación
```
HSis.UI/frmNuevoTicket.cs
HSis.UI/frmNuevoTicket.Designer.cs
HSis.UI/frmNuevoTicket.resx
```

#### Componentes UI
```
┌────────────────────────────────────┐
│     Crear Nuevo Ticket             │
├────────────────────────────────────┤
│                                     │
│  Descripción del Problema:          │
│  ┌─────────────────────────────┐  │
│  │ [RichTextBox - rtbDescripcion] │
│  │ (Multiline, 150px height)     │
│  │                                 │
│  └─────────────────────────────┘  │
│                                     │
│  ┌──────────┐         ┌──────────┐ │
│  │ Guardar  │         │ Cancelar │ │
│  │ (Verde)  │         │ (Rojo)   │ │
│  └──────────┘         └──────────┘ │
│                                     │
└────────────────────────────────────┘
```

#### Propiedades de Control
| Control | Propiedad | Valor |
|---------|-----------|-------|
| Form | FormBorderStyle | FixedDialog |
| Form | MaximizeBox | false |
| Form | MinimizeBox | false |
| Form | StartPosition | CenterParent |
| Form | Size | 484, 250 |
| rtbDescripcion | Font | Segoe UI, 9pt |
| rtbDescripcion | Multiline | true |
| btnGuardar | BackColor | Green |
| btnGuardar | ForeColor | White |
| btnCancelar | BackColor | Red |
| btnCancelar | ForeColor | White |

#### Lógica (btnGuardar_Click)

```csharp
Validar:
  ├─ Si rtbDescripcion.Text vacío → Mostrar error y retornar
  └─ Si vacío es OK, continuar

Crear Ticket:
  ├─ IdUsuario = SesionSistema.IdUsuario
  ├─ Status = ConstantesEstatus.ABIERTO ("Abierto")
  ├─ Alta = DateTime.Now
  ├─ Descripción = rtbDescripcion.Text.Trim()
  └─ IdTecnico = null (sin asignar)

Guardar:
  ├─ await _ticketService.CrearTicketAsync(nuevoTicket)
  ├─ Mostrar: "Ticket creado exitosamente con Folio: {IdTicket}"
  ├─ DialogResult = OK
  └─ Close()
```

#### Manejo de Excepciones
```csharp
try { ... }
catch (Exception ex)
{
    MessageBox.Show($"Error al crear el ticket: {ex.Message}", 
        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
}
```

#### Return Values
- `DialogResult.OK`: Ticket creado exitosamente
- `DialogResult.Cancel`: Usuario canceló

---

### 4.2 frmDetalleCliente.cs

#### Propósito
Formulario de solo lectura para que clientes (Rol 3) vean el progreso de sus tickets.

#### Ubicación
```
HSis.UI/frmDetalleCliente.cs
HSis.UI/frmDetalleCliente.Designer.cs
HSis.UI/frmDetalleCliente.resx
```

#### Componentes UI
```
┌──────────────────────────────────────┐
│    Detalle del Ticket - Solo Lectura │
├──────────────────────────────────────┤
│                                       │
│  Folio:              123              │
│  Fecha Alta:         01/02/2024 10:30 │
│  Estatus:  [ En proceso ]  (AMARILLO) │
│  Técnico Asignado:  Juan Pérez        │
│                                       │
│  Descripción:                         │
│  ┌──────────────────────────────────┐ │
│  │ La impresora no funciona         │ │
│  │ desde las 9 de la mañana         │ │
│  └──────────────────────────────────┘ │
│                                       │
│  Solución:                            │
│  ┌──────────────────────────────────┐ │
│  │ Se reinstalaron los drivers      │ │
│  └──────────────────────────────────┘ │
│                                       │
│                        ┌──────────┐   │
│                        │  Cerrar  │   │
│                        └──────────┘   │
│                                       │
└──────────────────────────────────────┘
```

#### Propiedades de Control
| Control | Propiedad | Valor |
|---------|-----------|-------|
| txtDescripcion | ReadOnly | true |
| txtDescripcion | BackColor | ControlLight |
| txtDescripcion | Multiline | true |
| txtSolucion | ReadOnly | true |
| txtSolucion | BackColor | ControlLight |
| txtSolucion | Multiline | true |
| lblEstatusValor | Padding | 5px |
| lblEstatusValor | Font | Bold |

#### Lógica (Load event)

```csharp
Cargar Ticket:
  ├─ _ticketActual = await _ticketService.ObtenerTicketPorIdAsync(_idTicket)
  ├─ Si null → Mostrar error y Close()
  └─ Si existe → Continuar

Mostrar Datos:
  ├─ lblFolioValor.Text = _ticketActual.IdTicket
  ├─ lblFechaAltaValor.Text = _ticketActual.Alta?.ToString("dd/MM/yyyy HH:mm")
  ├─ lblEstatusValor.Text = _ticketActual.Status
  ├─ lblTecnicoValor.Text = _ticketActual.IdTecnicoNavigation?.Nombre ?? "Sin asignar"
  ├─ txtDescripcion.Text = _ticketActual.Descripción
  └─ txtSolucion.Text = _ticketActual.Solución

Aplicar Estilo:
  └─ AplicarEstiloEstatus(_ticketActual.Status)
```

#### Estilos de Estatus
```csharp
Si Status == "Abierto":
    lblEstatusValor.BackColor = LightCoral
    lblEstatusValor.ForeColor = DarkRed

Si Status == "En proceso":
    lblEstatusValor.BackColor = LightYellow
    lblEstatusValor.ForeColor = DarkGoldenrod

Si Status == "Cerrado":
    lblEstatusValor.BackColor = LightGreen
    lblEstatusValor.ForeColor = DarkGreen

Si Status == "Reabierto":
    lblEstatusValor.BackColor = LightBlue
    lblEstatusValor.ForeColor = DarkBlue
```

#### Características de Seguridad
- ✅ Todo ReadOnly
- ✅ Solo botón Cerrar
- ✅ No permite ediciones
- ✅ No permite cambios de estado
- ✅ No permite asignar técnico

---

### 4.3 frmTicket.cs (Refinamiento Existente)

#### Propósito
Formulario para editar tickets operativamente (Admin y Técnico).

#### RBAC Implementado

```csharp
// En Load event (línea 72-73)
if (SesionSistema.IdRolUsuario != 1) // 1 = Admin
{
    cmbAtendido.Enabled = false; // Solo Admin puede asignar técnicos
}
```

#### Flujo de Roles
```
┌─────────────────────────────┐
│ Usuario abre frmTicket      │
├─────────────────────────────┤
│                              │
├─ IdRolUsuario == 1 (Admin)  │ → cmbAtendido.Enabled = true
├─ IdRolUsuario == 2 (Técnico)│ → cmbAtendido.Enabled = false
└─ IdRolUsuario == 3 (Cliente)│ → NO ACCESO (no abrir este form)
```

#### Lógica de Fechas Automáticas (YA IMPLEMENTADA)

```csharp
// En btnGuardar_Click (línea 118-124)

// Cuando Status cambia a "En proceso"
if (_ticketActual.Status == ConstantesEstatus.EN_PROCESO 
    && _ticketActual.Atención == null)
{
    _ticketActual.Atención = DateTime.Now;
}

// Cuando Status cambia a "Cerrado"
else if (_ticketActual.Status == ConstantesEstatus.CERRADO 
    && _ticketActual.Cierre == null)
{
    _ticketActual.Cierre = DateTime.Now;
}
```

#### Actualización Transaccional
```csharp
await _ticketService.ActualizarTicketConHistorialAsync(
    _ticketActual, 
    SesionSistema.IdUsuario
);
```

---

## 5. Integración con Dashboards

### 5.1 Dashboard Admin (frmDashboardAdmin.cs)

```csharp
private void btnNuevoTicket_Click(object sender, EventArgs e)
{
    using (var form = new frmNuevoTicket())
    {
        if (form.ShowDialog() == DialogResult.OK)
        {
            CargarTodosTicketsAsync();
        }
    }
}

private void dgvTickets_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
{
    if (e.RowIndex < 0) return;

    int idTicket = (int)dgvTickets.Rows[e.RowIndex].Cells["IdTicket"].Value;

    using (var form = new frmTicket(idTicket))
    {
        if (form.ShowDialog() == DialogResult.OK)
        {
            CargarTodosTicketsAsync();
        }
    }
}

private async Task CargarTodosTicketsAsync()
{
    var tickets = await _ticketService.ObtenerTicketsAsync();
    dgvTickets.DataSource = tickets;
}
```

### 5.2 Dashboard Técnico (frmDashboardTecnico.cs)

```csharp
private void btnNuevoTicket_Click(object sender, EventArgs e)
{
    using (var form = new frmNuevoTicket())
    {
        if (form.ShowDialog() == DialogResult.OK)
        {
            CargarTicketsAsignadosAsync();
        }
    }
}

private void dgvTickets_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
{
    if (e.RowIndex < 0) return;

    int idTicket = (int)dgvTickets.Rows[e.RowIndex].Cells["IdTicket"].Value;

    using (var form = new frmTicket(idTicket))
    {
        if (form.ShowDialog() == DialogResult.OK)
        {
            CargarTicketsAsignadosAsync();
        }
    }
}

private async Task CargarTicketsAsignadosAsync()
{
    var tickets = await _ticketService
        .ObtenerTicketsAsignadosATecnicoAsync(SesionSistema.IdUsuario);
    dgvTickets.DataSource = tickets;
}
```

### 5.3 Dashboard Cliente (frmDashboardCliente.cs)

```csharp
private void btnNuevoTicket_Click(object sender, EventArgs e)
{
    using (var form = new frmNuevoTicket())
    {
        if (form.ShowDialog() == DialogResult.OK)
        {
            CargarTicketsDelClienteAsync();
        }
    }
}

private void dgvTickets_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
{
    if (e.RowIndex < 0) return;

    int idTicket = (int)dgvTickets.Rows[e.RowIndex].Cells["IdTicket"].Value;

    using (var form = new frmDetalleCliente(idTicket))
    {
        form.ShowDialog();
    }
}

private async Task CargarTicketsDelClienteAsync()
{
    var tickets = await _ticketService
        .ObtenerTicketsPorUsuarioAsync(SesionSistema.IdUsuario);
    dgvTickets.DataSource = tickets;
}
```

---

## 6. Modelo de Datos

### 6.1 Entidades Involucradas

```
Ticket
├─ IdTicket (PK)
├─ IdUsuario (FK → Usuario)
├─ IdTecnico (FK → Usuario, nullable)
├─ Alta (DateTime)
├─ Atención (DateTime, nullable)
├─ Cierre (DateTime, nullable)
├─ Status (string)
├─ Descripción (string)
├─ Solución (string)
└─ Relaciones:
   ├─ IdUsuarioNavigation → Usuario (cliente)
   ├─ IdTecnicoNavigation → Usuario (técnico)
   └─ HistorialCambiosTickets → Colección

HistorialCambiosTicket
├─ IdHistorial (PK)
├─ IdTicket (FK)
├─ CampoModificado (string)
├─ ValorAnterior (string)
├─ ValorNuevo (string)
├─ IdUsuarioCambio (FK → Usuario)
├─ FechaMovimiento (DateTime)
└─ Relaciones:
   ├─ Ticket → Ticket
   └─ IdUsuarioCambioNavigation → Usuario

Usuario
├─ IdUsuario (PK)
├─ Nombre (string)
├─ IdRol (FK)
└─ Relaciones:
   └─ IdRolNavigation → Rol
```

### 6.2 Estados de Ticket (ConstantesEstatus)

```csharp
public const string ABIERTO = "Abierto";
public const string EN_PROCESO = "En proceso";
public const string CERRADO = "Cerrado";
public const string REABIERTO = "Reabierto";
```

### 6.3 Roles de Usuario (SesionSistema)

```csharp
public static int IdRolUsuario { get; set; }

1 = Administrador (Acceso completo)
2 = Técnico (Acceso limitado)
3 = Cliente (Solo lectura de propios tickets)
```

---

## 7. Flujos de Casos de Uso

### 7.1 Crear Ticket (Todos los roles)

```
Actor: Usuario (Rol 1, 2, o 3)
Precondición: Usuario iniciado de sesión
Flujo Principal:
  1. Usuario hace click en "Nuevo Ticket"
  2. Sistema abre frmNuevoTicket
  3. Usuario ingresa descripción del problema
  4. Usuario hace click "Guardar"
  5. Sistema valida descripción no vacía
  6. Sistema crea Ticket {
       IdUsuario = SesionSistema.IdUsuario,
       Status = "Abierto",
       Alta = NOW,
       Descripción = input
     }
  7. Sistema llama CrearTicketAsync()
  8. Sistema retorna folio generado
  9. Sistema muestra "Ticket creado exitosamente con Folio: XXX"
  10. Sistema cierra frmNuevoTicket con DialogResult.OK
Postcondición: Ticket guardado en BD, visible en grilla
```

### 7.2 Ver Detalle (Cliente - Rol 3)

```
Actor: Cliente
Precondición: Cliente tiene tickets
Flujo Principal:
  1. Cliente abre su dashboard
  2. Cliente ve lista de sus tickets
  3. Cliente hace doble-click en un ticket
  4. Sistema abre frmDetalleCliente
  5. Sistema carga ticket por ID
  6. Sistema muestra: Folio, Fecha Alta, Estatus, Técnico, Descripción, Solución
  7. Sistema aplica color según estatus
  8. Cliente VE (modo lectura):
       - Amarillo: En proceso
       - Verde: Cerrado
       - Rojo: Abierto
  9. Cliente solo puede cerrar la ventana
Postcondición: frmDetalleCliente cerrada
```

### 7.3 Editar Ticket (Admin - Rol 1)

```
Actor: Admin
Precondición: Admin tiene tickets en su vista
Flujo Principal:
  1. Admin abre frmTicket
  2. Sistema verifica: SesionSistema.IdRolUsuario == 1 ✓
  3. cmbAtendido está HABILITADO
  4. Admin puede:
       - Cambiar Status
       - Cambiar Técnico asignado
       - Escribir Solución
  5. Admin hace click "Guardar"
  6. Sistema valida cambios
  7. Si Status = "En proceso" y Atención es null → Atención = NOW
  8. Si Status = "Cerrado" y Cierre es null → Cierre = NOW
  9. Sistema llama ActualizarTicketConHistorialAsync()
  10. HistorialCambiosTicket registra cambios
  11. Sistema cierra frmTicket con DialogResult.OK
Postcondición: Cambios guardados, historial actualizado
```

### 7.4 Editar Ticket (Técnico - Rol 2)

```
Actor: Técnico
Precondición: Técnico tiene tickets asignados
Flujo Principal:
  1. Técnico abre frmTicket
  2. Sistema verifica: SesionSistema.IdRolUsuario == 2 → != 1 ✓
  3. cmbAtendido está DESHABILITADO (gris)
  4. Técnico puede:
       - Cambiar Status
       - Escribir Solución
       - VER Técnico asignado (pero no cambiar)
  5. Técnico hace click "Guardar"
  6. Sistema valida cambios
  7. Si Status = "En proceso" y Atención es null → Atención = NOW
  8. Si Status = "Cerrado" y Cierre es null → Cierre = NOW
  9. Sistema llama ActualizarTicketConHistorialAsync()
  10. HistorialCambiosTicket registra cambios
  11. Sistema cierra frmTicket con DialogResult.OK
Postcondición: Cambios guardados, RBAC respetado
```

---

## 8. Seguridad y Validaciones

### 8.1 Validaciones Cliente-lado

```
frmNuevoTicket:
  ✓ Descripción no vacía
  ✓ Trim() de espacios en blanco

frmDetalleCliente:
  ✓ Verificar que ticket existe antes de mostrar
  ✓ Todos los TextBox ReadOnly = true

frmTicket:
  ✓ Ticket no null
  ✓ cmbAtendido deshabilitado si Rol != 1
```

### 8.2 Validaciones Servidor-lado (Recomendado)

```csharp
// En TicketService.cs (para futuro)
public async Task ActualizarTicketSeguroAsync(
    Ticket ticket,
    int idUsuarioModifica,
    int rolUsuario)
{
    // Validar rol
    if (rolUsuario != 1 && rolUsuario != 2)
        throw new UnauthorizedAccessException("No tienes permiso.");

    // Si es técnico, validar que sea su ticket
    if (rolUsuario == 2)
    {
        var original = await db.Tickets
            .FirstOrDefaultAsync(t => t.IdTicket == ticket.IdTicket);

        if (original.IdTecnico != idUsuarioModifica)
            throw new UnauthorizedAccessException("No es tu ticket.");
    }

    await ActualizarTicketConHistorialAsync(ticket, idUsuarioModifica);
}
```

---

## 9. Testing

### 9.1 Unit Tests (Recomendado)

```csharp
[TestClass]
public class TicketServiceTests
{
    [TestMethod]
    public async Task CrearTicket_DebeAsignarIdUsuario()
    {
        // Arrange
        var ticket = new Ticket { Descripción = "Test" };

        // Act
        var resultado = await _service.CrearTicketAsync(ticket);

        // Assert
        Assert.IsTrue(resultado.IdTicket > 0);
    }
}
```

### 9.2 Integration Tests

```
1. Crear ticket como Cliente
   - Verificar IdUsuario asignado correctamente
   - Verificar Status = "Abierto"
   - Verificar Alta = DateTime.Now (±1 segundo)

2. Ver detalle como Cliente
   - Verificar frmDetalleCliente carga correctamente
   - Verificar campos en lectura
   - Verificar colores según estatus

3. Editar ticket como Admin
   - Verificar cmbAtendido habilitado
   - Cambiar técnico
   - Verificar historial registrado

4. Editar ticket como Técnico
   - Verificar cmbAtendido deshabilitado
   - Cambiar status a "En proceso"
   - Verificar Atención = NOW
```

---

## 10. Deployment

### 10.1 Pre-requisitos
- [ ] Proyecto compila sin errores
- [ ] Base de datos actualizada (HistorialCambiosTicket debe existir)
- [ ] SesionSistema.IdUsuario se asigna en login
- [ ] ConstantesEstatus definidas correctamente

### 10.2 Procedimiento
1. Compilar solución: `dotnet build`
2. Ejecutar tests: `dotnet test`
3. Deploy a producción
4. Actualizar documentación de usuarios

---

## 11. Performance

### 11.1 Métricas Objetivo
- Crear ticket: < 500ms
- Cargar detalle: < 1s
- Editar ticket: < 1s
- Cargar historial: < 1s

### 11.2 Optimizaciones Implementadas
- ✓ Include() para relaciones (evitar N+1)
- ✓ AsNoTracking() donde sea posible
- ✓ Índices en BD (FK, Status)

---

## 12. Mantenibilidad

### 12.1 Principios Aplicados
- **SRP**: Cada formulario tiene responsabilidad única
- **RBAC**: Control de acceso por rol
- **DRY**: No repetir lógica
- **KISS**: Código simple y entendible

### 12.2 Convenciones
- Métodos async terminan en `Async`
- Validaciones al inicio de métodos
- Manejo de excepciones centralizado
- Comentarios XML en métodos públicos

---

## 13. Referencias

- **Proyecto**: HSis (C# WinForms)
- **Framework**: .NET 10
- **ORM**: Entity Framework Core
- **Base de Datos**: SQL Server

---

**Documento de Especificación Técnica**
**Versión**: 1.0
**Fecha**: 2024
**Estado**: ✅ Aprobado
