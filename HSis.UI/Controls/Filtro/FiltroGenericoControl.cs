#nullable enable
using System.Drawing.Drawing2D;
using HSis.UI.Helpers;

namespace HSis.UI.Controls
{
    public partial class FiltroGenericoControl : UserControl
    {
        private readonly Dictionary<string, Control> _controlesEntrada = [];
        private readonly Dictionary<string, TipoFiltroControl> _tipos = [];
        private bool _suspenderEventos = false;

        public event EventHandler? FiltroCambiado;
        public event EventHandler? RecargarClic;
        public event EventHandler? LimpiarClic;

        public FiltroGenericoControl()
        {
            InitializeComponent();
            DoubleBuffered = true;
            SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Limpiar fondo con el color del padre
            Color colorPadre = (Parent?.BackColor != null && Parent.BackColor != Color.Transparent && Parent.BackColor.A > 0)
                ? Parent.BackColor
                : TemaVisual.FondoApp;
            using (var brushPadre = new SolidBrush(colorPadre))
            {
                g.FillRectangle(brushPadre, 0, 0, Width, Height);
            }

            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            using var path = TemaVisual.CrearRectanguloRedondeado(rect, 8);

            // Fondo blanco de tarjeta
            using (var brushFondo = new SolidBrush(Color.White))
            {
                g.FillPath(brushFondo, path);
            }

            // Borde sutil
            using var penBorde = new Pen(Color.FromArgb(226, 232, 240), 1f);
            g.DrawPath(penBorde, path);
        }

