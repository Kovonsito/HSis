# GUÍA TÉCNICA - DASHBOARDS POR ROL

## 📚 ÍNDICE
1. [Estructura de Clases](#estructura-de-clases)
2. [Flujo de Datos](#flujo-de-datos)
3. [Ejemplos de Uso](#ejemplos-de-uso)
4. [Extensión del Sistema](#extensión-del-sistema)

---

## Estructura de Clases

### DTO: TicketClienteDto
```csharp
public class TicketClienteDto
{
    public int IdTicket { get; set; }
    public string Folio => $"TK-{IdTicket:D6}";           // TK-000001
    public DateTime? FechaAlta { get; set; }              // Cuando se creó
    public string Status { get; set; }                   // Abierto/Cerrado/etc
    public string TecnicoAsignado { get; set; }          // Nombre del técnico
    public string Descripcion { get; set; }              // Resumen 50 caracteres
}
```

### DTO: TicketOperativoDto
```csharp
public class TicketOperativoDto
{
    public int IdTicket { get; set; }
    public string Folio => $"TK-{IdTicket:D6}";           // TK-000001
    public DateTime? FechaAlta { get; set; }              // Cuando se creó
    public string Status { get; set; }                   // Estado actual
    public string Usuario { get; set; }                  // Quién reportó
    public string Descripcion { get; set; }              // Resumen 50 caracteres
}
```

### SesionSistema (Existente)
```csharp
public static class SesionSistema
{
    public static int IdUsuario { get; set; }            // ID del usuario logueado
    public static string NombreUsuario { get; set; }     // Nombre completo
    public static int IdRolUsuario { get; set; }         // 1=Admin, 2=Técnico, 3=Cliente
}
```

### TicketService (Extensiones)
```csharp
// Obtiene tickets creados por un usuario específico
public async Task<List<Ticket>> ObtenerTicketsPorUsuarioAsync(int idUsuario)

// Obtiene tickets asignados a un técnico (que no están cerrados)
public async Task<List<Ticket>> ObtenerTicketsAsignadosATecnicoAsync(int idTecnico)

// Obtiene tickets disponibles para que técnicos los asignen
public async Task<List<Ticket>> ObtenerTicketsDisponiblesAsync()

// Crea un nuevo ticket
public async Task<Ticket> CrearTicketAsync(Ticket ticket)
```

---

## Flujo de Datos

### Flujo 1: Inicio de Sesión y Enrutamiento
```
┌─────────────────────────┐
│  frmIniciarSesion       │
│  Usuario ingresa datos  │
└────────┬────────────────┘
         │
         ├─→ UsuarioService.AutenticarAsync()
         │
         ├─→ SesionSistema.IdRolUsuario = usuario.IdRol
         │
         └─→ Switch(IdRolUsuario)
             ├─→ 1: new frmDashboardAdmin()
             ├─→ 2: new frmDashboardTecnico()
             └─→ 3: new frmDashboardCliente()
```

### Flujo 2: Cliente Crea Nuevo Reporte
```
┌─────────────────────────────┐
│  frmDashboardCliente        │
│  Click: btnNuevoReporte     │
└────────┬────────────────────┘
         │
         ├─→ new frmNuevoReporte()
         │   ShowDialog()
         │
         ├─→ Usuario escribe descripción
         │
         ├─→ Click: btnGuardar
         │   ├─→ Validar descripción
         │   ├─→ new Ticket()
         │   ├─→ IdUsuario = SesionSistema.IdUsuario
         │   ├─→ Status = ConstantesEstatus.ABIERTO
         │   ├─→ Alta = DateTime.Now
         │   └─→ Descripción = rtbDescripcion.Text
         │
         ├─→ TicketService.CrearTicketAsync(ticket)
         │
         ├─→ DialogResult = OK
         │
         └─→ frmDashboardCliente.CargarTicketsAsync()
             Recarga el DataGridView
```

### Flujo 3: Técnico Visualiza Tickets Asignados
```
┌──────────────────────────┐
│  frmDashboardTecnico     │
│  Load Event              │
└────────┬─────────────────┘
         │
         ├─→ CargarIndicadoresAsync()
         │   ├─→ ObtenerTicketsAsignadosATecnicoAsync(IdUsuario)
         │   └─→ ObtenerTicketsDisponiblesAsync()
         │
         ├─→ Mostrar:
         │   ├─→ ucMisAsignados: Cantidad = 3
         │   └─→ ucDisponibles: Cantidad = 5
         │
         └─→ Mostrar:
             dgvTicketsOperativos vacío
             (aguarda click en indicador)

         Click en ucMisAsignados
         │
         ├─→ CargarTicketsMisAsignadosAsync()
         │   └─→ ObtenerTicketsAsignadosATecnicoAsync(IdUsuario)
         │
         └─→ dgvTicketsOperativos actualiza
             con los 3 tickets asignados
```

### Flujo 4: Técnico Edita un Ticket
```
┌─────────────────────────────┐
│  dgvTicketsOperativos       │
│  CellDoubleClick: row       │
└────────┬────────────────────┘
         │
         ├─→ Extraer IdTicket
         │
         ├─→ new frmTicket(idTicket)
         │   ShowDialog()
         │
         ├─→ Usuario edita ticket
         │   (cambios, solución, etc)
         │
         ├─→ Click: btnGuardar
         │   ├─→ TicketService.ActualizarTicketConHistorialAsync()
         │   └─→ Registra cambios en historial
         │
         ├─→ frmTicket.Close()
         │
         └─→ frmDashboardTecnico
             ├─→ CargarIndicadoresAsync()
             │   (actualiza conteos)
             │
             └─→ CargarTicketsMisAsignadosAsync()
                 (recarga grid)
```

---

## Ejemplos de Uso

### Ejemplo 1: Cliente Creando un Ticket
```csharp
// En frmNuevoReporte.btnGuardar_Click
private async void btnGuardar_Click(object sender, EventArgs e)
{
    var nuevoTicket = new Ticket
    {
        IdUsuario = SesionSistema.IdUsuario,          // ID del cliente logueado
        Descripción = rtbDescripcion.Text,            // Lo que escribió
        Status = ConstantesEstatus.ABIERTO,           // Estado inicial
        Alta = DateTime.Now                           // Fecha/hora actual
    };

    // Guardar en BD
    await _ticketService.CrearTicketAsync(nuevoTicket);

    MessageBox.Show("Reporte creado exitosamente");
    this.DialogResult = DialogResult.OK;
}
```

### Ejemplo 2: Técnico Visualizando sus Tickets
```csharp
// En frmDashboardTecnico.CargarTicketsMisAsignadosAsync
private async Task CargarTicketsMisAsignadosAsync()
{
    // Obtiene TODOS los tickets asignados a este técnico
    var tickets = await _ticketService
        .ObtenerTicketsAsignadosATecnicoAsync(SesionSistema.IdUsuario);

    // Convierte a DTO para mostrar en grid
    var ticketsDto = tickets.ConvertAll(t => new TicketOperativoDto
    {
        IdTicket = t.IdTicket,
        FechaAlta = t.Alta,
        Status = t.Status,
        Usuario = t.IdUsuarioNavigation?.Nombre ?? "Desconocido",
        Descripcion = t.Descripción?.Length > 50 
            ? t.Descripción.Substring(0, 50) + "..." 
            : t.Descripción
    });

    dgvTicketsOperativos.DataSource = ticketsDto;
}
```

### Ejemplo 3: Mostrar Tickets Disponibles
```csharp
// En frmDashboardTecnico.CargarTicketsDisponiblesAsync
private async Task CargarTicketsDisponiblesAsync()
{
    // Obtiene tickets abiertos SIN técnico asignado
    var tickets = await _ticketService
        .ObtenerTicketsDisponiblesAsync();

    // Actualizar indicador
    ucDisponibles.Cantidad = tickets.Count.ToString();

    // Convertir y mostrar
    var ticketsDto = tickets.ConvertAll(/* ... */);
    dgvTicketsOperativos.DataSource = ticketsDto;
}
```

### Ejemplo 4: Flujo Completo del Cliente
```csharp
// Usuario login → Rol 3 (Cliente) → frmDashboardCliente abre

// Load del dashboard
private async void frmDashboardCliente_Load(object sender, EventArgs e)
{
    // 1. Cargar mis tickets
    await CargarTicketsAsync();

    // 2. Actualizar indicador
    await ActualizarIndicadorAsync();
}

// Cargar tickets del usuario
private async Task CargarTicketsAsync()
{
    var tickets = await _ticketService
        .ObtenerTicketsPorUsuarioAsync(SesionSistema.IdUsuario);

    // Mostrar en grid
    // ...
}

// Actualizar el indicador visual
private async Task ActualizarIndicadorAsync()
{
    var tickets = await _ticketService
        .ObtenerTicketsPorUsuarioAsync(SesionSistema.IdUsuario);

    var activos = tickets.FindAll(t => t.Status != ConstantesEstatus.CERRADO);

    ucMisActivos.Cantidad = activos.Count.ToString();
    ucMisActivos.Titulo = "Mis Tickets Activos";
    ucMisActivos.ColorFondo = Color.FromArgb(41, 128, 185);
}
```

---

## Extensión del Sistema

### Agregar un Nuevo Rol (Ej: Supervisor)

**Paso 1: Agregar método en TicketService**
```csharp
public async Task<List<Ticket>> ObtenerTicketsPorDepartamentoAsync(int idDepartamento)
{
    using var db = new HSisDbContext();
    return await db.Tickets
        .Include(t => t.IdUsuarioNavigation)
        .Include(t => t.IdTecnicoNavigation)
        .Where(t => t.IdUsuarioNavigation.IdDepartamento == idDepartamento)
        .ToListAsync();
}
```

**Paso 2: Agregar en switch de login**
```csharp
Form dashboardForm = SesionSistema.IdRolUsuario switch
{
    1 => new frmDashboardAdmin(),
    2 => new frmDashboardTecnico(),
    3 => new frmDashboardCliente(),
    4 => new frmDashboardSupervisor(),  // NUEVO
    _ => new frmDashboardAdmin()
};
```

**Paso 3: Crear frmDashboardSupervisor**
```csharp
public partial class frmDashboardSupervisor : Form
{
    private readonly TicketService _ticketService;

    public frmDashboardSupervisor()
    {
        InitializeComponent();
        _ticketService = new TicketService();
    }

    private async void frmDashboardSupervisor_Load(object sender, EventArgs e)
    {
        var tickets = await _ticketService
            .ObtenerTicketsPorDepartamentoAsync(SesionSistema.IdDepartamento);

        // Mostrar en grid...
    }
}
```

### Agregar Filtros Adicionales
```csharp
// En TicketService, agregar método con múltiples filtros
public async Task<List<Ticket>> ObtenerTicketsFiltradosAsync(
    int? idUsuario = null,
    int? idTecnico = null,
    string status = null,
    DateTime? fechaDesde = null,
    DateTime? fechaHasta = null)
{
    using var db = new HSisDbContext();

    var query = db.Tickets.AsQueryable();

    if (idUsuario.HasValue)
        query = query.Where(t => t.IdUsuario == idUsuario.Value);

    if (idTecnico.HasValue)
        query = query.Where(t => t.IdTecnico == idTecnico.Value);

    if (!string.IsNullOrEmpty(status))
        query = query.Where(t => t.Status == status);

    if (fechaDesde.HasValue)
        query = query.Where(t => t.Alta >= fechaDesde.Value);

    if (fechaHasta.HasValue)
        query = query.Where(t => t.Alta <= fechaHasta.Value);

    return await query
        .Include(t => t.IdUsuarioNavigation)
        .Include(t => t.IdTecnicoNavigation)
        .OrderByDescending(t => t.Alta)
        .ToListAsync();
}
```

---

## Notas Importantes

⚠️ **Seguridad:**
- Los datos se filtran por `SesionSistema.IdUsuario` (lado servidor)
- No se debe confiar solo en filtros de UI

⚠️ **Rendimiento:**
- Todos los métodos incluyen `.ToListAsync()` para ejecutar en BD
- No traer datos innecesarios (usar `.AsNoTracking()` si solo es lectura)

⚠️ **Transacciones:**
- `ActualizarTicketConHistorialAsync` usa transacción explícita
- Asegura que cambios + historial se guardan juntos o no se guardan

✅ **Buenas Prácticas:**
- DTOs para separar datos de presentación de modelos
- Async/await en todas las operaciones BD
- Validaciones antes de guardar
- Mensajes de error al usuario

