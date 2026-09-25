#nullable enable
using System.Runtime.Versioning;
using HSis.Contracts.DTOs;
using HSis.Contracts.Services;
using HSis.UI.Controls;
using HSis.UI.Helpers;

namespace HSis.UI.Forms.Catalogos;

[SupportedOSPlatform("windows")]
public sealed class PuestoCatalogoForm : FormularioCatalogoBase
{
    private readonly IPuestoService _service;
    private readonly CajaTextoModerna _txtNombre = new() { Placeholder = "Nombre del puesto" };
    private readonly CajaTextoModerna _txtDescripcion = new() { Multiline = true, Placeholder = "Descripción opcional" };
    private int? _idPuesto;

    public PuestoCatalogoForm(IPuestoService service) : base("Nuevo puesto", 300)
    {
        _service = service;
        AgregarCampo("Nombre", _txtNombre);
        AgregarCampo("Descripción", _txtDescripcion, 82);
        BtnGuardar.Click += async (_, _) => await GuardarAsync();
    }

    public void Editar(PuestoDto puesto)
    {
        _idPuesto = puesto.IdPuesto;
        _txtNombre.Text = puesto.Nombre;
        _txtDescripcion.Text = puesto.Descripcion ?? string.Empty;
        ConfigurarModoEdicion("puesto");
    }

    private async Task GuardarAsync()
    {
        if (string.IsNullOrWhiteSpace(_txtNombre.Text))
        {
            MostrarRequerido("Nombre", _txtNombre);
            return;
        }

        var request = new PuestoCatalogoRequestDto
        {
            Nombre = _txtNombre.Text.Trim(),
            Descripcion = string.IsNullOrWhiteSpace(_txtDescripcion.Text) ? null : _txtDescripcion.Text.Trim()
        };
        await this.EjecutarOperacionAsync(async () =>
        {
            if (_idPuesto.HasValue)
            {
                _ = (await _service.ActualizarPuestoAsync(_idPuesto.Value, request))
                    ?? throw new KeyNotFoundException("El puesto ya no existe.");
            }
            else
            {
                await _service.CrearPuestoAsync(request);
            }

            DialogResult = DialogResult.OK;
            Close();
        }, "Error al guardar el puesto", BtnGuardar, BtnCancelar);
    }
}
