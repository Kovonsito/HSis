#nullable enable
using System.Runtime.Versioning;
using HSis.Contracts.DTOs;
using HSis.Contracts.Services;
using HSis.UI.Controls;
using HSis.UI.Helpers;

namespace HSis.UI.Forms.Catalogos;

[SupportedOSPlatform("windows")]
public sealed class RolUsuarioCatalogoForm : FormularioCatalogoBase
{
    private readonly IRolUsuarioService _service;
    private readonly CajaTextoModerna _txtDescripcion = new() { Placeholder = "Descripción del rol" };
    private int? _idRol;

    public RolUsuarioCatalogoForm(IRolUsuarioService service) : base("Nuevo rol", 250)
    {
        _service = service;
        AgregarCampo("Descripción", _txtDescripcion);
        BtnGuardar.Click += async (_, _) => await GuardarAsync();
    }

    public void Editar(RolUsuarioDto rol)
    {
        _idRol = rol.IdRol;
        _txtDescripcion.Text = rol.Descripcion;
        ConfigurarModoEdicion("rol");
    }

    private async Task GuardarAsync()
    {
        if (string.IsNullOrWhiteSpace(_txtDescripcion.Text))
        {
            MostrarRequerido("Descripción", _txtDescripcion);
            return;
        }

        var request = new RolUsuarioCatalogoRequestDto { Descripcion = _txtDescripcion.Text.Trim() };
        await this.EjecutarOperacionAsync(async () =>
        {
            if (_idRol.HasValue)
            {
                _ = (await _service.ActualizarRolUsuarioAsync(_idRol.Value, request))
                    ?? throw new KeyNotFoundException("El rol ya no existe.");
            }
            else
            {
                await _service.CrearRolUsuarioAsync(request);
            }

            DialogResult = DialogResult.OK;
            Close();
        }, "Error al guardar el rol", "rol-guardar", BtnGuardar, BtnCancelar);
    }
}
