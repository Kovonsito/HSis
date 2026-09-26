#nullable enable
using System.Runtime.Versioning;
using HSis.Contracts.DTOs;
using HSis.Contracts.Services;
using HSis.UI.Controls;
using HSis.UI.Helpers;

namespace HSis.UI.Forms.Catalogos;

[SupportedOSPlatform("windows")]
public sealed class SucursalCatalogoForm : FormularioCatalogoBase
{
    private readonly ISucursalService _service;
    private readonly IEmpresaService _empresaService;
    private readonly CajaTextoModerna _txtNombre = new() { Placeholder = "Nombre de la sucursal" };
    private readonly CajaTextoModerna _txtCalle = new() { Placeholder = "Calle" };
    private readonly CajaTextoModerna _txtNumero = new() { Placeholder = "Número" };
    private readonly CajaTextoModerna _txtColonia = new() { Placeholder = "Colonia" };
    private readonly CajaTextoModerna _txtTelefono = new() { Placeholder = "Teléfono" };
    private readonly ComboModerno _cmbEmpresa = new();
    private int? _idSucursal;
    private int? _empresaEnEdicion;

    public SucursalCatalogoForm(ISucursalService service, IEmpresaService empresaService) : base("Nueva sucursal", 430)
    {
        _service = service;
        _empresaService = empresaService;
        _cmbEmpresa.DropDownStyle = ComboBoxStyle.DropDownList;
        AgregarCampo("Nombre", _txtNombre);
        AgregarCampo("Calle", _txtCalle);
        AgregarCampo("Número", _txtNumero);
        AgregarCampo("Colonia", _txtColonia);
        AgregarCampo("Teléfono", _txtTelefono);
        AgregarCampo("Empresa", _cmbEmpresa);
        Load += async (_, _) => await CargarEmpresasAsync();
        BtnGuardar.Click += async (_, _) => await GuardarAsync();
    }

    public void Editar(SucursalDto sucursal)
    {
        _idSucursal = sucursal.IdSucursal;
        _empresaEnEdicion = sucursal.IdEmpresa;
        _txtNombre.Text = sucursal.Nombre;
        _txtCalle.Text = sucursal.Calle ?? string.Empty;
        _txtNumero.Text = sucursal.Numero ?? string.Empty;
        _txtColonia.Text = sucursal.Colonia ?? string.Empty;
        _txtTelefono.Text = sucursal.Telefono ?? string.Empty;
        ConfigurarModoEdicion("sucursal");
    }

    private async Task CargarEmpresasAsync()
    {
        await this.EjecutarOperacionAsync(async () =>
        {
            var empresas = await _empresaService.ObtenerEmpresasAsync();
            _cmbEmpresa.Items.Clear();
            foreach (var empresa in empresas.OrderBy(empresa => empresa.Nombre))
            {
                _cmbEmpresa.Items.Add(new ElementoCombo<int>(empresa.Nombre, empresa.IdEmpresa));
            }

            if (_empresaEnEdicion.HasValue)
            {
                SeleccionarEmpresa(_empresaEnEdicion.Value);
            }
        }, "Error al cargar las empresas", "sucursal-empresas", _cmbEmpresa);
    }

    private void SeleccionarEmpresa(int idEmpresa)
    {
        var elemento = _cmbEmpresa.Items
            .OfType<ElementoCombo<int>>()
            .FirstOrDefault(item => item.Valor == idEmpresa);
        _cmbEmpresa.SelectedItem = elemento;
    }

    private async Task GuardarAsync()
    {
        if (string.IsNullOrWhiteSpace(_txtNombre.Text))
        {
            MostrarRequerido("Nombre", _txtNombre);
            return;
        }

        if (_cmbEmpresa.SelectedItem is not ElementoCombo<int> empresa)
        {
            MostrarRequerido("Empresa", _cmbEmpresa);
            return;
        }

        var request = new SucursalCatalogoRequestDto
        {
            Nombre = _txtNombre.Text.Trim(),
            Calle = TextoOpcional(_txtCalle),
            Numero = TextoOpcional(_txtNumero),
            Colonia = TextoOpcional(_txtColonia),
            Telefono = TextoOpcional(_txtTelefono),
            IdEmpresa = empresa.Valor
        };
        await this.EjecutarOperacionAsync(async () =>
        {
            if (_idSucursal.HasValue)
            {
                _ = (await _service.ActualizarSucursalAsync(_idSucursal.Value, request))
                    ?? throw new KeyNotFoundException("La sucursal ya no existe.");
            }
            else
            {
                await _service.CrearSucursalAsync(request);
            }

            DialogResult = DialogResult.OK;
            Close();
        }, "Error al guardar la sucursal", "sucursal-guardar", BtnGuardar, BtnCancelar);
    }

    private static string? TextoOpcional(CajaTextoModerna control)
        => string.IsNullOrWhiteSpace(control.Text) ? null : control.Text.Trim();
}
