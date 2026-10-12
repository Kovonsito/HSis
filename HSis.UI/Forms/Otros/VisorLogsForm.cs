#nullable enable
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.Versioning;
using FontAwesome.Sharp;
using HSis.UI.Controls;
using HSis.UI.Helpers;
using HSis.UI.Models;
using HSis.UI.Services;

namespace HSis.UI.Forms.Otros;

[SupportedOSPlatform("windows")]
public partial class VisorLogsForm : Form
{
    private readonly LectorLogsService _lectorLogsService;
    private readonly ComboModerno _cmbArchivos = new();
    private readonly ComboModerno _cmbNiveles = new();
    private readonly CajaTextoModerna _txtBuscar = new() { Placeholder = "Buscar texto o código de seguimiento / TraceId..." };
    private readonly BotonModerno _btnActualizar = new() { Text = "Actualizar", Icono = IconChar.RotateRight, IconoTamano = 14, Size = new Size(115, 36) };
    private readonly BotonModerno _btnAbrirCarpeta = new() { Text = "Abrir carpeta", Icono = IconChar.FolderOpen, IconoTamano = 14, Estilo = EstiloBotonModerno.Secundario, Size = new Size(130, 36) };
    private readonly BotonModerno _btnCopiarDetalle = new() { Text = "Copiar traza", Icono = IconChar.Copy, IconoTamano = 14, Estilo = EstiloBotonModerno.Secundario, Size = new Size(120, 32) };
    private readonly DataGridView _dgvLogs = new();
    private readonly TextBox _txtDetalle = new();
    private readonly Label _lblInfoSeleccion = new();
    private readonly Label _lblEstadoTotal = new();

    private List<EntradaLogItem> _entradasCargadas = [];

    public VisorLogsForm(LectorLogsService lectorLogsService)
    {
        _lectorLogsService = lectorLogsService;
        InitializeComponent();
        ConfigurarEstilosYEventos();
    }

    private void InitializeComponent()
    {
        Text = "Visor de Eventos y Diagnóstico de Logs";
        Name = "VisorLogsForm";
        StartPosition = FormStartPosition.CenterParent;
        Size = new Size(1140, 720);
        MinimumSize = new Size(860, 520);
        BackColor = TemaVisual.FondoApp;
        Font = TemaVisual.FuenteNormal;

        // Panel Principal
        var pnlPrincipal = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(16),
            BackColor = TemaVisual.FondoApp
        };

        // Encabezado Superior
        var pnlEncabezado = new Panel
        {
            Dock = DockStyle.Top,
            Height = 54,
            BackColor = Color.Transparent
        };

        var lblTitulo = new Label
        {
            Text = "Visor de Eventos y Diagnóstico del Sistema",
            Font = new Font("Segoe UI", 13.5f, FontStyle.Bold),
            ForeColor = TemaVisual.TextoPrincipal,
            AutoSize = true,
            Location = new Point(0, 2)
        };

        var lblSubtitulo = new Label
        {
            Text = "Monitoree excepciones, registros de auditoría y rastree errores por código de seguimiento o TraceId.",
            Font = TemaVisual.FuentePequena,
            ForeColor = TemaVisual.TextoSecundario,
            AutoSize = true,
            Location = new Point(0, 28)
        };

        pnlEncabezado.Controls.Add(lblTitulo);
        pnlEncabezado.Controls.Add(lblSubtitulo);

        // Barra de Filtros
        var pnlFiltrosTarjeta = new PanelCardModerno
        {
            Dock = DockStyle.Top,
            Height = 64,
            BackColor = TemaVisual.FondoTarjeta,
            Padding = new Padding(12, 10, 12, 10),
            Margin = new Padding(0, 0, 0, 10)
        };

