#nullable enable
using System.Runtime.Versioning;
using HSis.Contracts.DTOs;
using HSis.Contracts.Services;
using HSis.UI.Controls;
using HSis.UI.Helpers;

namespace HSis.UI.Forms.Catalogos;

[SupportedOSPlatform("windows")]
public sealed class UsuarioCatalogoForm : FormularioCatalogoBase
{
    private readonly IUsuarioService _service;
    private readonly IDepartamentoService _departamentoService;
    private readonly IPuestoService _puestoService;
    private readonly ISucursalService _sucursalService;
    private readonly IRolUsuarioService _rolService;
    private readonly CajaTextoModerna _txtNombre = new() { Placeholder = "Nombre de usuario" };
    private readonly CajaTextoModerna _txtContraseña = new() { Placeholder = "Contraseña", UseSystemPasswordChar = true };
    private readonly ComboModerno _cmbDepartamento = new();
    private readonly ComboModerno _cmbPuesto = new();
    private readonly ComboModerno _cmbSucursal = new();
    private readonly ComboModerno _cmbRol = new();
    private int? _idUsuario;
    private int? _departamentoEnEdicion;
    private int? _puestoEnEdicion;
    private int? _sucursalEnEdicion;
    private int? _rolEnEdicion;

    public UsuarioCatalogoForm(
        IUsuarioService service,
        IDepartamentoService departamentoService,
        IPuestoService puestoService,
        ISucursalService sucursalService,
        IRolUsuarioService rolService) : base("Nuevo usuario", 520)
    {
        _service = service;
        _departamentoService = departamentoService;
        _puestoService = puestoService;
        _sucursalService = sucursalService;
        _rolService = rolService;

        _cmbDepartamento.DropDownStyle = ComboBoxStyle.DropDownList;
        _cmbPuesto.DropDownStyle = ComboBoxStyle.DropDownList;
        _cmbSucursal.DropDownStyle = ComboBoxStyle.DropDownList;
        _cmbRol.DropDownStyle = ComboBoxStyle.DropDownList;
        AgregarCampo("Nombre", _txtNombre);
        AgregarCampo("Contraseña", _txtContraseña);
        AgregarCampo("Departamento", _cmbDepartamento);
        AgregarCampo("Puesto", _cmbPuesto);
        AgregarCampo("Sucursal", _cmbSucursal);
        AgregarCampo("Rol", _cmbRol);
        Load += async (_, _) => await CargarCatalogosAsync();
        BtnGuardar.Click += async (_, _) => await GuardarAsync();
    }

    public void Editar(UsuarioDto usuario)
    {
        _idUsuario = usuario.IdUsuario;
        _departamentoEnEdicion = usuario.IdDepartamento;
        _puestoEnEdicion = usuario.IdPuesto;
        _sucursalEnEdicion = usuario.IdSucursal;
        _rolEnEdicion = usuario.IdRol;
        _txtNombre.Text = usuario.Nombre ?? string.Empty;
        ConfigurarModoEdicion("usuario");
    }

    private async Task CargarCatalogosAsync()
    {
        await this.EjecutarOperacionAsync(async () =>
        {
            var departamentosTask = _departamentoService.ObtenerDepartamentosAsync();
            var puestosTask = _puestoService.ObtenerPuestosAsync();
            var sucursalesTask = _sucursalService.ObtenerSucursalesAsync();
            var rolesTask = _rolService.ObtenerRolesUsuarioAsync();
            await Task.WhenAll(departamentosTask, puestosTask, sucursalesTask, rolesTask);

            CargarDepartamentos(await departamentosTask);
            CargarPuestos(await puestosTask);
            CargarSucursales(await sucursalesTask);
            CargarRoles(await rolesTask);
        }, "Error al cargar los catálogos del usuario", _cmbDepartamento, _cmbPuesto, _cmbSucursal, _cmbRol);
    }

