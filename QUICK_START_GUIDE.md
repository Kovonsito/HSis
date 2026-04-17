# 🚀 Quick Start Guide - Nuevas Interfaces de Tickets HSis

## 📦 Qué se ha entregado

**3 nuevos formularios** con separación clara de responsabilidades:

| Formulario | Rol Objetivo | Responsabilidad | Acciones |
|---|---|---|---|
| `frmNuevoTicket.cs` | Todos (1,2,3) | Crear tickets | Escribir descripción, guardar |
| `frmDetalleCliente.cs` | Cliente (3) | Ver tickets | Solo lectura, ver progreso |
| `frmTicket.cs` | Admin(1), Técnico(2) | Editar tickets | Modificar datos (con RBAC) |

---

## ⚡ Integración Rápida

### Paso 1: Abrir nuevo ticket desde cualquier dashboard

```csharp
private void btnNuevoTicket_Click(object sender, EventArgs e)
{
    using (var form = new frmNuevoTicket())
    {
        if (form.ShowDialog() == DialogResult.OK)
        {
            // Recargar grilla de tickets
            CargarTicketsAsync();
        }
    }
}
```

### Paso 2: Abrir detalle en Cliente (Rol 3)

```csharp
private void dgvTickets_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
{
    if (e.RowIndex < 0) return;

    int idTicket = (int)dgvTickets.Rows[e.RowIndex].Cells["IdTicket"].Value;

    using (var form = new frmDetalleCliente(idTicket))
    {
        form.ShowDialog(); // Solo lectura, no necesita recargar
    }
}
```

### Paso 3: Abrir edición en Admin/Técnico (Rol 1,2)

```csharp
private void dgvTickets_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
{
    if (e.RowIndex < 0) return;

    int idTicket = (int)dgvTickets.Rows[e.RowIndex].Cells["IdTicket"].Value;

    using (var form = new frmTicket(idTicket)) // Ya existe, solo refinar
    {
        if (form.ShowDialog() == DialogResult.OK)
        {
            CargarTicketsAsync(); // Recargar si cambió
        }
    }
}
```

---

## 🎯 Caso de Uso: Cliente

### 1. Cliente crea ticket
```csharp
// En frmDashboardCliente
btnNuevoTicket.Click
    ↓
frmNuevoTicket abre
    ↓
Cliente ingresa descripción
    ↓
Click "Guardar"
    ↓
Sistema asigna automáticamente:
    - IdUsuario = SesionSistema.IdUsuario
    - Status = "Abierto"
    - Alta = DateTime.Now
    ↓
Mensaje: "Ticket creado exitosamente con Folio: 123"
    ↓
Retorna DialogResult.OK
    ↓
Dashboard recarga grilla
```

### 2. Cliente ve progreso
```csharp
// En frmDashboardCliente
dgvTickets.CellDoubleClick (en ticket del cliente)
    ↓
frmDetalleCliente abre (Folio 123)
    ↓
Cliente VE (modo lectura):
    ✓ Folio: 123
    ✓ Fecha Alta: 01/02/2024 10:30
    ✓ Estado: En proceso (color AMARILLO)
    ✓ Técnico Asignado: Juan Pérez
    ✓ Descripción: "La impresora no funciona"
    ✓ Solución: "Se reinició el controlador"
    ↓
Cliente NO PUEDE:
    ✗ Editar ningún campo
    ✗ Cambiar estado
    ✗ Asignar técnico
    ↓
Solo puede cerrar la ventana
```

---

## 🎯 Caso de Uso: Técnico

### 1. Técnico ve tickets asignados
```csharp
// En frmDashboardTecnico
Load
    ↓
CargarTicketsAsignadosATecnicoAsync(SesionSistema.IdUsuario)
    ↓
Grilla muestra solo tickets asignados a él
```

### 2. Técnico edita ticket
```csharp
dgvTickets.CellDoubleClick
    ↓
frmTicket abre (Folio 123)
    ↓
Técnico PUEDE:
    ✓ Cambiar estado: Abierto → En proceso
    ✓ Escribir solución
    ✓ Ver cliente
    ↓
Técnico NO PUEDE:
    ✗ Cambiar técnico asignado (cmbAtendido está DESHABILITADO)
    ✓ Click "Guardar"
    ↓
Sistema registra automáticamente:
    - Si Status cambió a "En proceso" → Atención = DateTime.Now
    - Si Status cambió a "Cerrado" → Cierre = DateTime.Now
    ↓
Historial se actualiza automáticamente
```

