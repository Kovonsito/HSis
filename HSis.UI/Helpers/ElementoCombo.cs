namespace HSis.UI.Helpers
{
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
