#nullable enable
using System.Drawing.Drawing2D;
using System.Runtime.Versioning;

namespace HSis.UI.Helpers
{
    [SupportedOSPlatform("windows")]
    public static class TemaVisual
    {
        // Paleta base (Clean SaaS / Modern Slate)
        public static readonly Color FondoApp = Color.FromArgb(248, 250, 252);        // #F8FAFC (Slate 50)
        public static readonly Color FondoTenue = Color.FromArgb(241, 245, 249);      // #F1F5F9 (Slate 100)
        public static readonly Color FondoTarjeta = Color.FromArgb(255, 255, 255);    // #FFFFFF
        public static readonly Color BordeSutil = Color.FromArgb(226, 232, 240);       // #E2E8F0 (Slate 200)
        public static readonly Color BordeHover = Color.FromArgb(203, 213, 225);       // #CBD5E1 (Slate 300)

        // Tipografía / Textos
        public static readonly Color TextoPrincipal = Color.FromArgb(15, 23, 42);     // #0F172A (Slate 900)
        public static readonly Color TextoOscuro = Color.FromArgb(51, 65, 85);        // #334155 (Slate 700)
        public static readonly Color TextoMedio = Color.FromArgb(71, 85, 105);        // #475569 (Slate 600)
        public static readonly Color TextoSecundario = Color.FromArgb(100, 116, 139); // #64748B (Slate 500)
        public static readonly Color TextoMuted = Color.FromArgb(148, 163, 184);      // #94A3B8 (Slate 400)

        // Colores de Acento / Primarios
        public static readonly Color Primario = Color.FromArgb(37, 99, 235);          // #2563EB (Royal Blue)
        public static readonly Color PrimarioHover = Color.FromArgb(29, 78, 216);     // #1D4ED8
        public static readonly Color PrimarioSuave = Color.FromArgb(239, 246, 255);   // #EFF6FF (Blue 50)
        public static readonly Color PrimarioBorde = Color.FromArgb(191, 219, 254);   // #BFDBFE (Blue 200)

        // Semántica de Tickets / KPIs / Estados
        public static readonly Color TicketDisponible = Color.FromArgb(59, 130, 246);   // #3B82F6 (Azul)
        public static readonly Color TicketDisponibleBg = Color.FromArgb(239, 246, 255); // #EFF6FF
        public static readonly Color TicketNuevo = TicketDisponible;
        public static readonly Color TicketNuevoBg = TicketDisponibleBg;

        public static readonly Color TicketEnProceso = Color.FromArgb(245, 158, 11);   // #F59E0B (Ámbar)
        public static readonly Color TicketEnProcesoBg = Color.FromArgb(254, 243, 199); // #FEF3C7

        public static readonly Color TicketUrgente = Color.FromArgb(239, 68, 68);      // #EF4444 (Rojo)
        public static readonly Color TicketUrgenteBg = Color.FromArgb(254, 226, 226);   // #FEE2E2

        public static readonly Color TicketCerrado = Color.FromArgb(16, 185, 129);     // #10B981 (Esmeralda)
        public static readonly Color TicketCerradoBg = Color.FromArgb(209, 250, 229);   // #D1FAE5

        public static readonly Color TicketReabierto = Color.FromArgb(139, 92, 246);   // #8B5CF6 (Púrpura)
        public static readonly Color TicketReabiertoBg = Color.FromArgb(237, 233, 254); // #EDE9FE

        public static readonly Color PrioridadMedia = Color.FromArgb(217, 119, 6);     // #D97706 (Ámbar oscuro)
        public static readonly Color PrioridadBaja = Color.FromArgb(71, 85, 105);      // #475569 (Slate 600)

        // Estados y Acciones (Éxito, Peligro, Advertencia)
        public static readonly Color Exito = Color.FromArgb(34, 197, 94);              // #22C55E
        public static readonly Color Peligro = Color.FromArgb(239, 68, 68);            // #EF4444
        public static readonly Color PeligroHover = Color.FromArgb(220, 38, 38);       // #DC2626
        public static readonly Color Advertencia = Color.FromArgb(251, 191, 36);       // #FBBF24

