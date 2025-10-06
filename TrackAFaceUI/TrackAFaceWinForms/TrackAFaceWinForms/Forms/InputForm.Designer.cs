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
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
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
            this.lblTitle.Location = new System.Drawing.Point(16, 11);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(437, 29);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Saisie des Données - Track-A-FACE";
            // 
            // txtSessionName
            // 
            this.txtSessionName.Location = new System.Drawing.Point(200, 58);
            this.txtSessionName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtSessionName.Name = "txtSessionName";
            this.txtSessionName.Size = new System.Drawing.Size(332, 22);
            this.txtSessionName.TabIndex = 2;
            // 
            // lblSessionName
            // 
            this.lblSessionName.AutoSize = true;
            this.lblSessionName.Location = new System.Drawing.Point(27, 62);
            this.lblSessionName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSessionName.Name = "lblSessionName";
            this.lblSessionName.Size = new System.Drawing.Size(122, 16);
            this.lblSessionName.TabIndex = 1;
            this.lblSessionName.Text = "Nom de la session:";
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
            this.cmbTheme.Location = new System.Drawing.Point(200, 101);
            this.cmbTheme.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbTheme.Name = "cmbTheme";
            this.cmbTheme.Size = new System.Drawing.Size(332, 24);
            this.cmbTheme.TabIndex = 4;
            // 
            // lblTheme
            // 
            this.lblTheme.AutoSize = true;
            this.lblTheme.Location = new System.Drawing.Point(27, 105);
            this.lblTheme.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTheme.Name = "lblTheme";
            this.lblTheme.Size = new System.Drawing.Size(133, 16);
            this.lblTheme.TabIndex = 3;
            this.lblTheme.Text = "Thème du restaurant:";
            // 
            // grpRevenue
            // 
            this.grpRevenue.Controls.Add(this.rbEnterprise);
            this.grpRevenue.Controls.Add(this.rbLarge);
            this.grpRevenue.Controls.Add(this.rbMedium);
            this.grpRevenue.Controls.Add(this.rbSmall);
            this.grpRevenue.Location = new System.Drawing.Point(27, 148);
            this.grpRevenue.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grpRevenue.Name = "grpRevenue";
            this.grpRevenue.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grpRevenue.Size = new System.Drawing.Size(507, 135);
            this.grpRevenue.TabIndex = 5;
            this.grpRevenue.TabStop = false;
            this.grpRevenue.Text = "Taille des Revenus";
            // 
            // rbEnterprise
            // 
            this.rbEnterprise.Location = new System.Drawing.Point(13, 98);
            this.rbEnterprise.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rbEnterprise.Name = "rbEnterprise";
            this.rbEnterprise.Size = new System.Drawing.Size(139, 30);
            this.rbEnterprise.TabIndex = 0;
            this.rbEnterprise.Text = "Enterprise (>10M CAD$)";
            // 
            // rbLarge
            // 
            this.rbLarge.Location = new System.Drawing.Point(13, 74);
            this.rbLarge.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rbLarge.Name = "rbLarge";
            this.rbLarge.Size = new System.Drawing.Size(139, 30);
            this.rbLarge.TabIndex = 1;
            this.rbLarge.Text = "Large (2M-10M CAD$)";
            // 
            // rbMedium
            // 
            this.rbMedium.Checked = true;
            this.rbMedium.Location = new System.Drawing.Point(13, 49);
            this.rbMedium.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rbMedium.Name = "rbMedium";
            this.rbMedium.Size = new System.Drawing.Size(139, 30);
            this.rbMedium.TabIndex = 2;
            this.rbMedium.TabStop = true;
            this.rbMedium.Text = "Medium (500k-2M CAD$)";
            // 
            // rbSmall
            // 
            this.rbSmall.Location = new System.Drawing.Point(13, 25);
            this.rbSmall.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rbSmall.Name = "rbSmall";
            this.rbSmall.Size = new System.Drawing.Size(139, 30);
            this.rbSmall.TabIndex = 3;
            this.rbSmall.Text = "Small (<500k CAD$)";
            // 
            // numStaffCount
            // 
            this.numStaffCount.Location = new System.Drawing.Point(200, 304);
            this.numStaffCount.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.numStaffCount.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numStaffCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numStaffCount.Name = "numStaffCount";
            this.numStaffCount.Size = new System.Drawing.Size(160, 22);
            this.numStaffCount.TabIndex = 7;
            this.numStaffCount.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // lblStaffCount
            // 
            this.lblStaffCount.AutoSize = true;
            this.lblStaffCount.Location = new System.Drawing.Point(27, 308);
            this.lblStaffCount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStaffCount.Name = "lblStaffCount";
            this.lblStaffCount.Size = new System.Drawing.Size(133, 16);
            this.lblStaffCount.TabIndex = 6;
            this.lblStaffCount.Text = "Nombre d\'employés:";
            // 
            // numRetrainingHours
            // 
            this.numRetrainingHours.Location = new System.Drawing.Point(200, 341);
            this.numRetrainingHours.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.numRetrainingHours.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.numRetrainingHours.Name = "numRetrainingHours";
            this.numRetrainingHours.Size = new System.Drawing.Size(160, 22);
            this.numRetrainingHours.TabIndex = 9;
            this.numRetrainingHours.Value = new decimal(new int[] {
            40,
            0,
            0,
            0});
            // 
            // lblRetrainingHours
            // 
            this.lblRetrainingHours.AutoSize = true;
            this.lblRetrainingHours.Location = new System.Drawing.Point(27, 345);
            this.lblRetrainingHours.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblRetrainingHours.Name = "lblRetrainingHours";
            this.lblRetrainingHours.Size = new System.Drawing.Size(131, 16);
            this.lblRetrainingHours.TabIndex = 8;
            this.lblRetrainingHours.Text = "Heures de formation:";
            // 
            // numKitchenSize
            // 
            this.numKitchenSize.Location = new System.Drawing.Point(733, 58);
            this.numKitchenSize.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.numKitchenSize.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numKitchenSize.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numKitchenSize.Name = "numKitchenSize";
            this.numKitchenSize.Size = new System.Drawing.Size(160, 22);
            this.numKitchenSize.TabIndex = 11;
            this.numKitchenSize.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // lblKitchenSize
            // 
            this.lblKitchenSize.AutoSize = true;
            this.lblKitchenSize.Location = new System.Drawing.Point(560, 62);
            this.lblKitchenSize.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblKitchenSize.Name = "lblKitchenSize";
            this.lblKitchenSize.Size = new System.Drawing.Size(115, 16);
            this.lblKitchenSize.TabIndex = 10;
            this.lblKitchenSize.Text = "Taille cuisine (m²):";
            // 
            // numRentMonthly
            // 
            this.numRentMonthly.Location = new System.Drawing.Point(733, 101);
            this.numRentMonthly.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.numRentMonthly.Maximum = new decimal(new int[] {
            50000,
            0,
            0,
            0});
            this.numRentMonthly.Name = "numRentMonthly";
            this.numRentMonthly.Size = new System.Drawing.Size(160, 22);
            this.numRentMonthly.TabIndex = 13;
            this.numRentMonthly.Value = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            // 
            // lblRentMonthly
            // 
            this.lblRentMonthly.AutoSize = true;
            this.lblRentMonthly.Location = new System.Drawing.Point(560, 105);
            this.lblRentMonthly.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblRentMonthly.Name = "lblRentMonthly";
            this.lblRentMonthly.Size = new System.Drawing.Size(144, 16);
            this.lblRentMonthly.TabIndex = 12;
            this.lblRentMonthly.Text = "Loyer mensuel (CAD$):";
            // 
            // cmbLocationType
            // 
            this.cmbLocationType.Items.AddRange(new object[] {
            "urban",
            "suburban",
            "rural"});
            this.cmbLocationType.Location = new System.Drawing.Point(733, 144);
            this.cmbLocationType.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbLocationType.Name = "cmbLocationType";
            this.cmbLocationType.Size = new System.Drawing.Size(160, 24);
            this.cmbLocationType.TabIndex = 15;
            // 
            // lblLocationType
            // 
            this.lblLocationType.AutoSize = true;
            this.lblLocationType.Location = new System.Drawing.Point(560, 148);
            this.lblLocationType.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLocationType.Name = "lblLocationType";
            this.lblLocationType.Size = new System.Drawing.Size(132, 16);
            this.lblLocationType.TabIndex = 14;
            this.lblLocationType.Text = "Type de localisation:";
            // 
            // numUtilities
            // 
            this.numUtilities.Location = new System.Drawing.Point(733, 187);
            this.numUtilities.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.numUtilities.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numUtilities.Name = "numUtilities";
            this.numUtilities.Size = new System.Drawing.Size(160, 22);
            this.numUtilities.TabIndex = 17;
            this.numUtilities.Value = new decimal(new int[] {
            1200,
            0,
            0,
            0});
            // 
            // lblUtilities
            // 
            this.lblUtilities.AutoSize = true;
            this.lblUtilities.Location = new System.Drawing.Point(560, 191);
            this.lblUtilities.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblUtilities.Name = "lblUtilities";
            this.lblUtilities.Size = new System.Drawing.Size(99, 16);
            this.lblUtilities.TabIndex = 16;
            this.lblUtilities.Text = "Utilities (CAD$):";
            // 
            // numEquipmentValue
            // 
            this.numEquipmentValue.Location = new System.Drawing.Point(737, 232);
            this.numEquipmentValue.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.numEquipmentValue.Maximum = new decimal(new int[] {
            5000000,
            0,
            0,
            0});
            this.numEquipmentValue.Name = "numEquipmentValue";
            this.numEquipmentValue.Size = new System.Drawing.Size(160, 22);
            this.numEquipmentValue.TabIndex = 19;
            this.numEquipmentValue.Value = new decimal(new int[] {
            50000,
            0,
            0,
            0});
            // 
            // lblEquipmentValue
            // 
            this.lblEquipmentValue.AutoSize = true;
            this.lblEquipmentValue.Location = new System.Drawing.Point(560, 234);
            this.lblEquipmentValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblEquipmentValue.Name = "lblEquipmentValue";
            this.lblEquipmentValue.Size = new System.Drawing.Size(169, 16);
            this.lblEquipmentValue.TabIndex = 18;
            this.lblEquipmentValue.Text = "Valeur équipement (CAD$):";
            // 
            // trkEquipmentCondition
            // 
            this.trkEquipmentCondition.Location = new System.Drawing.Point(733, 273);
            this.trkEquipmentCondition.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.trkEquipmentCondition.Maximum = 100;
            this.trkEquipmentCondition.Name = "trkEquipmentCondition";
            this.trkEquipmentCondition.Size = new System.Drawing.Size(200, 56);
            this.trkEquipmentCondition.TabIndex = 21;
            this.trkEquipmentCondition.Value = 80;
            this.trkEquipmentCondition.Scroll += new System.EventHandler(this.trkEquipmentCondition_Scroll);
            // 
            // lblEquipmentCondition
            // 
            this.lblEquipmentCondition.AutoSize = true;
            this.lblEquipmentCondition.Location = new System.Drawing.Point(560, 277);
            this.lblEquipmentCondition.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblEquipmentCondition.Name = "lblEquipmentCondition";
            this.lblEquipmentCondition.Size = new System.Drawing.Size(89, 16);
            this.lblEquipmentCondition.TabIndex = 20;
            this.lblEquipmentCondition.Text = "Condition (%):";
            // 
            // lblConditionValue
            // 
            this.lblConditionValue.AutoSize = true;
            this.lblConditionValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lblConditionValue.Location = new System.Drawing.Point(947, 277);
            this.lblConditionValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblConditionValue.Name = "lblConditionValue";
            this.lblConditionValue.Size = new System.Drawing.Size(45, 20);
            this.lblConditionValue.TabIndex = 22;
            this.lblConditionValue.Text = "80%";
            // 
            // numEquipmentAge
            // 
            this.numEquipmentAge.Location = new System.Drawing.Point(733, 335);
            this.numEquipmentAge.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.numEquipmentAge.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numEquipmentAge.Name = "numEquipmentAge";
            this.numEquipmentAge.Size = new System.Drawing.Size(160, 22);
            this.numEquipmentAge.TabIndex = 24;
            this.numEquipmentAge.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // lblEquipmentAge
            // 
            this.lblEquipmentAge.AutoSize = true;
            this.lblEquipmentAge.Location = new System.Drawing.Point(560, 338);
            this.lblEquipmentAge.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblEquipmentAge.Name = "lblEquipmentAge";
            this.lblEquipmentAge.Size = new System.Drawing.Size(142, 16);
            this.lblEquipmentAge.TabIndex = 23;
            this.lblEquipmentAge.Text = "Âge équipement (ans):";
            // 
            // numCapacity
            // 
            this.numCapacity.Location = new System.Drawing.Point(200, 390);
            this.numCapacity.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.numCapacity.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numCapacity.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numCapacity.Name = "numCapacity";
            this.numCapacity.Size = new System.Drawing.Size(160, 22);
            this.numCapacity.TabIndex = 26;
            this.numCapacity.Value = new decimal(new int[] {
            150,
            0,
            0,
            0});
            // 
            // lblCapacity
            // 
            this.lblCapacity.AutoSize = true;
            this.lblCapacity.Location = new System.Drawing.Point(27, 394);
            this.lblCapacity.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCapacity.Name = "lblCapacity";
            this.lblCapacity.Size = new System.Drawing.Size(152, 16);
            this.lblCapacity.TabIndex = 25;
            this.lblCapacity.Text = "Capacité (couverts/jour):";
            // 
            // btnCalculate
            // 
            this.btnCalculate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(175)))), ((int)(((byte)(80)))));
            this.btnCalculate.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.btnCalculate.ForeColor = System.Drawing.Color.White;
            this.btnCalculate.Location = new System.Drawing.Point(600, 455);
            this.btnCalculate.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnCalculate.Name = "btnCalculate";
            this.btnCalculate.Size = new System.Drawing.Size(160, 49);
            this.btnCalculate.TabIndex = 27;
            this.btnCalculate.Text = "Calculer";
            this.btnCalculate.UseVisualStyleBackColor = false;
            this.btnCalculate.Click += new System.EventHandler(this.btnCalculate_Click);
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(773, 455);
            this.btnReset.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(120, 49);
            this.btnReset.TabIndex = 28;
            this.btnReset.Text = "Réinitialiser";
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(907, 455);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(120, 49);
            this.btnSave.TabIndex = 29;
            this.btnSave.Text = "Sauvegarder";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            this.btnLoad.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.btnLoad.ForeColor = System.Drawing.Color.White;
            this.btnLoad.Location = new System.Drawing.Point(1040, 455);
            this.btnLoad.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(120, 49);
            this.btnLoad.TabIndex = 30;
            this.btnLoad.Text = "Charger";
            this.btnLoad.UseVisualStyleBackColor = false;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(158)))), ((int)(((byte)(158)))), ((int)(((byte)(158)))));
            this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(1173, 455);
            this.btnClose.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(120, 49);
            this.btnClose.TabIndex = 31;
            this.btnClose.Text = "Fermer";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // InputForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1333, 554);
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
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.btnClose);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
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
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnClose;
    }
}