        public void InicializarFiltros(List<FiltroCampo> campos)
        {
            _suspenderEventos = true;
            flowLayoutPanelMain.Controls.Clear();
            _controlesEntrada.Clear();
            _tipos.Clear();

            foreach (var campo in campos)
            {
                var container = new TableLayoutPanel
                {
                    ColumnCount = 1,
                    RowCount = 2,
                    Width = campo.Ancho,
                    Height = 52,
                    Margin = new Padding(4, 1, 4, 1),
                    Padding = new Padding(0),
                    BackColor = Color.White
                };
                container.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                container.RowStyles.Add(new RowStyle(SizeType.Absolute, 16F));
                container.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

                var lbl = new Label
                {
                    Text = campo.Etiqueta,
                    AutoSize = false,
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Font = new Font("Segoe UI Semibold", 8F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(100, 116, 139),
                    Margin = new Padding(0),
                    BackColor = Color.White
                };

                Control input;
                switch (campo.Tipo)
                {
                    case TipoFiltroControl.Texto:
                        var txt = new CajaTextoModerna
                        {
                            Placeholder = campo.Etiqueta
                        };
                        txt.TextChanged += (s, e) => LanzarFiltroCambiado();
                        input = txt;
                        break;

                    case TipoFiltroControl.ComboSeleccion:
                        var cmb = new ComboModerno
                        {
                            DropDownStyle = ComboBoxStyle.DropDownList
                        };
                        if (campo.ValoresCombo != null)
                        {
                            cmb.Items.AddRange(campo.ValoresCombo);
                            if (cmb.Items.Count > 0) cmb.SelectedIndex = 0;
                        }
                        cmb.SelectedIndexChanged += (s, e) =>
                        {
                            if (campo.NombrePropiedad == "Periodo" && cmb.SelectedItem?.ToString() is string periodo)
                            {
                                AplicarAtajoPeriodo(periodo);
                            }
                            LanzarFiltroCambiado();
                        };
                        input = cmb;
                        break;

                    case TipoFiltroControl.Fecha:
                        var dtp = new SelectorFechaModerno
                        {
                            Format = DateTimePickerFormat.Short
                        };
                        if (campo.ValorDefecto is DateTime dt)
                        {
                            dtp.Value = dt;
                        }
                        dtp.ValueChanged += (s, e) =>
                        {
                            if (!_suspenderEventos && (campo.NombrePropiedad == "FechaInicio" || campo.NombrePropiedad == "FechaFin"))
                            {
                                if (_controlesEntrada.TryGetValue("Periodo", out var ctrlPeriodo))
                                {
                                    string? actual = ctrlPeriodo switch
                                    {
                                        ComboModerno cmbMod => cmbMod.SelectedItem?.ToString(),
                                        ComboBox cb => cb.SelectedItem?.ToString(),
                                        _ => null
                                    };

                                    if (actual != "Personalizado")
                                    {
                                        bool prev = _suspenderEventos;
                                        _suspenderEventos = true;
                                        try
                                        {
                                            if (ctrlPeriodo is ComboModerno cmbMod)
                                                cmbMod.SelectedItem = "Personalizado";
                                            else if (ctrlPeriodo is ComboBox cb)
                                                cb.SelectedItem = "Personalizado";
                                        }
                                        finally
                                        {
                                            _suspenderEventos = prev;
                                        }
                                    }
                                }
                            }
                            LanzarFiltroCambiado();
                        };
                        input = dtp;
                        break;

                    default:
                        continue;
                }

                input.Dock = DockStyle.Fill;
                input.Margin = new Padding(0);

                _controlesEntrada[campo.NombrePropiedad] = input;
                _tipos[campo.NombrePropiedad] = campo.Tipo;

                container.Controls.Add(lbl, 0, 0);
                container.Controls.Add(input, 0, 1);
                flowLayoutPanelMain.Controls.Add(container);
            }

            _suspenderEventos = false;
        }

        private void Limpiar_Click()
        {
            LimpiarFiltros(ConfiguracionFiltrosTickets.ObtenerValoresDefecto());
            LimpiarClic?.Invoke(this, EventArgs.Empty);
        }

        public void ActualizarCombo(string nombrePropiedad, object dataSource, string displayMember, string valueMember)
        {
            if (_controlesEntrada.TryGetValue(nombrePropiedad, out var control))
            {
                _suspenderEventos = true;
                if (control is ComboModerno cmbModerno)
                {
                    cmbModerno.DataSource = null;
                    cmbModerno.Items.Clear();
                    cmbModerno.DisplayMember = displayMember;
                    cmbModerno.ValueMember = valueMember;
                    cmbModerno.DataSource = dataSource;
                    if (cmbModerno.Items.Count > 0) cmbModerno.SelectedIndex = 0;
                }
                else if (control is ComboBox cmb)
                {
                    cmb.DataSource = null;
                    cmb.Items.Clear();
                    cmb.DisplayMember = displayMember;
                    cmb.ValueMember = valueMember;
                    cmb.DataSource = dataSource;
                    if (cmb.Items.Count > 0) cmb.SelectedIndex = 0;
                }
                _suspenderEventos = false;
            }
        }

        public void ConfigurarOpcionesCombo(string nombrePropiedad, System.Collections.IEnumerable opciones)
        {
            if (_controlesEntrada.TryGetValue(nombrePropiedad, out var control))
            {
                _suspenderEventos = true;
                if (control is ComboModerno cmbModerno)
                {
                    cmbModerno.DataSource = null;
                    cmbModerno.Items.Clear();

                    if (opciones is System.Collections.IEnumerable lista)
                    {
                        var enumerator = lista.GetEnumerator();
                        if (enumerator.MoveNext() && enumerator.Current != null)
                        {
                            var primerElemento = enumerator.Current;
                            var tipo = primerElemento.GetType();
                            var propId = tipo.GetProperty("Id") ?? tipo.GetProperty("IdUsuario");
                            var propNombre = tipo.GetProperty("Nombre") ?? tipo.GetProperty("Descripcion");

                            if (propId != null && propNombre != null)
                            {
                                cmbModerno.DisplayMember = propNombre.Name;
                                cmbModerno.ValueMember = propId.Name;
                                cmbModerno.DataSource = opciones;
                                if (cmbModerno.Items.Count > 0) cmbModerno.SelectedIndex = 0;
                                _suspenderEventos = false;
                                return;
                            }
                        }

                        foreach (var item in lista)
                        {
                            cmbModerno.Items.Add(item);
                        }
                        if (cmbModerno.Items.Count > 0) cmbModerno.SelectedIndex = 0;
                    }
                }
                else if (control is ComboBox cmb)
                {
                    cmb.DataSource = null;
                    cmb.Items.Clear();

                    if (opciones is System.Collections.IEnumerable lista)
                    {
                        var enumerator = lista.GetEnumerator();
                        if (enumerator.MoveNext() && enumerator.Current != null)
                        {
                            var primerElemento = enumerator.Current;
                            var tipo = primerElemento.GetType();
                            var propId = tipo.GetProperty("Id") ?? tipo.GetProperty("IdUsuario");
                            var propNombre = tipo.GetProperty("Nombre") ?? tipo.GetProperty("Descripcion");

                            if (propId != null && propNombre != null)
                            {
                                cmb.DisplayMember = propNombre.Name;
                                cmb.ValueMember = propId.Name;
                                cmb.DataSource = opciones;
                                if (cmb.Items.Count > 0) cmb.SelectedIndex = 0;
                                _suspenderEventos = false;
                                return;
                            }
                        }

                        foreach (var item in lista)
                        {
                            cmb.Items.Add(item);
                        }
                        if (cmb.Items.Count > 0) cmb.SelectedIndex = 0;
                    }
                }
                _suspenderEventos = false;
            }
        }

        public bool CargarOpcionesCombo(string nombrePropiedad, IEnumerable<object> opciones)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(nombrePropiedad);
            ArgumentNullException.ThrowIfNull(opciones);

            if (!_controlesEntrada.ContainsKey(nombrePropiedad))
            {
                return false;
            }

            ConfigurarOpcionesCombo(nombrePropiedad, opciones);
            return true;
        }

