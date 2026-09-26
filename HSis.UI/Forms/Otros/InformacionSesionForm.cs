#nullable enable
using System.Drawing.Drawing2D;
using System.Runtime.Versioning;
using FontAwesome.Sharp;
using HSis.UI.Controls;
using HSis.UI.Helpers;

namespace HSis.UI.Forms.Otros;

[SupportedOSPlatform("windows")]
public sealed class InformacionSesionForm : Form
{
    public InformacionSesionForm(
        IWin32Window? propietario,
        string usuario,
        string rol,
        string departamento,
        string puesto,
        string sucursal)
    {
        Text = "Información de Sesión";
        Name = "InformacionSesionForm";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        ShowInTaskbar = false;
        MaximizeBox = false;
        MinimizeBox = false;
        ClientSize = new Size(520, 390);
        MinimumSize = new Size(480, 360);
        BackColor = TemaVisual.FondoApp;
        Font = TemaVisual.FuenteNormal;

        var pnlPrincipal = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(24),
            BackColor = TemaVisual.FondoApp
        };

        var tarjeta = new PanelCardModerno
        {
            BackColor = TemaVisual.FondoTarjeta,
            Dock = DockStyle.Fill,
            Icono = IconChar.UserCircle,
            Titulo = "Sesión activa",
            Padding = new Padding(24, 52, 24, 18)
        };

        var tblContenido = new TableLayoutPanel
        {
            BackColor = Color.Transparent,
            ColumnCount = 1,
            Dock = DockStyle.Fill,
            Margin = new Padding(0),
            RowCount = 3
        };
        tblContenido.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tblContenido.RowStyles.Add(new RowStyle(SizeType.Absolute, 64F));
        tblContenido.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        tblContenido.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));

        var pnlUsuario = new Panel
        {
            BackColor = TemaVisual.PrimarioSuave,
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 0, 0, 12),
            Padding = new Padding(14, 8, 14, 8)
        };
        pnlUsuario.Paint += (_, e) =>
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using var pen = new Pen(TemaVisual.PrimarioBorde, 1F);
            using var path = TemaVisual.CrearRectanguloRedondeado(new Rectangle(0, 0, pnlUsuario.Width - 1, pnlUsuario.Height - 1), 8);
            e.Graphics.DrawPath(pen, path);
        };

        var lblUsuario = new Label
        {
            AutoSize = false,
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 11F, FontStyle.Bold),
            ForeColor = TemaVisual.TextoPrincipal,
            Text = usuario,
            TextAlign = ContentAlignment.MiddleLeft
        };
        var lblRol = new Label
        {
            AutoSize = false,
            Dock = DockStyle.Right,
            Font = TemaVisual.FuentePequena,
            ForeColor = TemaVisual.Primario,
            Text = rol,
            TextAlign = ContentAlignment.MiddleRight,
            Width = 120
        };
        pnlUsuario.Controls.Add(lblUsuario);
        pnlUsuario.Controls.Add(lblRol);

        var tblDatos = new TableLayoutPanel
        {
            BackColor = Color.Transparent,
            ColumnCount = 2,
            Dock = DockStyle.Fill,
            Margin = new Padding(0),
            RowCount = 3
        };
        tblDatos.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 142F));
        tblDatos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        for (int i = 0; i < 3; i++)
        {
            tblDatos.RowStyles.Add(new RowStyle(SizeType.Percent, 33.333F));
        }

        AgregarDato(tblDatos, 0, "Departamento", departamento);
        AgregarDato(tblDatos, 1, "Puesto", puesto);
        AgregarDato(tblDatos, 2, "Sucursal", sucursal);

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
        var btnCerrar = new BotonModerno
        {
            DialogResult = DialogResult.OK,
            Estilo = EstiloBotonModerno.Secundario,
            Icono = IconChar.Xmark,
            IconoTamano = 14,
            Margin = new Padding(0),
            Size = new Size(120, 36),
            Text = "Cerrar"
        };
        pnlAcciones.Controls.Add(btnCerrar);

        tblContenido.Controls.Add(pnlUsuario, 0, 0);
        tblContenido.Controls.Add(tblDatos, 0, 1);
        tblContenido.Controls.Add(pnlAcciones, 0, 2);
        tarjeta.Controls.Add(tblContenido);
        pnlPrincipal.Controls.Add(tarjeta);
        Controls.Add(pnlPrincipal);
        AcceptButton = btnCerrar;
        CancelButton = btnCerrar;

        if (propietario is Form formulario)
        {
            Owner = formulario;
        }
    }

    private static void AgregarDato(TableLayoutPanel tabla, int fila, string etiqueta, string valor)
    {
        tabla.Controls.Add(new Label
        {
            AutoSize = false,
            Dock = DockStyle.Fill,
            Font = TemaVisual.FuentePequena,
            ForeColor = TemaVisual.TextoSecundario,
            Text = etiqueta,
            TextAlign = ContentAlignment.MiddleLeft
        }, 0, fila);
        tabla.Controls.Add(new Label
        {
            AutoSize = false,
            Dock = DockStyle.Fill,
            Font = TemaVisual.FuenteNormal,
            ForeColor = TemaVisual.TextoPrincipal,
            Text = valor,
            TextAlign = ContentAlignment.MiddleLeft
        }, 1, fila);
    }
}
