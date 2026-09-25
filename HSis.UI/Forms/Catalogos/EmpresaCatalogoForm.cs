#nullable enable
using System.Runtime.Versioning;
using HSis.Contracts.DTOs;
using HSis.Contracts.Services;
using HSis.UI.Controls;
using HSis.UI.Helpers;

namespace HSis.UI.Forms.Catalogos;

[SupportedOSPlatform("windows")]
public sealed class EmpresaCatalogoForm : FormularioCatalogoBase
{
    private readonly IEmpresaService _service;
    private readonly CajaTextoModerna _txtNombre = new() { Placeholder = "Nombre de la empresa" };
    private readonly CajaTextoModerna _txtCalle = new() { Placeholder = "Calle" };
    private readonly CajaTextoModerna _txtNumero = new() { Placeholder = "Número" };
    private readonly CajaTextoModerna _txtColonia = new() { Placeholder = "Colonia" };
    private readonly CajaTextoModerna _txtTelefono = new() { Placeholder = "Teléfono" };
    private int? _idEmpresa;

    public EmpresaCatalogoForm(IEmpresaService service) : base("Nueva empresa", 390)
    {
        _service = service;
        AgregarCampo("Nombre", _txtNombre);
        AgregarCampo("Calle", _txtCalle);
        AgregarCampo("Número", _txtNumero);
        AgregarCampo("Colonia", _txtColonia);
        AgregarCampo("Teléfono", _txtTelefono);
        BtnGuardar.Click += async (_, _) => await GuardarAsync();
    }

    public void Editar(EmpresaDto empresa)
    {
        _idEmpresa = empresa.IdEmpresa;
        _txtNombre.Text = empresa.Nombre;
        _txtCalle.Text = empresa.Calle ?? string.Empty;
        _txtNumero.Text = empresa.Numero ?? string.Empty;
        _txtColonia.Text = empresa.Colonia ?? string.Empty;
        _txtTelefono.Text = empresa.Telefono ?? string.Empty;
        ConfigurarModoEdicion("empresa");
    }

    private async Task GuardarAsync()
    {
        if (string.IsNullOrWhiteSpace(_txtNombre.Text))
        {
            MostrarRequerido("Nombre", _txtNombre);
            return;
        }

        var request = new EmpresaCatalogoRequestDto
        {
            Nombre = _txtNombre.Text.Trim(),
            Calle = TextoOpcional(_txtCalle),
            Numero = TextoOpcional(_txtNumero),
            Colonia = TextoOpcional(_txtColonia),
            Telefono = TextoOpcional(_txtTelefono)
        };
        await this.EjecutarOperacionAsync(async () =>
        {
            if (_idEmpresa.HasValue)
            {
                _ = (await _service.ActualizarEmpresaAsync(_idEmpresa.Value, request))
                    ?? throw new KeyNotFoundException("La empresa ya no existe.");
            }
            else
            {
                await _service.CrearEmpresaAsync(request);
            }

            DialogResult = DialogResult.OK;
            Close();
        }, "Error al guardar la empresa", BtnGuardar, BtnCancelar);
    }

    private static string? TextoOpcional(CajaTextoModerna control)
        => string.IsNullOrWhiteSpace(control.Text) ? null : control.Text.Trim();
}
