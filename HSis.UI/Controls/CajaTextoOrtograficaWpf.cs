#nullable enable
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms.Integration;
using System.Windows.Markup;
using WPF = System.Windows.Controls;

namespace HSis.UI.Controls
{
    /// <summary>
    /// Control personalizado que combina Windows Forms y WPF para proporcionar
    /// corrección ortográfica nativa en tiempo real con subrayado rojo y sugerencias.
    /// </summary>
    [DesignerCategory("code")]
    public class CajaTextoOrtograficaWpf : ElementHost
    {
        private readonly WPF.TextBox _cajaTextoWpf;

        public CajaTextoOrtograficaWpf()
        {
            _cajaTextoWpf = new WPF.TextBox
            {
                AcceptsReturn = true,
                TextWrapping = System.Windows.TextWrapping.Wrap,
                VerticalScrollBarVisibility = WPF.ScrollBarVisibility.Auto,
                FontFamily = new System.Windows.Media.FontFamily("Segoe UI"),
                FontSize = 14.5, // Aproximadamente 11pt, alineado con el resto del formulario
                BorderThickness = new System.Windows.Thickness(1),
                BorderBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(171, 173, 179)), // Borde Windows clásico
                Padding = new System.Windows.Thickness(4),
                AcceptsTab = true
            };

            // Habilitar el corrector ortográfico nativo en español
            _cajaTextoWpf.SpellCheck.IsEnabled = true;
            _cajaTextoWpf.Language = XmlLanguage.GetLanguage("es-ES");

            this.Child = _cajaTextoWpf;
            this.Size = new Size(250, 100);
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [AllowNull]
        public override string Text
        {
            get => _cajaTextoWpf.Text;
            set => _cajaTextoWpf.Text = value ?? string.Empty;
        }

        [Browsable(true)]
        [DefaultValue(false)]
        public bool SoloLectura
        {
            get => _cajaTextoWpf.IsReadOnly;
            set
            {
                _cajaTextoWpf.IsReadOnly = value;
                // Ajustar el color de fondo para que coincida con el estilo de Windows Forms de sólo lectura
                if (value)
                {
                    _cajaTextoWpf.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(240, 240, 240));
                }
                else
                {
                    _cajaTextoWpf.Background = System.Windows.Media.Brushes.White;
                }
            }
        }

        [Browsable(true)]
        [DefaultValue("es-ES")]
        public string IdiomaCorreccion
        {
            get => _cajaTextoWpf.Language.IetfLanguageTag;
            set => _cajaTextoWpf.Language = XmlLanguage.GetLanguage(value);
        }

        [Browsable(true)]
        [DefaultValue(true)]
        public bool CorreccionHabilitada
        {
            get => _cajaTextoWpf.SpellCheck.IsEnabled;
            set => _cajaTextoWpf.SpellCheck.IsEnabled = value;
        }

        /// <summary>
        /// Limpia el contenido del campo de texto.
        /// </summary>
        public void Limpiar()
        {
            _cajaTextoWpf.Clear();
        }


        /// <summary>
        /// Evento que se dispara cuando el texto cambia.
        /// </summary>
        public new event EventHandler? TextChanged;

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            _cajaTextoWpf.TextChanged += (s, e) => TextChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
