using HSis.Contracts.Errors;
using HSis.Contracts.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace HSis.Server.Controllers
{
    [ApiController]
    [Route("api/CatalogosLegacy")]
    public class CatalogosController(
        ICatalogoService catalogoService,
        ILogger<CatalogosController> logger) : ControllerBase
    {

        [HttpGet("{entidad}")]
        public async Task<ActionResult> ObtenerTodos(string entidad)
        {
            var type = ObtenerTipoEntidad(entidad);
            if (type == null) return CatalogoNoEncontrado(entidad);

            var data = await catalogoService.ObtenerTodosPorTipoAsync(type);
            return Ok(data);
        }

        [HttpGet("{entidad}/registros")]
        public async Task<ActionResult> ObtenerRegistros(string entidad)
        {
            var type = ObtenerTipoEntidad(entidad);
            if (type == null) return CatalogoNoEncontrado(entidad);

            var data = await catalogoService.ObtenerTodosPorTipoAsync(type);
            var registros = data.Select(registro => ConvertirRegistro(registro, type));
            return Ok(registros);
        }

        [HttpPost("{entidad}/registros")]
        public async Task<ActionResult> CrearRegistro(string entidad, [FromBody] Dictionary<string, JsonElement> valores)
        {
            var type = ObtenerTipoEntidad(entidad);
            if (type == null) return CatalogoNoEncontrado(entidad);

            object entidadCreada;
            try
            {
                entidadCreada = CrearEntidad(type, valores);
            }
            catch (JsonException ex)
            {
                logger.LogWarning(ex, "Payload no válido al crear un registro del catálogo {Catalogo}.", entidad);
                return SolicitudNoValida("Los valores enviados no tienen un formato válido para el catálogo seleccionado.");
            }

            var metodo = typeof(ICatalogoService).GetMethod("CrearAsync")!.MakeGenericMethod(type);
            Task task = (Task)metodo.Invoke(catalogoService, [entidadCreada])!;
            await task;

            return Ok(ConvertirRegistro(entidadCreada, type));
        }

        [HttpDelete("{entidad}/registros/{id}")]
        public async Task<ActionResult> EliminarRegistro(string entidad, string id)
        {
            var type = ObtenerTipoEntidad(entidad);
            if (type == null) return CatalogoNoEncontrado(entidad);

            var tipoClave = type.GetProperty(ObtenerNombreClave(type))?.PropertyType;
            if (tipoClave == null) return SolicitudNoValida("El catálogo no tiene una clave primaria compatible para realizar esta operación.");

            object clave;
            try
            {
                clave = ConvertirValor(id, tipoClave);
            }
            catch (FormatException)
            {
                return SolicitudNoValida($"La clave '{id}' no tiene un formato válido para el catálogo seleccionado.");
            }

            var metodo = typeof(ICatalogoService).GetMethod("EliminarAsync")!.MakeGenericMethod(type);
            Task task = (Task)metodo.Invoke(catalogoService, [clave])!;
            await task;

            return NoContent();
        }

        [HttpPost("{entidad}")]
        public async Task<ActionResult> Crear(string entidad, [FromBody] System.Text.Json.JsonElement payload)
        {
            var type = ObtenerTipoEntidad(entidad);
            if (type == null) return CatalogoNoEncontrado(entidad);

            object? dto;
            try
            {
                dto = System.Text.Json.JsonSerializer.Deserialize(payload.GetRawText(), type);
            }
            catch (JsonException ex)
            {
                logger.LogWarning(ex, "Payload no válido al crear el catálogo {Catalogo}.", entidad);
                return SolicitudNoValida("El cuerpo de la solicitud no contiene datos válidos para el catálogo seleccionado.");
            }

            if (dto == null) return SolicitudNoValida("El cuerpo de la solicitud está vacío o no tiene un formato válido.");

            var metodo = typeof(ICatalogoService).GetMethod("CrearAsync")!.MakeGenericMethod(type);
            Task task = (Task)metodo.Invoke(catalogoService, [dto])!;
            await task;

            return Ok(dto);
        }

        [HttpPut("{entidad}")]
        public async Task<ActionResult> Actualizar(string entidad, [FromBody] System.Text.Json.JsonElement payload)
        {
            var type = ObtenerTipoEntidad(entidad);
            if (type == null) return CatalogoNoEncontrado(entidad);

            object? dto;
            try
            {
                dto = System.Text.Json.JsonSerializer.Deserialize(payload.GetRawText(), type);
            }
            catch (JsonException ex)
            {
                logger.LogWarning(ex, "Payload no válido al actualizar el catálogo {Catalogo}.", entidad);
                return SolicitudNoValida("El cuerpo de la solicitud no contiene datos válidos para el catálogo seleccionado.");
            }

            if (dto == null) return SolicitudNoValida("El cuerpo de la solicitud está vacío o no tiene un formato válido.");

            var metodo = typeof(ICatalogoService).GetMethod("ActualizarAsync")!.MakeGenericMethod(type);
            Task task = (Task)metodo.Invoke(catalogoService, [dto])!;
            await task;

            return Ok(dto);
        }

        [HttpDelete("{entidad}/{id}")]
        public async Task<ActionResult> Eliminar(string entidad, string id)
        {
            var type = ObtenerTipoEntidad(entidad);
            if (type == null) return CatalogoNoEncontrado(entidad);

            object idParsed = int.TryParse(id, out int idInt) ? idInt : id;

            var metodo = typeof(ICatalogoService).GetMethod("EliminarAsync")!.MakeGenericMethod(type);
            Task task = (Task)metodo.Invoke(catalogoService, [idParsed])!;
            await task;

            return NoContent();
        }

        private ObjectResult CatalogoNoEncontrado(string entidad)
            => Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Catálogo no encontrado",
                detail: $"No se encontró el catálogo '{entidad}'. Compruebe el nombre e inténtelo de nuevo.",
                extensions: CrearExtensiones(ApiErrorCodes.ResourceNotFound));

        private ObjectResult SolicitudNoValida(string detalle)
            => Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Datos del catálogo no válidos",
                detail: detalle,
                extensions: CrearExtensiones(ApiErrorCodes.BadRequest));

        private Dictionary<string, object?> CrearExtensiones(string code)
            => new()
            {
                ["code"] = code,
                ["traceId"] = HttpContext.TraceIdentifier
            };

        private static Type? ObtenerTipoEntidad(string entidad)
        {
            var assembly = typeof(Data.Models.Ticket).Assembly;
            var cleanName = entidad.EndsWith("Dto", StringComparison.OrdinalIgnoreCase)
                ? entidad[..^3]
                : entidad;
            return assembly.GetTypes().FirstOrDefault(t => t.Name.Equals(cleanName, StringComparison.OrdinalIgnoreCase));
        }

        private static Dictionary<string, object?> ConvertirRegistro(object registro, Type tipo)
        {
            var valores = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
            foreach (var propiedad in tipo.GetProperties().Where(propiedad => propiedad.CanRead && EsValorSimple(propiedad.PropertyType)))
            {
                valores[propiedad.Name] = propiedad.GetValue(registro);
            }

            return valores;
        }

        private static object CrearEntidad(Type tipo, IReadOnlyDictionary<string, JsonElement> valores)
        {
            var entidad = Activator.CreateInstance(tipo)
                ?? throw new JsonException($"No se pudo crear la entidad '{tipo.Name}'.");

            foreach (var (nombre, valor) in valores)
            {
                var propiedad = tipo.GetProperty(nombre);
                if (propiedad == null || !propiedad.CanWrite || !EsValorSimple(propiedad.PropertyType))
                {
                    continue;
                }

                propiedad.SetValue(entidad, ConvertirValor(valor, propiedad.PropertyType));
            }

            return entidad;
        }

        private static string ObtenerNombreClave(Type tipo)
        {
            return tipo.Name.Equals("RolUsuario", StringComparison.OrdinalIgnoreCase)
                ? "IdRol"
                : $"Id{tipo.Name}";
        }

        private static object? ConvertirValor(JsonElement valor, Type tipo)
        {
            if (valor.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined)
            {
                return null;
            }

            var tipoBase = Nullable.GetUnderlyingType(tipo) ?? tipo;
            return JsonSerializer.Deserialize(valor.GetRawText(), tipoBase, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        private static object ConvertirValor(string valor, Type tipo)
        {
            var tipoBase = Nullable.GetUnderlyingType(tipo) ?? tipo;
            if (tipoBase == typeof(string)) return valor;
            if (tipoBase.IsEnum) return Enum.Parse(tipoBase, valor, true);
            return Convert.ChangeType(valor, tipoBase, System.Globalization.CultureInfo.InvariantCulture);
        }

        private static bool EsValorSimple(Type tipo)
        {
            var tipoBase = Nullable.GetUnderlyingType(tipo) ?? tipo;
            return tipoBase.IsPrimitive || tipoBase.IsEnum || tipoBase == typeof(string) ||
                   tipoBase == typeof(decimal) || tipoBase == typeof(DateTime) || tipoBase == typeof(Guid);
        }
    }
}

