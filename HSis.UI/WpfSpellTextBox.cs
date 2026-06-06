#nullable enable
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Markup;
using System.Diagnostics.CodeAnalysis;
using WPF = System.Windows.Controls;

namespace HSis.UI
{
    /// <summary>
    /// Control personalizado que combina Windows Forms y WPF para proporcionar
    /// corrección ortográfica nativa en tiempo real con subrayado rojo y sugerencias.
    /// </summary>
    [DesignerCategory("code")]
    public class WpfSpellTextBox : ElementHost
    {
        private readonly WPF.TextBox _wpfTextBox;

        public WpfSpellTextBox()
        {
            _wpfTextBox = new WPF.TextBox
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
            _wpfTextBox.SpellCheck.IsEnabled = true;
            _wpfTextBox.Language = XmlLanguage.GetLanguage("es-ES");

            this.Child = _wpfTextBox;
            this.Size = new Size(250, 100);
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [AllowNull]
        public override string Text
        {
            get => _wpfTextBox.Text;
            set => _wpfTextBox.Text = value ?? string.Empty;
        }

        [Browsable(true)]
        [DefaultValue(false)]
        public bool ReadOnly
        {
            get => _wpfTextBox.IsReadOnly;
            set
            {
                _wpfTextBox.IsReadOnly = value;
                // Ajustar el color de fondo para que coincida con el estilo de Windows Forms de sólo lectura
                if (value)
                {
                    _wpfTextBox.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(240, 240, 240));
                }
                else
                {
                    _wpfTextBox.Background = System.Windows.Media.Brushes.White;
                }
            }
        }

        [Browsable(true)]
        [DefaultValue("es-ES")]
        public string SpellCheckLanguage
        {
            get => _wpfTextBox.Language.IetfLanguageTag;
            set => _wpfTextBox.Language = XmlLanguage.GetLanguage(value);
        }

        [Browsable(true)]
        [DefaultValue(true)]
        public bool SpellCheckEnabled
        {
            get => _wpfTextBox.SpellCheck.IsEnabled;
            set => _wpfTextBox.SpellCheck.IsEnabled = value;
        }

        /// <summary>
        /// Limpia el contenido del campo de texto.
        /// </summary>
        public void Clear()
        {
            _wpfTextBox.Clear();
        }

        /// <summary>
        /// Evento que se dispara cuando el texto cambia.
        /// </summary>
        public new event EventHandler? TextChanged;

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            _wpfTextBox.TextChanged += (s, e) => TextChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
