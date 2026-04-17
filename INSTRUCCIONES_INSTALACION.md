# 🔧 INSTRUCCIONES DE INSTALACIÓN Y DESPLIEGUE

## 📋 Pre-requisitos

Antes de comenzar, verifica que tienes:

- ✅ Visual Studio 2026 Community (18.5.0 o superior)
- ✅ .NET 10 SDK instalado
- ✅ SQL Server (para BD)
- ✅ Proyecto HSis compilable
- ✅ Acceso a `C:\HSis\` (workspace)

---

## 📦 Paso 1: Verificar Archivos Entregados

Los siguientes archivos han sido creados en `HSis.UI/`:

```
✅ frmNuevoTicket.cs
✅ frmNuevoTicket.Designer.cs
✅ frmNuevoTicket.resx
✅ frmDetalleCliente.cs
✅ frmDetalleCliente.Designer.cs
✅ frmDetalleCliente.resx
✅ IntegracionEjemplos.cs (Ejemplos comentados)
```

Documentación:
```
✅ IMPLEMENTACION_RESUMEN.md
✅ QUICK_START_GUIDE.md
✅ ESPECIFICACION_TECNICA.md
✅ 00_RESUMEN_EJECUTIVO_FINAL.md (Este archivo)
✅ INSTRUCCIONES_INSTALACION.md (Este archivo)
```

---

## 🔨 Paso 2: Verificar que el Proyecto Compila

Abre Visual Studio y carga la solución:

```powershell
cd C:\HSis\
```

Compila la solución:

```powershell
dotnet clean
dotnet build
```

**Resultado esperado:**
```
Build succeeded. 0 Warning(s)
```

Si hay errores, verifica:
- ✓ Archivo `ConstantesEstatus.cs` existe en HSis.Logic
- ✓ Archivo `TicketService.cs` tiene método `CrearTicketAsync()`
- ✓ `SesionSistema.cs` existe en HSis.UI
- ✓ Todos los Using statements están presentes

---

## 🖥️ Paso 3: Integración en Dashboard Cliente (frmDashboardCliente.cs)

### 3.1 Agregar Using
En la parte superior de `frmDashboardCliente.cs`, agrega:
```csharp
using HSis.UI;
using HSis.Logic.Services;
```

### 3.2 Agregar campo privado
```csharp
private readonly TicketService _ticketService;
```

### 3.3 Inicializar en constructor
```csharp
public frmDashboardCliente()
{
    InitializeComponent();
    _ticketService = new TicketService();
}
```

### 3.4 Implementar botón "Nuevo Ticket"
```csharp
private void btnNuevoTicket_Click(object sender, EventArgs e)
{
    try
    {
        using (var form = new frmNuevoTicket())
        {
            if (form.ShowDialog() == DialogResult.OK)
            {
                CargarTicketsDelClienteAsync();
                MessageBox.Show("Ticket creado exitosamente.", 
                    "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Error: {ex.Message}", "Error");
    }
}
```

### 3.5 Implementar doble-click en grid
```csharp
private void dgvTickets_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
{
    try
    {
        if (e.RowIndex < 0) return;

        int idTicket = (int)dgvTickets.Rows[e.RowIndex].Cells["IdTicket"].Value;

        using (var form = new frmDetalleCliente(idTicket))
        {
            form.ShowDialog();
            // No recargar porque es solo lectura
        }
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Error: {ex.Message}", "Error");
    }
}
```

### 3.6 Implementar método para cargar tickets
```csharp
private async void CargarTicketsDelClienteAsync()
{
    try
    {
        var tickets = await _ticketService
            .ObtenerTicketsPorUsuarioAsync(SesionSistema.IdUsuario);

        dgvTickets.DataSource = tickets;

        // Personalizar columnas
        if (dgvTickets.Columns.Count > 0)
        {
            dgvTickets.Columns["IdTicket"].HeaderText = "Folio";
            dgvTickets.Columns["Alta"].HeaderText = "Fecha";
            dgvTickets.Columns["Status"].HeaderText = "Estado";
            dgvTickets.Columns["Descripción"].HeaderText = "Problema";
        }

        lblTotal.Text = $"Total: {tickets.Count} tickets";
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Error al cargar: {ex.Message}", "Error");
    }
}
```

---

## 🖥️ Paso 4: Integración en Dashboard Admin (frmDashboardAdmin.cs)

### 4.1 Agregar Using
```csharp
using HSis.UI;
using HSis.Logic.Services;
```

### 4.2 Agregar campo y inicializar
```csharp
private readonly TicketService _ticketService = new TicketService();
```

### 4.3 Implementar botón "Nuevo Ticket"
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
```

### 4.4 Implementar doble-click
```csharp
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
```

### 4.5 Implementar cargar todos
```csharp
private async void CargarTodosTicketsAsync()
{
    try
    {
        var tickets = await _ticketService.ObtenerTicketsAsync();
        dgvTickets.DataSource = tickets;
        lblTotal.Text = $"Total: {tickets.Count} tickets";
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Error: {ex.Message}", "Error");
    }
}
```

---

## 🖥️ Paso 5: Integración en Dashboard Técnico (frmDashboardTecnico.cs)

### 5.1 Similar a Admin, pero cargar solo tickets asignados
```csharp
private async void CargarTicketsAsignadosAsync()
{
    try
    {
        var tickets = await _ticketService
            .ObtenerTicketsAsignadosATecnicoAsync(SesionSistema.IdUsuario);

        dgvTickets.DataSource = tickets;
        lblTotal.Text = $"Total: {tickets.Count} tickets asignados";
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Error: {ex.Message}", "Error");
    }
}
```

### 5.2 El formulario frmTicket se abrirá con RBAC:
- cmbAtendido estará DESHABILITADO para técnicos (Rol 2)
- Ya está implementado en frmTicket.cs línea 72-73

---

## ✅ Paso 6: Compilar y Verificar

```powershell
dotnet build
```

Si hay errores, revisa:
1. Using statements correctos
2. Nombres de métodos (case-sensitive)
3. TicketService instanciado correctamente

Si compila exitosamente: ✅

---

## 🧪 Paso 7: Testing

### Test 1: Cliente crea ticket
1. Ejecutar aplicación
2. Login como Cliente (Rol 3)
3. Click "Nuevo Ticket"
4. Escribir descripción: "Test de creación"
5. Click "Guardar"
6. ✓ Debe mostrar: "Ticket creado exitosamente con Folio: XXX"
7. ✓ Ticket aparece en grilla

### Test 2: Cliente ve detalle
1. En grilla, doble-click en ticket creado
2. frmDetalleCliente abre
3. ✓ Ver folio, fecha, estado, técnico
4. ✓ Todo está en lectura (no puede editar)
5. Click "Cerrar"

### Test 3: Admin edita
1. Logout cliente
2. Login como Admin (Rol 1)
3. Doble-click en ticket
4. frmTicket abre
5. ✓ cmbAtendido está HABILITADO (puede cambiar técnico)
6. Cambiar estatus a "En proceso"
7. Click "Guardar"
8. ✓ Atención se registra automáticamente

### Test 4: Técnico edita (sin asignar)
1. Logout admin
2. Login como Técnico (Rol 2)
3. Doble-click en ticket asignado
4. frmTicket abre
5. ✓ cmbAtendido está DESHABILITADO (gris)
6. Cambiar estatus a "Cerrado"
7. Click "Guardar"
8. ✓ Cierre se registra automáticamente
9. ✓ Pero NO pudo cambiar técnico (RBAC)

---

## 🚀 Paso 8: Deploy a Producción

### Pre-requisitos
- [ ] Testing completado y exitoso
- [ ] Documentación revisada
- [ ] BD respaldada
- [ ] Validaciones servidor-lado implementadas (opcional)

### Procedimiento
```powershell
# 1. Compilar release
dotnet build -c Release

# 2. Publicar
dotnet publish HSis.UI -c Release -o C:\Deploy\HSis

# 3. Copiar archivos a servidor
# Usar instalador o manual según proceso

# 4. Verificar en producción
# Pruebas de regresión rápidas
```

---

## 📋 Checklist Final

- [ ] Archivos copiados a HSis.UI
- [ ] Proyecto compila sin errores
- [ ] Using statements agregados a dashboards
- [ ] frmNuevoTicket integrado (crear tickets)
- [ ] frmDetalleCliente integrado (ver detalle)
- [ ] frmTicket funciona con RBAC
- [ ] Test 1 (crear): PASS ✓
- [ ] Test 2 (ver detalle): PASS ✓
- [ ] Test 3 (admin edita): PASS ✓
- [ ] Test 4 (técnico edita): PASS ✓
- [ ] Documentación revisada
- [ ] Ready for production

---

## 🆘 Troubleshooting

### Error: "frmNuevoTicket not found"
**Solución:** Verificar que `using HSis.UI;` está en el archivo

### Error: "TicketService not found"
**Solución:** Verificar que `using HSis.Logic.Services;` está presente

### frmTicket abre pero cmbAtendido no se deshabilita
**Solución:** Verificar que `SesionSistema.IdRolUsuario` se asigna correctamente en login

### Las fechas no se registran
**Solución:** Verificar que Status cambia exactamente a `ConstantesEstatus.EN_PROCESO` o `ConstantesEstatus.CERRADO`

### "Ticket no encontrado" en frmDetalleCliente
**Solución:** Verificar que el idTicket es válido en la BD

---

## 📞 Soporte

Si encuentras problemas:

1. Revisa los **documentos técnicos**:
   - QUICK_START_GUIDE.md
   - ESPECIFICACION_TECNICA.md
   - IntegracionEjemplos.cs

2. Verifica **errores de compilación** en el Output window

3. Consulta **logs de excepción** en MessageBox

4. Revisa **test cases** en QUICK_START_GUIDE.md

---

## 📅 Cronograma Sugerido

| Fase | Duración | Tareas |
|------|----------|--------|
| **Setup** | 1 día | Copiar archivos, compilar, verificar |
| **Integración** | 2 días | Integrar en dashboards |
| **Testing** | 1 día | Ejecutar test cases |
| **UAT** | 3 días | Validación de usuario |
| **Deploy** | 0.5 día | Deploy a producción |

**Total: ~7-8 días**

---

## ✨ Post-Deploy

Después del deploy, se recomienda:

1. [ ] Monitorear logs
2. [ ] Recopilar feedback de usuarios
3. [ ] Documentar issues
4. [ ] Planificar mejoras
5. [ ] Agregar validaciones servidor-lado (opcional)

---

**Documento de Instalación**
**Versión:** 1.0
**Fecha:** 2024
**Status:** ✅ Listo para seguir
