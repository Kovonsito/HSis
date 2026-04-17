# ARQUITECTURA DEL SISTEMA - DASHBOARDS POR ROL

## 🏗️ DIAGRAMA GENERAL

```
┌─────────────────────────────────────────────────────────────────┐
│                     APLICACIÓN HSIS                              │
├─────────────────────────────────────────────────────────────────┤
│                                                                   │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │            CAPA DE PRESENTACIÓN (HSis.UI)              │   │
│  ├─────────────────────────────────────────────────────────┤   │
│  │                                                         │   │
│  │  ┌──────────────────────────────────────────────────┐ │   │
│  │  │        frmIniciarSesion                          │ │   │
│  │  │  • Autenticación usuario                         │ │   │
│  │  │  • Llena SesionSistema                           │ │   │
│  │  │  • Switch por IdRolUsuario                       │ │   │
│  │  └──────────────────────────────────────────────────┘ │   │
│  │                    ↓                                  │   │
│  │  ┌──────────────────────────────────────────────────┐ │   │
│  │  │     Dashboard Router (Switch en Login)           │ │   │
│  │  │  IdRol = 1 → frmDashboardAdmin (Existente)      │ │   │
│  │  │  IdRol = 2 → frmDashboardTecnico (Nuevo)        │ │   │
│  │  │  IdRol = 3 → frmDashboardCliente (Nuevo)        │ │   │
│  │  └──────────────────────────────────────────────────┘ │   │
│  │          ↙              ↓              ↘              │   │
│  │                                                         │   │
│  │ ┌─────────────────────┐  ┌──────────────────────────┐ │   │
│  │ │frmDashboardTecnico  │  │frmDashboardCliente       │ │   │
│  │ ├─────────────────────┤  ├──────────────────────────┤ │   │
│  │ │ • 2 Indicadores     │  │ • 1 Indicador            │ │   │
│  │ │   (Asignados/Disp)  │  │   (Mis Activos)          │ │   │
│  │ │ • Grid Operativo    │  │ • Grid de Mis Tickets    │ │   │
│  │ │ • Clic en indicador │  │ • Botón Nuevo Reporte    │ │   │
│  │ │ • Double-click tick.│  │ • Double-click ticket    │ │   │
│  │ │ • Recarga después   │  │ • Recarga después        │ │   │
│  │ │   de editar         │  │   de guardar             │ │   │
│  │ └─────────────────────┘  └──────────────────────────┘ │   │
│  │          ↓                              ↓               │   │
│  │ ┌─────────────────────┐  ┌──────────────────────────┐ │   │
│  │ │ frmTicket (Existente)   frmNuevoReporte (Nuevo)    │ │   │
│  │ ├─────────────────────┤  ├──────────────────────────┤ │   │
│  │ │ • Edita ticket      │  │ • Captura descripción    │ │   │
│  │ │ • Registra historial│  │ • Crea Ticket            │ │   │
│  │ │ • Guarda cambios    │  │ • Retorna DialogResult   │ │   │
│  │ └─────────────────────┘  └──────────────────────────┘ │   │
│  │                                                         │   │
│  │  ucIndicador (Control Reutilizado)                    │   │
│  │  • Muestra título + cantidad                          │   │
│  │  • Configurable: color, títulos                       │   │
│  │  • Dispara eventos al click                           │   │
│  │                                                         │   │
│  └─────────────────────────────────────────────────────────┘   │
│                                                                   │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │        CAPA DE LÓGICA (HSis.Logic)                     │   │
│  ├─────────────────────────────────────────────────────────┤   │
│  │                                                         │   │
│  │  TicketService                                         │   │
│  │  ────────────────────────────────────────────────────  │   │
│  │  MÉTODOS EXISTENTES:                                  │   │
│  │  • ObtenerTicketsAsync()                              │   │
│  │  • ObtenerTicketPorIdAsync()                           │   │
│  │  • ObtenerTicketsPorSLAAsync()                         │   │
│  │  • ObtenerTicketsPorEstatusAsync()                     │   │
│  │  • ActualizarTicketConHistorialAsync()                │   │
│  │                                                         │   │
│  │  NUEVOS MÉTODOS (Filtrados por Rol):                  │   │
│  │  • ObtenerTicketsPorUsuarioAsync(idUsuario)            │   │
│  │    └─ Retorna tickets creados por el usuario          │   │
│  │                                                         │   │
│  │  • ObtenerTicketsAsignadosATecnicoAsync(idTecnico)     │   │
│  │    └─ Retorna tickets asignados al técnico            │   │
│  │                                                         │   │
│  │  • ObtenerTicketsDisponiblesAsync()                    │   │
│  │    └─ Retorna abiertos sin asignar                    │   │
│  │                                                         │   │
│  │  • CrearTicketAsync(ticket)                            │   │
│  │    └─ Crea nuevo ticket en BD                          │   │
│  │                                                         │   │
│  │  SesionSistema (Static Class)                          │   │
│  │  ────────────────────────────────────────────────────  │   │
│  │  • IdUsuario (int)                                    │   │
│  │  • NombreUsuario (string)                             │   │
│  │  • IdRolUsuario (int) → 1=Admin, 2=Técnico, 3=Client │   │
│  │                                                         │   │
│  │  ConstantesEstatus                                     │   │
│  │  ────────────────────────────────────────────────────  │   │
│  │  • ABIERTO = "Abierto"                                │   │
│  │  • EN_PROCESO = "En proceso"                          │   │
│  │  • CERRADO = "Cerrado"                                │   │
│  │  • REABIERTO = "Reabierto"                            │   │
│  │                                                         │   │
│  │  DTOs (Data Transfer Objects)                          │   │
│  │  ────────────────────────────────────────────────────  │   │
│  │  • TicketClienteDto - Para vista de cliente            │   │
│  │  • TicketOperativoDto - Para vista de técnico         │   │
│  │                                                         │   │
│  │  UsuarioService                                        │   │
│  │  ────────────────────────────────────────────────────  │   │
│  │  • AutenticarAsync(usuario, contraseña)                │   │
│  │                                                         │   │
│  └─────────────────────────────────────────────────────────┘   │
│                                                                   │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │          CAPA DE DATOS (HSis.Data)                     │   │
│  ├─────────────────────────────────────────────────────────┤   │
│  │                                                         │   │
│  │  Entity Framework Core                                │   │
│  │  ├─ HSisDbContext (DbContext)                          │   │
│  │  │  ├─ DbSet<Ticket>                                  │   │
│  │  │  ├─ DbSet<Usuario>                                 │   │
│  │  │  ├─ DbSet<HistorialCambiosTicket>                  │   │
│  │  │  └─ DbSet<RolUsuario>                              │   │
│  │  │                                                     │   │
│  │  │  Modelos:                                           │   │
│  │  │  ├─ Ticket                                          │   │
│  │  │  │  ├─ IdTicket                                    │   │
│  │  │  │  ├─ IdUsuario                                   │   │
│  │  │  │  ├─ IdTecnico (nullable)                        │   │
│  │  │  │  ├─ Status                                      │   │
│  │  │  │  ├─ Descripción                                 │   │
│  │  │  │  ├─ Solución                                    │   │
│  │  │  │  ├─ Alta                                        │   │
│  │  │  │  ├─ Atención                                    │   │
│  │  │  │  ├─ Cierre                                      │   │
│  │  │  │  ├─ IdUsuarioNavigation (Foreign Key)           │   │
│  │  │  │  └─ IdTecnicoNavigation (Foreign Key)           │   │
│  │  │  │                                                 │   │
│  │  │  ├─ Usuario                                        │   │
│  │  │  │  ├─ IdUsuario                                   │   │
│  │  │  │  ├─ Nombre                                      │   │
│  │  │  │  ├─ IdRol                                       │   │
│  │  │  │  ├─ Contraseña                                  │   │
│  │  │  │  └─ IdRolNavigation (Foreign Key)               │   │
│  │  │  │                                                 │   │
│  │  │  ├─ RolUsuario                                     │   │
│  │  │  │  ├─ IdRol                                       │   │
│  │  │  │  └─ NombreRol                                   │   │
│  │  │  │                                                 │   │
│  │  │  └─ HistorialCambiosTicket                         │   │
│  │  │     ├─ IdHistorial                                 │   │
│  │  │     ├─ IdTicket                                    │   │
│  │  │     ├─ CampoModificado                             │   │
│  │  │     ├─ ValorAnterior                               │   │
│  │  │     ├─ ValorNuevo                                  │   │
│  │  │     └─ FechaMovimiento                             │   │
│  │  │                                                     │   │
│  │  SQL Server Database                                 │   │
│  │  ├─ Tabla: Tickets                                    │   │
│  │  ├─ Tabla: Usuarios                                   │   │
│  │  ├─ Tabla: RolesUsuario                               │   │
│  │  └─ Tabla: HistorialCambiosTickets                    │   │
│  │                                                         │   │
│  └─────────────────────────────────────────────────────────┘   │
│                                                                   │
└─────────────────────────────────────────────────────────────────┘
```

