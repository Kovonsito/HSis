# GUÍA DE PRUEBAS - DASHBOARDS POR ROL

## 🧪 ESCENARIOS DE PRUEBA

### ESCENARIO 1: Login de Cliente

**Pasos:**
1. Ejecutar la aplicación
2. Ingresar credenciales de un usuario con Rol 3 (Cliente)
3. Click en "Iniciar Sesión"

**Resultado Esperado:**
- ✅ Mensaje: "Inicio de sesión exitoso"
- ✅ Abre `frmDashboardCliente`
- ✅ Se oculta `frmIniciarSesion`
- ✅ Carga los tickets del usuario
- ✅ Muestra indicador "Mis Tickets Activos" (azul)
- ✅ Grid muestra: Folio, Fecha, Estatus, Técnico, Descripción

**Verificar:**
```
SELECT COUNT(*) FROM Tickets 
WHERE IdUsuario = [IdUsuario] AND Status != 'Cerrado'
```
El número debe coincidir con el indicador

---

### ESCENARIO 2: Cliente Crea Nuevo Reporte

**Pasos:**
1. Estar en `frmDashboardCliente`
2. Click en botón "+ Nuevo Reporte"
3. Se abre `frmNuevoReporte`
4. Escribir en RichTextBox: "Mi impresora no funciona"
5. Click en "Guardar"

**Resultado Esperado:**
- ✅ Mensaje: "Reporte creado exitosamente"
- ✅ Se cierra `frmNuevoReporte`
- ✅ Grid en `frmDashboardCliente` se recarga
- ✅ Aparece el nuevo ticket en la lista

**Verificar en BD:**
```sql
SELECT * FROM Tickets 
WHERE IdUsuario = [IdUsuario] 
ORDER BY Alta DESC 
LIMIT 1
```
Debe mostrar:
- Status = 'Abierto'
- Alta = hoy, ahora
- Descripción = "Mi impresora no funciona"
- IdTecnico = NULL

---

### ESCENARIO 3: Cliente Intenta Guardar Reporte Vacío

**Pasos:**
1. Estar en `frmDashboardCliente`
2. Click en "+ Nuevo Reporte"
3. Dejar RichTextBox vacío
4. Click en "Guardar"

**Resultado Esperado:**
- ✅ Mensaje: "Por favor, describe el problema"
- ✅ No se guarda nada
- ✅ `frmNuevoReporte` permanece abierto

---

### ESCENARIO 4: Cliente Abre un Ticket

**Pasos:**
1. Estar en `frmDashboardCliente`
2. Double-click en una fila del grid

**Resultado Esperado:**
- ✅ Se abre `frmTicket` con los datos del ticket
- ✅ Muestra Folio, Usuario, Estatus, Fecha, etc.

---

### ESCENARIO 5: Login de Técnico

**Pasos:**
1. Cerrar la aplicación
2. Ejecutar nuevamente
3. Ingresar credenciales de un usuario con Rol 2 (Técnico)
4. Click en "Iniciar Sesión"

**Resultado Esperado:**
- ✅ Mensaje: "Inicio de sesión exitoso"
- ✅ Abre `frmDashboardTecnico`
- ✅ Se oculta `frmIniciarSesion`
- ✅ Muestra dos indicadores:
  - "Mis Asignados" (azul)
  - "Disponibles" (amarillo)
- ✅ Grid vacío (aguarda click en indicador)

---

### ESCENARIO 6: Técnico Visualiza sus Tickets Asignados

**Pasos:**
1. Estar en `frmDashboardTecnico`
2. Click en indicador "Mis Asignados" (azul)

**Resultado Esperado:**
- ✅ Se carga el grid con tickets asignados al técnico
- ✅ Mostrar: Folio, Fecha, Estatus, Usuario, Descripción
- ✅ Todos los tickets tienen Status != 'Cerrado'
- ✅ Todos tienen IdTecnico = [IdTecnico del usuario]

**Verificar:**
```sql
SELECT COUNT(*) FROM Tickets 
WHERE IdTecnico = [IdTecnico] 
AND Status != 'Cerrado'
```
Debe coincidir con cantidad de filas en grid

---

### ESCENARIO 7: Técnico Visualiza Tickets Disponibles

**Pasos:**
1. Estar en `frmDashboardTecnico`
2. Click en indicador "Disponibles" (amarillo)

**Resultado Esperado:**
- ✅ Se carga el grid con tickets disponibles
- ✅ Mostrar: Folio, Fecha, Estatus, Usuario, Descripción
- ✅ Todos los tickets tienen Status = 'Abierto'
- ✅ Todos tienen IdTecnico = NULL

**Verificar:**
```sql
SELECT COUNT(*) FROM Tickets 
WHERE Status = 'Abierto' 
AND IdTecnico IS NULL
```
Debe coincidir con cantidad de filas en grid

---

### ESCENARIO 8: Técnico Edita un Ticket

**Pasos:**
1. Estar en `frmDashboardTecnico`
2. Estar en vista "Mis Asignados"
3. Double-click en una fila

