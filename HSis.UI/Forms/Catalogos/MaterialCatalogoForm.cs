#nullable enable
using System.Runtime.Versioning;
using HSis.Contracts.DTOs;
using HSis.Contracts.Services;
using HSis.UI.Controls;
using HSis.UI.Helpers;

namespace HSis.UI.Forms.Catalogos;

[SupportedOSPlatform("windows")]
public sealed class MaterialCatalogoForm : FormularioCatalogoBase
{
    private readonly IMaterialService _service;
    private readonly CajaTextoModerna _txtNombre = new() { Placeholder = "Nombre del material" };
    private readonly NumericUpDown _numCosto = new() { DecimalPlaces = 2, Minimum = 0, Maximum = 1000000000, Increment = 0.01m, ThousandsSeparator = true };
    private readonly NumericUpDown _numInventario = new() { Minimum = 0, Maximum = 1000000000, ThousandsSeparator = true };
    private readonly CajaTextoModerna _txtUnidad = new() { Placeholder = "Pieza, metro, caja..." };
    private int? _idMaterial;

    public MaterialCatalogoForm(IMaterialService service) : base("Nuevo material", 330)
    {
        _service = service;
        AgregarCampo("Nombre", _txtNombre);
        AgregarCampo("Costo unitario", _numCosto);
        AgregarCampo("Existencias", _numInventario);
        AgregarCampo("Unidad de medida", _txtUnidad);
        BtnGuardar.Click += async (_, _) => await GuardarAsync();
    }

    public void Editar(MaterialDto material)
    {
        _idMaterial = material.IdMaterial;
        _txtNombre.Text = material.Nombre;
        _numCosto.Value = Math.Clamp(material.CostoUnitario, _numCosto.Minimum, _numCosto.Maximum);
        _numInventario.Value = Math.Clamp(material.StockActual, (int)_numInventario.Minimum, (int)_numInventario.Maximum);
        _txtUnidad.Text = material.UnidadMedida;
        ConfigurarModoEdicion("material");
    }

    private async Task GuardarAsync()
    {
        if (string.IsNullOrWhiteSpace(_txtNombre.Text))
        {
            MostrarRequerido("Nombre", _txtNombre);
            return;
        }

        if (string.IsNullOrWhiteSpace(_txtUnidad.Text))
        {
            MostrarRequerido("Unidad de medida", _txtUnidad);
            return;
        }

        var request = new MaterialCatalogoRequestDto
        {
            Nombre = _txtNombre.Text.Trim(),
            CostoUnitario = _numCosto.Value,
            StockActual = (int)_numInventario.Value,
            UnidadMedida = _txtUnidad.Text.Trim()
        };

        await this.EjecutarOperacionAsync(async () =>
        {
            if (_idMaterial.HasValue)
            {
                _ = (await _service.ActualizarMaterialAsync(_idMaterial.Value, request))
                    ?? throw new KeyNotFoundException("El material ya no existe.");
            }
            else
            {
                await _service.CrearMaterialAsync(request);
            }

            DialogResult = DialogResult.OK;
            Close();
        }, "Error al guardar el material", BtnGuardar, BtnCancelar);
    }
}
