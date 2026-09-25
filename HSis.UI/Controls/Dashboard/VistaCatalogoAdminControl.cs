#nullable enable
using System.Runtime.Versioning;
using FontAwesome.Sharp;
using HSis.UI.Helpers;

namespace HSis.UI.Controls;

[SupportedOSPlatform("windows")]
public sealed class VistaCatalogoAdminControl : UserControl
{
    private readonly DataGridView _grid;
    private readonly BotonModerno _btnActualizar;
    private readonly BotonModerno _btnNuevo;
    private readonly BotonModerno _btnEditar;
    private readonly BotonModerno _btnEliminar;
    private bool _cargando;

    public VistaCatalogoAdminControl(string titulo)
    {
        Name = $"vista{titulo.Replace(" ", string.Empty)}";
        Dock = DockStyle.Fill;
        BackColor = TemaVisual.FondoApp;
        Padding = new Padding(12);

        var lblTitulo = new Label
        {
            Dock = DockStyle.Top,
            Height = 30,
            Text = titulo,
            Font = new Font("Segoe UI", 13f, FontStyle.Bold),
            ForeColor = TemaVisual.TextoPrincipal,
            TextAlign = ContentAlignment.MiddleLeft
        };

        _grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            Name = $"dgv{titulo.Replace(" ", string.Empty)}",
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            MultiSelect = false,
            AutoGenerateColumns = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            RowHeadersVisible = false,
            BackgroundColor = TemaVisual.FondoTarjeta,
            BorderStyle = BorderStyle.None
        };
        _grid.AplicarTemaModerno();
        _grid.SelectionChanged += (_, _) => ActualizarEstadoBotones();
        _grid.CellDoubleClick += (_, args) =>
        {
            if (args.RowIndex >= 0)
            {
                EditarClic?.Invoke(this, EventArgs.Empty);
            }
        };

        _btnActualizar = CrearBoton("Actualizar", IconChar.Rotate, EstiloBotonModerno.Secundario, 130);
        _btnNuevo = CrearBoton("Nuevo", IconChar.Plus, EstiloBotonModerno.Exito, 120);
        _btnEditar = CrearBoton("Editar", IconChar.Pen, EstiloBotonModerno.Primario, 120);
        _btnEliminar = CrearBoton("Eliminar", IconChar.Trash, EstiloBotonModerno.Peligro, 130);
        _btnActualizar.Click += (_, _) => ActualizarClic?.Invoke(this, EventArgs.Empty);
        _btnNuevo.Click += (_, _) => NuevoClic?.Invoke(this, EventArgs.Empty);
        _btnEditar.Click += (_, _) => EditarClic?.Invoke(this, EventArgs.Empty);
        _btnEliminar.Click += (_, _) => EliminarClic?.Invoke(this, EventArgs.Empty);

        var panelAcciones = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 48,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Padding = new Padding(0, 0, 0, 8),
            BackColor = TemaVisual.FondoApp
        };
        panelAcciones.Controls.Add(_btnActualizar);
        panelAcciones.Controls.Add(_btnNuevo);
        panelAcciones.Controls.Add(_btnEditar);
        panelAcciones.Controls.Add(_btnEliminar);

        Controls.Add(_grid);
        Controls.Add(panelAcciones);
        Controls.Add(lblTitulo);
        ActualizarEstadoBotones();
    }

    public DataGridView Grid => _grid;

    public bool EstaCargando
    {
        get => _cargando;
        set
        {
            _cargando = value;
            _btnActualizar.Enabled = !value;
            _btnNuevo.Enabled = !value;
            ActualizarEstadoBotones();
        }
    }

    public event EventHandler? ActualizarClic;
    public event EventHandler? NuevoClic;
    public event EventHandler? EditarClic;
    public event EventHandler? EliminarClic;

    public void ConfigurarColumnas(params (string Nombre, string Titulo, int Ancho)[] columnas)
    {
        _grid.Columns.Clear();
        foreach (var columna in columnas)
        {
            _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = columna.Nombre,
                HeaderText = columna.Titulo,
                FillWeight = Math.Max(50, columna.Ancho),
                MinimumWidth = Math.Min(Math.Max(50, columna.Ancho), 90),
                SortMode = DataGridViewColumnSortMode.NotSortable
            });
        }
    }

    public void CargarFilas(IEnumerable<FilaCatalogoAdmin> filas)
    {
        _grid.SuspendLayout();
        try
        {
            _grid.Rows.Clear();
            foreach (var fila in filas)
            {
                var rowIndex = _grid.Rows.Add(fila.Valores.ToArray());
                _grid.Rows[rowIndex].Tag = fila.Registro;
            }
        }
        finally
        {
            _grid.ResumeLayout();
            ActualizarEstadoBotones();
        }
    }

    public T? ObtenerSeleccionado<T>() where T : class
        => _grid.SelectedRows.Count == 0 ? null : _grid.SelectedRows[0].Tag as T;

    public object? ObtenerSeleccionado()
        => _grid.SelectedRows.Count == 0 ? null : _grid.SelectedRows[0].Tag;

    private void ActualizarEstadoBotones()
    {
        var habilitado = !_cargando && _grid.SelectedRows.Count > 0;
        _btnEditar.Enabled = habilitado;
        _btnEliminar.Enabled = habilitado;
    }

    private static BotonModerno CrearBoton(string texto, IconChar icono, EstiloBotonModerno estilo, int ancho)
        => new()
        {
            Text = texto,
            Icono = icono,
            IconoTamano = 14,
            Estilo = estilo,
            Size = new Size(ancho, 36),
            Margin = new Padding(0, 0, 8, 0)
        };
}

public sealed record FilaCatalogoAdmin(object Registro, IReadOnlyList<object?> Valores);
