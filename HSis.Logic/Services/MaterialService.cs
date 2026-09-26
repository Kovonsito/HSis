using HSis.Contracts.Services;
using HSis.Contracts.Constants;
using HSis.Data.Models;
using HSis.Contracts.DTOs;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace HSis.Logic.Services
{
    /// <summary>
    /// Servicio para gestionar operaciones relacionadas con Materiales e inventario.
    /// </summary>
    public class MaterialService(
        IDbContextFactory<HSisDbContext> dbContextFactory,
        IValidator<Material>? validator = null,
        INotificacionDestinatariosService? notificationRecipients = null,
        IServerNotificationDispatcher? notificationDispatcher = null,
        ICurrentUserService? currentUserService = null,
        ILogger<MaterialService>? logger = null) : IMaterialService
    {
        public async Task<MaterialDto> CrearMaterialAsync(MaterialCatalogoRequestDto request)
        {
            var material = new Material
            {
                Nombre = request.Nombre,
                Costo = request.CostoUnitario,
                Inventario = request.StockActual,
                UnidadMedida = request.UnidadMedida
            };
            await CatalogoValidation.ValidarAsync(validator, material);

            using var db = dbContextFactory.CreateDbContext();
            db.Materials.Add(material);
            await db.SaveChangesAsync();
            return Convertir(material);
        }

        public async Task<MaterialDto?> ActualizarMaterialAsync(int idMaterial, MaterialCatalogoRequestDto request)
        {
            using var db = dbContextFactory.CreateDbContext();
            var material = await db.Materials.FindAsync(idMaterial);
            if (material is null)
            {
                return null;
            }

            material.Nombre = request.Nombre;
            material.Costo = request.CostoUnitario;
            material.Inventario = request.StockActual;
            material.UnidadMedida = request.UnidadMedida;
            await CatalogoValidation.ValidarAsync(validator, material);
            await db.SaveChangesAsync();
            return Convertir(material);
        }

        public async Task<bool> EliminarMaterialAsync(int idMaterial)
        {
            using var db = dbContextFactory.CreateDbContext();
            var material = await db.Materials.FindAsync(idMaterial);
            if (material is null)
            {
                return false;
            }

            db.Materials.Remove(material);
            await db.SaveChangesAsync();
            return true;
        }

        public async Task ActualizarCostoMaterialAsync(int idMaterial, decimal nuevoCosto)
        {
            using var db = dbContextFactory.CreateDbContext();
            var material = await db.Materials.FindAsync(idMaterial);
            if (material != null)
            {
                material.Costo = nuevoCosto;
                db.Materials.Update(material);
                await db.SaveChangesAsync();
            }
        }

        public async Task<List<MaterialDto>> ObtenerMaterialesAsync()
        {
            using var db = dbContextFactory.CreateDbContext();
            return await db.Materials
                .AsNoTracking()
                .Select(m => new MaterialDto
                {
                    IdMaterial = m.IdMaterial,
                    Nombre = m.Nombre,
                    CostoUnitario = m.Costo,
                    StockActual = m.Inventario,
                    UnidadMedida = m.UnidadMedida
                })
                .ToListAsync();
        }

        private static MaterialDto Convertir(Material material) => new()
        {
            IdMaterial = material.IdMaterial,
            Nombre = material.Nombre,
            CostoUnitario = material.Costo,
            StockActual = material.Inventario,
            UnidadMedida = material.UnidadMedida
        };

        public async Task<List<KardexMovimientoDto>> ObtenerKardexPorMaterialAsync(int idMaterial)
        {
            using var db = dbContextFactory.CreateDbContext();
            return await db.VHistorialInventarios
                .AsNoTracking()
                .Where(h => h.IdMaterial == idMaterial)
                .OrderByDescending(h => h.Fecha)
                .Select(h => new KardexMovimientoDto
                {
                    IdMaterial = h.IdMaterial,
                    MaterialNombre = h.Material,
                    Fecha = h.Fecha ?? DateTime.Now,
                    TipoMovimiento = h.TipoMovimiento,
                    Cantidad = h.Cantidad ?? 0,
                    CostoUnitario = h.CostoUnitario,
                    UsuarioNombre = h.UsuarioResponsable ?? string.Empty,
                    Motivo = h.Motivo ?? string.Empty
                })
                .ToListAsync();
        }

        public async Task RegistrarMovimientoAsync(KardexMovimientoDto movimiento)
        {
            using var db = dbContextFactory.CreateDbContext();
            var material = await db.Materials.FindAsync(movimiento.IdMaterial)
                ?? throw new KeyNotFoundException($"No existe el material {movimiento.IdMaterial}.");

            var usuarioId = currentUserService?.GetCurrentUserId() is > 0
                ? currentUserService.GetCurrentUserId()
                : movimiento.IdUsuario;
            var destinatarios = notificationRecipients is null
                ? Array.Empty<int>()
                : await notificationRecipients.ObtenerDestinatariosMovimientoMaterialAsync();

            var entity = new MovimientoMaterial
            {
                IdMaterial = movimiento.IdMaterial,
                Cantidad = movimiento.Cantidad,
                CostoUnitario = movimiento.CostoUnitario,
                FechaMovimiento = movimiento.Fecha == default ? DateTime.Now : movimiento.Fecha,
                IdUsuario = usuarioId,
                Motivo = movimiento.Motivo
            };

            var existenciaPosterior = material.Inventario + movimiento.Cantidad;
            await EjecutarEnTransaccionAsync(db, async () =>
            {
                db.MovimientosMateriales.Add(entity);
                material.Inventario = existenciaPosterior;
                if (movimiento.CostoUnitario > 0)
                {
                    material.Costo = movimiento.CostoUnitario;
                }

                await db.SaveChangesAsync();

                if (destinatarios.Count > 0)
                {
                    var mensaje = NotificacionFactory.CrearMensajeMovimientoMaterial(
                        material.IdMaterial,
                        material.Nombre,
                        movimiento.TipoMovimiento,
                        movimiento.Cantidad,
                        existenciaPosterior,
                        movimiento.Motivo);
                    db.Notificaciones.AddRange(destinatarios.Select(idUsuarioDestino => NotificacionFactory.Crear(
                        idUsuarioDestino,
                        null,
                        ConstantesTiposNotificacion.MovimientoMaterial,
                        mensaje,
                        materialId: material.IdMaterial)));
                    await db.SaveChangesAsync();
                }
            });

            if (notificationDispatcher is not null)
            {
                await PublicarNotificacionSeguraAsync(
                    () => notificationDispatcher.NotifyMaterialChangeAsync(
                        destinatarios,
                        null,
                        material.IdMaterial,
                        ConstantesTiposNotificacion.MovimientoMaterial,
                        NotificacionFactory.CrearMensajeMovimientoMaterial(
                            material.IdMaterial,
                            material.Nombre,
                            movimiento.TipoMovimiento,
                            movimiento.Cantidad,
                            existenciaPosterior,
                            movimiento.Motivo)),
                    "movimiento de material",
                    material.IdMaterial);
            }
        }

        private static async Task EjecutarEnTransaccionAsync(
            HSisDbContext db,
            Func<Task> operacion)
        {
            IDbContextTransaction? transaction = null;
            if (db.Database.IsRelational())
            {
                transaction = await db.Database.BeginTransactionAsync();
            }

            try
            {
                await operacion();
                if (transaction is not null)
                {
                    await transaction.CommitAsync();
                }
            }
            catch
            {
                if (transaction is not null)
                {
                    await transaction.RollbackAsync();
                }

                throw;
            }
            finally
            {
                if (transaction is not null)
                {
                    await transaction.DisposeAsync();
                }
            }
        }

        private async Task PublicarNotificacionSeguraAsync(
            Func<Task> publicar,
            string evento,
            int materialId)
        {
            try
            {
                await publicar();
            }
            catch (Exception ex)
            {
                logger?.LogError(
                    ex,
                    "No se pudo publicar la notificación de {Evento} para el material {MaterialId}. El historial persistido queda disponible para sincronización.",
                    evento,
                    materialId);
            }
        }
    }
}