        var flpBarra = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            AutoScroll = false,
            BackColor = Color.Transparent
        };

        _cmbArchivos.DropDownStyle = ComboBoxStyle.DropDownList;
        _cmbArchivos.Width = 220;

        _cmbNiveles.DropDownStyle = ComboBoxStyle.DropDownList;
        _cmbNiveles.Width = 175;
        _cmbNiveles.Items.AddRange(["Todos", "Errores y Advertencias", "Solo Errores", "Solo Advertencias", "Información"]);
        _cmbNiveles.SelectedIndex = 1; // "Errores y Advertencias" por defecto

        _txtBuscar.Width = 320;
        _txtBuscar.Margin = new Padding(8, 0, 8, 0);

        flpBarra.Controls.Add(_cmbArchivos);
        flpBarra.Controls.Add(_cmbNiveles);
        flpBarra.Controls.Add(_txtBuscar);
        flpBarra.Controls.Add(_btnActualizar);
        flpBarra.Controls.Add(_btnAbrirCarpeta);
        pnlFiltrosTarjeta.Controls.Add(flpBarra);

        // SplitContainer para Grid arriba y Detalle abajo
        var split = new SplitContainer
        {
            Dock = DockStyle.Fill,
            Orientation = Orientation.Horizontal,
            SplitterDistance = 350,
            SplitterWidth = 6,
            BackColor = TemaVisual.BordeSutil
        };

        // Contenedor Grid (Arriba)
        var pnlGridTarjeta = new PanelCardModerno
        {
            Dock = DockStyle.Fill,
            BackColor = TemaVisual.FondoTarjeta,
            Padding = new Padding(10)
        };

        ConfigurarGrid();
        pnlGridTarjeta.Controls.Add(_dgvLogs);
        split.Panel1.Controls.Add(pnlGridTarjeta);

        // Contenedor Detalle (Abajo)
        var pnlDetalleTarjeta = new PanelCardModerno
        {
            Dock = DockStyle.Fill,
            BackColor = TemaVisual.FondoTarjeta,
            Padding = new Padding(12)
        };

        var pnlDetalleBarra = new Panel
        {
            Dock = DockStyle.Top,
            Height = 36,
            BackColor = Color.Transparent
        };

        _lblInfoSeleccion.Text = "Seleccione un evento para inspeccionar la traza técnica completa";
        _lblInfoSeleccion.Font = TemaVisual.FuenteSubtitulo;
        _lblInfoSeleccion.ForeColor = TemaVisual.TextoOscuro;
        _lblInfoSeleccion.AutoSize = true;
        _lblInfoSeleccion.Location = new Point(0, 8);

        _btnCopiarDetalle.Dock = DockStyle.Right;

        pnlDetalleBarra.Controls.Add(_lblInfoSeleccion);
        pnlDetalleBarra.Controls.Add(_btnCopiarDetalle);

        _txtDetalle.Dock = DockStyle.Fill;
        _txtDetalle.Multiline = true;
        _txtDetalle.ReadOnly = true;
        _txtDetalle.ScrollBars = ScrollBars.Both;
        _txtDetalle.WordWrap = false;
        _txtDetalle.Font = new Font("Consolas", 9.5f, FontStyle.Regular);
        _txtDetalle.BackColor = Color.FromArgb(250, 252, 255);
        _txtDetalle.ForeColor = TemaVisual.TextoPrincipal;
        _txtDetalle.BorderStyle = BorderStyle.FixedSingle;

        pnlDetalleTarjeta.Controls.Add(_txtDetalle);
        pnlDetalleTarjeta.Controls.Add(pnlDetalleBarra);
        split.Panel2.Controls.Add(pnlDetalleTarjeta);

        // Barra de estado inferior
        var pnlEstado = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 32,
            BackColor = Color.Transparent,
            Padding = new Padding(4, 6, 4, 0)
        };

        _lblEstadoTotal.Dock = DockStyle.Fill;
        _lblEstadoTotal.Font = TemaVisual.FuentePequena;
        _lblEstadoTotal.ForeColor = TemaVisual.TextoSecundario;
        _lblEstadoTotal.Text = "Cargando registros...";
        pnlEstado.Controls.Add(_lblEstadoTotal);

        // Ensamblado
        var pnlContenidoCentro = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(0, 10, 0, 6),
            BackColor = Color.Transparent
        };
        pnlContenidoCentro.Controls.Add(split);

        pnlPrincipal.Controls.Add(pnlContenidoCentro);
        pnlPrincipal.Controls.Add(pnlFiltrosTarjeta);
        pnlPrincipal.Controls.Add(pnlEncabezado);
        pnlPrincipal.Controls.Add(pnlEstado);

        Controls.Add(pnlPrincipal);
    }

    private void ConfigurarGrid()
    {
        _dgvLogs.Dock = DockStyle.Fill;
        _dgvLogs.AutoGenerateColumns = false;
        _dgvLogs.AllowUserToAddRows = false;
        _dgvLogs.AllowUserToDeleteRows = false;
        _dgvLogs.ReadOnly = true;
        _dgvLogs.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _dgvLogs.MultiSelect = false;
        _dgvLogs.RowHeadersVisible = false;
        _dgvLogs.BorderStyle = BorderStyle.None;
        _dgvLogs.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;

        _dgvLogs.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = "FechaHoraTexto",
            HeaderText = "Fecha / Hora",
            Width = 180
        });

        _dgvLogs.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = "Nivel",
            HeaderText = "Nivel",
            Width = 115
        });

        _dgvLogs.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = "TraceId",
            HeaderText = "Seguimiento / TraceId",
            Width = 200
        });

        _dgvLogs.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = "MensajeCorto",
            HeaderText = "Mensaje",
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        });

        _dgvLogs.AplicarTemaModerno();
        _dgvLogs.RowTemplate.Height = 32;

        _dgvLogs.CellPainting += DgvLogs_CellPainting;
        _dgvLogs.SelectionChanged += DgvLogs_SelectionChanged;
    }

    private void ConfigurarEstilosYEventos()
    {
        Load += async (_, _) => await IniciarCargaAsync();
        _btnActualizar.Click += async (_, _) => await CargarLogsActualesAsync();
        _cmbArchivos.SelectedIndexChanged += async (_, _) => await CargarLogsActualesAsync();
        _cmbNiveles.SelectedIndexChanged += async (_, _) => await CargarLogsActualesAsync();
        _txtBuscar.KeyDown += async (_, e) =>
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                await CargarLogsActualesAsync();
            }
        };

        _btnAbrirCarpeta.Click += (_, _) => AbrirCarpetaLogs();
        _btnCopiarDetalle.Click += (_, _) => CopiarDetalleAlPortapapeles();
    }

    private async Task IniciarCargaAsync()
    {
        await this.EjecutarOperacionAsync(async () =>
        {
            var archivos = await _lectorLogsService.ObtenerArchivosLogAsync();
            _cmbArchivos.Items.Clear();

            if (archivos.Count == 0)
            {
                _lblEstadoTotal.Text = "No se encontraron archivos de log en la carpeta de bitácora.";
                return;
            }

            foreach (var f in archivos)
            {
                string etiqueta = f.LastWriteTime.Date == DateTime.Today
                    ? $"{f.Name} (Hoy)"
                    : f.Name;
                _cmbArchivos.Items.Add(new ElementoCombo<string>(etiqueta, f.FullName));
            }

            _cmbArchivos.SelectedIndex = 0;
        }, "Error al listar archivos de log", "logs-listar", _btnActualizar);
    }

    private async Task CargarLogsActualesAsync()
    {
        if (_cmbArchivos.SelectedItem is not ElementoCombo<string> archivoSeleccionado)
        {
            return;
        }

        string rutaArchivo = archivoSeleccionado.Valor;
        string? nivel = _cmbNiveles.SelectedItem?.ToString();
        string? texto = string.IsNullOrWhiteSpace(_txtBuscar.Text) ? null : _txtBuscar.Text.Trim();

        await this.EjecutarOperacionAsync(async () =>
        {
            _entradasCargadas = await _lectorLogsService.LeerEntradasAsync(rutaArchivo, texto, nivel);
            _dgvLogs.DataSource = new ListaVinculableOrdenable<EntradaLogItem>(_entradasCargadas);

            int errores = _entradasCargadas.Count(e => e.EsError);
            int advertencias = _entradasCargadas.Count(e => e.EsAdvertencia);
            int info = _entradasCargadas.Count - errores - advertencias;

            _lblEstadoTotal.Text = $"Mostrando {_entradasCargadas.Count} eventos  •  {errores} errores  •  {advertencias} advertencias  •  {info} informativos  |  Archivo: {Path.GetFileName(rutaArchivo)}";

            if (_entradasCargadas.Count == 0)
            {
                _txtDetalle.Clear();
                _lblInfoSeleccion.Text = "No se encontraron eventos con los filtros seleccionados";
            }
        }, "Error al leer eventos de log", "logs-cargar", _btnActualizar);
    }

    private void DgvLogs_SelectionChanged(object? sender, EventArgs e)
    {
        if (_dgvLogs.CurrentRow?.DataBoundItem is EntradaLogItem item)
        {
            _txtDetalle.Text = item.DetalleCompleto;
            string extra = string.IsNullOrWhiteSpace(item.TraceId)
                ? string.Empty
                : $" | Seguimiento: {item.TraceId}";
            _lblInfoSeleccion.Text = $"[{item.Nivel}] {item.FechaHoraTexto}{extra}";
        }
        else
        {
            _txtDetalle.Clear();
            _lblInfoSeleccion.Text = "Seleccione un evento para inspeccionar la traza técnica completa";
        }
    }

    private void DgvLogs_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
    {
        if (e.RowIndex < 0) return;

        // Pintar columna Nivel con píldora de color moderna
        if (e.ColumnIndex == 1 && e.Value is string nivel)
        {
            e.Paint(e.CellBounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.ContentForeground);

            Color colorFondo;
            Color colorTexto;

            if (string.Equals(nivel, "Error", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(nivel, "Fatal", StringComparison.OrdinalIgnoreCase))
            {
                colorFondo = Color.FromArgb(254, 226, 226);
                colorTexto = Color.FromArgb(220, 38, 38);
            }
            else if (string.Equals(nivel, "Advertencia", StringComparison.OrdinalIgnoreCase))
            {
                colorFondo = Color.FromArgb(254, 243, 199);
                colorTexto = Color.FromArgb(180, 83, 9);
            }
            else
            {
                colorFondo = Color.FromArgb(239, 246, 255);
                colorTexto = Color.FromArgb(29, 78, 216);
            }

            var g = e.Graphics;
            if (g is null) return;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var rectPildora = new Rectangle(e.CellBounds.X + 8, e.CellBounds.Y + 4, e.CellBounds.Width - 16, e.CellBounds.Height - 8);
            using var brushFondo = new SolidBrush(colorFondo);
            using var brushTexto = new SolidBrush(colorTexto);

            using var path = GenerarRutaRedondeada(rectPildora, 4);
            g.FillPath(brushFondo, path);

            var sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            using var fontBadge = new Font("Segoe UI", 8.5f, FontStyle.Bold);
            g.DrawString(nivel, fontBadge, brushTexto, rectPildora, sf);

            e.Handled = true;
        }
    }

    private static GraphicsPath GenerarRutaRedondeada(Rectangle rect, int radio)
    {
        var path = new GraphicsPath();
        int d = radio * 2;
        path.AddArc(rect.X, rect.Y, d, d, 180, 90);
        path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
        path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
        path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
        path.CloseFigure();
        return path;
    }

    private void AbrirCarpetaLogs()
    {
        try
        {
            string carpeta = _lectorLogsService.ObtenerCarpetaLogs();
            if (Directory.Exists(carpeta))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = carpeta,
                    UseShellExecute = true
                });
            }
            else
            {
                DialogoUIHelper.MostrarAdvertencia("La carpeta de logs aún no ha sido creada.", "Bitácora", this);
            }
        }
        catch (Exception ex)
        {
            DialogoUIHelper.MostrarError($"No fue posible abrir la carpeta de logs: {ex.Message}", "Error", this);
        }
    }

    private void CopiarDetalleAlPortapapeles()
    {
        if (string.IsNullOrWhiteSpace(_txtDetalle.Text))
        {
            return;
        }

        try
        {
            Clipboard.SetText(_txtDetalle.Text);
            DialogoUIHelper.MostrarInformacion("Traza del evento copiada al portapapeles.", "Visor de Logs", this);
        }
        catch
        {
            // Fallback silencioso si el portapapeles está bloqueado por otro proceso
        }
    }

    /// <summary>
    /// Permite abrir el visor buscando directamente un código de seguimiento específico.
    /// </summary>
    public void BuscarPorTraceId(string traceId)
    {
        _txtBuscar.Text = traceId;
        _cmbNiveles.SelectedIndex = 0; // "Todos"
        _ = CargarLogsActualesAsync();
    }
}