**Resultado Esperado:**
- ✅ Se abre `frmTicket` con datos del ticket
- ✅ Técnico puede editar Status, Solución, etc.
- ✅ Click "Guardar" -> Actualiza BD
- ✅ Se cierra `frmTicket`
- ✅ `frmDashboardTecnico` recarga:
  - Indicadores (conteos actualizados)
  - Grid (con tickets actualizados)

**Verificar en Historial:**
```sql
SELECT * FROM HistorialCambiosTickets 
WHERE IdTicket = [IdTicket]
ORDER BY FechaMovimiento DESC
```
Debe mostrar registros de cambios

---

### ESCENARIO 9: Login de Admin (Verificación)

**Pasos:**
1. Cerrar la aplicación
2. Ejecutar nuevamente
3. Ingresar credenciales de un usuario con Rol 1 (Admin)
4. Click en "Iniciar Sesión"

**Resultado Esperado:**
- ✅ Abre `frmDashboardAdmin` (no se debe romper)
- ✅ El sistema sigue funcionando correctamente

---

## ✅ CHECKLIST DE VALIDACIÓN

### Funcionalidad de Capa Lógica
- [ ] `ObtenerTicketsPorUsuarioAsync()` retorna solo tickets del usuario
- [ ] `ObtenerTicketsAsignadosATecnicoAsync()` filtra por técnico Y no cerrados
- [ ] `ObtenerTicketsDisponiblesAsync()` retorna abiertos sin técnico
- [ ] `CrearTicketAsync()` guarda correctamente en BD

### Funcionalidad de Presentación
- [ ] Enrutamiento por rol funciona para Rol 1, 2, 3
- [ ] `frmDashboardCliente` carga tickets correctamente
- [ ] `frmNuevoReporte` valida descripción no vacía
- [ ] `frmNuevoReporte` crea ticket con datos correctos
- [ ] `frmDashboardTecnico` muestra dos indicadores
- [ ] Click en indicadores recarga grid correctamente

### UI/UX
- [ ] Indicadores muestran colores correctos
- [ ] Grid es ReadOnly (no editable)
- [ ] Grid permite FullRowSelect
- [ ] Double-click abre `frmTicket`
- [ ] Botones tienen colores apropiados
- [ ] Títulos son descriptivos

### Datos
- [ ] Los conteos de indicadores coinciden con BD
- [ ] Los datos del grid corresponden a los filtros
- [ ] Las fechas se muestran correctamente
- [ ] Los nombres de usuarios se cargan correctamente
- [ ] El folio se formatea como "TK-000001"

### Flujos
- [ ] Nuevo reporte → Recarga grid del cliente
- [ ] Editar ticket → Recarga indicadores y grid del técnico
- [ ] Cambio de vista (Mi Asignados/Disponibles) → Grid actualiza
- [ ] Cerrar y abrir aplicación → Mantiene sesión

---

## 🔍 CASOS DE BORDE

### CB-1: Usuario sin Tickets
**Escenario:** Cliente que nunca ha creado tickets
**Esperado:** Grid vacío, indicador muestra "0"

### CB-2: Técnico sin Asignaciones
**Escenario:** Técnico recién agregado al sistema
**Esperado:** Indicador "Mis Asignados" = 0, "Disponibles" muestra cantidad total

### CB-3: Descripción muy larga
**Escenario:** Ticket con descripción de 500+ caracteres
**Esperado:** Grid trunca a 50 caracteres + "..."

### CB-4: Usuario sin rol asignado
**Escenario:** Usuario con IdRol = NULL
**Esperado:** Cae al default (Admin)

### CB-5: Datos NULL en navegación
**Escenario:** Ticket con IdTecnico = NULL o IdUsuario inválido
**Esperado:** Muestra "Sin asignar" o "Desconocido"

---

## 📊 PRUEBAS DE RENDIMIENTO

### PT-1: Grid con 1000 tickets
- [ ] Cargar 1000 tickets < 2 segundos
- [ ] Grid responsivo al scroll
- [ ] Doble-click abre ticket rápidamente

### PT-2: Indicadores con datos agregados
- [ ] Contar 1000 tickets < 1 segundo
- [ ] Actualización al cambiar de vista < 1 segundo

### PT-3: Creación múltiple de tickets
- [ ] Crear 10 tickets en serie sin errores
- [ ] Grid se recarga correctamente cada vez

---

## 🐛 REPORTE DE PROBLEMAS

Si encuentras problemas, verifica:

1. **No carga grid**
   - Verificar conexión a BD
   - Verificar que existen tickets en BD
   - Ver Output window para excepciones

2. **Indicador muestra 0**
   - Verificar filtro de rol en BD
   - Confirmar que hay tickets que coinciden

3. **Doble-click no abre ticket**
   - Verificar que IdTicket se extrae correctamente
   - Comprobar que frmTicket existe

4. **Nuevo reporte no aparece**
   - Verificar que CrearTicketAsync completó
   - Recargar grid manualmente
   - Verificar en BD directamente