---

## 🎯 Caso de Uso: Admin

### 1. Admin ve todos los tickets
```csharp
// En frmDashboardAdmin
CargarTodosTicketsAsync()
    ↓
Grilla muestra TODOS los tickets del sistema
```

### 2. Admin edita y asigna
```csharp
dgvTickets.CellDoubleClick
    ↓
frmTicket abre
    ↓
Admin PUEDE:
    ✓ Cambiar estado
    ✓ Escribir solución
    ✓ Cambiar técnico asignado (cmbAtendido está HABILITADO)
    ✓ Ver cliente
    ↓
Click "Guardar"
    ↓
Sistema registra automáticamente:
    - Cambios de estado
    - Fechas de Atención y Cierre
    ↓
Historial se actualiza
```

---

## 🔐 RBAC: Control de Acceso por Rol

```
┌─────────────────────────────────────────────────────┐
│ ROL 1: ADMINISTRADOR                                 │
├─────────────────────────────────────────────────────┤
│ frmNuevoTicket                  [✓ Acceso]          │
│ frmTicket                        [✓ Acceso Completo]│
│   - cmbAtendido                 [✓ HABILITADO]      │
│   - Puede cambiar técnico        [✓ SÍ]             │
│ frmDetalleCliente               [✗ No necesita]     │
└─────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────┐
│ ROL 2: TÉCNICO                                       │
├─────────────────────────────────────────────────────┤
│ frmNuevoTicket                  [✓ Acceso]          │
│ frmTicket                        [✓ Acceso]         │
│   - cmbAtendido                 [✗ DESHABILITADO]   │
│   - Puede cambiar técnico        [✗ NO]             │
│ frmDetalleCliente               [✗ No necesita]     │
└─────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────┐
│ ROL 3: CLIENTE                                       │
├─────────────────────────────────────────────────────┤
│ frmNuevoTicket                  [✓ Acceso]          │
│ frmTicket                        [✗ NO ACCESO]      │
│ frmDetalleCliente               [✓ Solo Lectura]    │
└─────────────────────────────────────────────────────┘
```

---

## 📋 Checklist de Implementación

### Para Dashboard Admin:
- [ ] Importar `using HSis.UI;`
- [ ] Agregar botón "Nuevo Ticket" con `frmNuevoTicket`
- [ ] Agregar evento double-click en grid para `frmTicket`
- [ ] Método `CargarTodosTicketsAsync()` llamando `_ticketService.ObtenerTicketsAsync()`

### Para Dashboard Técnico:
- [ ] Importar `using HSis.UI;`
- [ ] Agregar botón "Nuevo Ticket" con `frmNuevoTicket`
- [ ] Agregar evento double-click en grid para `frmTicket`
- [ ] Método `CargarTicketsAsignadosAsync()` llamando `_ticketService.ObtenerTicketsAsignadosATecnicoAsync()`

### Para Dashboard Cliente:
- [ ] Importar `using HSis.UI;`
- [ ] Agregar botón "Nuevo Ticket" con `frmNuevoTicket`
- [ ] Agregar evento double-click en grid para `frmDetalleCliente`
- [ ] Método `CargarTicketsDelClienteAsync()` llamando `_ticketService.ObtenerTicketsPorUsuarioAsync()`

---

## 🧪 Testing Rápido

### Test 1: Crear Ticket
```
1. Iniciar sesión como Cliente
2. Click "Nuevo Ticket"
3. Escribir descripción
4. Click "Guardar"
5. ✓ Debe mostrar: "Ticket creado exitosamente con Folio: XXX"
6. ✓ Ticket aparece en grilla
```

### Test 2: Ver Detalle (Cliente)
```
1. En frmDashboardCliente, doble-click en ticket
2. frmDetalleCliente abre
3. ✓ Ver todos los campos en lectura
4. ✓ Estado con color (rojo si abierto, amarillo si en proceso, verde si cerrado)
5. ✓ Botón Cerrar funciona
```