---

## 🔄 FLUJO DE DATOS

### Flujo 1: Autenticación y Enrutamiento
```
Usuario Input
    ↓
frmIniciarSesion
    ↓
UsuarioService.AutenticarAsync()
    ↓
Validar en BD (Usuarios)
    ↓
SesionSistema ← Llenar IdUsuario, NombreUsuario, IdRolUsuario
    ↓
Switch(IdRolUsuario)
    ├─→ 1: frmDashboardAdmin()
    ├─→ 2: frmDashboardTecnico()
    └─→ 3: frmDashboardCliente()
```

### Flujo 2: Cliente - Crear Ticket
```
Usuario escribir descripción
    ↓
btnNuevoReporte_Click
    ↓
frmNuevoReporte (ShowDialog)
    ↓
Usuario click "Guardar"
    ↓
Validar descripción != null
    ↓
new Ticket()
    IdUsuario = SesionSistema.IdUsuario
    Status = "Abierto"
    Alta = Now
    Descripción = rtb.Text
    ↓
TicketService.CrearTicketAsync(ticket)
    ↓
db.Tickets.Add() + SaveChanges()
    ↓
INSERT INTO Tickets (...)
    ↓
DialogResult = OK
    ↓
frmDashboardCliente.CargarTicketsAsync()
    ↓
TicketService.ObtenerTicketsPorUsuarioAsync(IdUsuario)
    ↓
SELECT * FROM Tickets WHERE IdUsuario = X
    ↓
DTO Mapping + Bind to Grid
```

