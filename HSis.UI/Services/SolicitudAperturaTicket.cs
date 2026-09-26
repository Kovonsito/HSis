namespace HSis.UI.Services;

public sealed class SolicitudAperturaTicket
{
    private readonly object _sync = new();
    private readonly Queue<int> _ticketsPendientes = [];

    public SolicitudAperturaTicket(string[] argumentos)
    {
        Agregar(argumentos);
    }

    public void Agregar(IReadOnlyList<string> argumentos)
    {
        var ticketId = ObtenerTicketId(argumentos);
        if (!ticketId.HasValue)
        {
            return;
        }

        lock (_sync)
        {
            if (!_ticketsPendientes.Contains(ticketId.Value))
            {
                _ticketsPendientes.Enqueue(ticketId.Value);
            }
        }
    }

    public int? Consumir()
    {
        lock (_sync)
        {
            return _ticketsPendientes.Count == 0
                ? null
                : _ticketsPendientes.Dequeue();
        }
    }

    private static int? ObtenerTicketId(IReadOnlyList<string> argumentos)
    {
        for (var index = 0; index < argumentos.Count; index++)
        {
            var argumento = argumentos[index];
            if (string.Equals(argumento, "--open-ticket", StringComparison.OrdinalIgnoreCase) &&
                index + 1 < argumentos.Count &&
                int.TryParse(argumentos[index + 1], out var ticketId) &&
                ticketId > 0)
            {
                return ticketId;
            }

            const string prefijo = "--open-ticket=";
            if (argumento.StartsWith(prefijo, StringComparison.OrdinalIgnoreCase) &&
                int.TryParse(argumento[prefijo.Length..], out ticketId) &&
                ticketId > 0)
            {
                return ticketId;
            }
        }

        return null;
    }
}
