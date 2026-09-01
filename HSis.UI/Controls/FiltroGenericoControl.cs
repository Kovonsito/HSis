#nullable enable
using HSis.UI.Helpers;

namespace HSis.UI.Controls
{
    public partial class FiltroGenericoControl : UserControl
    {
        private readonly Dictionary<string, Control> _controlesEntrada = [];
        private readonly Dictionary<string, TipoFiltroControl> _tipos = [];
        private bool _suspenderEventos = false;

        public event EventHandler? FiltroCambiado;

        public FiltroGenericoControl()
        {
            InitializeComponent();
        }

        public void InicializarFiltros(List<FiltroCampo> campos)
        {
            _suspenderEventos = true;
            flowLayoutPanelMain.Controls.Clear();
            _controlesEntrada.Clear();
            _tipos.Clear();

            foreach (var campo in campos)
            {
                var container = new FlowLayoutPanel
                {
                    FlowDirection = FlowDirection.TopDown,
                    WrapContents = false,
                    Width = campo.Ancho,
                    Height = 52,
                    Margin = new Padding(5, 2, 5, 2)
                };

                var lbl = new Label
                {
                    Text = campo.Etiqueta,
                    AutoSize = true,
                    Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                    ForeColor = Color.FromArgb(75, 85, 99), // Slate gray style
                    Margin = new Padding(0, 0, 0, 3)
                };

                Control input;
                switch (campo.Tipo)
                {
                    case TipoFiltroControl.Texto:
                        var txt = new TextBox
                        {
                            Width = campo.Ancho - 10,
                            Font = new Font("Segoe UI", 10F)
                        };
                        txt.TextChanged += (s, e) => LanzarFiltroCambiado();
                        input = txt;
                        break;

                    case TipoFiltroControl.ComboSeleccion:
                        var cmb = new ComboBox
                        {
                            DropDownStyle = ComboBoxStyle.DropDownList,
                            Width = campo.Ancho - 10,
                            Font = new Font("Segoe UI", 10F)
                        };
                        if (campo.ValoresCombo != null)
                        {
                            cmb.Items.AddRange(campo.ValoresCombo);
                            if (cmb.Items.Count > 0) cmb.SelectedIndex = 0;
                        }
                        cmb.SelectedIndexChanged += (s, e) => LanzarFiltroCambiado();
                        input = cmb;
                        break;

                    case TipoFiltroControl.Fecha:
                        var dtp = new DateTimePicker
                        {
                            Format = DateTimePickerFormat.Short,
                            Width = campo.Ancho - 10,
                            Font = new Font("Segoe UI", 10F)
                        };
                        if (campo.ValorDefecto is DateTime dt)
                        {
                            dtp.Value = dt;
                        }
                        dtp.ValueChanged += (s, e) => LanzarFiltroCambiado();
                        input = dtp;
                        break;

                    default:
                        continue;
                }

                _controlesEntrada[campo.NombrePropiedad] = input;
                _tipos[campo.NombrePropiedad] = campo.Tipo;

                container.Controls.Add(lbl);
                container.Controls.Add(input);
                flowLayoutPanelMain.Controls.Add(container);
            }

            _suspenderEventos = false;
        }

        public void ActualizarCombo(string nombrePropiedad, object dataSource, string displayMember, string valueMember)
        {
            if (_controlesEntrada.TryGetValue(nombrePropiedad, out var control) && control is ComboBox cmb)
            {
                _suspenderEventos = true;
                cmb.DataSource = null;
                cmb.Items.Clear();
                cmb.DisplayMember = displayMember;
                cmb.ValueMember = valueMember;
                cmb.DataSource = dataSource;
                if (cmb.Items.Count > 0) cmb.SelectedIndex = 0;
                _suspenderEventos = false;
            }
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
                        var cmb = (ComboBox)control;
                        // For anonymous types or bound datasources, SelectedValue is correct.
                        // For simple lists, SelectedItem is used if SelectedValue is null.
                        valores[nombre] = cmb.SelectedValue ?? cmb.SelectedItem;
                        break;

                    case TipoFiltroControl.Fecha:
                        var dtp = (DateTimePicker)control;
                        valores[nombre] = dtp.Value;
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
                        var cmb = (ComboBox)control;
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
                        break;

                    case TipoFiltroControl.Fecha:
                        var dtp = (DateTimePicker)control;
                        if (valorDefecto is DateTime dt)
                        {
                            dtp.Value = dt;
                        }
                        else
                        {
                            dtp.Value = DateTime.Today;
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

