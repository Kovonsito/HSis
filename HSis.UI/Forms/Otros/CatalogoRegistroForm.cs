#nullable enable
using System.Globalization;
using System.Runtime.Versioning;
using System.Text.Json;
using FontAwesome.Sharp;
using HSis.Contracts.DTOs;
using HSis.Contracts.Services;
using HSis.UI.Controls;
using HSis.UI.Helpers;

namespace HSis.UI.Forms.Otros;

[SupportedOSPlatform("windows")]
public sealed class CatalogoRegistroForm : Form
{
    private readonly CatalogoDefinicion _definicion;
    private readonly ICatalogoGestionService _catalogoService;
    private readonly Dictionary<string, Control> _controlesPorPropiedad = new(StringComparer.OrdinalIgnoreCase);
    private readonly BotonModerno _btnGuardar;
    private readonly BotonModerno _btnCancelar;
    private bool _referenciasCargadas;

    public CatalogoRegistroForm(
        CatalogoDefinicion definicion,
        ICatalogoGestionService catalogoService)
    {
        _definicion = definicion ?? throw new ArgumentNullException(nameof(definicion));
        _catalogoService = catalogoService ?? throw new ArgumentNullException(nameof(catalogoService));

        Text = $"Nuevo registro - {_definicion.Titulo}";
        Name = $"frmNuevo{_definicion.Clave}";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MinimizeBox = false;
        MaximizeBox = false;
        ShowInTaskbar = false;
        BackColor = TemaVisual.FondoApp;
        ClientSize = new Size(650, CalcularAlto());

        var titulo = new Label
        {
            Text = $"Nuevo registro de {_definicion.Titulo}",
            Dock = DockStyle.Top,
            Height = 52,
            Padding = new Padding(16, 14, 16, 0),
            Font = TemaVisual.FuenteTitulo,
            ForeColor = TemaVisual.TextoPrincipal,
            BackColor = TemaVisual.FondoApp
        };

        var panelCampos = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            Padding = new Padding(16, 4, 16, 8),
            BackColor = TemaVisual.FondoApp
        };
        panelCampos.Controls.Add(CrearTablaCampos());

