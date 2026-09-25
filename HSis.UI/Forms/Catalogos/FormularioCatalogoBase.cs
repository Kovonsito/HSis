#nullable enable
using System.Runtime.Versioning;
using HSis.UI.Controls;
using HSis.UI.Helpers;

namespace HSis.UI.Forms.Catalogos;

[SupportedOSPlatform("windows")]
public abstract class FormularioCatalogoBase : Form
{
    protected readonly TableLayoutPanel Campos;
    protected readonly BotonModerno BtnGuardar;
    protected readonly BotonModerno BtnCancelar;

    protected FormularioCatalogoBase(string titulo, int alto = 420)
    {
        Text = titulo;
        Name = GetType().Name;
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ClientSize = new Size(560, alto);
        MinimumSize = new Size(560, alto);
        BackColor = TemaVisual.FondoApp;
        Font = TemaVisual.FuenteNormal;

        var panelPrincipal = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(18),
            BackColor = TemaVisual.FondoApp
        };

        var panelTitulo = new Panel
        {
            Dock = DockStyle.Top,
            Height = 54,
            BackColor = TemaVisual.FondoApp
        };
        panelTitulo.Controls.Add(new Label
        {
            Dock = DockStyle.Fill,
            Text = titulo,
            Font = new Font("Segoe UI", 15f, FontStyle.Bold),
            ForeColor = TemaVisual.TextoPrincipal,
            TextAlign = ContentAlignment.MiddleLeft
        });

        var panelBotones = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 52,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false,
            Padding = new Padding(0, 8, 0, 0),
            BackColor = TemaVisual.FondoApp
        };

        BtnCancelar = new BotonModerno
        {
            Text = "Cancelar",
            Estilo = EstiloBotonModerno.Secundario,
            Width = 125,
            Height = 36,
            DialogResult = DialogResult.Cancel
        };
        BtnGuardar = new BotonModerno
        {
            Text = "Guardar",
            Estilo = EstiloBotonModerno.Exito,
            Width = 125,
            Height = 36
        };
        panelBotones.Controls.Add(BtnCancelar);
        panelBotones.Controls.Add(BtnGuardar);

        var panelCampos = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            Padding = new Padding(0, 4, 0, 4),
            BackColor = TemaVisual.FondoApp
        };
        Campos = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            ColumnCount = 2,
            RowCount = 0,
            Padding = new Padding(0),
            BackColor = TemaVisual.FondoApp
        };
        Campos.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 155));
        Campos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        panelCampos.Controls.Add(Campos);

        panelPrincipal.Controls.Add(panelCampos);
        panelPrincipal.Controls.Add(panelBotones);
        panelPrincipal.Controls.Add(panelTitulo);
        Controls.Add(panelPrincipal);

        AcceptButton = BtnGuardar;
        CancelButton = BtnCancelar;
    }

    protected void AgregarCampo(string etiqueta, Control editor, int alto = 44)
    {
        var fila = Campos.RowCount;
        Campos.RowCount++;
        Campos.RowStyles.Add(new RowStyle(SizeType.Absolute, alto));
        Campos.Controls.Add(new Label
        {
            Text = etiqueta,
            Dock = DockStyle.Fill,
            AutoSize = true,
            ForeColor = TemaVisual.TextoPrincipal,
            Font = new Font("Segoe UI Semibold", 9.5f, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft,
            Margin = new Padding(0, 3, 12, 3)
        }, 0, fila);
        editor.Dock = DockStyle.Fill;
        editor.Margin = new Padding(0, 3, 0, 3);
        Campos.Controls.Add(editor, 1, fila);
    }

    protected void ConfigurarModoEdicion(string entidad)
    {
        Text = $"Editar {entidad}";
    }

    protected static void MostrarRequerido(string campo, Control control)
    {
        DialogoUIHelper.MostrarAdvertencia($"El campo {campo} es obligatorio.", "Validación requerida");
        control.Focus();
    }
}
