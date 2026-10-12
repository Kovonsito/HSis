#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HSis.UI.Models;

namespace HSis.UI.Services;

/// <summary>
/// Servicio para descubrir, leer y estructurar eventos de log generados por Serilog.
/// </summary>
public sealed class LectorLogsService
{
    private static readonly Regex RegexInicioEntrada = new(
        @"^(\d{4}-\d{2}-\d{2}\s+\d{2}:\d{2}:\d{2}(?:\.\d+)?(?:\s+[+-]\d{2}:\d{2})?)\s+\[(INF|WRN|ERR|FTL|DBG|VRB)\]\s*(.*)$",
        RegexOptions.Compiled);

    private static readonly Regex RegexTraceId = new(
        @"(?:TraceId|Seguimiento|Código de seguimiento|Referencia):\s*([^\s;,\.\r\n]+)",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex RegexCorrelacionBrackets = new(
        @"\[([A-Fa-f0-9]{8})\]",
        RegexOptions.Compiled);

    /// <summary>
    /// Devuelve la carpeta donde residen los logs más activos del sistema.
    /// </summary>
    public string ObtenerCarpetaLogs()
    {
        var rutasCandidatas = new[]
        {
            Path.Combine(AppContext.BaseDirectory, "Logs"),
            Path.Combine(Environment.CurrentDirectory, "Logs"),
            Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Logs")),
            Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "HSis.UI", "Logs")),
            Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Logs"))
        };

        foreach (var ruta in rutasCandidatas)
        {
            if (Directory.Exists(ruta) && Directory.EnumerateFiles(ruta, "*.txt").Any())
            {
                return ruta;
            }
        }

        // Si ninguna contiene archivos, devuelve la primera que exista o la estándar en BaseDirectory
        var primeraExistente = rutasCandidatas.FirstOrDefault(Directory.Exists);
        return primeraExistente ?? Path.Combine(AppContext.BaseDirectory, "Logs");
    }

    /// <summary>
    /// Lista los archivos de bitácora disponibles, ordenados del más reciente al más antiguo.
    /// </summary>
    public Task<List<FileInfo>> ObtenerArchivosLogAsync()
    {
        return Task.Run(() =>
        {
            var carpeta = ObtenerCarpetaLogs();
            if (!Directory.Exists(carpeta))
            {
                return [];
            }

            var dirInfo = new DirectoryInfo(carpeta);
            return dirInfo.GetFiles("*.txt")
                .OrderByDescending(f => f.LastWriteTime)
                .ToList();
        });
    }

    /// <summary>
    /// Lee y analiza las entradas del archivo de log indicado, aplicando los filtros deseados.
    /// </summary>
    public Task<List<EntradaLogItem>> LeerEntradasAsync(
        string rutaArchivo,
        string? filtroTexto = null,
        string? filtroNivel = null)
    {
        return Task.Run(() =>
        {
            if (!File.Exists(rutaArchivo))
            {
                return [];
            }

            var entradas = new List<EntradaLogItem>();
            string nombreArchivo = Path.GetFileName(rutaArchivo);

            try
            {
                // Usar FileShare.ReadWrite para evitar bloqueos si Serilog está escribiendo en el archivo
                using var fs = new FileStream(rutaArchivo, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var reader = new StreamReader(fs, Encoding.UTF8);

                EntradaLogItem? entradaActual = null;
                var sbDetalle = new StringBuilder();

                string? linea;
                while ((linea = reader.ReadLine()) != null)
                {
                    var match = RegexInicioEntrada.Match(linea);
                    if (match.Success)
                    {
                        // Finalizar entrada previa si existe
                        if (entradaActual != null)
                        {
                            entradaActual.DetalleCompleto = sbDetalle.ToString().TrimEnd();
                            entradas.Add(entradaActual);
                            sbDetalle.Clear();
                        }

                        string fechaStr = match.Groups[1].Value.Trim();
                        string nivelAbrev = match.Groups[2].Value.Trim();
                        string mensaje = match.Groups[3].Value.Trim();

                        DateTime fechaParsed;
                        if (!DateTime.TryParse(fechaStr, CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaParsed))
                        {
                            _ = DateTime.TryParse(fechaStr, out fechaParsed);
                        }

                        entradaActual = new EntradaLogItem
                        {
                            FechaHora = fechaParsed == default ? DateTime.Now : fechaParsed,
                            Nivel = NormalizarNivel(nivelAbrev),
                            MensajeCorto = mensaje,
                            ArchivoOrigen = nombreArchivo
                        };

                        sbDetalle.AppendLine(linea);
                    }
                    else
                    {
                        // Línea adicional perteneciente a la entrada actual (stack trace, payload, etc.)
                        if (entradaActual != null)
                        {
                            sbDetalle.AppendLine(linea);
                        }
                    }
                }

                // Guardar la última entrada procesada
                if (entradaActual != null)
                {
                    entradaActual.DetalleCompleto = sbDetalle.ToString().TrimEnd();
                    entradas.Add(entradaActual);
                }
            }
            catch (Exception)
            {
                // Devolver las entradas parseadas hasta el momento en caso de error de E/S
            }

            // Extraer códigos de seguimiento / TraceId de cada entrada
            foreach (var item in entradas)
            {
                item.TraceId = ExtraerTraceId(item.MensajeCorto, item.DetalleCompleto);
            }

            // Aplicar filtros
            IEnumerable<EntradaLogItem> query = entradas;

            if (!string.IsNullOrWhiteSpace(filtroNivel) && !filtroNivel.Equals("Todos", StringComparison.OrdinalIgnoreCase))
            {
                query = filtroNivel switch
                {
                    "Errores" or "Solo Errores" => query.Where(e => e.EsError),
                    "Advertencias" or "Solo Advertencias" => query.Where(e => e.EsAdvertencia),
                    "Errores y Advertencias" => query.Where(e => e.EsError || e.EsAdvertencia),
                    "Información" => query.Where(e => string.Equals(e.Nivel, "Información", StringComparison.OrdinalIgnoreCase)),
                    _ => query
                };
            }

            if (!string.IsNullOrWhiteSpace(filtroTexto))
            {
                var term = filtroTexto.Trim();
                query = query.Where(e =>
                    e.MensajeCorto.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    e.DetalleCompleto.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    (e.TraceId != null && e.TraceId.Contains(term, StringComparison.OrdinalIgnoreCase)));
            }

            // Devolver las entradas ordenadas cronológicamente en forma descendente (los más recientes primero)
            return query.OrderByDescending(e => e.FechaHora).ToList();
        });
    }

    private static string NormalizarNivel(string nivelAbrev) => nivelAbrev.ToUpperInvariant() switch
    {
        "ERR" => "Error",
        "FTL" => "Fatal",
        "WRN" => "Advertencia",
        "INF" => "Información",
        "DBG" => "Depuración",
        "VRB" => "Detallado",
        _ => nivelAbrev
    };

    private static string? ExtraerTraceId(string mensaje, string detalle)
    {
        var match = RegexTraceId.Match(mensaje);
        if (match.Success)
        {
            return match.Groups[1].Value;
        }

        match = RegexTraceId.Match(detalle);
        if (match.Success)
        {
            return match.Groups[1].Value;
        }

        var matchCorr = RegexCorrelacionBrackets.Match(mensaje);
        if (matchCorr.Success)
        {
            return matchCorr.Groups[1].Value;
        }

        matchCorr = RegexCorrelacionBrackets.Match(detalle);
        return matchCorr.Success ? matchCorr.Groups[1].Value : null;
    }
}
