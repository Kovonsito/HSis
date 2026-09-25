#nullable enable
using System.Runtime.Versioning;
using System.Text.Json;
using FontAwesome.Sharp;
using HSis.Contracts.DTOs;
using HSis.Contracts.Services;
using HSis.UI.Helpers;

namespace HSis.UI.Controls;

[SupportedOSPlatform("windows")]
public sealed class CatalogoTabControl : UserControl
{
    private readonly CatalogoDefinicion _definicion;
    private readonly ICatalogoGestionService _catalogoService;
    private readonly DataGridView _grid;
    private readonly BotonModerno _btnActualizar;
    private readonly BotonModerno _btnNuevo;
    private readonly BotonModerno _btnEliminar;
    private IReadOnlyList<CatalogoRegistroDto> _registros = [];
    private bool _cargando;

    public CatalogoTabControl(CatalogoDefinicion definicion, ICatalogoGestionService catalogoService)
    {
        _definicion = definicion ?? throw new ArgumentNullException(nameof(definicion));
        _catalogoService = catalogoService ?? throw new ArgumentNullException(nameof(catalogoService));

        BackColor = TemaVisual.FondoApp;
        Dock = DockStyle.Fill;
        Padding = new Padding(12);

        _grid = CrearGrid();
        _btnActualizar = CrearBoton("Actualizar", IconChar.Rotate, EstiloBotonModerno.Secundario, 130);
        _btnNuevo = CrearBoton("Nuevo", IconChar.Plus, EstiloBotonModerno.Exito, 120);
        _btnEliminar = CrearBoton("Eliminar", IconChar.Trash, EstiloBotonModerno.Peligro, 130);

        _btnActualizar.Click += async (_, _) => await CargarAsync();
        _btnNuevo.Click += (_, _) => NuevoClic?.Invoke(this, EventArgs.Empty);
        _btnEliminar.Click += async (_, _) => await EliminarSeleccionadoAsync();
        _grid.SelectionChanged += (_, _) => ActualizarEstadoEliminar();

        var panelAcciones = CrearPanelAcciones();
        Controls.Add(_grid);
        Controls.Add(panelAcciones);

        ActualizarEstadoEliminar();
    }

    public CatalogoDefinicion Definicion => _definicion;

    public DataGridView Grid => _grid;

    public string? ClaveSeleccionada => ObtenerClave(ObtenerRegistroSeleccionado());

    public event EventHandler? NuevoClic;

    public event EventHandler? RegistroEliminado;

    public async Task CargarAsync()
    {
        if (IsDisposed || _cargando)
        {
            return;
        }

        EstablecerEstadoCarga(true);
        try
        {
            var registros = await _catalogoService.ObtenerRegistrosAsync(_definicion.EntidadApi);
            if (IsDisposed)
            {
                return;
            }

            _registros = registros ?? [];
            _grid.SuspendLayout();
            _grid.Rows.Clear();

            foreach (var registro in _registros)
            {
                var fila = _grid.Rows[_grid.Rows.Add()];
                fila.Tag = registro;

                foreach (var columna in _definicion.Columnas)
                {
                    fila.Cells[columna.Propiedad].Value = ConvertirParaMostrar(registro.Obtener(columna.Propiedad));
                }
            }
        }
        catch (Exception ex)
        {
            DialogoUIHelper.MostrarExcepcion(
                ex,
                $"No se pudo cargar el catálogo de {_definicion.Titulo.ToLowerInvariant()}");
        }
        finally
        {
            _grid.ResumeLayout();
            EstablecerEstadoCarga(false);
        }
    }

    private DataGridView CrearGrid()
    {
        var grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            Name = $"dgv{_definicion.Clave}",
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            MultiSelect = false,
            AutoGenerateColumns = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            RowHeadersVisible = false,
            BackgroundColor = TemaVisual.FondoTarjeta,
            BorderStyle = BorderStyle.None,
            Margin = new Padding(0, 8, 0, 0)
        };
        grid.AplicarTemaModerno();

