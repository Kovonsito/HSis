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
    private readonly CajaTextoModerna _txtNombre = new() { Placeholder = "Nombre de la empresa", MaxLength = 50 };
    private readonly CajaTextoModerna _txtCalle = new() { Placeholder = "Calle", MaxLength = 50 };
    private readonly CajaTextoModerna _txtNumero = new() { Placeholder = "Número", MaxLength = 10 };
    private readonly CajaTextoModerna _txtColonia = new() { Placeholder = "Colonia", MaxLength = 50 };
    private readonly CajaTextoModerna _txtTelefono = new() { Placeholder = "(664) 123-4567", EsFormatoTelefono = true };
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

        if (!_txtTelefono.EsTelefonoValido)
        {
            MostrarAdvertenciaValidacion("El teléfono debe contener 10 dígitos o dejarse en blanco.", _txtTelefono);
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
        }, "Error al guardar la empresa", "empresa-guardar", BtnGuardar, BtnCancelar);
    }

    private static string? TextoOpcional(CajaTextoModerna control)
        => string.IsNullOrWhiteSpace(control.Text) ? null : control.Text.Trim();
}
