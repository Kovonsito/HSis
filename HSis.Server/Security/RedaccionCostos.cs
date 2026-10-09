using System.Security.Claims;
using HSis.Contracts.DTOs;

namespace HSis.Server.Security;

// El costo es informacion de administracion: al resto de roles se les entrega en cero para no exponerlo por la API.
public static class RedaccionCostos
{
    public static List<MaterialDto> Para(ClaimsPrincipal usuario, List<MaterialDto> materiales)
    {
        if (usuario.EsAdministrador())
        {
            return materiales;
        }

        return [.. materiales.Select(m => new MaterialDto
        {
            IdMaterial = m.IdMaterial,
            Nombre = m.Nombre,
            StockActual = m.StockActual,
            UnidadMedida = m.UnidadMedida
        })];
    }

    public static List<KardexMovimientoDto> Para(ClaimsPrincipal usuario, List<KardexMovimientoDto> movimientos)
    {
        if (usuario.EsAdministrador())
        {
            return movimientos;
        }

        return [.. movimientos.Select(m => new KardexMovimientoDto
        {
            IdMovimiento = m.IdMovimiento,
            IdMaterial = m.IdMaterial,
            MaterialNombre = m.MaterialNombre,
            IdUsuario = m.IdUsuario,
            UsuarioNombre = m.UsuarioNombre,
            Fecha = m.Fecha,
            Cantidad = m.Cantidad,
            Motivo = m.Motivo,
            TipoMovimiento = m.TipoMovimiento
        })];
    }
}