        public Dictionary<string, object?> ObtenerValoresFiltros()
        {
            var valores = new Dictionary<string, object?>();
            foreach (var kvp in _controlesEntrada)
            {
                var nombre = kvp.Key;
                var control = kvp.Value;
                var tipo = _tipos[nombre];

                switch (tipo)
                {
                    case TipoFiltroControl.Texto:
                        valores[nombre] = control.Text.Trim();
                        break;

                    case TipoFiltroControl.ComboSeleccion:
                        if (control is ComboModerno cmbModerno)
                        {
                            valores[nombre] = cmbModerno.SelectedValue ?? cmbModerno.SelectedItem;
                        }
                        else if (control is ComboBox cmbLegacy)
                        {
                            valores[nombre] = cmbLegacy.SelectedValue ?? cmbLegacy.SelectedItem;
                        }
                        break;

                    case TipoFiltroControl.Fecha:
                        if (control is SelectorFechaModerno sfm)
                        {
                            valores[nombre] = sfm.Value;
                        }
                        else if (control is DateTimePicker dtpLegacy)
                        {
                            valores[nombre] = dtpLegacy.Value;
                        }
                        break;
                }
            }
            return valores;
        }

        public void LimpiarFiltros(Dictionary<string, object?>? valoresPorDefecto = null)
        {
            _suspenderEventos = true;
            foreach (var kvp in _controlesEntrada)
            {
                var nombre = kvp.Key;
                var control = kvp.Value;
                var tipo = _tipos[nombre];

                object? valorDefecto = null;
                valoresPorDefecto?.TryGetValue(nombre, out valorDefecto);

                switch (tipo)
                {
                    case TipoFiltroControl.Texto:
                        control.Text = (valorDefecto as string) ?? string.Empty;
                        break;

                    case TipoFiltroControl.ComboSeleccion:
                        if (control is ComboModerno cmbModerno)
                        {
                            if (valorDefecto != null)
                            {
                                if (cmbModerno.DataSource != null)
                                {
                                    cmbModerno.SelectedValue = valorDefecto;
                                }
                                else
                                {
                                    cmbModerno.SelectedItem = valorDefecto;
                                }
                            }
                            else
                            {
                                if (cmbModerno.Items.Count > 0) cmbModerno.SelectedIndex = 0;
                            }
                        }
                        else if (control is ComboBox cmb)
                        {
                            if (valorDefecto != null)
                            {
                                if (cmb.DataSource != null)
                                {
                                    cmb.SelectedValue = valorDefecto;
                                }
                                else
                                {
                                    cmb.SelectedItem = valorDefecto;
                                }
                            }
                            else
                            {
                                if (cmb.Items.Count > 0) cmb.SelectedIndex = 0;
                            }
                        }
                        break;

                    case TipoFiltroControl.Fecha:
                        var targetDate = (valorDefecto is DateTime dt) ? dt : DateTime.Today;
                        if (control is SelectorFechaModerno sfm)
                        {
                            sfm.Value = targetDate;
                        }
                        else if (control is DateTimePicker dtpLegacy)
                        {
                            dtpLegacy.Value = targetDate;
                        }
                        break;
                }
            }
            _suspenderEventos = false;
            LanzarFiltroCambiado();
        }

        public void EstablecerValorFiltro(string nombrePropiedad, object? valor)
        {
            if (_controlesEntrada.TryGetValue(nombrePropiedad, out var control))
            {
                var tipo = _tipos[nombrePropiedad];
                _suspenderEventos = true;
                switch (tipo)
                {
                    case TipoFiltroControl.Texto:
                        control.Text = valor?.ToString() ?? string.Empty;
                        break;

                    case TipoFiltroControl.ComboSeleccion:
                        var cmb = (ComboBox)control;
                        if (cmb.DataSource != null)
                        {
                            cmb.SelectedValue = valor!;
                        }
                        else
                        {
                            cmb.SelectedItem = valor;
                        }
                        break;

                    case TipoFiltroControl.Fecha:
                        var dtp = (DateTimePicker)control;
                        if (valor is DateTime dt)
                        {
                            dtp.Value = dt;
                        }
                        break;
                }
                _suspenderEventos = false;
                LanzarFiltroCambiado();
            }
        }

        private void LanzarFiltroCambiado()
        {
            if (!_suspenderEventos)
            {
                FiltroCambiado?.Invoke(this, EventArgs.Empty);
            }
        }