        // Indicador de Conexión / Píldoras de Estado
        public static readonly Color OnlineBg = Color.FromArgb(236, 253, 245);        // #ECFDF5
        public static readonly Color OnlineBorde = Color.FromArgb(167, 243, 208);     // #A7F3D0
        public static readonly Color OnlinePunto = Color.FromArgb(16, 185, 129);      // #10B981
        public static readonly Color OnlineTexto = Color.FromArgb(6, 95, 70);         // #065F46

        public static readonly Color OfflineBg = Color.FromArgb(254, 243, 199);       // #FEF3C7
        public static readonly Color OfflineBorde = Color.FromArgb(253, 230, 138);    // #FDE68A
        public static readonly Color OfflinePunto = Color.FromArgb(245, 158, 11);     // #F59E0B
        public static readonly Color OfflineTexto = Color.FromArgb(146, 64, 14);      // #92400E

        // Calificación y Encuestas (Estrellas / CSAT)
        public static readonly Color EstrellaActiva = Color.FromArgb(241, 196, 15);   // #F1C40F (Dorado)
        public static readonly Color EstrellaInactiva = Color.FromArgb(203, 213, 225);// #CBD5E1 (Slate 300)

        // Sidebar
        public static readonly Color SidebarFondo = Color.FromArgb(15, 23, 42);        // #0F172A
        public static readonly Color SidebarItemHover = Color.FromArgb(30, 41, 59);    // #1E293B
        public static readonly Color SidebarItemActivo = Color.FromArgb(37, 99, 235);   // #2563EB
        public static readonly Color SidebarItemTexto = Color.FromArgb(203, 213, 225); // #CBD5E1

        // Fuentes estándar
        public static readonly Font FuenteTitulo = new("Segoe UI", 12f, FontStyle.Bold);
        public static readonly Font FuenteSubtitulo = new("Segoe UI", 10f, FontStyle.Bold);
        public static readonly Font FuenteNormal = new("Segoe UI", 9.5f, FontStyle.Regular);
        public static readonly Font FuentePequena = new("Segoe UI", 8.5f, FontStyle.Regular);
        public static readonly Font FuenteBadge = new("Segoe UI", 8.5f, FontStyle.Bold);

        /// <summary>
        /// Obtiene los colores de badge (Texto, Fondo) según el estatus o prioridad
        /// </summary>
        public static (Color Texto, Color Fondo) ObtenerColoresBadge(string? valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return (TextoSecundario, FondoTenue);

            string v = valor.Trim().ToUpperInvariant();

            if (v.Contains("NUEVO"))
                return (TicketNuevo, TicketNuevoBg);
            if (v.Contains("PROCESO"))
                return (TicketEnProceso, TicketEnProcesoBg);
            if (v.Contains("URGENTE") || v.Contains("ALTA"))
                return (TicketUrgente, TicketUrgenteBg);
            if (v.Contains("CERRADO") || v.Contains("RESUELTO") || v.Contains("ÉXITO"))
                return (TicketCerrado, TicketCerradoBg);
            if (v.Contains("REABIERTO"))
                return (TicketReabierto, TicketReabiertoBg);
            if (v.Contains("MEDIA"))
                return (PrioridadMedia, TicketEnProcesoBg);
            if (v.Contains("BAJA"))
                return (PrioridadBaja, FondoTenue);

            return (TextoPrincipal, FondoTenue);
        }

