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
            GenerarControles();
        }

        private void GenerarControles()
        {
            int y = 20;
            var props = _entidad.GetType().GetProperties().Where(p => 
                p.CanWrite && 
                !p.PropertyType.IsGenericType && // omitir ICollection
                !p.Name.EndsWith("Navigation") // omitir relaciones virtuales
            ).ToList();

            foreach(var prop in props)
            {
                bool isId = prop.Name == "Id" + _entidad.GetType().Name;

                Label lbl = new Label { Text = prop.Name, Location = new Point(20, y + 3), AutoSize = true };
                TextBox txt = new TextBox { Name = prop.Name, Location = new Point(150, y), Width = 200 };
                
                if (isId)
                {
                    txt.ReadOnly = true;
                    txt.Enabled = false;
                }

                var val = prop.GetValue(_entidad);
                txt.Text = val?.ToString() ?? "";

                this.Controls.Add(lbl);
                this.Controls.Add(txt);
                y += 35;
            }

            Button btnGuardar = new Button { Text = "Guardar", Location = new Point(150, y + 10), DialogResult = DialogResult.OK };
            btnGuardar.Click += BtnGuardar_Click;
            
            Button btnCancelar = new Button { Text = "Cancelar", Location = new Point(240, y + 10), DialogResult = DialogResult.Cancel };
            
            this.Controls.Add(btnGuardar);
            this.Controls.Add(btnCancelar);
            
            this.Height = y + 100;
            this.Width = 400;
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

                var txt = this.Controls.Find(prop.Name, false).FirstOrDefault() as TextBox;
                if (txt != null)
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
