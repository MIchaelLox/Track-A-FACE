using System;
using System.Drawing;
using System.Windows.Forms;

namespace FaceWebAppUI;

partial class MainForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
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
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this.tabControl = new TabControl();
        this.tabInputs = new TabPage();
        this.tabResults = new TabPage();
        this.tabWorkflow = new TabPage();
        
        // Labels
        this.lblTitle = new Label();
        this.lblSessionName = new Label();
        this.lblTheme = new Label();
        this.lblRevenueSize = new Label();
        this.lblKitchenSize = new Label();
        this.lblWorkstations = new Label();
        this.lblCapacity = new Label();
        this.lblStaffCount = new Label();
        this.lblExperience = new Label();
        this.lblTrainingHours = new Label();
        this.lblEquipmentAge = new Label();
        this.lblEquipmentCondition = new Label();
        this.lblEquipmentValue = new Label();
        this.lblRentPerSqm = new Label();
        
        // Input controls
        this.txtSessionName = new TextBox();
        this.cmbTheme = new ComboBox();
        this.cmbRevenueSize = new ComboBox();
        this.numKitchenSize = new NumericUpDown();
        this.numWorkstations = new NumericUpDown();
        this.numCapacity = new NumericUpDown();
        this.numStaffCount = new NumericUpDown();
        this.cmbExperience = new ComboBox();
        this.numTrainingHours = new NumericUpDown();
        this.numEquipmentAge = new NumericUpDown();
        this.cmbEquipmentCondition = new ComboBox();
        this.numEquipmentValue = new NumericUpDown();
        this.numRentPerSqm = new NumericUpDown();
        
        // Buttons
        this.btnCalculate = new Button();
        this.btnReset = new Button();
        this.btnSave = new Button();
        this.btnLoad = new Button();
        this.btnExportPDF = new Button();
        this.btnExportCSV = new Button();
        
        // Results display
        this.dgvResults = new DataGridView();
        this.lblTotalCost = new Label();
        this.lblStaffCosts = new Label();
        this.lblEquipmentCosts = new Label();
        this.lblLocationCosts = new Label();
        this.lblOperationalCosts = new Label();
        
        // Progress and status
        this.progressBar = new ProgressBar();
        this.statusStrip = new StatusStrip();
        this.statusLabel = new ToolStripStatusLabel();
        
        this.SuspendLayout();
        
        // Configure TabControl
        this.tabControl.Dock = DockStyle.Fill;
        this.tabControl.Controls.Add(this.tabInputs);
        this.tabControl.Controls.Add(this.tabResults);
        this.tabControl.Controls.Add(this.tabWorkflow);
        
        // Configure Input Tab
        this.tabInputs.Text = "Paramètres d'entrée";
        this.tabInputs.UseVisualStyleBackColor = true;
        
        // Configure Results Tab
        this.tabResults.Text = "Résultats";
        this.tabResults.UseVisualStyleBackColor = true;
        
        // Configure Workflow Tab
        this.tabWorkflow.Text = "Workflow";
        this.tabWorkflow.UseVisualStyleBackColor = true;
        
        // Main Form
        this.AutoScaleDimensions = new SizeF(8F, 20F);
        this.AutoScaleMode = AutoScaleMode.Font;
        this.ClientSize = new Size(1200, 800);
        this.Controls.Add(this.tabControl);
        this.Controls.Add(this.statusStrip);
        this.MinimumSize = new Size(1000, 600);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Text = "Track-A-FACE - Calculateur d'Analyse Financière";
        this.WindowState = FormWindowState.Maximized;
        
        // Status Strip
        this.statusStrip.Items.Add(this.statusLabel);
        this.statusLabel.Text = "Prêt";
        
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    #endregion
    
    private TabControl tabControl;
    private TabPage tabInputs;
    private TabPage tabResults;
    private TabPage tabWorkflow;
    
    // Labels
    private Label lblTitle;
    internal Label lblSessionName;
    internal Label lblTheme;
    internal Label lblRevenueSize;
    internal Label lblKitchenSize;
    internal Label lblWorkstations;
    internal Label lblCapacity;
    internal Label lblStaffCount;
    internal Label lblExperience;
    internal Label lblTrainingHours;
    internal Label lblEquipmentAge;
    internal Label lblEquipmentCondition;
    internal Label lblEquipmentValue;
    internal Label lblRentPerSqm;
    
    // Input controls
    internal TextBox txtSessionName;
    internal ComboBox cmbTheme;
    internal ComboBox cmbRevenueSize;
    internal NumericUpDown numKitchenSize;
    internal NumericUpDown numWorkstations;
    internal NumericUpDown numCapacity;
    internal NumericUpDown numStaffCount;
    internal ComboBox cmbExperience;
    internal NumericUpDown numTrainingHours;
    internal NumericUpDown numEquipmentAge;
    internal ComboBox cmbEquipmentCondition;
    internal NumericUpDown numEquipmentValue;
    internal NumericUpDown numRentPerSqm;
    
    // Buttons
    internal Button btnCalculate;
    private Button btnReset;
    internal Button btnSave;
    internal Button btnLoad;
    internal Button btnExportPDF;
    internal Button btnExportCSV;
    
    // Results display
    internal DataGridView dgvResults;
    internal Label lblTotalCost;
    internal Label lblStaffCosts;
    internal Label lblEquipmentCosts;
    internal Label lblLocationCosts;
    internal Label lblOperationalCosts;
    
    // Progress and status
    private ProgressBar progressBar;
    private StatusStrip statusStrip;
    private ToolStripStatusLabel statusLabel;
}
