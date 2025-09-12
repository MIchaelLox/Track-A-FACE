using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace FaceWebAppUI
{
    public partial class MainForm : Form
    {
        private readonly string _pythonScriptPath;
        private readonly string _projectRoot;
        
        public MainForm()
        {
            InitializeComponent();
            
            // Initialiser les chemins
            _projectRoot = Path.GetDirectoryName(Path.GetDirectoryName(Application.StartupPath)) ?? "";
            _pythonScriptPath = Path.Combine(_projectRoot, "engine_api.py");
            
            // Configurer l'interface
            InitializeFormLayout();
            
            // Ajouter le menu de tests (en mode développement)
            #if DEBUG
            AddTestMenu();
            #endif
            
            PopulateComboBoxes();
            SetDefaultValues();
        }

        private void InitializeFormLayout()
        {
            // Configure Input Tab Layout
            ConfigureInputTab();
            
            // Configure Results Tab Layout
            ConfigureResultsTab();
            
            // Configure Comparison Tab Layout
            ConfigureComparisonTab();
        }

        private void ConfigureInputTab()
        {
            var panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 8,
                Padding = new Padding(20),
                AutoScroll = true
            };

            // Configure column styles
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));

            // Row 1: Session Info
            AddLabelAndControl(panel, 0, 0, "Nom de session:", txtSessionName);
            AddLabelAndControl(panel, 0, 2, "Thème restaurant:", cmbTheme);

            // Row 2: Business Info
            AddLabelAndControl(panel, 1, 0, "Taille revenus:", cmbRevenueSize);
            AddLabelAndControl(panel, 1, 2, "Taille cuisine (m²):", numKitchenSize);

            // Row 3: Kitchen Info
            AddLabelAndControl(panel, 2, 0, "Postes de travail:", numWorkstations);
            AddLabelAndControl(panel, 2, 2, "Capacité/jour:", numCapacity);

            // Row 4: Staff Info
            AddLabelAndControl(panel, 3, 0, "Nombre employés:", numStaffCount);
            AddLabelAndControl(panel, 3, 2, "Expérience:", cmbExperience);

            // Row 5: Training Info
            AddLabelAndControl(panel, 4, 0, "Heures formation:", numTrainingHours);
            AddLabelAndControl(panel, 4, 2, "Âge équipement:", numEquipmentAge);

            // Row 6: Equipment Info
            AddLabelAndControl(panel, 5, 0, "État équipement:", cmbEquipmentCondition);
            AddLabelAndControl(panel, 5, 2, "Valeur équipement:", numEquipmentValue);

            // Row 7: Location Info
            AddLabelAndControl(panel, 6, 0, "Loyer/m² (CAD$):", numRentPerSqm);

            // Row 8: Buttons
            var buttonPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            btnCalculate.Text = "🧮 Calculer";
            btnCalculate.Size = new Size(120, 35);
            btnCalculate.BackColor = Color.FromArgb(0, 123, 255);
            btnCalculate.ForeColor = Color.White;
            btnCalculate.FlatStyle = FlatStyle.Flat;
            btnCalculate.Click += BtnCalculate_Click;

            btnReset.Text = "🔄 Reset";
            btnReset.Size = new Size(100, 35);
            btnReset.BackColor = Color.FromArgb(108, 117, 125);
            btnReset.ForeColor = Color.White;
            btnReset.FlatStyle = FlatStyle.Flat;
            btnReset.Click += BtnReset_Click;

            btnSave.Text = "💾 Sauver";
            btnSave.Size = new Size(100, 35);
            btnSave.BackColor = Color.FromArgb(40, 167, 69);
            btnSave.ForeColor = Color.White;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.Click += BtnSave_Click;

            btnLoad.Text = "📂 Charger";
            btnLoad.Size = new Size(100, 35);
            btnLoad.BackColor = Color.FromArgb(255, 193, 7);
            btnLoad.ForeColor = Color.Black;
            btnLoad.FlatStyle = FlatStyle.Flat;
            btnLoad.Click += BtnLoad_Click;

            buttonPanel.Controls.AddRange(new Control[] { btnCalculate, btnReset, btnSave, btnLoad });
            panel.Controls.Add(buttonPanel, 0, 7);
            panel.SetColumnSpan(buttonPanel, 4);

            tabInputs.Controls.Add(panel);
        }

        private void ConfigureResultsTab()
        {
            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 3,
                ColumnCount = 1,
                Padding = new Padding(20)
            };

            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 120F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));

            // Summary Panel
            var summaryPanel = CreateSummaryPanel();
            mainPanel.Controls.Add(summaryPanel, 0, 0);

            // Results Grid
            ConfigureResultsGrid();
            mainPanel.Controls.Add(dgvResults, 0, 1);

            // Export Panel
            var exportPanel = CreateExportPanel();
            mainPanel.Controls.Add(exportPanel, 0, 2);

            tabResults.Controls.Add(mainPanel);
        }

        private void ConfigureComparisonTab()
        {
            var label = new Label
            {
                Text = "Fonctionnalité de comparaison - À implémenter",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 12F, FontStyle.Italic)
            };
            tabComparison.Controls.Add(label);
        }

        private Panel CreateSummaryPanel()
        {
            var panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 5,
                RowCount = 2,
                BackColor = Color.FromArgb(248, 249, 250)
            };

            // Configure styles
            for (int i = 0; i < 5; i++)
                panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));

            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

            // Create summary labels
            lblTotalCost = CreateSummaryLabel("TOTAL", "0 CAD$", Color.FromArgb(220, 53, 69));
            lblStaffCosts = CreateSummaryLabel("Personnel", "0 CAD$", Color.FromArgb(0, 123, 255));
            lblEquipmentCosts = CreateSummaryLabel("Équipement", "0 CAD$", Color.FromArgb(40, 167, 69));
            lblLocationCosts = CreateSummaryLabel("Immobilier", "0 CAD$", Color.FromArgb(255, 193, 7));
            lblOperationalCosts = CreateSummaryLabel("Opérationnel", "0 CAD$", Color.FromArgb(108, 117, 125));

            panel.Controls.Add(lblTotalCost, 0, 0);
            panel.Controls.Add(lblStaffCosts, 1, 0);
            panel.Controls.Add(lblEquipmentCosts, 2, 0);
            panel.Controls.Add(lblLocationCosts, 3, 0);
            panel.Controls.Add(lblOperationalCosts, 4, 0);

            return panel;
        }

        private Label CreateSummaryLabel(string title, string value, Color color)
        {
            var label = new Label
            {
                Text = $"{title}\n{value}",
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = color,
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle
            };
            return label;
        }

        private void ConfigureResultsGrid()
        {
            dgvResults.Dock = DockStyle.Fill;
            dgvResults.AutoGenerateColumns = false;
            dgvResults.AllowUserToAddRows = false;
            dgvResults.AllowUserToDeleteRows = false;
            dgvResults.ReadOnly = true;
            dgvResults.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Add columns
            dgvResults.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Category",
                HeaderText = "Catégorie",
                DataPropertyName = "Category",
                Width = 150
            });

            dgvResults.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Subcategory",
                HeaderText = "Sous-catégorie",
                DataPropertyName = "Subcategory",
                Width = 200
            });

            dgvResults.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Amount",
                HeaderText = "Montant (CAD$)",
                DataPropertyName = "Amount",
                Width = 150,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" }
            });

            dgvResults.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Formula",
                HeaderText = "Formule",
                DataPropertyName = "Formula",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });
        }

        private Panel CreateExportPanel()
        {
            var panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(10)
            };

            btnExportPDF.Text = "📄 Export PDF";
            btnExportPDF.Size = new Size(120, 35);
            btnExportPDF.BackColor = Color.FromArgb(220, 53, 69);
            btnExportPDF.ForeColor = Color.White;
            btnExportPDF.FlatStyle = FlatStyle.Flat;
            btnExportPDF.Click += BtnExportPDF_Click;

            btnExportCSV.Text = "📊 Export CSV";
            btnExportCSV.Size = new Size(120, 35);
            btnExportCSV.BackColor = Color.FromArgb(40, 167, 69);
            btnExportCSV.ForeColor = Color.White;
            btnExportCSV.FlatStyle = FlatStyle.Flat;
            btnExportCSV.Click += BtnExportCSV_Click;

            progressBar.Size = new Size(200, 25);
            progressBar.Visible = false;

            panel.Controls.AddRange(new Control[] { btnExportPDF, btnExportCSV, progressBar });
            return panel;
        }

        private void AddLabelAndControl(TableLayoutPanel panel, int row, int col, string labelText, Control control)
        {
            var label = new Label
            {
                Text = labelText,
                TextAlign = ContentAlignment.MiddleRight,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9F, FontStyle.Regular)
            };

            control.Dock = DockStyle.Fill;
            control.Font = new Font("Segoe UI", 9F);

            panel.Controls.Add(label, col, row);
            panel.Controls.Add(control, col + 1, row);
        }

        private void PopulateComboBoxes()
        {
            // Restaurant themes
            cmbTheme.Items.AddRange(new string[]
            {
                "fast_food",
                "casual_dining",
                "fine_dining",
                "cloud_kitchen",
                "food_truck"
            });

            // Revenue sizes
            cmbRevenueSize.Items.AddRange(new string[]
            {
                "small",
                "medium",
                "large",
                "enterprise"
            });

            // Experience levels
            cmbExperience.Items.AddRange(new string[]
            {
                "beginner",
                "intermediate",
                "experienced",
                "expert"
            });

            // Equipment conditions
            cmbEquipmentCondition.Items.AddRange(new string[]
            {
                "excellent",
                "good",
                "fair",
                "poor"
            });
        }

        private void SetDefaultValues()
        {
            txtSessionName.Text = "Nouveau Restaurant";
            cmbTheme.SelectedIndex = 1; // casual_dining
            cmbRevenueSize.SelectedIndex = 1; // medium
            numKitchenSize.Value = 100;
            numKitchenSize.Minimum = 10;
            numKitchenSize.Maximum = 1000;
            numWorkstations.Value = 8;
            numWorkstations.Minimum = 1;
            numWorkstations.Maximum = 50;
            numCapacity.Value = 200;
            numCapacity.Minimum = 10;
            numCapacity.Maximum = 2000;
            numStaffCount.Value = 15;
            numStaffCount.Minimum = 1;
            numStaffCount.Maximum = 200;
            cmbExperience.SelectedIndex = 1; // intermediate
            numTrainingHours.Value = 40;
            numTrainingHours.Maximum = 500;
            numEquipmentAge.Value = 2;
            numEquipmentAge.Maximum = 30;
            cmbEquipmentCondition.SelectedIndex = 1; // good
            numEquipmentValue.Value = 120000;
            numEquipmentValue.Maximum = 1000000;
            numRentPerSqm.Value = 40;
            numRentPerSqm.Maximum = 200;
        }

        private async void BtnCalculate_Click(object sender, EventArgs e)
        {
            try
            {
                statusLabel.Text = "Calcul en cours...";
                progressBar.Visible = true;
                progressBar.Style = ProgressBarStyle.Marquee;
                btnCalculate.Enabled = false;

                var inputData = CollectInputData();
                var result = await ExecutePythonCalculation(inputData);
                
                if (result != null)
                {
                    DisplayResults(result);
                    tabControl.SelectedTab = tabResults;
                    statusLabel.Text = "Calcul terminé avec succès";
                }
                else
                {
                    statusLabel.Text = "Erreur lors du calcul";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur: {ex.Message}", "Erreur de calcul", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                statusLabel.Text = "Erreur";
            }
            finally
            {
                progressBar.Visible = false;
                btnCalculate.Enabled = true;
            }
        }

        private Dictionary<string, object> CollectInputData()
        {
            return new Dictionary<string, object>
            {
                ["session_name"] = txtSessionName.Text,
                ["restaurant_theme"] = cmbTheme.SelectedItem?.ToString() ?? "casual_dining",
                ["revenue_size"] = cmbRevenueSize.SelectedItem?.ToString() ?? "medium",
                ["kitchen_size_sqm"] = (double)numKitchenSize.Value,
                ["kitchen_workstations"] = (int)numWorkstations.Value,
                ["daily_capacity"] = (int)numCapacity.Value,
                ["staff_count"] = (int)numStaffCount.Value,
                ["staff_experience_level"] = cmbExperience.SelectedItem?.ToString() ?? "intermediate",
                ["training_hours_needed"] = (int)numTrainingHours.Value,
                ["equipment_age_years"] = (int)numEquipmentAge.Value,
                ["equipment_condition"] = cmbEquipmentCondition.SelectedItem?.ToString() ?? "good",
                ["equipment_value"] = (double)numEquipmentValue.Value,
                ["location_rent_sqm"] = (double)numRentPerSqm.Value
            };
        }

        private async Task<dynamic> ExecutePythonCalculation(Dictionary<string, object> inputData)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var bridge = new PythonBridge(_projectRoot);
                    
                    // Vérifier l'environnement Python
                    if (!bridge.ValidateEnvironment(out string errorMessage))
                    {
                        throw new Exception($"Environnement Python invalide: {errorMessage}");
                    }

                    // Exécuter le calcul via le bridge
                    var result = bridge.ExecuteCalculationAsync(inputData).Result;
                    
                    if (result == null)
                    {
                        throw new Exception("Aucun résultat retourné par le moteur de calcul");
                    }

                    // Convertir en format compatible avec l'affichage existant
                    return new
                    {
                        total_cost = result.TotalCost,
                        staff_costs = result.StaffCosts,
                        equipment_costs = result.EquipmentCosts,
                        location_costs = result.LocationCosts,
                        operational_costs = result.OperationalCosts,
                        cost_breakdowns = result.CostBreakdowns.Select(cb => new
                        {
                            category = cb.Category,
                            subcategory = cb.Subcategory,
                            amount = cb.Amount,
                            formula = cb.Formula
                        }).ToArray()
                    };
                }
                catch (Exception ex)
                {
                    throw new Exception($"Erreur d'exécution Python: {ex.Message}");
                }
            });
        }

        private void DisplayResults(dynamic result)
        {
            // Update summary labels
            lblTotalCost.Text = $"TOTAL\n{result.total_cost:N2} CAD$";
            lblStaffCosts.Text = $"Personnel\n{result.staff_costs:N2} CAD$";
            lblEquipmentCosts.Text = $"Équipement\n{result.equipment_costs:N2} CAD$";
            lblLocationCosts.Text = $"Immobilier\n{result.location_costs:N2} CAD$";
            lblOperationalCosts.Text = $"Opérationnel\n{result.operational_costs:N2} CAD$";

            // Populate results grid
            var rows = new List<object>();
            foreach (var breakdown in result.cost_breakdowns)
            {
                rows.Add(new
                {
                    Category = breakdown.category,
                    Subcategory = breakdown.subcategory,
                    Amount = (double)breakdown.amount,
                    Formula = breakdown.formula
                });
            }

            dgvResults.DataSource = rows;
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            SetDefaultValues();
            dgvResults.DataSource = null;
            lblTotalCost.Text = "TOTAL\n0 CAD$";
            lblStaffCosts.Text = "Personnel\n0 CAD$";
            lblEquipmentCosts.Text = "Équipement\n0 CAD$";
            lblLocationCosts.Text = "Immobilier\n0 CAD$";
            lblOperationalCosts.Text = "Opérationnel\n0 CAD$";
            statusLabel.Text = "Formulaire réinitialisé";
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                var saveDialog = new SaveFileDialog
                {
                    Filter = "JSON files (*.json)|*.json",
                    DefaultExt = "json",
                    FileName = $"{txtSessionName.Text}_{DateTime.Now:yyyyMMdd_HHmmss}.json"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    var data = CollectInputData();
                    var json = JsonConvert.SerializeObject(data, Formatting.Indented);
                    File.WriteAllText(saveDialog.FileName, json);
                    statusLabel.Text = "Session sauvegardée";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur de sauvegarde: {ex.Message}", "Erreur", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                var openDialog = new OpenFileDialog
                {
                    Filter = "JSON files (*.json)|*.json",
                    DefaultExt = "json"
                };

                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    var json = File.ReadAllText(openDialog.FileName);
                    var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                    LoadInputData(data);
                    statusLabel.Text = "Session chargée";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur de chargement: {ex.Message}", "Erreur", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadInputData(Dictionary<string, object> data)
        {
            if (data.ContainsKey("session_name")) txtSessionName.Text = data["session_name"].ToString();
            if (data.ContainsKey("restaurant_theme")) cmbTheme.SelectedItem = data["restaurant_theme"].ToString();
            if (data.ContainsKey("revenue_size")) cmbRevenueSize.SelectedItem = data["revenue_size"].ToString();
            if (data.ContainsKey("kitchen_size_sqm")) numKitchenSize.Value = Convert.ToDecimal(data["kitchen_size_sqm"]);
            if (data.ContainsKey("kitchen_workstations")) numWorkstations.Value = Convert.ToDecimal(data["kitchen_workstations"]);
            if (data.ContainsKey("daily_capacity")) numCapacity.Value = Convert.ToDecimal(data["daily_capacity"]);
            if (data.ContainsKey("staff_count")) numStaffCount.Value = Convert.ToDecimal(data["staff_count"]);
            if (data.ContainsKey("staff_experience_level")) cmbExperience.SelectedItem = data["staff_experience_level"].ToString();
            if (data.ContainsKey("training_hours_needed")) numTrainingHours.Value = Convert.ToDecimal(data["training_hours_needed"]);
            if (data.ContainsKey("equipment_age_years")) numEquipmentAge.Value = Convert.ToDecimal(data["equipment_age_years"]);
            if (data.ContainsKey("equipment_condition")) cmbEquipmentCondition.SelectedItem = data["equipment_condition"].ToString();
            if (data.ContainsKey("equipment_value")) numEquipmentValue.Value = Convert.ToDecimal(data["equipment_value"]);
            if (data.ContainsKey("location_rent_sqm")) numRentPerSqm.Value = Convert.ToDecimal(data["location_rent_sqm"]);
        }

        private void BtnExportPDF_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Fonctionnalité d'export PDF à implémenter", "Info", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnExportCSV_Click(object sender, EventArgs e)
        {
            try
            {
                var saveDialog = new SaveFileDialog
                {
                    Filter = "CSV files (*.csv)|*.csv",
                    DefaultExt = "csv",
                    FileName = $"results_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK && dgvResults.DataSource != null)
                {
                    ExportToCSV(saveDialog.FileName);
                    statusLabel.Text = "Export CSV terminé";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur d'export: {ex.Message}", "Erreur", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportToCSV(string fileName)
        {
            using var writer = new StreamWriter(fileName);
            
            // Headers
            writer.WriteLine("Catégorie,Sous-catégorie,Montant (CAD$),Formule");
            
            // Data rows
            foreach (DataGridViewRow row in dgvResults.Rows)
            {
                if (row.IsNewRow) continue;
                
                var values = new string[4];
                for (int i = 0; i < 4; i++)
                {
                    values[i] = row.Cells[i].Value?.ToString()?.Replace(",", ";") ?? "";
                }
                writer.WriteLine(string.Join(",", values));
            }
        }

        /// <summary>
        /// Expose la méthode CollectInputData pour les tests
        /// </summary>
        public Dictionary<string, object> CollectInputData()
        {
            return new Dictionary<string, object>
            {
                ["session_name"] = txtSessionName.Text,
                ["restaurant_theme"] = cmbTheme.SelectedItem?.ToString() ?? "casual_dining",
                ["revenue_size"] = cmbRevenueSize.SelectedItem?.ToString() ?? "medium",
                ["kitchen_size_sqm"] = (double)numKitchenSize.Value,
                ["kitchen_workstations"] = (int)numWorkstations.Value,
                ["daily_capacity"] = (int)numCapacity.Value,
                ["staff_count"] = (int)numStaffCount.Value,
                ["staff_experience_level"] = cmbExperience.SelectedItem?.ToString() ?? "intermediate",
                ["training_hours_needed"] = (int)numTrainingHours.Value,
                ["equipment_age_years"] = (int)numEquipmentAge.Value,
                ["equipment_condition"] = cmbEquipmentCondition.SelectedItem?.ToString() ?? "good",
                ["equipment_value"] = (double)numEquipmentValue.Value,
                ["location_rent_sqm"] = (double)numRentPerSqm.Value
            };
        }

        /// <summary>
        /// Exécute les tests d'intégration
        /// </summary>
        private async void RunIntegrationTests()
        {
            var testRunner = new TestRunner(this);
            statusLabel.Text = "Exécution des tests d'intégration...";
            
            try
            {
                await testRunner.RunAllTestsAsync();
                testRunner.ShowTestResults();
                statusLabel.Text = "Tests d'intégration terminés";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors des tests: {ex.Message}", "Erreur de test", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                statusLabel.Text = "Erreur lors des tests";
            }
        }

        /// <summary>
        /// Ajoute un menu pour les tests (pour le développement)
        /// </summary>
        private void AddTestMenu()
        {
            var menuStrip = new MenuStrip();
            var testMenu = new ToolStripMenuItem("Tests");
            var runTestsItem = new ToolStripMenuItem("Exécuter tests d'intégration");
            
            runTestsItem.Click += (s, e) => RunIntegrationTests();
            testMenu.DropDownItems.Add(runTestsItem);
            menuStrip.Items.Add(testMenu);
            
            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);
        }
    }
}