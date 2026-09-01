#nullable enable
using System;

namespace HSis.UI.Helpers
{
    public static class FormatoVisualHelper
    {
        public static string FormatearEstrellas(int? puntuacion, int maximo = 5)
        {
            int score = Math.Clamp(puntuacion ?? 0, 0, maximo);
            return new string('★', score) + new string('☆', maximo - score);
        }

        public static string TruncarTexto(string? texto, int longitudMaxima = 50)
        {
            if (string.IsNullOrEmpty(texto)) return string.Empty;
            return texto.Length > longitudMaxima
                ? string.Concat(texto.AsSpan(0, longitudMaxima), "...")
                : texto;
        }

        public static string FormatearFecha(DateTime? fecha, string formato = "dd/MM/yyyy HH:mm")
        {
            return fecha.HasValue ? fecha.Value.ToString(formato) : "N/A";
        }
    }

    public class ElementoCombo<T>(string texto, T valor)
    {
        public string Texto { get; set; } = texto;
        public T Valor { get; set; } = valor;

        public override string ToString()
        {
            return Texto;
        }
    }
}

