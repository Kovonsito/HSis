#nullable enable
using System.Runtime.Versioning;
using FontAwesome.Sharp;
using HSis.Contracts.Services;
using HSis.UI.Controls;
using HSis.UI.Factories;

namespace HSis.UI.Helpers
{
    /// <summary>
    /// Proveedor centralizado de menús del Sidebar organizados por rol.
    /// Evita instanciar arreglos repetidos en cada formulario de Dashboard.
    /// </summary>
    public static class FabricaMenusSidebar
    {
        public static ItemSidebar[] ParaAdmin() =>
        [
            new() { Clave = "tickets",       Titulo = "Tickets",       Icono = IconChar.TicketAlt },
            new() { Clave = "inventario",    Titulo = "Inventario",    Icono = IconChar.BoxesStacked },
            new() { Clave = "usuarios",      Titulo = "Usuarios",      Icono = IconChar.Users },
            new() { Clave = "departamentos", Titulo = "Departamentos", Icono = IconChar.Building },
            new() { Clave = "sucursales",    Titulo = "Sucursales",    Icono = IconChar.MapMarkerAlt },
            new() { Clave = "empresas",      Titulo = "Empresas",      Icono = IconChar.Landmark },
            new() { Clave = "puestos",       Titulo = "Puestos",       Icono = IconChar.Briefcase },
            new() { Clave = "roles",         Titulo = "Roles",         Icono = IconChar.Key },
            new() { Clave = "reportes",      Titulo = "Reportes",      Icono = IconChar.ChartBar }
        ];

        public static ItemSidebar[] ParaTecnico() =>
        [
            new() { Clave = "asignados",     Titulo = "Mis Asignados",  Icono = IconChar.ClipboardCheck },
            new() { Clave = "disponibles",   Titulo = "Disponibles",     Icono = IconChar.Inbox },
            new() { Clave = "cerrados",      Titulo = "Mis Cerrados",    Icono = IconChar.CheckCircle },
            new() { Clave = "calificaciones",Titulo = "Calificaciones", Icono = IconChar.Star },
            new() { Clave = "kardex",        Titulo = "Almacén / Kardex", Icono = IconChar.BoxesStacked }
        ];

        public static ItemSidebar[] ParaCliente() =>
        [
            new() { Clave = "activos",  Titulo = "Mis Activos",        Icono = IconChar.Ticket },
            new() { Clave = "cerrados", Titulo = "Historial Cerrados", Icono = IconChar.ClockRotateLeft }
        ];
    }
}
