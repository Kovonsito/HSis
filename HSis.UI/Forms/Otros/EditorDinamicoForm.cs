using System.ComponentModel;
using System.Reflection;
using System.Runtime.Versioning;
using HSis.UI.Presenters;

namespace HSis.UI.Forms.Otros
{
    [SupportedOSPlatform("windows")]
    public partial class EditorDinamicoForm : Form, IEditorDinamicoView
    {
        private readonly object _entidad;
        private readonly EditorDinamicoPresenter _presenter;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object Entidad => _entidad;

        public EditorDinamicoForm(object entidad, string titulo, EditorDinamicoPresenter presenter)
        {
            _entidad = entidad;
            _presenter = presenter;
            _presenter.SetView(this);
            InitializeComponent();
            this.Text = titulo;
        }

        public void MostrarError(string mensaje)
        {
            MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }


        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            await GenerarControlesAsync();
        }

        private async Task GenerarControlesAsync()
        {
            int y = 30;
            this.AutoScroll = true;
            var props = ObtenerPropiedadesEditables();

            foreach (var prop in props)
            {
                await CrearControlParaPropiedadAsync(prop, y);
                y += 45;
            }

            Button btnGuardar = new()
            {
                Text = "Guardar",
                Location = new Point(230, y + 20),
                Width = 100,
                Height = 35,
                DialogResult = DialogResult.OK,
                BackColor = Color.FromArgb(39, 174, 96),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold, GraphicsUnit.Point),
                ForeColor = Color.White,
                UseVisualStyleBackColor = false
            };
            btnGuardar.Click += BtnGuardar_Click;

