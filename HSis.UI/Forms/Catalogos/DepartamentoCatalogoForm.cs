#nullable enable
using System.Runtime.Versioning;
using HSis.Contracts.DTOs;
using HSis.Contracts.Services;
using HSis.UI.Controls;
using HSis.UI.Helpers;

namespace HSis.UI.Forms.Catalogos;

[SupportedOSPlatform("windows")]
public sealed class DepartamentoCatalogoForm : FormularioCatalogoBase
{
    private readonly IDepartamentoService _service;
    private readonly CajaTextoModerna _txtNombre = new() { Placeholder = "Nombre del departamento" };
    private readonly CajaTextoModerna _txtDescripcion = new() { Multiline = true, Placeholder = "Descripción opcional" };
    private int? _idDepartamento;

    public DepartamentoCatalogoForm(IDepartamentoService service) : base("Nuevo departamento", 300)
    {
        _service = service;
        AgregarCampo("Nombre", _txtNombre);
        AgregarCampo("Descripción", _txtDescripcion, 82);
        BtnGuardar.Click += async (_, _) => await GuardarAsync();
    }

    public void Editar(DepartamentoDto departamento)
    {
        _idDepartamento = departamento.IdDepartamento;
        _txtNombre.Text = departamento.Nombre;
        _txtDescripcion.Text = departamento.Descripcion ?? string.Empty;
        ConfigurarModoEdicion("departamento");
    }

    private async Task GuardarAsync()
    {
        if (string.IsNullOrWhiteSpace(_txtNombre.Text))
        {
            MostrarRequerido("Nombre", _txtNombre);
            return;
        }

        var request = new DepartamentoCatalogoRequestDto
        {
            Nombre = _txtNombre.Text.Trim(),
            Descripcion = string.IsNullOrWhiteSpace(_txtDescripcion.Text) ? null : _txtDescripcion.Text.Trim()
        };
        await this.EjecutarOperacionAsync(async () =>
        {
            if (_idDepartamento.HasValue)
            {
                _ = (await _service.ActualizarDepartamentoAsync(_idDepartamento.Value, request))
                    ?? throw new KeyNotFoundException("El departamento ya no existe.");
            }
            else
            {
                await _service.CrearDepartamentoAsync(request);
            }

            DialogResult = DialogResult.OK;
            Close();
        }, "Error al guardar el departamento", "departamento-guardar", BtnGuardar, BtnCancelar);
    }
}
