using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace HSis.UI
{
    public partial class frmEditorDinamico : Form
    {
        private object _entidad;

        public frmEditorDinamico(object entidad, string titulo = "Editor")
        {
            _entidad = entidad;
            InitializeComponent();
            this.Text = titulo;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            GenerarControles();
        }

        private void GenerarControles()
        {
            int y = 30;
            this.AutoScroll = true;
            var props = _entidad.GetType().GetProperties().Where(p => 
                p.CanWrite && 
                !p.PropertyType.IsGenericType && // omitir ICollection
                !p.Name.EndsWith("Navigation") // omitir relaciones virtuales
            ).ToList();

            foreach(var prop in props)
            {
                string idPk = "Id" + (_entidad.GetType().Name == "RolUsuario" ? "Rol" : _entidad.GetType().Name);
                bool isId = prop.Name == idPk;

                Label lbl = new Label { Text = prop.Name, Location = new Point(30, y + 5), AutoSize = true };
                
                var navProp = _entidad.GetType().GetProperty(prop.Name + "Navigation");
                if (navProp != null && (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(int?)))
                {
                    ComboBox cmb = new ComboBox { Name = prop.Name, Location = new Point(230, y), Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
                    
                    Type navType = navProp.PropertyType;
                    using var db = new HSis.Data.Models.HSisDbContext();
                    var queryableMethod = typeof(Microsoft.EntityFrameworkCore.DbContext).GetMethod("Set", Type.EmptyTypes)?.MakeGenericMethod(navType);
                    var dbSet = queryableMethod?.Invoke(db, null);
                    
                    var toListMethod = typeof(System.Linq.Enumerable).GetMethod("ToList")?.MakeGenericMethod(navType);
                    var list = toListMethod?.Invoke(null, new object[] { dbSet! });

                    string displayMember = "Id" + navType.Name;
                    if (navType.GetProperty("Nombre") != null) displayMember = "Nombre";
                    else if (navType.GetProperty("Descripción") != null) displayMember = "Descripción";

                    cmb.DisplayMember = displayMember;
                    cmb.ValueMember = "Id" + (navType.Name == "RolUsuario" ? "Rol" : navType.Name);
                    cmb.DataSource = list;

                    var val = prop.GetValue(_entidad);
                    if (val != null) cmb.SelectedValue = val;

                    this.Controls.Add(lbl);
                    this.Controls.Add(cmb);
                }
                else
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
                            using var db = new HSis.Data.Models.HSisDbContext();
                            var queryableMethod = typeof(Microsoft.EntityFrameworkCore.DbContext).GetMethod("Set", Type.EmptyTypes)?.MakeGenericMethod(_entidad.GetType());
                            var dbSet = queryableMethod?.Invoke(db, null) as System.Collections.IEnumerable;
                            if (dbSet != null)
                            {
                                var list = dbSet.Cast<object>().ToList();
                                if (list.Any())
                                {
                                    var lastId = list.Max(o => Convert.ToInt32(prop.GetValue(o)));
                                    txt.Text = (lastId + 1).ToString();
                                    lbl.Text += " (Sig. Sugerido)";
                                }
                                else
                                {
                                    txt.Text = "1";
                                    lbl.Text += " (Primero)";
                                }
                            }
                        }
                        catch { /* Ignorar si no se puede predecir */ }
                    }

                    this.Controls.Add(lbl);
                    this.Controls.Add(txt);
                }
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
            var props = _entidad.GetType().GetProperties().Where(p => 
                p.CanWrite && 
                !p.PropertyType.IsGenericType && 
                !p.Name.EndsWith("Navigation")
            ).ToList();

            foreach(var prop in props)
            {
                bool isId = prop.Name == "Id" + _entidad.GetType().Name;
                if (isId) continue; // No actualizamos el ID

                var control = this.Controls.Find(prop.Name, false).FirstOrDefault();
                
                if (control is ComboBox cmb)
                {
                    if (cmb.SelectedValue != null)
                    {
                        if (prop.PropertyType == typeof(int)) prop.SetValue(_entidad, Convert.ToInt32(cmb.SelectedValue));
                        else if (prop.PropertyType == typeof(int?)) prop.SetValue(_entidad, (int?)Convert.ToInt32(cmb.SelectedValue));
                    }
                    else
                    {
                        if (prop.PropertyType == typeof(int?)) prop.SetValue(_entidad, null);
                    }
                }
                else if (control is TextBox txt)
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
            }
        }
    }
}