        private void AplicarAtajoPeriodo(string periodo)
        {
            if (periodo == "Personalizado")
            {
                return;
            }

            var hoy = DateTime.Today;
            DateTime? inicio = null;
            DateTime? fin = null;

            switch (periodo)
            {
                case "Últimos 30 días":
                    inicio = hoy.AddDays(-30);
                    fin = hoy.AddDays(1).AddTicks(-1);
                    break;
                case "Hoy":
                    inicio = hoy;
                    fin = hoy.AddDays(1).AddTicks(-1);
                    break;
                case "Ayer":
                    inicio = hoy.AddDays(-1);
                    fin = hoy.AddTicks(-1);
                    break;
                case "Esta semana":
                    int diasAlLunes = (int)hoy.DayOfWeek - (int)DayOfWeek.Monday;
                    if (diasAlLunes < 0) diasAlLunes += 7;
                    inicio = hoy.AddDays(-diasAlLunes);
                    fin = hoy.AddDays(1).AddTicks(-1);
                    break;
                case "Semana anterior":
                    int offsetLunes = (int)hoy.DayOfWeek - (int)DayOfWeek.Monday;
                    if (offsetLunes < 0) offsetLunes += 7;
                    inicio = hoy.AddDays(-offsetLunes - 7);
                    fin = hoy.AddDays(-offsetLunes).AddTicks(-1);
                    break;
                case "Este mes":
                    inicio = new DateTime(hoy.Year, hoy.Month, 1);
                    fin = hoy.AddDays(1).AddTicks(-1);
                    break;
                case "Mes anterior":
                    var mesAnt = hoy.AddMonths(-1);
                    inicio = new DateTime(mesAnt.Year, mesAnt.Month, 1);
                    fin = new DateTime(hoy.Year, hoy.Month, 1).AddTicks(-1);
                    break;
                case "Este año":
                    inicio = new DateTime(hoy.Year, 1, 1);
                    fin = hoy.AddDays(1).AddTicks(-1);
                    break;
                case "Año anterior":
                    inicio = new DateTime(hoy.Year - 1, 1, 1);
                    fin = new DateTime(hoy.Year, 1, 1).AddTicks(-1);
                    break;
                case "Enero":
                case "Febrero":
                case "Marzo":
                case "Abril":
                case "Mayo":
                case "Junio":
                case "Julio":
                case "Agosto":
                case "Septiembre":
                case "Octubre":
                case "Noviembre":
                case "Diciembre":
                    int numMes = periodo switch
                    {
                        "Enero" => 1,
                        "Febrero" => 2,
                        "Marzo" => 3,
                        "Abril" => 4,
                        "Mayo" => 5,
                        "Junio" => 6,
                        "Julio" => 7,
                        "Agosto" => 8,
                        "Septiembre" => 9,
                        "Octubre" => 10,
                        "Noviembre" => 11,
                        "Diciembre" => 12,
                        _ => hoy.Month
                    };
                    inicio = new DateTime(hoy.Year, numMes, 1);
                    fin = inicio.Value.AddMonths(1).AddTicks(-1);
                    break;
                case "Todos":
                    inicio = new DateTime(2020, 1, 1);
                    fin = hoy.AddYears(1);
                    break;
            }

            if (inicio.HasValue && fin.HasValue)
            {
                bool prevSuspender = _suspenderEventos;
                _suspenderEventos = true;
                try
                {
                    if (_controlesEntrada.TryGetValue("FechaInicio", out var ctrlInicio))
                    {
                        if (ctrlInicio is SelectorFechaModerno sfm) sfm.Value = inicio.Value;
                        else if (ctrlInicio is DateTimePicker dtp) dtp.Value = inicio.Value;
                    }
                    if (_controlesEntrada.TryGetValue("FechaFin", out var ctrlFin))
                    {
                        if (ctrlFin is SelectorFechaModerno sfm) sfm.Value = fin.Value;
                        else if (ctrlFin is DateTimePicker dtp) dtp.Value = fin.Value;
                    }
                }
                finally
                {
                    _suspenderEventos = prevSuspender;
                }
            }
        }
    }
}

namespace HSis.UI.Helpers
{
    public enum TipoFiltroControl
    {
        Texto,
        ComboSeleccion,
        Fecha
    }

    public class FiltroCampo
    {
        public string NombrePropiedad { get; set; } = string.Empty;
        public string Etiqueta { get; set; } = string.Empty;
        public TipoFiltroControl Tipo { get; set; }
        public object[]? ValoresCombo { get; set; }
        public int Ancho { get; set; } = 150;
        public object? ValorDefecto { get; set; }
    }
}