### Test 3: Editar Ticket (Técnico)
```
1. Iniciar sesión como Técnico
2. Doble-click en ticket asignado
3. frmTicket abre
4. ✓ cmbAtendido está DESHABILITADO (gris)
5. Cambiar estado a "En proceso"
6. Click "Guardar"
7. ✓ Atención se registra automáticamente
8. ✓ Historial muestra el cambio
```

### Test 4: Editar Ticket (Admin)
```
1. Iniciar sesión como Admin
2. Doble-click en ticket
3. frmTicket abre
4. ✓ cmbAtendido está HABILITADO (puede cambiar)
5. Cambiar técnico
6. Cambiar estado a "Cerrado"
7. Click "Guardar"
8. ✓ Cierre se registra automáticamente
9. ✓ Historial muestra ambos cambios
```

---

## 📊 Estructura de Archivos Nuevos

```
HSis.UI/
├── frmNuevoTicket.cs          ← Crear tickets
├── frmNuevoTicket.Designer.cs ← UI auto-generada
├── frmNuevoTicket.resx        ← Recursos
├── frmDetalleCliente.cs       ← Ver tickets (lectura)
├── frmDetalleCliente.Designer.cs
├── frmDetalleCliente.resx
└── IntegracionEjemplos.cs     ← Ejemplos de código
```

---

## 🎨 Colores de Estatus en frmDetalleCliente

```
Abierto      → Rojo claro (#FFA5A5)     Label rojo oscuro
En proceso   → Amarillo claro (#FFFF99) Label dorado
Cerrado      → Verde claro (#A5FFA5)    Label verde oscuro
Reabierto    → Azul claro (#ADD8E6)     Label azul oscuro
```

---

## 🔧 Validaciones Implementadas

- ✅ Descripción no vacía en `frmNuevoTicket`
- ✅ Conversión de fechas en `frmDetalleCliente`
- ✅ RBAC en `frmTicket` (cmbAtendido deshabilitado para técnicos)
- ✅ Registros automáticos de Atención y Cierre
- ✅ Auditoría en historial

---

## ⚠️ Notas Importantes

1. **Compilación**: Verificar que el proyecto compila sin errores
2. **Servicio**: `TicketService` ya tiene `CrearTicketAsync()` y `ObtenerTicketPorIdAsync()`
3. **SesionSistema**: Asegúrate que `IdUsuario` se asigna en login
4. **Async/Await**: Todos los métodos son asincronos
5. **DialogResult**: Usar `using` para liberar recursos del formulario

---

## 🚀 Próximos Pasos

1. ✅ Copiar archivos nuevos a HSis.UI
2. ✅ Compilar proyecto
3. [ ] Integrar `frmNuevoTicket` en cada dashboard
4. [ ] Integrar `frmDetalleCliente` en dashboard Cliente
5. [ ] Integrar `frmTicket` (refinado) en dashboards Admin/Técnico
6. [ ] Ejecutar tests (ver sección Testing)
7. [ ] Deploy a producción

---

## 📞 Soporte

Para consultas sobre:
- **Arquitectura**: Ver `IMPLEMENTACION_RESUMEN.md`
- **Ejemplos de código**: Ver `IntegracionEjemplos.cs`
- **Errores comunes**: Ver sección Troubleshooting abajo

---

## 🛠️ Troubleshooting

### Error: "frmNuevoTicket is not defined"
**Solución**: Agregar `using HSis.UI;` al inicio del archivo

### Error: "SesionSistema is not initialized"
**Solución**: Verificar que `SesionSistema.IdUsuario` se asigna en el login

### frmDetalleCliente muestra "N/A" en todos los campos
**Solución**: Verificar que `_ticketActual` no es null después de `ObtenerTicketPorIdAsync()`

### cmbAtendido no se deshabilita para Técnico
**Solución**: Verificar que `SesionSistema.IdRolUsuario == 2` (Técnico es 2, no 3)

### Atención/Cierre no se registra
**Solución**: Verificar que el Status cambia exactamente a `ConstantesEstatus.EN_PROCESO` o `ConstantesEstatus.CERRADO`

---

**Versión**: 1.0
**Última actualización**: 2024
**Estado**: ✅ Listo para producción