    private void CargarDepartamentos(IEnumerable<DepartamentoDto> departamentos)
    {
        _cmbDepartamento.Items.Clear();
        _cmbDepartamento.Items.Add(new ElementoCombo<int?>("-- Sin departamento --", null));
        foreach (var departamento in departamentos.OrderBy(item => item.Nombre))
        {
            _cmbDepartamento.Items.Add(new ElementoCombo<int?>(departamento.Nombre, departamento.IdDepartamento));
        }
        SeleccionarNullable(_cmbDepartamento, _departamentoEnEdicion);
    }

    private void CargarPuestos(IEnumerable<PuestoDto> puestos)
    {
        _cmbPuesto.Items.Clear();
        _cmbPuesto.Items.Add(new ElementoCombo<int?>("-- Sin puesto --", null));
        foreach (var puesto in puestos.OrderBy(item => item.Nombre))
        {
            _cmbPuesto.Items.Add(new ElementoCombo<int?>(puesto.Nombre, puesto.IdPuesto));
        }
        SeleccionarNullable(_cmbPuesto, _puestoEnEdicion);
    }

    private void CargarSucursales(IEnumerable<SucursalDto> sucursales)
    {
        _cmbSucursal.Items.Clear();
        _cmbSucursal.Items.Add(new ElementoCombo<int?>("-- Sin sucursal --", null));
        foreach (var sucursal in sucursales.OrderBy(item => item.Nombre))
        {
            _cmbSucursal.Items.Add(new ElementoCombo<int?>(sucursal.Nombre, sucursal.IdSucursal));
        }
        SeleccionarNullable(_cmbSucursal, _sucursalEnEdicion);
    }

    private void CargarRoles(IEnumerable<RolUsuarioDto> roles)
    {
        _cmbRol.Items.Clear();
        foreach (var rol in roles.OrderBy(item => item.Descripcion))
        {
            _cmbRol.Items.Add(new ElementoCombo<int?>(rol.Descripcion, rol.IdRol));
        }
        SeleccionarNullable(_cmbRol, _rolEnEdicion);
    }

    private async Task GuardarAsync()
    {
        if (string.IsNullOrWhiteSpace(_txtNombre.Text))
        {
            MostrarRequerido("Nombre", _txtNombre);
            return;
        }

        if (!_idUsuario.HasValue && string.IsNullOrWhiteSpace(_txtContraseña.Text))
        {
            MostrarRequerido("Contraseña", _txtContraseña);
            return;
        }

        if (_cmbRol.SelectedItem is not ElementoCombo<int?> rol || !rol.Valor.HasValue)
        {
            MostrarRequerido("Rol", _cmbRol);
            return;
        }

        var request = new UsuarioCatalogoRequestDto
        {
            Nombre = _txtNombre.Text.Trim(),
            Contraseña = string.IsNullOrWhiteSpace(_txtContraseña.Text) ? null : _txtContraseña.Text,
            IdDepartamento = ObtenerValor(_cmbDepartamento),
            IdPuesto = ObtenerValor(_cmbPuesto),
            IdSucursal = ObtenerValor(_cmbSucursal),
            IdRol = rol.Valor
        };
        await this.EjecutarOperacionAsync(async () =>
        {
            if (_idUsuario.HasValue)
            {
                _ = (await _service.ActualizarUsuarioAsync(_idUsuario.Value, request))
                    ?? throw new KeyNotFoundException("El usuario ya no existe.");
            }
            else
            {
                await _service.CrearUsuarioAsync(request);
            }

            DialogResult = DialogResult.OK;
            Close();
        }, "Error al guardar el usuario", BtnGuardar, BtnCancelar);
    }

    private static void SeleccionarNullable(ComboModerno combo, int? valor)
    {
        var elemento = combo.Items
            .OfType<ElementoCombo<int?>>()
            .FirstOrDefault(item => item.Valor == valor);
        combo.SelectedItem = elemento ?? combo.Items.OfType<ElementoCombo<int?>>().FirstOrDefault();
    }

    private static int? ObtenerValor(ComboModerno combo)
        => (combo.SelectedItem as ElementoCombo<int?>)?.Valor;
}
