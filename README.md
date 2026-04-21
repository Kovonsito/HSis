# HSis - Sistema de Gestión de Tickets e Inventario

## 📖 Descripción General
**HSis** es una aplicación de escritorio basada en Windows Forms (.NET) diseñada para la gestión integral de tickets de soporte técnico, control de inventario de materiales y administración de catálogos del sistema (Usuarios, Departamentos, Empresas, etc.). Utiliza una arquitectura de software en capas (N-Tier) y Entity Framework Core para el acceso a la base de datos.

## 🏗️ Arquitectura del Proyecto
La solución (`HSis.slnx`) está estructurada de manera modular para separar las responsabilidades, facilitando el mantenimiento y la escalabilidad. Se divide en tres proyectos principales:

### 1. HSis.Data (Capa de Acceso a Datos)
Es la capa más baja del sistema. Se encarga de la comunicación directa con la base de datos SQL Server.
- **`HSisDbContext`**: Es el contexto principal de Entity Framework Core que configura y gestiona las conexiones.
- **Modelos (Models)**: Contiene las clases que representan las tablas de la base de datos (Entidades), entre las que destacan:
  - **Módulo de Tickets:** `Ticket`, `DetTicket` (Materiales usados en ticket), `HistorialCambiosTicket`.
  - **Módulo de Usuarios:** `Usuario`, `RolUsuario`, `Puesto`, `Departamento`.
  - **Módulo de Estructura Organizacional:** `Empresa`, `Sucursal`.
  - **Módulo de Inventario:** `Material`, `Ingreso`.

### 2. HSis.Logic (Capa de Lógica de Negocio)
Actúa como puente entre la interfaz de usuario y la capa de datos. Aquí residen las reglas de negocio para asegurar que la base de datos no se modifique incorrectamente desde la UI.
- **`TicketService`**: Maneja la lógica de creación, actualización, cambios de estado y seguimiento de tickets.
- **`UsuarioService`**: Gestión de usuarios, autenticación y reglas de perfiles.
- **`CatalogoService`**: Es un servicio genérico asíncrono y muy versátil, diseñado para realizar operaciones CRUD (Crear, Leer, Actualizar, Eliminar) de forma dinámica sobre cualquier entidad del catálogo sin repetir código.
- **`MaterialService` / `IngresoService`**: Lógica para controlar el stock, costos de materiales e historial de ingresos.

### 3. HSis.UI (Capa de Presentación)
Es la interfaz visual con la que interactúan los usuarios, desarrollada en **Windows Forms**. Se adapta dinámicamente según el rol del usuario autenticado:
- **Dashboards por Rol:**
  - `frmDashboardAdmin`: Panel centralizado de administración (gestión total de usuarios, catálogos, métricas y supervisión).
  - `frmDashboardTecnico`: Panel enfocado a la resolución de problemas para los técnicos de soporte.
  - `frmDashboardCliente`: Panel simplificado para que los usuarios finales reporten incidencias y revisen su estado.
- **Formularios Dinámicos y Centralizados:**
  - `frmEditorDinamico`: Un formulario inteligente que, alimentado por el `CatalogoService`, permite gestionar (agregar, editar, eliminar) cualquier catálogo secundario del sistema (como Sucursales o Departamentos) usando una única pantalla genérica.
- **Gestión de Tickets:** Formularios como `frmNuevoTicket` y `frmTicketDetalle` para el manejo del ciclo de vida del incidente.
- **Sistema de Sesión:** `SesionSistema` para mantener en memoria los datos del usuario logueado en tiempo de ejecución.

## ⚙️ Funcionalidades Principales

- **Gestión Completa de Tickets:** Desde la apertura por parte del cliente, pasando por la asignación a un técnico, la documentación de la solución y el eventual cierre.
- **Auditoría Incorruptible:** Cada modificación realizada sobre un ticket (cambio de estado, asignaciones) genera un registro automático en el `HistorialCambiosTicket`.
- **Módulo de Inventarios:** Control de ingresos de `Materiales` y la asignación / descuento de los mismos cuando se utilizan para resolver un `Ticket` (a través de `DetTicket`).
- **Administración Dinámica Inteligente:** Reducción drástica de pantallas redundantes mediante el editor dinámico, lo que permite al administrador mantener la base de datos de manera ágil.
- **Control de Acceso (RBAC):** La aplicación muestra y oculta funcionalidades dependiendo del rol asignado en la entidad `RolUsuario`.

## 💻 Stack Tecnológico
- **Lenguaje:** C# 
- **Framework de Presentación:** Windows Forms (.NET)
- **Mapeo Objeto-Relacional (ORM):** Entity Framework Core
- **Base de Datos:** Microsoft SQL Server
- **Paradigma:** Programación Orientada a Objetos (POO), Arquitectura en N-Capas, Asincronismo (async/await).