        var panelBotones = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 58,
            Padding = new Padding(16, 8, 16, 10),
            BackColor = TemaVisual.FondoApp
        };

        _btnGuardar = CrearBoton("Guardar", IconChar.Check, EstiloBotonModerno.Exito, 125);
        _btnCancelar = CrearBoton("Cancelar", IconChar.Xmark, EstiloBotonModerno.Secundario, 125);
        _btnGuardar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        _btnCancelar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        _btnGuardar.Location = new Point(panelBotones.ClientSize.Width - _btnGuardar.Width - _btnCancelar.Width - 8, 8);
        _btnCancelar.Location = new Point(panelBotones.ClientSize.Width - _btnCancelar.Width, 8);
        _btnGuardar.Click += async (_, _) => await GuardarAsync();
        _btnCancelar.Click += (_, _) => DialogResult = DialogResult.Cancel;
        panelBotones.Resize += (_, _) =>
        {
            _btnGuardar.Location = new Point(panelBotones.ClientSize.Width - _btnGuardar.Width - _btnCancelar.Width - 8, 8);
            _btnCancelar.Location = new Point(panelBotones.ClientSize.Width - _btnCancelar.Width, 8);
        };
        panelBotones.Controls.Add(_btnGuardar);
        panelBotones.Controls.Add(_btnCancelar);

        Controls.Add(panelCampos);
        Controls.Add(panelBotones);
        Controls.Add(titulo);

        AcceptButton = _btnGuardar;
        CancelButton = _btnCancelar;
        Shown += async (_, _) => await CargarReferenciasAsync();
    }

    private int CalcularAlto()
    {
        var altoCampos = _definicion.Campos.Sum(campo =>
            campo.Tipo == TipoCampoCatalogo.TextoMultilinea ? 98 : 58);
        return Math.Clamp(altoCampos + 150, 300, 720);
    }

    private TableLayoutPanel CrearTablaCampos()
    {
        var tabla = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 2,
            RowCount = _definicion.Campos.Count,
            BackColor = TemaVisual.FondoApp,
            GrowStyle = TableLayoutPanelGrowStyle.AddRows
        };
        tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 190));
        tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        for (var indice = 0; indice < _definicion.Campos.Count; indice++)
        {
            var campo = _definicion.Campos[indice];
            var etiqueta = new Label
            {
                Text = campo.Requerido ? $"{campo.Etiqueta} *" : campo.Etiqueta,
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                Font = TemaVisual.FuenteNormal,
                ForeColor = TemaVisual.TextoPrincipal,
                Margin = new Padding(0, 10, 12, 0)
            };
            var editor = CrearEditor(campo);
            _controlesPorPropiedad[campo.Propiedad] = editor;

            tabla.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tabla.Controls.Add(etiqueta, 0, indice);
            tabla.Controls.Add(editor, 1, indice);
        }

        return tabla;
    }

    private static Control CrearEditor(CatalogoCampo campo)
    {
        if (campo.Tipo == TipoCampoCatalogo.Combo)
        {
            return new ComboModerno
            {
                Dock = DockStyle.Fill,
                Height = 38,
                Margin = new Padding(0, 4, 0, 4),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
        }

        var caja = new CajaTextoModerna
        {
            Dock = DockStyle.Fill,
            Height = campo.Tipo == TipoCampoCatalogo.TextoMultilinea ? 82 : 38,
            MinimumSize = new Size(0, campo.Tipo == TipoCampoCatalogo.TextoMultilinea ? 82 : 38),
            Margin = new Padding(0, 4, 0, 4),
            Multiline = campo.Tipo == TipoCampoCatalogo.TextoMultilinea,
            Placeholder = ObtenerMarcador(campo.Tipo)
        };

        if (campo.Tipo == TipoCampoCatalogo.Password)
        {
            caja.UseSystemPasswordChar = true;
        }

        if (campo.ValorInicial != null)
        {
            caja.Text = ConvertirTexto(campo.ValorInicial) ?? string.Empty;
        }

        return caja;
    }

    private async Task CargarReferenciasAsync()
    {
        if (_referenciasCargadas || IsDisposed)
        {
            return;
        }

        _btnGuardar.Enabled = false;
        await this.EjecutarOperacionAsync(async () =>
        {
            foreach (var campo in _definicion.Campos.Where(campo => campo.Referencia != null))
            {
                if (_controlesPorPropiedad[campo.Propiedad] is not ComboModerno combo || campo.Referencia is null)
                {
                    continue;
                }

                var registros = await _catalogoService.ObtenerRegistrosAsync(campo.Referencia.Entidad);
                var opciones = new List<ElementoCombo<string>>
                {
                    new("-- Seleccione --", string.Empty)
                };
                var clavesAgregadas = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                foreach (var registro in registros)
                {
                    var clave = ConvertirTexto(registro.Obtener(campo.Referencia.Clave));
                    if (string.IsNullOrWhiteSpace(clave) || !clavesAgregadas.Add(clave))
                    {
                        continue;
                    }

                    var texto = ConvertirTexto(registro.Obtener(campo.Referencia.Texto));
                    opciones.Add(new ElementoCombo<string>(
                        string.IsNullOrWhiteSpace(texto) ? clave : texto,
                        clave));
                }

                combo.DataSource = null;
                combo.DisplayMember = nameof(ElementoCombo<string>.Texto);
                combo.ValueMember = nameof(ElementoCombo<string>.Valor);
                combo.DataSource = opciones;

                if (campo.ValorInicial != null)
                {
                    combo.SelectedValue = ConvertirTexto(campo.ValorInicial);
                }
            }

            _referenciasCargadas = true;
        }, "No se pudieron cargar las opciones de los catálogos relacionados", _btnGuardar);
        _btnGuardar.Enabled = true;
    }

    private async Task GuardarAsync()
    {
        if (!TryConstruirRegistro(out var registro))
        {
            return;
        }

        await this.EjecutarOperacionAsync(async () =>
        {
            await _catalogoService.CrearRegistroAsync(_definicion.EntidadApi, registro);
            DialogResult = DialogResult.OK;
            Close();
        }, "No se pudo crear el registro del catálogo", _btnGuardar, _btnCancelar);
    }

    private bool TryConstruirRegistro(out CatalogoRegistroDto registro)
    {
        registro = new CatalogoRegistroDto();
        var errores = new List<string>();

        foreach (var campo in _definicion.Campos)
        {
            var control = _controlesPorPropiedad[campo.Propiedad];
            switch (campo.Tipo)
            {
                case TipoCampoCatalogo.Combo:
                    var clave = ObtenerValorCombo(control as ComboModerno);
                    if (campo.Requerido && string.IsNullOrWhiteSpace(clave))
                    {
                        errores.Add($"Seleccione {campo.Etiqueta.ToLowerInvariant()}.");
                    }
                    registro.Valores[campo.Propiedad] = string.IsNullOrWhiteSpace(clave)
                        ? null
                        : ConvertirNumeroSiProcede(clave);
                    break;

                case TipoCampoCatalogo.Entero:
                    var textoEntero = ObtenerTexto(control);
                    if (string.IsNullOrWhiteSpace(textoEntero) && !campo.Requerido)
                    {
                        registro.Valores[campo.Propiedad] = null;
                    }
                    else if (!int.TryParse(textoEntero, NumberStyles.Integer, CultureInfo.CurrentCulture, out var entero) || entero < 0)
                    {
                        errores.Add($"{campo.Etiqueta} debe ser un número entero no negativo.");
                    }
                    else
                    {
                        registro.Valores[campo.Propiedad] = entero;
                    }
                    break;

                case TipoCampoCatalogo.Decimal:
                    var textoDecimal = ObtenerTexto(control);
                    if (string.IsNullOrWhiteSpace(textoDecimal) && !campo.Requerido)
                    {
                        registro.Valores[campo.Propiedad] = null;
                    }
                    else if (!decimal.TryParse(textoDecimal, NumberStyles.Number, CultureInfo.CurrentCulture, out var decimalValue) || decimalValue < 0)
                    {
                        errores.Add($"{campo.Etiqueta} debe ser un número decimal no negativo.");
                    }
                    else
                    {
                        registro.Valores[campo.Propiedad] = decimalValue;
                    }
                    break;

                default:
                    var texto = ObtenerTexto(control);
                    if (campo.Requerido && string.IsNullOrWhiteSpace(texto))
                    {
                        errores.Add($"Capture {campo.Etiqueta.ToLowerInvariant()}.");
                    }
                    registro.Valores[campo.Propiedad] = campo.Tipo == TipoCampoCatalogo.Password
                        ? texto
                        : texto?.Trim();
                    break;
            }
        }

        if (errores.Count > 0)
        {
            DialogoUIHelper.MostrarAdvertencia(
                string.Join(Environment.NewLine, errores.Select(error => $"• {error}")),
                "Validación de datos");
            return false;
        }

        return true;
    }

    private static string? ObtenerValorCombo(ComboModerno? combo)
    {
        if (combo?.SelectedItem is ElementoCombo<string> opcion)
        {
            return opcion.Valor;
        }

        return combo?.SelectedValue is null
            ? null
            : ConvertirTexto(combo.SelectedValue);
    }

    private static string? ObtenerTexto(Control control)
    {
        return control is CajaTextoModerna caja
            ? caja.Text
            : control.Text;
    }

    private static object ConvertirNumeroSiProcede(string valor)
    {
        return int.TryParse(valor, NumberStyles.Integer, CultureInfo.InvariantCulture, out var numero)
            ? numero
            : valor;
    }

    private static string? ConvertirTexto(object? valor)
    {
        if (valor == null)
        {
            return null;
        }

        if (valor is JsonElement elemento)
        {
            return elemento.ValueKind switch
            {
                JsonValueKind.Null or JsonValueKind.Undefined => null,
                JsonValueKind.String => elemento.GetString(),
                _ => elemento.ToString()
            };
        }

        return Convert.ToString(valor, CultureInfo.CurrentCulture);
    }

    private static string ObtenerMarcador(TipoCampoCatalogo tipo)
    {
        return tipo switch
        {
            TipoCampoCatalogo.Entero => "Ingrese un número entero",
            TipoCampoCatalogo.Decimal => "Ingrese un importe",
            TipoCampoCatalogo.Password => "Ingrese una contraseña",
            _ => string.Empty
        };
    }

    private static BotonModerno CrearBoton(
        string texto,
        IconChar icono,
        EstiloBotonModerno estilo,
        int ancho)
    {
        return new BotonModerno
        {
            Text = texto,
            Icono = icono,
            IconoTamano = 14,
            Estilo = estilo,
            Size = new Size(ancho, 36)
        };
    }
}
