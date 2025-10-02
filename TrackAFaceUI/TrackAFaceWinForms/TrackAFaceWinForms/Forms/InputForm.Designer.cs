namespace TrackAFaceWinForms.Forms
{
    partial class InputForm
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
            this.lblTitle = new System.Windows.Forms.Label();
            this.txtSessionName = new System.Windows.Forms.TextBox();
            this.lblSessionName = new System.Windows.Forms.Label();
            this.cmbTheme = new System.Windows.Forms.ComboBox();
            this.lblTheme = new System.Windows.Forms.Label();
            this.grpRevenue = new System.Windows.Forms.GroupBox();
            this.rbEnterprise = new System.Windows.Forms.RadioButton();
            this.rbLarge = new System.Windows.Forms.RadioButton();
            this.rbMedium = new System.Windows.Forms.RadioButton();
            this.rbSmall = new System.Windows.Forms.RadioButton();
            this.numStaffCount = new System.Windows.Forms.NumericUpDown();
            this.lblStaffCount = new System.Windows.Forms.Label();
            this.numRetrainingHours = new System.Windows.Forms.NumericUpDown();
            this.lblRetrainingHours = new System.Windows.Forms.Label();
            this.numKitchenSize = new System.Windows.Forms.NumericUpDown();
            this.lblKitchenSize = new System.Windows.Forms.Label();
            this.numRentMonthly = new System.Windows.Forms.NumericUpDown();
            this.lblRentMonthly = new System.Windows.Forms.Label();
            this.cmbLocationType = new System.Windows.Forms.ComboBox();
            this.lblLocationType = new System.Windows.Forms.Label();
            this.numUtilities = new System.Windows.Forms.NumericUpDown();
            this.lblUtilities = new System.Windows.Forms.Label();
            this.numEquipmentValue = new System.Windows.Forms.NumericUpDown();
            this.lblEquipmentValue = new System.Windows.Forms.Label();
            this.trkEquipmentCondition = new System.Windows.Forms.TrackBar();
            this.lblEquipmentCondition = new System.Windows.Forms.Label();
            this.lblConditionValue = new System.Windows.Forms.Label();
            this.numEquipmentAge = new System.Windows.Forms.NumericUpDown();
            this.lblEquipmentAge = new System.Windows.Forms.Label();
            this.numCapacity = new System.Windows.Forms.NumericUpDown();
            this.lblCapacity = new System.Windows.Forms.Label();
            this.btnCalculate = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.grpRevenue.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numStaffCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRetrainingHours)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numKitchenSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRentMonthly)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUtilities)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numEquipmentValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkEquipmentCondition)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numEquipmentAge)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCapacity)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(12, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(300, 24);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Saisie des Données - Track-A-FACE";
            // 
            // lblSessionName
            // 
            this.lblSessionName.AutoSize = true;
            this.lblSessionName.Location = new System.Drawing.Point(20, 50);
            this.lblSessionName.Name = "lblSessionName";
            this.lblSessionName.Size = new System.Drawing.Size(100, 13);
            this.lblSessionName.Text = "Nom de la session:";
            // 
            // txtSessionName
            // 
            this.txtSessionName.Location = new System.Drawing.Point(150, 47);
            this.txtSessionName.Name = "txtSessionName";
            this.txtSessionName.Size = new System.Drawing.Size(250, 20);
            // 
            // lblTheme
            // 
            this.lblTheme.AutoSize = true;
            this.lblTheme.Location = new System.Drawing.Point(20, 85);
            this.lblTheme.Name = "lblTheme";
            this.lblTheme.Size = new System.Drawing.Size(104, 13);
            this.lblTheme.Text = "Thème du restaurant:";
            // 
            // cmbTheme
            // 
            this.cmbTheme.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTheme.FormattingEnabled = true;
            this.cmbTheme.Items.AddRange(new object[] {
            "fast_food",
            "casual_dining",
            "fine_dining",
            "cloud_kitchen",
            "food_truck"});
            this.cmbTheme.Location = new System.Drawing.Point(150, 82);
            this.cmbTheme.Name = "cmbTheme";
            this.cmbTheme.Size = new System.Drawing.Size(250, 21);
            // 
            // grpRevenue
            // 
            this.grpRevenue.Controls.Add(this.rbEnterprise);
            this.grpRevenue.Controls.Add(this.rbLarge);
            this.grpRevenue.Controls.Add(this.rbMedium);
            this.grpRevenue.Controls.Add(this.rbSmall);
            this.grpRevenue.Location = new System.Drawing.Point(20, 120);
            this.grpRevenue.Name = "grpRevenue";
            this.grpRevenue.Size = new System.Drawing.Size(380, 110);
            this.grpRevenue.Text = "Taille des Revenus";
            // 
            // rbSmall, rbMedium, rbLarge, rbEnterprise
            // 
            this.rbSmall.Location = new System.Drawing.Point(10, 20);
            this.rbSmall.Text = "Small (<500k CAD$)";
            this.rbMedium.Location = new System.Drawing.Point(10, 40);
            this.rbMedium.Text = "Medium (500k-2M CAD$)";
            this.rbMedium.Checked = true;
            this.rbLarge.Location = new System.Drawing.Point(10, 60);
            this.rbLarge.Text = "Large (2M-10M CAD$)";
            this.rbEnterprise.Location = new System.Drawing.Point(10, 80);
            this.rbEnterprise.Text = "Enterprise (>10M CAD$)";
            // 
            // Personnel
            // 
            this.lblStaffCount.Location = new System.Drawing.Point(20, 250);
            this.lblStaffCount.Text = "Nombre d'employés:";
            this.numStaffCount.Location = new System.Drawing.Point(150, 247);
            this.numStaffCount.Minimum = 1;
            this.numStaffCount.Maximum = 500;
            this.numStaffCount.Value = 10;

            this.lblRetrainingHours.Location = new System.Drawing.Point(20, 280);
            this.lblRetrainingHours.Text = "Heures de formation:";
            this.numRetrainingHours.Location = new System.Drawing.Point(150, 277);
            this.numRetrainingHours.Maximum = 200;
            this.numRetrainingHours.Value = 40;
            // 
            // Immobilier
            // 
            this.lblKitchenSize.Location = new System.Drawing.Point(420, 50);
            this.lblKitchenSize.Text = "Taille cuisine (m²):";
            this.numKitchenSize.Location = new System.Drawing.Point(550, 47);
            this.numKitchenSize.Minimum = 10;
            this.numKitchenSize.Maximum = 1000;
            this.numKitchenSize.Value = 100;

            this.lblRentMonthly.Location = new System.Drawing.Point(420, 85);
            this.lblRentMonthly.Text = "Loyer mensuel (CAD$):";
            this.numRentMonthly.Location = new System.Drawing.Point(550, 82);
            this.numRentMonthly.Maximum = 50000;
            this.numRentMonthly.Value = 5000;

            this.lblLocationType.Location = new System.Drawing.Point(420, 120);
            this.lblLocationType.Text = "Type de localisation:";
            this.cmbLocationType.Location = new System.Drawing.Point(550, 117);
            this.cmbLocationType.Items.AddRange(new object[] { "urban", "suburban", "rural" });

            this.lblUtilities.Location = new System.Drawing.Point(420, 155);
            this.lblUtilities.Text = "Utilities (CAD$):";
            this.numUtilities.Location = new System.Drawing.Point(550, 152);
            this.numUtilities.Maximum = 10000;
            this.numUtilities.Value = 1200;
            // 
            // Équipement
            // 
            this.lblEquipmentValue.Location = new System.Drawing.Point(420, 190);
            this.lblEquipmentValue.Text = "Valeur équipement (CAD$):";
            this.numEquipmentValue.Location = new System.Drawing.Point(550, 187);
            this.numEquipmentValue.Maximum = 5000000;
            this.numEquipmentValue.Value = 50000;

            this.lblEquipmentCondition.Location = new System.Drawing.Point(420, 225);
            this.lblEquipmentCondition.Text = "Condition (%):";
            this.trkEquipmentCondition.Location = new System.Drawing.Point(550, 222);
            this.trkEquipmentCondition.Maximum = 100;
            this.trkEquipmentCondition.Value = 80;
            this.trkEquipmentCondition.Size = new System.Drawing.Size(150, 45);
            this.trkEquipmentCondition.Scroll += new System.EventHandler(this.trkEquipmentCondition_Scroll);

            this.lblConditionValue.Location = new System.Drawing.Point(710, 225);
            this.lblConditionValue.Text = "80%";
            this.lblConditionValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);

            this.lblEquipmentAge.Location = new System.Drawing.Point(420, 275);
            this.lblEquipmentAge.Text = "Âge équipement (ans):";
            this.numEquipmentAge.Location = new System.Drawing.Point(550, 272);
            this.numEquipmentAge.Maximum = 20;
            this.numEquipmentAge.Value = 2;
            // 
            // Opérationnel
            // 
            this.lblCapacity.Location = new System.Drawing.Point(20, 320);
            this.lblCapacity.Text = "Capacité (couverts/jour):";
            this.numCapacity.Location = new System.Drawing.Point(150, 317);
            this.numCapacity.Minimum = 10;
            this.numCapacity.Maximum = 500;
            this.numCapacity.Value = 150;
            // 
            // Boutons
            // 
            this.btnCalculate.Location = new System.Drawing.Point(450, 370);
            this.btnCalculate.Size = new System.Drawing.Size(120, 40);
            this.btnCalculate.Text = "Calculer";
            this.btnCalculate.BackColor = System.Drawing.Color.FromArgb(76, 175, 80);
            this.btnCalculate.ForeColor = System.Drawing.Color.White;
            this.btnCalculate.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.btnCalculate.Click += new System.EventHandler(this.btnCalculate_Click);

            this.btnReset.Location = new System.Drawing.Point(580, 370);
            this.btnReset.Size = new System.Drawing.Size(90, 40);
            this.btnReset.Text = "Réinitialiser";
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);

            this.btnSave.Location = new System.Drawing.Point(680, 370);
            this.btnSave.Size = new System.Drawing.Size(90, 40);
            this.btnSave.Text = "Sauvegarder";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // InputForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblSessionName);
            this.Controls.Add(this.txtSessionName);
            this.Controls.Add(this.lblTheme);
            this.Controls.Add(this.cmbTheme);
            this.Controls.Add(this.grpRevenue);
            this.Controls.Add(this.lblStaffCount);
            this.Controls.Add(this.numStaffCount);
            this.Controls.Add(this.lblRetrainingHours);
            this.Controls.Add(this.numRetrainingHours);
            this.Controls.Add(this.lblKitchenSize);
            this.Controls.Add(this.numKitchenSize);
            this.Controls.Add(this.lblRentMonthly);
            this.Controls.Add(this.numRentMonthly);
            this.Controls.Add(this.lblLocationType);
            this.Controls.Add(this.cmbLocationType);
            this.Controls.Add(this.lblUtilities);
            this.Controls.Add(this.numUtilities);
            this.Controls.Add(this.lblEquipmentValue);
            this.Controls.Add(this.numEquipmentValue);
            this.Controls.Add(this.lblEquipmentCondition);
            this.Controls.Add(this.trkEquipmentCondition);
            this.Controls.Add(this.lblConditionValue);
            this.Controls.Add(this.lblEquipmentAge);
            this.Controls.Add(this.numEquipmentAge);
            this.Controls.Add(this.lblCapacity);
            this.Controls.Add(this.numCapacity);
            this.Controls.Add(this.btnCalculate);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnSave);
            this.Name = "InputForm";
            this.Text = "Saisie - Track-A-FACE";
            this.grpRevenue.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numStaffCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRetrainingHours)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numKitchenSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRentMonthly)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUtilities)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numEquipmentValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkEquipmentCondition)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numEquipmentAge)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCapacity)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSessionName;
        private System.Windows.Forms.TextBox txtSessionName;
        private System.Windows.Forms.Label lblTheme;
        private System.Windows.Forms.ComboBox cmbTheme;
        private System.Windows.Forms.GroupBox grpRevenue;
        private System.Windows.Forms.RadioButton rbSmall;
        private System.Windows.Forms.RadioButton rbMedium;
        private System.Windows.Forms.RadioButton rbLarge;
        private System.Windows.Forms.RadioButton rbEnterprise;
        private System.Windows.Forms.Label lblStaffCount;
        private System.Windows.Forms.NumericUpDown numStaffCount;
        private System.Windows.Forms.Label lblRetrainingHours;
        private System.Windows.Forms.NumericUpDown numRetrainingHours;
        private System.Windows.Forms.Label lblKitchenSize;
        private System.Windows.Forms.NumericUpDown numKitchenSize;
        private System.Windows.Forms.Label lblRentMonthly;
        private System.Windows.Forms.NumericUpDown numRentMonthly;
        private System.Windows.Forms.Label lblLocationType;
        private System.Windows.Forms.ComboBox cmbLocationType;
        private System.Windows.Forms.Label lblUtilities;
        private System.Windows.Forms.NumericUpDown numUtilities;
        private System.Windows.Forms.Label lblEquipmentValue;
        private System.Windows.Forms.NumericUpDown numEquipmentValue;
        private System.Windows.Forms.Label lblEquipmentCondition;
        private System.Windows.Forms.TrackBar trkEquipmentCondition;
        private System.Windows.Forms.Label lblConditionValue;
        private System.Windows.Forms.Label lblEquipmentAge;
        private System.Windows.Forms.NumericUpDown numEquipmentAge;
        private System.Windows.Forms.Label lblCapacity;
        private System.Windows.Forms.NumericUpDown numCapacity;
        private System.Windows.Forms.Button btnCalculate;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnSave;
    }
}