            Button btnCancelar = new()
            {
                Text = "Cancelar",
                Location = new Point(340, y + 20),
                Width = 100,
                Height = 35,
                DialogResult = DialogResult.Cancel,
                BackColor = Color.FromArgb(231, 76, 60),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold, GraphicsUnit.Point),
                ForeColor = Color.White,
                UseVisualStyleBackColor = false
            };

            this.Controls.Add(btnGuardar);
            this.Controls.Add(btnCancelar);

            this.Height = Math.Min(y + 150, 600);
            this.Width = 600;
        }

        private void BtnGuardar_Click(object? sender, EventArgs e)
        {
            var props = ObtenerPropiedadesEditables();

            foreach (var prop in props)
            {
                bool isId = prop.Name == "Id" + _entidad.GetType().Name;
                if (isId) continue;

                var control = this.Controls.Find(prop.Name, false).FirstOrDefault();

                if (control is ComboBox cmb)
                {
                    AsignarValorComboBox(prop, cmb);
                }
                else if (control is TextBox txt)
                {
                    AsignarValorTextBox(prop, txt);
                }
            }
        }

        private void AsignarValorComboBox(PropertyInfo prop, ComboBox cmb)
        {
            if (prop.PropertyType == typeof(string))
            {
                prop.SetValue(_entidad, cmb.SelectedItem?.ToString() ?? cmb.Text);
            }
            else if (cmb.SelectedValue != null)
            {
                if (prop.PropertyType == typeof(int)) prop.SetValue(_entidad, Convert.ToInt32(cmb.SelectedValue));
                else if (prop.PropertyType == typeof(int?)) prop.SetValue(_entidad, (int?)Convert.ToInt32(cmb.SelectedValue));
            }
            else if (prop.PropertyType == typeof(int?))
            {
                prop.SetValue(_entidad, null);
            }
        }

        private void AsignarValorTextBox(PropertyInfo prop, TextBox txt)
        {
            if (prop.PropertyType == typeof(string))
            {
                prop.SetValue(_entidad, txt.Text);
            }
            else if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(int?))
            {
                if (int.TryParse(txt.Text, out int v)) prop.SetValue(_entidad, v);
                else if (prop.PropertyType == typeof(int?)) prop.SetValue(_entidad, null);
            }
            else if (prop.PropertyType == typeof(decimal) || prop.PropertyType == typeof(decimal?))
            {
                if (decimal.TryParse(txt.Text, out decimal v)) prop.SetValue(_entidad, v);
                else if (prop.PropertyType == typeof(decimal?)) prop.SetValue(_entidad, null);
            }
            else if (prop.PropertyType == typeof(short) || prop.PropertyType == typeof(short?))
            {
                if (short.TryParse(txt.Text, out short v)) prop.SetValue(_entidad, v);
                else if (prop.PropertyType == typeof(short?)) prop.SetValue(_entidad, null);
            }
        }

        private List<PropertyInfo> ObtenerPropiedadesEditables()
        {
            return _entidad.GetType().GetProperties().Where(p =>
                                                                                 p.CanWrite &&
                                                                                 !(p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() != typeof(Nullable<>)) &&
                                                                                 !p.Name.EndsWith("Navigation") &&
                                                                                 !(_entidad.GetType().Name == "Material" && (p.Name == "Costo" || p.Name == "Inventario"))
            ).ToList();
        }


        private async Task CrearControlParaPropiedadAsync(PropertyInfo prop, int y)
        {
            string idPk = "Id" + (_entidad.GetType().Name == "RolUsuario" ? "Rol" : (_entidad.GetType().Name == "MovimientoMaterial" ? "Movimiento" : _entidad.GetType().Name));
            bool isId = prop.Name == idPk;

            string labelText = prop.Name;
            var navProp = _entidad.GetType().GetProperty(prop.Name);

            if (navProp != null && (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(int?)))
            {
                if (labelText.StartsWith("Id") && labelText.Length > 2)
                    labelText = labelText.Substring(2);
            }

            Label lbl = new() { Text = labelText, Location = new Point(30, y + 5), AutoSize = true };

            if (prop.Name == "Motivo")
            {
                await AgregarComboBoxDeMotivosAsync(prop, lbl, y);
            }
            else if (navProp != null && (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(int?)))
            {
                await AgregarComboBoxAsync(prop, navProp, lbl, y);
            }
            else
            {
                await AgregarTextBoxAsync(prop, isId, lbl, y);
            }
        }

        private Task AgregarComboBoxDeMotivosAsync(PropertyInfo prop, Label lbl, int y)
        {
            ComboBox cmb = new() { Name = prop.Name, Location = new Point(230, y), Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };

            this.Controls.Add(lbl);
            this.Controls.Add(cmb);

            string[] motivos = [
                "Ingreso por Compra",
                "Ajuste por Error de Captura",
                "Ajuste por Pérdida",
                "Ajuste por Daño de Almacén",
                "Ajuste por Devolución"
            ];

            cmb.DataSource = motivos;

            var val = prop.GetValue(_entidad) as string;
            if (!string.IsNullOrEmpty(val) && motivos.Contains(val))
            {
                cmb.SelectedItem = val;
            }
            else
            {
                cmb.SelectedIndex = 0;
            }
            return Task.CompletedTask;
        }

        private async Task AgregarComboBoxAsync(PropertyInfo prop, PropertyInfo navProp, Label lbl, int y)
        {
            ComboBox cmb = new() { Name = prop.Name, Location = new Point(230, y), Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };

            this.Controls.Add(lbl);
            this.Controls.Add(cmb);

            Type navType = navProp.PropertyType;
            var list = await _presenter.ObtenerTodosPorTipoAsync(navType);

            var typedArray = Array.CreateInstance(navType, list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                typedArray.SetValue(list[i], i);
            }

            string displayMember = "Id" + navType.Name;
            if (navType.GetProperty("Nombre") != null) displayMember = "Nombre";
            else if (navType.GetProperty("Descripcion") != null) displayMember = "Descripcion";

            cmb.DisplayMember = displayMember;
            cmb.ValueMember = "Id" + (navType.Name == "RolUsuario" ? "Rol" : navType.Name);
            cmb.DataSource = typedArray;

            var val = prop.GetValue(_entidad);
            if (val != null)
            {
                Type? underlyingType = Nullable.GetUnderlyingType(prop.PropertyType);
                if (underlyingType != null)
                {
                    cmb.SelectedValue = Convert.ChangeType(val, underlyingType);
                }
                else
                {
                    cmb.SelectedValue = val;
                }
            }
            else
            {
                cmb.SelectedIndex = -1;
            }

            if ((_entidad.GetType().Name.StartsWith("Ingreso") || _entidad.GetType().Name == "MovimientoMaterial") && prop.Name == "IdUsuario")
            {
                cmb.Enabled = false;
            }
        }

        private async Task AgregarTextBoxAsync(PropertyInfo prop, bool isId, Label lbl, int y)
        {
            TextBox txt = new() { Name = prop.Name, Location = new Point(230, y), Width = 250 };

            if (isId || prop.Name == "FechaMovimiento")
            {
                txt.ReadOnly = true;
                txt.Enabled = false;
            }

            if (prop.Name.Equals("Contraseña", StringComparison.OrdinalIgnoreCase) || prop.Name.Equals("Password", StringComparison.OrdinalIgnoreCase))
            {
                txt.UseSystemPasswordChar = true;
            }

            var val = prop.GetValue(_entidad);
            txt.Text = val?.ToString() ?? "";

            if (isId && val?.ToString() == "0")
            {
                try
                {
                    int nextId = await _presenter.ObtenerSiguienteIdAsync(_entidad.GetType(), prop.Name);
                    txt.Text = nextId.ToString();
                    lbl.Text += nextId == 1 ? " (Primero)" : " (Sig. Sugerido)";
                }
                catch { /* Ignorar */ }
            }

            this.Controls.Add(lbl);
            this.Controls.Add(txt);
        }
    }
}