### Flujo 3: Técnico - Visualizar Asignados
```
frmDashboardTecnico_Load
    ↓
CargarIndicadoresAsync()
    ├─→ ObtenerTicketsAsignadosATecnicoAsync(IdUsuario)
    │   ↓
    │   SELECT * FROM Tickets 
    │   WHERE IdTecnico = X AND Status != 'Cerrado'
    │
    └─→ ObtenerTicketsDisponiblesAsync()
        ↓
        SELECT * FROM Tickets 
        WHERE Status = 'Abierto' AND IdTecnico IS NULL
    ↓
ucMisAsignados.Cantidad = count
ucDisponibles.Cantidad = count
    ↓
Grid vacío (aguarda click)
    ↓
Usuario click ucMisAsignados
    ↓
CargarTicketsMisAsignadosAsync()
    ↓
DTO Mapping + Bind to Grid
```

### Flujo 4: Técnico - Editar Ticket
```
dgvTicketsOperativos_CellDoubleClick
    ↓
Extraer IdTicket de la fila
    ↓
new frmTicket(IdTicket)
    ↓
frmTicket_Load
    ├─→ ObtenerTicketPorIdAsync(IdTicket)
    └─→ Mostrar datos
    ↓
Usuario edita campos
    ↓
btnGuardar_Click
    ↓
ActualizarTicketConHistorialAsync(ticket, idUsuario)
    ├─→ BEGIN TRANSACTION
    ├─→ Comparar original vs nuevo
    ├─→ Registrar cambios en Historial
    ├─→ UPDATE Tickets
    ├─→ COMMIT
    └─→ Fin
    ↓
frmTicket.Close()
    ↓
frmDashboardTecnico
    ├─→ CargarIndicadoresAsync()
    └─→ CargarTicketsAsync() (grid actualiza)
```

---

## 📊 COMPONENTES REUTILIZABLES

