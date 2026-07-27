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
            this.lblSolicitante = new System.Windows.Forms.Label();
            this.cmbSolicitante = new System.Windows.Forms.ComboBox();
            this.lblPrioridad = new System.Windows.Forms.Label();
            this.cmbPrioridad = new System.Windows.Forms.ComboBox();
            this.lblTecnico = new System.Windows.Forms.Label();
            this.cmbTecnico = new System.Windows.Forms.ComboBox();
            this.lblDescripcion = new System.Windows.Forms.Label();
            this.rtbDescripcion = new CajaTextoOrtograficaWpf();
            this.chkSolicitanteEnRepresentacion = new System.Windows.Forms.CheckBox();
            this.txtNombreSolicitante = new System.Windows.Forms.TextBox();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.SuspendLayout();

            // chkSolicitanteEnRepresentacion
            this.chkSolicitanteEnRepresentacion.AutoSize = true;
            this.chkSolicitanteEnRepresentacion.Text = "Solicitante no registrado en el sistema";
            this.chkSolicitanteEnRepresentacion.CheckedChanged += new System.EventHandler(this.chkSolicitanteEnRepresentacion_CheckedChanged);

            // txtNombreSolicitante
            this.txtNombreSolicitante.Enabled = false;
            this.txtNombreSolicitante.PlaceholderText = "Nombre del solicitante no registrado...";

            // lblSolicitante
            this.lblSolicitante.AutoSize = true;
            this.lblSolicitante.Text = "Cliente / Solicitante:";
            // cmbSolicitante
            this.cmbSolicitante.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;

            // lblPrioridad
            this.lblPrioridad.AutoSize = true;
            this.lblPrioridad.Text = "Prioridad:";
            // cmbPrioridad
            this.cmbPrioridad.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;

            // lblTecnico
            this.lblTecnico.AutoSize = true;
            this.lblTecnico.Text = "Asignar Técnico:";
            // cmbTecnico
            this.cmbTecnico.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;

            // lblDescripcion
            this.lblDescripcion.AutoSize = true;
            this.lblDescripcion.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblDescripcion.Location = new System.Drawing.Point(12, 15);
            this.lblDescripcion.Name = "lblDescripcion";
            this.lblDescripcion.Size = new System.Drawing.Size(198, 19);
            this.lblDescripcion.TabIndex = 0;
            this.lblDescripcion.Text = "Descripción del Problema:";

            // rtbDescripcion
            this.rtbDescripcion.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.rtbDescripcion.Location = new System.Drawing.Point(12, 40);
            this.rtbDescripcion.Name = "rtbDescripcion";
            this.rtbDescripcion.Size = new System.Drawing.Size(460, 150);
            this.rtbDescripcion.TabIndex = 1;
            this.rtbDescripcion.Text = "";

            // btnGuardar
            this.btnGuardar.BackColor = System.Drawing.Color.FromArgb(39, 174, 96);
            this.btnGuardar.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnGuardar.ForeColor = System.Drawing.Color.White;
            this.btnGuardar.Location = new System.Drawing.Point(267, 205);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(95, 33);
            this.btnGuardar.TabIndex = 2;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.UseVisualStyleBackColor = false;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);

            // btnCancelar
            this.btnCancelar.BackColor = System.Drawing.Color.FromArgb(231, 76, 60);
            this.btnCancelar.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnCancelar.ForeColor = System.Drawing.Color.White;
            this.btnCancelar.Location = new System.Drawing.Point(377, 205);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(95, 33);
            this.btnCancelar.TabIndex = 3;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);

            // NuevoTicketForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(560, 420);
            this.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NuevoTicketForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Crear / Registrar Nuevo Ticket";
            this.Load += new System.EventHandler(this.frmNuevoTicket_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblSolicitante;
        private System.Windows.Forms.ComboBox cmbSolicitante;
        private System.Windows.Forms.Label lblPrioridad;
        private System.Windows.Forms.ComboBox cmbPrioridad;
        private System.Windows.Forms.Label lblTecnico;
        private System.Windows.Forms.ComboBox cmbTecnico;
        private System.Windows.Forms.Label lblDescripcion;
        private CajaTextoOrtograficaWpf rtbDescripcion;
        private System.Windows.Forms.CheckBox chkSolicitanteEnRepresentacion;
        private System.Windows.Forms.TextBox txtNombreSolicitante;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnCancelar;
    }
}