        /// <summary>
        /// Aplica una apariencia moderna, limpia y espaciosa a un DataGridView
        /// </summary>
        public static void AplicarTemaModerno(this DataGridView dgv)
        {
            dgv.BackgroundColor = FondoTarjeta;
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.GridColor = BordeSutil;
            dgv.EnableHeadersVisualStyles = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.RowHeadersVisible = false;
            dgv.AutoGenerateColumns = true;
            dgv.AllowUserToResizeRows = false;
            dgv.ShowCellToolTips = true;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Fila de encabezado
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = FondoTenue;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = TextoMedio;
            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = FondoTenue;
            dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor = TextoMedio;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(12, 10, 12, 10);
            dgv.ColumnHeadersHeight = 44;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            // Filas de datos
            dgv.DefaultCellStyle.BackColor = FondoTarjeta;
            dgv.DefaultCellStyle.ForeColor = TextoPrincipal;
            dgv.DefaultCellStyle.Font = FuenteNormal;
            dgv.DefaultCellStyle.Padding = new Padding(12, 6, 12, 6);
            dgv.DefaultCellStyle.SelectionBackColor = PrimarioSuave;
            dgv.DefaultCellStyle.SelectionForeColor = TextoPrincipal;
            dgv.RowTemplate.Height = 42;

            // Filas alternas sutiles
            dgv.AlternatingRowsDefaultCellStyle.BackColor = FondoApp;
            dgv.AlternatingRowsDefaultCellStyle.SelectionBackColor = PrimarioSuave;
            dgv.AlternatingRowsDefaultCellStyle.SelectionForeColor = TextoPrincipal;

            // Pintado personalizado de Badges para columnas como Estatus o Prioridad
            dgv.CellPainting -= Dgv_CellPainting;
            dgv.CellPainting += Dgv_CellPainting;
        }

        private static void Dgv_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.ColumnIndex < 0 || sender is not DataGridView dgv || e.Graphics == null)
                return;

            // 1. Evitar resaltado estridente en encabezados de columna
            if (e.RowIndex == -1)
            {
                using var brushH = new SolidBrush(FondoTenue);
                e.Graphics.FillRectangle(brushH, e.CellBounds);

                using var penBorder = new Pen(BordeSutil, 1f);
                e.Graphics.DrawLine(penBorder, e.CellBounds.Left, e.CellBounds.Bottom - 1, e.CellBounds.Right, e.CellBounds.Bottom - 1);

                e.Paint(e.CellBounds, DataGridViewPaintParts.ContentForeground | DataGridViewPaintParts.ContentBackground);
                e.Handled = true;
                return;
            }

            if (e.RowIndex < 0) return;

            string colName = dgv.Columns[e.ColumnIndex].Name.ToLowerInvariant();
            if (colName.Contains("estatus") || colName.Contains("prioridad") || colName.Contains("status"))
            {
                e.PaintBackground(e.CellBounds, true);

                string valor = e.Value?.ToString() ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(valor))
                {
                    var (colorTexto, colorFondo) = ObtenerColoresBadge(valor);

                    using var sf = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };

                    var textoSize = e.Graphics.MeasureString(valor, FuenteBadge);
                    int badgeWidth = Math.Max((int)textoSize.Width + 16, 70);
                    int badgeHeight = 24;

                    int x = e.CellBounds.X + 8;
                    int y = e.CellBounds.Y + (e.CellBounds.Height - badgeHeight) / 2;
                    var badgeRect = new Rectangle(x, y, badgeWidth, badgeHeight);

                    // Dibujar pastilla redondeada
                    using var path = CrearRectanguloRedondeado(badgeRect, 6);
                    using var brushFondo = new SolidBrush(colorFondo);
                    using var brushTexto = new SolidBrush(colorTexto);
                    using var penBorde = new Pen(Color.FromArgb(40, colorTexto), 1f);

                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    e.Graphics.FillPath(brushFondo, path);
                    e.Graphics.DrawPath(penBorde, path);
                    e.Graphics.DrawString(valor, FuenteBadge, brushTexto, badgeRect, sf);
                }

                e.Handled = true;
            }
        }

        public static GraphicsPath CrearRectanguloRedondeado(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            Size size = new(diameter, diameter);
            Rectangle arc = new(bounds.Location, size);
            GraphicsPath path = new();

            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            // Top left
            path.AddArc(arc, 180, 90);

            // Top right
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);

            // Bottom right
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // Bottom left
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }
    }
}
