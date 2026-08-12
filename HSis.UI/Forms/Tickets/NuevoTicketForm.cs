using System.ComponentModel;
using System.Runtime.Versioning;
using HSis.Data.Models;
using HSis.Logic.Constants;
using HSis.Logic.Services;
using HSis.UI.Controls;
using HSis.UI.Helpers;
using HSis.UI.Presenters;

namespace HSis.UI.Forms.Tickets
{
    [SupportedOSPlatform("windows")]
    public partial class NuevoTicketForm : Form, INuevoTicketView
    {
        private readonly NuevoTicketPresenter _presenter;
        private CajaTextoOrtograficaWpf rtbDescripcion = null!;

        public NuevoTicketForm(NuevoTicketPresenter presenter)
        {
            InitializeComponent();
            _presenter = presenter;
            _presenter.SetView(this);
            InicializarLayoutNuevoTicket();
        }

        #region Propiedades de INuevoTicketView
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Descripcion
        {
            get => rtbDescripcion.Text;
            set => rtbDescripcion.Text = value;
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string NombreSolicitanteTercero
        {
            get => txtNombreSolicitante.Text;
            set => txtNombreSolicitante.Text = value;
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool EsEnRepresentacion
        {
            get => chkSolicitanteEnRepresentacion.Checked;
            set => chkSolicitanteEnRepresentacion.Checked = value;
        }

        public void CargarClientes(List<Usuario> clientes, int idUsuarioSesion)
        {
            cmbSolicitante.Items.Clear();
            foreach (var u in clientes)
            {
                string label = string.IsNullOrWhiteSpace(u.Nombre) ? $"Usuario #{u.IdUsuario}" : u.Nombre;
                if (u.Departamento != null && !string.IsNullOrWhiteSpace(u.Departamento.Nombre))
                {
                    label += $" ({u.Departamento.Nombre})";
                }
                cmbSolicitante.Items.Add(new ElementoCombo<int>(label, u.IdUsuario));
            }

            var elementoUsuarioActual = cmbSolicitante.Items.OfType<ElementoCombo<int>>().FirstOrDefault(x => x.Valor == idUsuarioSesion);
            if (elementoUsuarioActual != null)
            {
                cmbSolicitante.SelectedItem = elementoUsuarioActual;
            }
            else if (cmbSolicitante.Items.Count > 0)
            {
                cmbSolicitante.SelectedIndex = 0;
            }
        }

        public void CargarTecnicos(List<Usuario> tecnicos, bool esTecnicoSesion, int idUsuarioSesion)
        {
            cmbTecnico.Items.Clear();
            cmbTecnico.Items.Add(new ElementoCombo<int?>("-- Sin Asignar --", null));

            foreach (var t in tecnicos)
            {
                string labelTecnico = t.Nombre ?? $"Técnico #{t.IdUsuario}";
                cmbTecnico.Items.Add(new ElementoCombo<int?>(labelTecnico, t.IdUsuario));
            }

            if (esTecnicoSesion)
            {
                var propioTecnico = cmbTecnico.Items.OfType<ElementoCombo<int?>>().FirstOrDefault(x => x.Valor == idUsuarioSesion);
                if (propioTecnico != null)
                {
                    cmbTecnico.SelectedItem = propioTecnico;
                }
                else
                {
                    cmbTecnico.SelectedIndex = 0;
                }
                cmbTecnico.Enabled = false;
            }
            else
            {
                cmbTecnico.SelectedIndex = 0;
                cmbTecnico.Enabled = true;
            }
        }

        public void CargarPrioridades()
        {
            cmbPrioridad.Items.Clear();
            cmbPrioridad.Items.Add(new ElementoCombo<string?>(ConstantesPrioridad.BAJA, ConstantesPrioridad.BAJA));
            cmbPrioridad.Items.Add(new ElementoCombo<string?>(ConstantesPrioridad.MEDIA, ConstantesPrioridad.MEDIA));
            cmbPrioridad.Items.Add(new ElementoCombo<string?>(ConstantesPrioridad.ALTA, ConstantesPrioridad.ALTA));
            cmbPrioridad.SelectedIndex = 0;
        }

        public void MostrarError(string titulo, string mensaje)
        {
            MessageBox.Show(mensaje, titulo, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public void MostrarExito(string mensaje)
        {
            MessageBox.Show(mensaje, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void CerrarExitoso()
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        public void MostrarCargando(bool cargando)
        {
            btnGuardar.Enabled = !cargando;
            this.UseWaitCursor = cargando;
        }
        #endregion

        #region UI Handlers


        private void chkSolicitanteEnRepresentacion_CheckedChanged(object? sender, EventArgs e)
        {
            txtNombreSolicitante.Enabled = chkSolicitanteEnRepresentacion.Checked;
            cmbSolicitante.Enabled = !chkSolicitanteEnRepresentacion.Checked;

            if (chkSolicitanteEnRepresentacion.Checked)
            {
                txtNombreSolicitante.Focus();
            }
            else
            {
                txtNombreSolicitante.Clear();
            }
        }

        private async void frmNuevoTicket_Load(object? sender, EventArgs e)
        {
            rtbDescripcion.Limpiar();
            if (SesionSistema.EsAdmin || SesionSistema.EsTecnico)
            {
                await _presenter.CargarCatalogosAsync();
            }
        }

        private async void btnGuardar_Click(object? sender, EventArgs e)
        {
            int idSolicitante = cmbSolicitante.SelectedItem is ElementoCombo<int> selSolicitante ? selSolicitante.Valor : SesionSistema.IdUsuario;
            int? idTecnico = cmbTecnico.SelectedItem is ElementoCombo<int?> selTecnico ? selTecnico.Valor : null;
            string? prioridad = cmbPrioridad.SelectedItem is ElementoCombo<string?> selPrioridad ? selPrioridad.Valor : null;

            await _presenter.GuardarTicketAsync(idSolicitante, idTecnico, prioridad);
        }

        private void btnCancelar_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        #endregion
    }
}
