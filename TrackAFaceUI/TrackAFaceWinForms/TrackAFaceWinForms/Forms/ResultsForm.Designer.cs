namespace TrackAFaceWinForms.Forms
{
    partial class ResultsForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        private void InitializeComponent()
        {
            this.pnlSummary = new System.Windows.Forms.Panel();
            this.lblTotalCost = new System.Windows.Forms.Label();
            this.lblSummaryTitle = new System.Windows.Forms.Label();
            this.lblStaffTitle = new System.Windows.Forms.Label();
            this.dgvStaff = new System.Windows.Forms.DataGridView();
            this.lblEquipmentTitle = new System.Windows.Forms.Label();
            this.dgvEquipment = new System.Windows.Forms.DataGridView();
            this.lblLocationTitle = new System.Windows.Forms.Label();
            this.dgvLocation = new System.Windows.Forms.DataGridView();
            this.lblOperationalTitle = new System.Windows.Forms.Label();
            this.dgvOperational = new System.Windows.Forms.DataGridView();
            this.btnExportCsv = new System.Windows.Forms.Button();
            this.btnExportPdf = new System.Windows.Forms.Button();
            this.btnNewAnalysis = new System.Windows.Forms.Button();
            this.pnlSummary.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStaff)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEquipment)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLocation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOperational)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlSummary
            // 
            this.pnlSummary.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.pnlSummary.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlSummary.Controls.Add(this.lblTotalCost);
            this.pnlSummary.Controls.Add(this.lblSummaryTitle);
            this.pnlSummary.Location = new System.Drawing.Point(20, 20);
            this.pnlSummary.Name = "pnlSummary";
            this.pnlSummary.Size = new System.Drawing.Size(760, 80);
            this.pnlSummary.TabIndex = 0;
            // 
            // lblSummaryTitle
            // 
            this.lblSummaryTitle.AutoSize = true;
            this.lblSummaryTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lblSummaryTitle.Location = new System.Drawing.Point(10, 10);
            this.lblSummaryTitle.Name = "lblSummaryTitle";
            this.lblSummaryTitle.Size = new System.Drawing.Size(88, 17);
            this.lblSummaryTitle.TabIndex = 0;
            this.lblSummaryTitle.Text = "Coût Total:";
            // 
            // lblTotalCost
            // 
            this.lblTotalCost.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold);
            this.lblTotalCost.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(67)))), ((int)(((byte)(54)))));
            this.lblTotalCost.Location = new System.Drawing.Point(10, 30);
            this.lblTotalCost.Name = "lblTotalCost";
            this.lblTotalCost.Size = new System.Drawing.Size(740, 40);
            this.lblTotalCost.TabIndex = 1;
            this.lblTotalCost.Text = "0.00 CAD$";
            this.lblTotalCost.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblStaffTitle
            // 
            this.lblStaffTitle.AutoSize = true;
            this.lblStaffTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lblStaffTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(175)))), ((int)(((byte)(80)))));
            this.lblStaffTitle.Location = new System.Drawing.Point(20, 115);
            this.lblStaffTitle.Name = "lblStaffTitle";
            this.lblStaffTitle.Size = new System.Drawing.Size(137, 17);
            this.lblStaffTitle.TabIndex = 1;
            this.lblStaffTitle.Text = "Coûts Personnel";
            // 
            // dgvStaff
            // 
            this.dgvStaff.AllowUserToAddRows = false;
            this.dgvStaff.AllowUserToDeleteRows = false;
            this.dgvStaff.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvStaff.Location = new System.Drawing.Point(20, 135);
            this.dgvStaff.Name = "dgvStaff";
            this.dgvStaff.ReadOnly = true;
            this.dgvStaff.Size = new System.Drawing.Size(760, 100);
            this.dgvStaff.TabIndex = 2;
            // 
            // lblEquipmentTitle
            // 
            this.lblEquipmentTitle.AutoSize = true;
            this.lblEquipmentTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lblEquipmentTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            this.lblEquipmentTitle.Location = new System.Drawing.Point(20, 245);
            this.lblEquipmentTitle.Name = "lblEquipmentTitle";
            this.lblEquipmentTitle.Size = new System.Drawing.Size(151, 17);
            this.lblEquipmentTitle.TabIndex = 3;
            this.lblEquipmentTitle.Text = "Coûts Équipement";
            // 
            // dgvEquipment
            // 
            this.dgvEquipment.AllowUserToAddRows = false;
            this.dgvEquipment.AllowUserToDeleteRows = false;
            this.dgvEquipment.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvEquipment.Location = new System.Drawing.Point(20, 265);
            this.dgvEquipment.Name = "dgvEquipment";
            this.dgvEquipment.ReadOnly = true;
            this.dgvEquipment.Size = new System.Drawing.Size(760, 100);
            this.dgvEquipment.TabIndex = 4;
            // 
            // lblLocationTitle
            // 
            this.lblLocationTitle.AutoSize = true;
            this.lblLocationTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lblLocationTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(152)))), ((int)(((byte)(0)))));
            this.lblLocationTitle.Location = new System.Drawing.Point(20, 375);
            this.lblLocationTitle.Name = "lblLocationTitle";
            this.lblLocationTitle.Size = new System.Drawing.Size(146, 17);
            this.lblLocationTitle.TabIndex = 5;
            this.lblLocationTitle.Text = "Coûts Immobilier";
            // 
            // dgvLocation
            // 
            this.dgvLocation.AllowUserToAddRows = false;
            this.dgvLocation.AllowUserToDeleteRows = false;
            this.dgvLocation.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLocation.Location = new System.Drawing.Point(20, 395);
            this.dgvLocation.Name = "dgvLocation";
            this.dgvLocation.ReadOnly = true;
            this.dgvLocation.Size = new System.Drawing.Size(760, 100);
            this.dgvLocation.TabIndex = 6;
            // 
            // lblOperationalTitle
            // 
            this.lblOperationalTitle.AutoSize = true;
            this.lblOperationalTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lblOperationalTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(39)))), ((int)(((byte)(176)))));
            this.lblOperationalTitle.Location = new System.Drawing.Point(20, 505);
            this.lblOperationalTitle.Name = "lblOperationalTitle";
            this.lblOperationalTitle.Size = new System.Drawing.Size(172, 17);
            this.lblOperationalTitle.TabIndex = 7;
            this.lblOperationalTitle.Text = "Coûts Opérationnels";
            // 
            // dgvOperational
            // 
            this.dgvOperational.AllowUserToAddRows = false;
            this.dgvOperational.AllowUserToDeleteRows = false;
            this.dgvOperational.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvOperational.Location = new System.Drawing.Point(20, 525);
            this.dgvOperational.Name = "dgvOperational";
            this.dgvOperational.ReadOnly = true;
            this.dgvOperational.Size = new System.Drawing.Size(760, 100);
            this.dgvOperational.TabIndex = 8;
            // 
            // btnExportCsv
            // 
            this.btnExportCsv.Location = new System.Drawing.Point(450, 640);
            this.btnExportCsv.Name = "btnExportCsv";
            this.btnExportCsv.Size = new System.Drawing.Size(100, 35);
            this.btnExportCsv.TabIndex = 9;
            this.btnExportCsv.Text = "Export CSV";
            this.btnExportCsv.UseVisualStyleBackColor = true;
            // 
            // btnExportPdf
            // 
            this.btnExportPdf.Location = new System.Drawing.Point(560, 640);
            this.btnExportPdf.Name = "btnExportPdf";
            this.btnExportPdf.Size = new System.Drawing.Size(100, 35);
            this.btnExportPdf.TabIndex = 10;
            this.btnExportPdf.Text = "Export PDF";
            this.btnExportPdf.UseVisualStyleBackColor = true;
            // 
            // btnNewAnalysis
            // 
            this.btnNewAnalysis.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(175)))), ((int)(((byte)(80)))));
            this.btnNewAnalysis.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.btnNewAnalysis.ForeColor = System.Drawing.Color.White;
            this.btnNewAnalysis.Location = new System.Drawing.Point(670, 640);
            this.btnNewAnalysis.Name = "btnNewAnalysis";
            this.btnNewAnalysis.Size = new System.Drawing.Size(110, 35);
            this.btnNewAnalysis.TabIndex = 11;
            this.btnNewAnalysis.Text = "Nouvelle Analyse";
            this.btnNewAnalysis.UseVisualStyleBackColor = false;
            this.btnNewAnalysis.Click += new System.EventHandler(this.btnNewAnalysis_Click);
            // 
            // ResultsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 690);
            this.Controls.Add(this.btnNewAnalysis);
            this.Controls.Add(this.btnExportPdf);
            this.Controls.Add(this.btnExportCsv);
            this.Controls.Add(this.dgvOperational);
            this.Controls.Add(this.lblOperationalTitle);
            this.Controls.Add(this.dgvLocation);
            this.Controls.Add(this.lblLocationTitle);
            this.Controls.Add(this.dgvEquipment);
            this.Controls.Add(this.lblEquipmentTitle);
            this.Controls.Add(this.dgvStaff);
            this.Controls.Add(this.lblStaffTitle);
            this.Controls.Add(this.pnlSummary);
            this.Name = "ResultsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Résultats - Track-A-FACE";
            this.pnlSummary.ResumeLayout(false);
            this.pnlSummary.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStaff)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEquipment)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLocation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOperational)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Panel pnlSummary;
        private System.Windows.Forms.Label lblSummaryTitle;
        private System.Windows.Forms.Label lblTotalCost;
        private System.Windows.Forms.Label lblStaffTitle;
        private System.Windows.Forms.DataGridView dgvStaff;
        private System.Windows.Forms.Label lblEquipmentTitle;
        private System.Windows.Forms.DataGridView dgvEquipment;
        private System.Windows.Forms.Label lblLocationTitle;
        private System.Windows.Forms.DataGridView dgvLocation;
        private System.Windows.Forms.Label lblOperationalTitle;
        private System.Windows.Forms.DataGridView dgvOperational;
        private System.Windows.Forms.Button btnExportCsv;
        private System.Windows.Forms.Button btnExportPdf;
        private System.Windows.Forms.Button btnNewAnalysis;
    }
}