### ucIndicador (Control Reutilizado)
```csharp
Properties:
├─ Titulo (string) → lblTitulo.Text
├─ Cantidad (string) → lblCantidad.Text
├─ ColorFondo (Color) → pnlPrincipal.BackColor
└─ ImagenFondo (Image) → pbxIcono.Image

Events:
└─ ucIndicadorEvent → Disparado en Click

Ubicaciones:
├─ frmDashboardTecnico
│  ├─ ucMisAsignados (Azul)
│  └─ ucDisponibles (Amarillo)
└─ frmDashboardCliente
   └─ ucMisActivos (Azul)
```

### DTOs (Data Transfer Objects)
```csharp
TicketClienteDto
├─ IdTicket
├─ Folio (computed: "TK-000001")
├─ FechaAlta
├─ Status
├─ TecnicoAsignado
└─ Descripcion (max 50 chars)

TicketOperativoDto
├─ IdTicket
├─ Folio (computed: "TK-000001")
├─ FechaAlta
├─ Status
├─ Usuario
└─ Descripcion (max 50 chars)
```

---

## 🔐 CONTROL DE ACCESO

```
Rol 1 (Admin)
└─ Acceso: frmDashboardAdmin (todos los tickets)

Rol 2 (Técnico)
├─ Puede ver:
│  ├─ Sus propios tickets asignados
│  └─ Tickets disponibles para asignar
├─ Acciones:
│  ├─ Editar tickets asignados
│  ├─ Registrar soluciones
│  └─ Cambiar estado

Rol 3 (Cliente)
├─ Puede ver:
│  ├─ Sus propios tickets
│  └─ Estado de reportes
├─ Acciones:
│  ├─ Crear nuevos reportes
│  └─ Ver historial de cambios
```

---

## 🗄️ ESQUEMA DE BD RELEVANTE

```sql
-- Tickets: El núcleo del sistema
CREATE TABLE Tickets (
    IdTicket INT PRIMARY KEY IDENTITY,
    IdUsuario INT NOT NULL REFERENCES Usuarios(IdUsuario),
    IdTecnico INT NULL REFERENCES Usuarios(IdUsuario),
    Status VARCHAR(20) DEFAULT 'Abierto',
    Descripción VARCHAR(MAX),
    Solución VARCHAR(MAX),
    Alta DATETIME,
    Atención DATETIME,
    Cierre DATETIME
);

-- Usuarios: Datos de acceso y rol
CREATE TABLE Usuarios (
    IdUsuario INT PRIMARY KEY IDENTITY,
    Nombre VARCHAR(100),
    Contraseña VARCHAR(255),
    IdRol INT REFERENCES RolesUsuario(IdRol),
    IdDepartamento INT,
    IdPuesto INT,
    IdSucursal INT
);

-- Roles: Define permisos
CREATE TABLE RolesUsuario (
    IdRol INT PRIMARY KEY IDENTITY,
    NombreRol VARCHAR(50)
    -- 1 = Admin, 2 = Técnico, 3 = Cliente
);

-- Historial: Auditoría de cambios
CREATE TABLE HistorialCambiosTickets (
    IdHistorial INT PRIMARY KEY IDENTITY,
    IdTicket INT REFERENCES Tickets(IdTicket),
    CampoModificado VARCHAR(50),
    ValorAnterior VARCHAR(MAX),
    ValorNuevo VARCHAR(MAX),
    IdUsuarioCambio INT REFERENCES Usuarios(IdUsuario),
    FechaMovimiento DATETIME
);

-- Índices para rendimiento
CREATE INDEX IX_Tickets_IdUsuario ON Tickets(IdUsuario);
CREATE INDEX IX_Tickets_IdTecnico ON Tickets(IdTecnico);
CREATE INDEX IX_Tickets_Status ON Tickets(Status);
CREATE INDEX IX_Historial_IdTicket ON HistorialCambiosTickets(IdTicket);
```

---

## 📈 ESCALABILIDAD

### Fácil de Agregar:
- ✅ Nuevos roles (agregar case en switch)
- ✅ Nuevos filtros (extender métodos en TicketService)
- ✅ Nuevas acciones (agregar métodos públicos)
- ✅ Nuevos indicadores (instanciar ucIndicador)

### Consideraciones:
- ⚠️ Usar índices en tablas grandes
- ⚠️ Paginación en grids con 10k+ registros
- ⚠️ Caché para datos referencia (usuarios, roles)
- ⚠️ Logging de operaciones críticas