        foreach (var columna in _definicion.Columnas)
        {
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = columna.Propiedad,
                HeaderText = columna.Titulo,
                FillWeight = Math.Max(50, columna.Ancho),
                MinimumWidth = Math.Min(Math.Max(50, columna.Ancho), 90),
                SortMode = DataGridViewColumnSortMode.Automatic
            });
        }

        return grid;
    }

    private Panel CrearPanelAcciones()
    {
        var panel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 52,
            BackColor = TemaVisual.FondoApp,
            Padding = new Padding(0, 0, 0, 8)
        };

        _btnActualizar.Location = new Point(0, 0);
        _btnNuevo.Location = new Point(_btnActualizar.Right + 8, 0);
        _btnEliminar.Location = new Point(_btnNuevo.Right + 8, 0);

        panel.Controls.Add(_btnActualizar);
        panel.Controls.Add(_btnNuevo);
        panel.Controls.Add(_btnEliminar);
        panel.Paint += (_, e) =>
        {
            using var pen = new Pen(TemaVisual.BordeSutil);
            e.Graphics.DrawLine(pen, 0, panel.Height - 9, panel.Width, panel.Height - 9);
        };

        return panel;
    }

    private static BotonModerno CrearBoton(
        string texto,
        IconChar icono,
        EstiloBotonModerno estilo,
        int ancho)
    {
        return new BotonModerno
        {
            Text = texto,
            Icono = icono,
            IconoTamano = 14,
            Estilo = estilo,
            Size = new Size(ancho, 36),
            Margin = new Padding(0, 0, 8, 0)
        };
    }

    private async Task EliminarSeleccionadoAsync()
    {
        var registro = ObtenerRegistroSeleccionado();
        var clave = ObtenerClave(registro);
        if (registro == null || string.IsNullOrWhiteSpace(clave))
        {
            DialogoUIHelper.MostrarAdvertencia("Seleccione un registro para eliminar.");
            return;
        }

        if (!DialogoUIHelper.Confirmar(
                $"¿Desea eliminar el registro con clave {clave} del catálogo de {_definicion.Titulo.ToLowerInvariant()}?",
                "Eliminar registro"))
        {
            return;
        }

        EstablecerEstadoCarga(true);
        try
        {
            await _catalogoService.EliminarRegistroAsync(_definicion.EntidadApi, clave);
            await CargarAsync();
            RegistroEliminado?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            DialogoUIHelper.MostrarExcepcion(ex, "No se pudo eliminar el registro seleccionado");
        }
        finally
        {
            EstablecerEstadoCarga(false);
        }
    }

    private CatalogoRegistroDto? ObtenerRegistroSeleccionado()
    {
        return _grid.SelectedRows.Count == 0
            ? null
            : _grid.SelectedRows[0].Tag as CatalogoRegistroDto;
    }

    private string? ObtenerClave(CatalogoRegistroDto? registro)
    {
        return registro == null
            ? null
            : ConvertirParaMostrar(registro.Obtener(_definicion.ClavePrimaria));
    }

    private void ActualizarEstadoEliminar()
    {
        _btnEliminar.Enabled = !_cargando && !string.IsNullOrWhiteSpace(ClaveSeleccionada);
    }

    private void EstablecerEstadoCarga(bool cargando)
    {
        _cargando = cargando;
        _btnActualizar.Enabled = !cargando;
        _btnNuevo.Enabled = !cargando;
        _grid.Enabled = !cargando;
        ActualizarEstadoEliminar();
        Cursor = cargando ? Cursors.WaitCursor : Cursors.Default;
    }

    private static string? ConvertirParaMostrar(object? valor)
    {
        if (valor == null)
        {
            return null;
        }

        if (valor is JsonElement elemento)
        {
            return elemento.ValueKind switch
            {
                JsonValueKind.Null or JsonValueKind.Undefined => null,
                JsonValueKind.String => elemento.GetString(),
                JsonValueKind.True => bool.TrueString,
                JsonValueKind.False => bool.FalseString,
                _ => elemento.ToString()
            };
        }

        return Convert.ToString(valor, System.Globalization.CultureInfo.CurrentCulture);
    }
}
