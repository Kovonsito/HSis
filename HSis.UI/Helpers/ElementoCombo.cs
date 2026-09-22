#nullable enable

namespace HSis.UI.Helpers
{
    /// <summary>
    /// Representa un elemento clave-valor genérico para enlazar en controles ComboBox de WinForms.
    /// </summary>
    /// <typeparam name="T">Tipo del valor subyacente.</typeparam>
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
