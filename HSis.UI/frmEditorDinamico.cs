using System.Reflection;
using HSis.Logic.Services;

namespace HSis.UI
{
    public partial class frmEditorDinamico : Form
    {
        private object _entidad;

        private readonly CatalogoService _catalogoService;

        public frmEditorDinamico(object entidad, string titulo, CatalogoService catalogoService)
        {
            _entidad = entidad;
            _catalogoService = catalogoService;
            InitializeComponent();
            this.Text = titulo;
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            await GenerarControlesAsync();
        }

        private async System.Threading.Tasks.Task GenerarControlesAsync()
        {
            int y = 30;
            this.AutoScroll = true;
            var props = ObtenerPropiedadesEditables();

            foreach (var prop in props)
            {
                await CrearControlParaPropiedadAsync(prop, y);
                y += 45;
            }



            Button btnGuardar = new Button { Text = "Guardar", Location = new Point(230, y + 20), Width = 100, Height = 35, DialogResult = DialogResult.OK };
            btnGuardar.Click += BtnGuardar_Click;

            Button btnCancelar = new Button { Text = "Cancelar", Location = new Point(340, y + 20), Width = 100, Height = 35, DialogResult = DialogResult.Cancel };

            this.Controls.Add(btnGuardar);
            this.Controls.Add(btnCancelar);

            this.Height = Math.Min(y + 150, 600); // Máximo 600 de alto, si es más usa scroll
            this.Width = 550;
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
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
            if (cmb.SelectedValue != null)
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
                !p.PropertyType.IsGenericType &&
                !p.Name.EndsWith("Navigation") &&
                !(_entidad.GetType().Name == "Material" && (p.Name == "Costo" || p.Name == "Inventario"))
            ).ToList();
        }

        private async Task CrearControlParaPropiedadAsync(PropertyInfo prop, int y)
        {
            string idPk = "Id" + (_entidad.GetType().Name == "RolUsuario" ? "Rol" : _entidad.GetType().Name);
            bool isId = prop.Name == idPk;

            string labelText = prop.Name;
            var navProp = _entidad.GetType().GetProperty(prop.Name + "Navigation");

            if (navProp != null && (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(int?)))
            {
                if (labelText.StartsWith("Id") && labelText.Length > 2)
                    labelText = labelText.Substring(2);
            }

            Label lbl = new Label { Text = labelText, Location = new Point(30, y + 5), AutoSize = true };

            if (navProp != null && (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(int?)))
            {
                await AgregarComboBoxAsync(prop, navProp, lbl, y);
            }
            else
            {
                await AgregarTextBoxAsync(prop, isId, lbl, y);
            }
        }

        private async Task AgregarComboBoxAsync(PropertyInfo prop, PropertyInfo navProp, Label lbl, int y)
        {
            ComboBox cmb = new ComboBox { Name = prop.Name, Location = new Point(230, y), Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };

            Type navType = navProp.PropertyType;
            var list = await _catalogoService.ObtenerTodosPorTipoAsync(navType);

            string displayMember = "Id" + navType.Name;
            if (navType.GetProperty("Nombre") != null) displayMember = "Nombre";
            else if (navType.GetProperty("Descripción") != null) displayMember = "Descripción";

            cmb.DisplayMember = displayMember;
            cmb.ValueMember = "Id" + (navType.Name == "RolUsuario" ? "Rol" : navType.Name);
            cmb.DataSource = list;

            var val = prop.GetValue(_entidad);
            if (val != null) cmb.SelectedValue = val;

            if (_entidad.GetType().Name.StartsWith("Ingreso") && prop.Name == "IdUsuario")
            {
                cmb.Enabled = false;
            }

            this.Controls.Add(lbl);
            this.Controls.Add(cmb);
        }

        private async Task AgregarTextBoxAsync(PropertyInfo prop, bool isId, Label lbl, int y)
        {
            TextBox txt = new TextBox { Name = prop.Name, Location = new Point(230, y), Width = 250 };

            if (isId)
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
                    int nextId = await _catalogoService.ObtenerSiguienteIdAsync(_entidad.GetType(), prop.Name);
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
