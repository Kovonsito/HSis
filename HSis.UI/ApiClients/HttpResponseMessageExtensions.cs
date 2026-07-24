using System.Net.Http;
using System.Text.Json;

namespace HSis.UI.ApiClients
{
    public static class HttpResponseMessageExtensions
    {
        public static async Task EnsureSuccessStatusCodeWithDetailsAsync(this HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode) return;

            string rawContent = await response.Content.ReadAsStringAsync();
            string mensajeDetallado = rawContent;

            if (!string.IsNullOrWhiteSpace(rawContent))
            {
                try
                {
                    using var doc = JsonDocument.Parse(rawContent);
                    var root = doc.RootElement;

                    if (root.ValueKind == JsonValueKind.Object)
                    {
                        if (root.TryGetProperty("Detalles", out var detallesProp) && detallesProp.ValueKind == JsonValueKind.Array)
                        {
                            var list = detallesProp.EnumerateArray().Select(x => x.GetString()).Where(x => !string.IsNullOrEmpty(x)).ToList();
                            if (list.Count > 0)
                            {
                                string head = root.TryGetProperty("Error", out var eProp) ? eProp.GetString()! : "Error de validación";
                                mensajeDetallado = $"{head}:\n" + string.Join("\n", list.Select(e => "- " + e));
                            }
                        }
                        else if (root.TryGetProperty("Detalle", out var detalleProp) && !string.IsNullOrWhiteSpace(detalleProp.GetString()))
                        {
                            mensajeDetallado = detalleProp.GetString()!;
                        }
                        else if (root.TryGetProperty("Error", out var errorProp) && !string.IsNullOrWhiteSpace(errorProp.GetString()))
                        {
                            mensajeDetallado = errorProp.GetString()!;
                        }
                        else if (root.TryGetProperty("Message", out var messageProp) && !string.IsNullOrWhiteSpace(messageProp.GetString()))
                        {
                            mensajeDetallado = messageProp.GetString()!;
                        }
                        else if (root.TryGetProperty("message", out var msgProp) && !string.IsNullOrWhiteSpace(msgProp.GetString()))
                        {
                            mensajeDetallado = msgProp.GetString()!;
                        }
                    }
                }
                catch
                {
                    // Si no es JSON válido, retenemos la cadena bruta
                }
            }

            throw new HttpRequestException($"Response status code does not indicate success: {(int)response.StatusCode} ({response.ReasonPhrase}). Detalle: {mensajeDetallado}");
        }
    }
}
