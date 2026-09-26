#nullable enable
using System.Drawing.Drawing2D;
using System.Runtime.Versioning;
using FontAwesome.Sharp;
using HSis.UI.Controls;
using HSis.UI.Helpers;

namespace HSis.UI.Forms.Otros;

[SupportedOSPlatform("windows")]
public sealed class DialogoModernoForm : Form
{
    private readonly Label _lblTitulo;
    private readonly Label _lblMensaje;
    private readonly Panel _pnlIcono;
    private readonly BotonModerno _btnAceptar;
    private readonly BotonModerno? _btnCancelar;
    private readonly DialogoTipo _tipo;

    private DialogoModernoForm(string titulo, string mensaje, DialogoTipo tipo, bool confirmacion)
    {
        _tipo = tipo;
        Text = titulo;
        Name = "DialogoModernoForm";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        ShowInTaskbar = false;
        MaximizeBox = false;
        MinimizeBox = false;
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(confirmacion ? 520 : 500, 250);
        MinimumSize = new Size(420, 220);
        BackColor = TemaVisual.FondoApp;
        Font = TemaVisual.FuenteNormal;
        KeyPreview = true;
        AcceptButton = null;
        CancelButton = null;

        var pnlPrincipal = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(24),
            BackColor = TemaVisual.FondoApp
        };

        var pnlTarjeta = new PanelCardModerno
        {
            Dock = DockStyle.Fill,
            BackColor = TemaVisual.FondoTarjeta,
            Padding = new Padding(24, 22, 24, 18)
        };

        var tblContenido = new TableLayoutPanel
        {
            BackColor = Color.Transparent,
            ColumnCount = 2,
            Dock = DockStyle.Fill,
            Margin = new Padding(0),
            RowCount = 2
        };
        tblContenido.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 58F));
        tblContenido.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tblContenido.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        tblContenido.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));

        _pnlIcono = new Panel
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 4, 16, 0),
            BackColor = Color.Transparent
        };
        _pnlIcono.Paint += PintarIcono;

        var pnlTexto = new Panel
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(0)
        };

        _lblTitulo = new Label
        {
            AutoSize = false,
            Dock = DockStyle.Top,
            Font = TemaVisual.FuenteSubtitulo,
            ForeColor = TemaVisual.TextoPrincipal,
            Height = 30,
            Text = titulo,
            TextAlign = ContentAlignment.MiddleLeft
        };

        _lblMensaje = new Label
        {
            AutoSize = false,
            Dock = DockStyle.Fill,
            Font = TemaVisual.FuenteNormal,
            ForeColor = TemaVisual.TextoMedio,
            Padding = new Padding(0, 2, 0, 0),
            Text = mensaje,
            TextAlign = ContentAlignment.TopLeft
        };

        pnlTexto.Controls.Add(_lblMensaje);
        pnlTexto.Controls.Add(_lblTitulo);
        tblContenido.Controls.Add(_pnlIcono, 0, 0);
        tblContenido.Controls.Add(pnlTexto, 1, 0);

        var pnlAcciones = new FlowLayoutPanel
        {
            AutoSize = false,
            BackColor = Color.Transparent,
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            Margin = new Padding(0),
            Padding = new Padding(0, 8, 0, 0),
            WrapContents = false
        };

        _btnAceptar = new BotonModerno
        {
            DialogResult = confirmacion ? DialogResult.Yes : DialogResult.OK,
            Estilo = EstiloBotonModerno.Primario,
            Icono = confirmacion ? IconChar.Check : IconChar.Check,
            IconoTamano = 14,
            Margin = new Padding(10, 0, 0, 0),
            Size = new Size(118, 36),
            Text = confirmacion ? "Sí" : "Aceptar"
        };
        pnlAcciones.Controls.Add(_btnAceptar);

        if (confirmacion)
        {
            _btnCancelar = new BotonModerno
            {
                DialogResult = DialogResult.No,
                Estilo = EstiloBotonModerno.Secundario,
                Icono = IconChar.Xmark,
                IconoTamano = 14,
                Margin = new Padding(0),
                Size = new Size(118, 36),
                Text = "No"
            };
            pnlAcciones.Controls.Add(_btnCancelar);
            CancelButton = _btnCancelar;
        }

        tblContenido.Controls.Add(pnlAcciones, 1, 1);
        pnlTarjeta.Controls.Add(tblContenido);
        pnlPrincipal.Controls.Add(pnlTarjeta);
        Controls.Add(pnlPrincipal);

        Shown += (_, _) => _btnAceptar.Focus();
        KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Escape)
            {
                DialogResult = confirmacion ? DialogResult.No : DialogResult.OK;
                Close();
            }
        };
        Resize += (_, _) => _pnlIcono.Invalidate();
    }

    public static DialogResult Mostrar(IWin32Window? propietario, string titulo, string mensaje, DialogoTipo tipo, bool confirmacion = false)
    {
        using var dialogo = new DialogoModernoForm(titulo, mensaje, tipo, confirmacion);
        return propietario is null ? dialogo.ShowDialog() : dialogo.ShowDialog(propietario);
    }

    private void PintarIcono(object? sender, PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        var color = ObtenerColor();
        using var brush = new SolidBrush(Color.FromArgb(25, color));
        e.Graphics.FillEllipse(brush, 2, 2, 46, 46);
        using var bmp = ObtenerIcono().ToBitmap(color, 22);
        e.Graphics.DrawImageUnscaled(bmp, 14, 14);
    }

    private Color ObtenerColor() => _tipo switch
    {
        DialogoTipo.Exito => TemaVisual.Exito,
        DialogoTipo.Advertencia => TemaVisual.Advertencia,
        DialogoTipo.Error => TemaVisual.Peligro,
        DialogoTipo.Confirmacion => TemaVisual.Primario,
        _ => TemaVisual.Primario
    };

    private IconChar ObtenerIcono() => _tipo switch
    {
        DialogoTipo.Exito => IconChar.CircleCheck,
        DialogoTipo.Advertencia => IconChar.TriangleExclamation,
        DialogoTipo.Error => IconChar.CircleXmark,
        DialogoTipo.Confirmacion => IconChar.CircleQuestion,
        _ => IconChar.CircleInfo
    };
}

public enum DialogoTipo
{
    Informacion,
    Exito,
    Advertencia,
    Error,
    Confirmacion
}
