using System;
using System.Drawing;
using System.Windows.Forms;
using HSis.UI.Controls;

namespace HSis.UI.Forms.Tickets
{
    partial class NuevoTicketForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblSolicitante = new Label();
            cmbSolicitante = new ComboBox();
            lblPrioridad = new Label();
            cmbPrioridad = new ComboBox();
            lblTecnico = new Label();
            cmbTecnico = new ComboBox();
            lblDescripcion = new Label();
            chkSolicitanteEnRepresentacion = new CheckBox();
            txtNombreSolicitante = new TextBox();
            btnGuardar = new Button();
            btnCancelar = new Button();
            SuspendLayout();
            // 
            // lblSolicitante
            // 
            lblSolicitante.AutoSize = true;
            lblSolicitante.Location = new Point(0, 0);
            lblSolicitante.Name = "lblSolicitante";
            lblSolicitante.Size = new Size(100, 23);
            lblSolicitante.TabIndex = 0;
            lblSolicitante.Text = "Cliente / Solicitante:";
            // 
            // cmbSolicitante
            // 
            cmbSolicitante.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSolicitante.Location = new Point(0, 0);
            cmbSolicitante.Name = "cmbSolicitante";
            cmbSolicitante.Size = new Size(121, 23);
            cmbSolicitante.TabIndex = 0;
            // 
            // lblPrioridad
            // 
            lblPrioridad.AutoSize = true;
            lblPrioridad.Location = new Point(0, 0);
            lblPrioridad.Name = "lblPrioridad";
            lblPrioridad.Size = new Size(100, 23);
            lblPrioridad.TabIndex = 0;
            lblPrioridad.Text = "Prioridad:";
            // 
            // cmbPrioridad
            // 
            cmbPrioridad.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPrioridad.Location = new Point(0, 0);
            cmbPrioridad.Name = "cmbPrioridad";
            cmbPrioridad.Size = new Size(121, 23);
            cmbPrioridad.TabIndex = 0;
            // 
            // lblTecnico
            // 
            lblTecnico.AutoSize = true;
            lblTecnico.Location = new Point(0, 0);
            lblTecnico.Name = "lblTecnico";
            lblTecnico.Size = new Size(100, 23);
            lblTecnico.TabIndex = 0;
            lblTecnico.Text = "Asignar Técnico:";
            // 
            // cmbTecnico
            // 
            cmbTecnico.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTecnico.Location = new Point(0, 0);
            cmbTecnico.Name = "cmbTecnico";
            cmbTecnico.Size = new Size(121, 23);
            cmbTecnico.TabIndex = 0;
            // 
            // lblDescripcion
            // 
            lblDescripcion.AutoSize = true;
            lblDescripcion.Font = new Font("Segoe UI", 11F);
            lblDescripcion.Location = new Point(12, 15);
            lblDescripcion.Name = "lblDescripcion";
            lblDescripcion.Size = new Size(198, 19);
            lblDescripcion.TabIndex = 0;
            lblDescripcion.Text = "Descripción del Problema:";
            // 
            // chkSolicitanteEnRepresentacion
            // 
            chkSolicitanteEnRepresentacion.AutoSize = true;
            chkSolicitanteEnRepresentacion.Location = new Point(0, 0);
            chkSolicitanteEnRepresentacion.Name = "chkSolicitanteEnRepresentacion";
            chkSolicitanteEnRepresentacion.Size = new Size(104, 24);
            chkSolicitanteEnRepresentacion.TabIndex = 0;
            chkSolicitanteEnRepresentacion.Text = "Solicitante no registrado en el sistema";
            chkSolicitanteEnRepresentacion.CheckedChanged += chkSolicitanteEnRepresentacion_CheckedChanged;
            // 
            // txtNombreSolicitante
            // 
            txtNombreSolicitante.Enabled = false;
            txtNombreSolicitante.Location = new Point(0, 0);
            txtNombreSolicitante.Name = "txtNombreSolicitante";
            txtNombreSolicitante.PlaceholderText = "Nombre del solicitante no registrado...";
            txtNombreSolicitante.Size = new Size(100, 23);
            txtNombreSolicitante.TabIndex = 0;
            // 
            // btnGuardar
            // 
            btnGuardar.BackColor = Color.FromArgb(39, 174, 96);
            btnGuardar.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnGuardar.ForeColor = Color.White;
            btnGuardar.Location = new Point(267, 205);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(95, 33);
            btnGuardar.TabIndex = 2;
            btnGuardar.Text = "Guardar";
            btnGuardar.UseVisualStyleBackColor = false;
            btnGuardar.Click += btnGuardar_Click;
            // 
            // btnCancelar
            // 
            btnCancelar.BackColor = Color.FromArgb(231, 76, 60);
            btnCancelar.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnCancelar.ForeColor = Color.White;
            btnCancelar.Location = new Point(377, 205);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(95, 33);
            btnCancelar.TabIndex = 3;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = false;
            btnCancelar.Click += btnCancelar_Click;
            // 
            // NuevoTicketForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(551, 466);
            Font = new Font("Segoe UI", 11F);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "NuevoTicketForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Crear / Registrar Nuevo Ticket";
            Load += frmNuevoTicket_Load;
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Label lblSolicitante;
        private System.Windows.Forms.ComboBox cmbSolicitante;
        private System.Windows.Forms.Label lblPrioridad;
        private System.Windows.Forms.ComboBox cmbPrioridad;
        private System.Windows.Forms.Label lblTecnico;
        private System.Windows.Forms.ComboBox cmbTecnico;
        private System.Windows.Forms.Label lblDescripcion;
        private System.Windows.Forms.CheckBox chkSolicitanteEnRepresentacion;
        private System.Windows.Forms.TextBox txtNombreSolicitante;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnCancelar;
        // rtbDescripcion será creado dinámicamente en el código del formulario
    }
}
