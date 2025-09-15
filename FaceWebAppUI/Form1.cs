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
        private OutputPipe _outputPipe;
        
        public MainForm()
        {
            InitializeComponent();
            
            // Initialiser les chemins
            _projectRoot = Path.GetDirectoryName(Path.GetDirectoryName(Application.StartupPath)) ?? "";
            _pythonScriptPath = Path.Combine(_projectRoot, "engine_api.py");
            
            // Configurer l'interface
            InitializeFormLayout();
            
            // Initialiser OutputPipe
            _outputPipe = new OutputPipe(this);
            
            // Ajouter le menu de tests (en mode développement)
            #if DEBUG
            AddTestMenu();
            #endif
            
            PopulateComboBoxes();
            SetDefaultValues();
            ApplyModernTheme();
        }

        private void InitializeFormLayout()
        {
            // Configure Input Tab Layout
            // ... (no changes)
        }

        private void ConfigureResultsGrid()
        {
            // Utiliser OutputPipe pour configurer le DataGridView avec style avancé
            _outputPipe.ConfigureResultsDataGrid(dgvResults);
            
            // Appliquer le style moderne
            dgvResults.ApplyModernStyle();
        }

        private Panel CreateExportPanel()
        {
            var panel = new FlowLayoutPanel
            // ... (no changes)
        }

        private async Task<dynamic> ExecutePythonCalculation(Dictionary<string, object> inputData)
        {
            try
            {
                var bridge = new PythonBridge(_projectRoot);
                
                // Diagnostic de l'environnement
                var diagnosticInfo = bridge.GetDiagnosticInfo();
                LogDiagnosticInfo(diagnosticInfo);
                
                // Vérifier l'environnement Python
                if (!bridge.ValidateEnvironment(out string errorMessage))
                {
                    ShowUserFriendlyError("Environnement Python", 
                        "Python n'est pas correctement configuré", 
                        errorMessage,
                        "Vérifiez que Python est installé et accessible depuis le PATH");
                    return null;
                }

                // Exécuter le calcul avec retry automatique
                var result = await bridge.ExecuteCalculationWithRetryAsync(inputData, 3);
                
                if (result == null)
                {
                    ShowUserFriendlyError("Calcul", 
                        "Aucun résultat retourné", 
                        "Le moteur de calcul n'a pas produit de résultat",
                        "Vérifiez les données d'entrée et réessayez");
                    return null;
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
            catch (PythonExecutionException ex)
            {
                ShowUserFriendlyError("Exécution Python", 
                    ex.ErrorDetails.UserFriendlyMessage,
                    ex.Message,
                    "Consultez les logs pour plus de détails");
                LogError(ex);
                return null;
            }
            catch (PythonCalculationException ex)
            {
                ShowUserFriendlyError("Calcul", 
                    "Erreur dans les calculs",
                    ex.Message,
                    ex.Details);
                LogError(ex);
                return null;
            }
            catch (Exception ex)
            {
                ShowUserFriendlyError("Système", 
                    "Erreur inattendue",
                    ex.Message,
                    "Contactez le support technique");
                LogError(ex);
                return null;
            }
        }

        private void DisplayResults(dynamic result)
        {
            // Update summary labels avec formatage amélioré
            UpdateSummaryLabelsWithStyle(result);

            // Utiliser OutputPipe pour afficher les résultats avec formatage avancé
            _outputPipe.DisplayCalculationResults(result);
        }
        
        private void UpdateSummaryLabelsWithStyle(dynamic result)
        {
            // Mise à jour avec animations et couleurs
            AnimateValueChange(lblTotalCost, $"TOTAL\n{result.total_cost:N2} CAD$", Color.FromArgb(220, 53, 69));
            AnimateValueChange(lblStaffCosts, $"Personnel\n{result.staff_costs:N2} CAD$", Color.FromArgb(0, 123, 255));
            AnimateValueChange(lblEquipmentCosts, $"Équipement\n{result.equipment_costs:N2} CAD$", Color.FromArgb(40, 167, 69));
            AnimateValueChange(lblLocationCosts, $"Immobilier\n{result.location_costs:N2} CAD$", Color.FromArgb(255, 193, 7));
            AnimateValueChange(lblOperationalCosts, $"Opérationnel\n{result.operational_costs:N2} CAD$", Color.FromArgb(108, 117, 125));
        }
        
        private void AnimateValueChange(Label label, string newText, Color color)
        {
            // Animation simple de changement de valeur
            var timer = new Timer { Interval = 50 };
            var steps = 10;
            var currentStep = 0;
            var originalColor = label.ForeColor;
            
            timer.Tick += (s, e) =>
            {
                currentStep++;
                if (currentStep <= steps)
                {
                    // Effet de pulsation
                    var alpha = (int)(255 * (0.5 + 0.5 * Math.Sin(currentStep * Math.PI / steps)));
                    label.ForeColor = Color.FromArgb(alpha, color);
                }
                else
                {
                    label.Text = newText;
                    label.ForeColor = color;
                    timer.Stop();
                    timer.Dispose();
                }
            };
            timer.Start();
        }

        private async void BtnCalculate_Click(object sender, EventArgs e)
        {
            // Validation préalable
            if (!ValidateAllInputs())
            {
                ShowValidationErrorSummary();
                return;
            }

            // Interface utilisateur - début du calcul
            SetCalculationInProgress(true);
            
            try
            {
                var inputData = CollectInputData();
                
                // Pré-validation des données
                if (!ValidateInputDataIntegrity(inputData))
                {
                    ShowUserFriendlyError("Validation", 
                        "Données incohérentes détectées",
                        "Certaines valeurs ne sont pas compatibles entre elles",
                        "Vérifiez la cohérence de vos paramètres");
                    return;
                }
                
                statusLabel.Text = "Connexion au moteur de calcul...";
                var result = await ExecutePythonCalculation(inputData);
                
                if (result != null)
                {
                    statusLabel.Text = "Traitement des résultats...";
                    DisplayResults(result);
                    tabControl.SelectedTab = tabResults;
                    
                    // Animation de succès
                    await ShowSuccessAnimation();
                    statusLabel.Text = $"✅ Calcul terminé - Total: {result.total_cost:N2} CAD$";
                }
                else
                {
                    statusLabel.Text = "❌ Échec du calcul";
                }
            }
            catch (InvalidOperationException ex)
            {
                ShowUserFriendlyError("Validation", 
                    "Données d'entrée invalides",
                    ex.Message,
                    "Corrigez les erreurs et réessayez");
                statusLabel.Text = "❌ Données invalides";
            }
            catch (Exception ex)
            {
                ShowUserFriendlyError("Système", 
                    "Erreur inattendue",
                    ex.Message,
                    "Réessayez ou contactez le support");
                statusLabel.Text = "❌ Erreur système";
                LogError(ex);
            }
            finally
            {
                SetCalculationInProgress(false);
            }
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            SetDefaultValues();
            // ... (no changes)
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
            try
            {
                var saveDialog = new SaveFileDialog
                {
                    Filter = "PDF files (*.pdf)|*.pdf",
                    DefaultExt = "pdf",
                    FileName = $"rapport_{txtSessionName.Text}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    _outputPipe.ExportToPDF(saveDialog.FileName, txtSessionName.Text);
                    statusLabel.Text = "Export PDF terminé";
                    MessageBox.Show("Rapport PDF généré avec succès!", "Export terminé", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'export PDF: {ex.Message}", "Erreur", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                    _outputPipe.ExportToCSV(saveDialog.FileName);
                    statusLabel.Text = "Export CSV terminé";
                    MessageBox.Show("Données exportées avec succès!", "Export terminé", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            // ... (no changes)
            
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
        /// Exécute les tests d'intégration complets
        /// </summary>
        private async void RunIntegrationTests()
        {
            var integrationTestRunner = new IntegrationTestRunner(this);
            statusLabel.Text = "🔄 Exécution des tests d'intégration...";
            
            // Désactiver l'interface pendant les tests
            SetCalculationInProgress(true);
            
            try
            {
                var results = await integrationTestRunner.RunAllIntegrationTestsAsync();
                integrationTestRunner.ShowTestResults();
                
                // Compter les succès et échecs
                int passed = 0, failed = 0;
                foreach (var result in results)
                {
                    if (result.Success) passed++;
                    else failed++;
                }
                
                if (failed == 0)
                {
                    statusLabel.Text = $"✅ Tous les tests réussis ({passed}/{results.Count})";
                    statusLabel.ForeColor = Color.FromArgb(40, 167, 69);
                }
                else
                {
                    statusLabel.Text = $"⚠️ Tests terminés: {passed} réussis, {failed} échoués";
                    statusLabel.ForeColor = Color.FromArgb(220, 53, 69);
                }
            }
            catch (Exception ex)
            {
                ShowUserFriendlyError("Tests d'intégration", 
                    "Erreur lors de l'exécution des tests",
                    ex.Message,
                    "Vérifiez l'environnement et réessayez");
                statusLabel.Text = "❌ Erreur lors des tests";
                statusLabel.ForeColor = Color.FromArgb(220, 53, 69);
                LogError(ex);
            }
            finally
            {
                SetCalculationInProgress(false);
            }
        }

        #region Validation Methods

        private void ValidateSessionName(object sender, EventArgs e)
        {
            var textBox = sender as TextBox;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.BackColor = Color.LightPink;
                statusLabel.Text = "Le nom de session est requis";
            }
            else if (textBox.Text.Length < 3)
            {
                textBox.BackColor = Color.LightYellow;
                statusLabel.Text = "Le nom de session doit contenir au moins 3 caractères";
            }
            else
            {
                textBox.BackColor = Color.White;
                statusLabel.Text = "Prêt";
            }
        }

        private void ValidateKitchenSize(object sender, EventArgs e)
        {
            var numericUpDown = sender as NumericUpDown;
            if (numericUpDown.Value < 20)
            {
                numericUpDown.BackColor = Color.LightYellow;
                statusLabel.Text = "Attention: Cuisine très petite (< 20m²)";
            }
            else if (numericUpDown.Value > 500)
            {
                numericUpDown.BackColor = Color.LightYellow;
                statusLabel.Text = "Attention: Cuisine très grande (> 500m²)";
            }
            else
            {
                numericUpDown.BackColor = Color.White;
                statusLabel.Text = "Prêt";
            }
        }

        private void ValidateWorkstations(object sender, EventArgs e)
        {
            var numericUpDown = sender as NumericUpDown;
            var kitchenSize = (double)numKitchenSize.Value;
            var workstationsPerSqm = (double)numericUpDown.Value / kitchenSize;
            
            if (workstationsPerSqm > 0.2) // Plus de 1 poste par 5m²
            {
                numericUpDown.BackColor = Color.LightYellow;
                statusLabel.Text = "Attention: Beaucoup de postes pour la taille de cuisine";
            }
            else if (workstationsPerSqm < 0.05) // Moins de 1 poste par 20m²
            {
                numericUpDown.BackColor = Color.LightYellow;
                statusLabel.Text = "Attention: Peu de postes pour la taille de cuisine";
            }
            else
            {
                numericUpDown.BackColor = Color.White;
                statusLabel.Text = "Prêt";
            }
        }

        private void ValidateCapacity(object sender, EventArgs e)
        {
            var numericUpDown = sender as NumericUpDown;
            var staffCount = (int)numStaffCount.Value;
            var capacityPerStaff = (double)numericUpDown.Value / staffCount;
            
            if (capacityPerStaff > 50)
            {
                numericUpDown.BackColor = Color.LightYellow;
                statusLabel.Text = "Attention: Capacité élevée par employé (> 50 clients/employé)";
            }
            else if (capacityPerStaff < 5)
            {
                numericUpDown.BackColor = Color.LightYellow;
                statusLabel.Text = "Attention: Capacité faible par employé (< 5 clients/employé)";
            }
            else
            {
                numericUpDown.BackColor = Color.White;
                statusLabel.Text = "Prêt";
            }
        }

        private void ValidateStaffCount(object sender, EventArgs e)
        {
            var numericUpDown = sender as NumericUpDown;
            var kitchenSize = (double)numKitchenSize.Value;
            var staffPerSqm = (double)numericUpDown.Value / kitchenSize;
            
            if (staffPerSqm > 0.5) // Plus de 1 employé par 2m²
            {
                numericUpDown.BackColor = Color.LightYellow;
                statusLabel.Text = "Attention: Beaucoup d'employés pour la taille de cuisine";
            }
            else if (staffPerSqm < 0.05) // Moins de 1 employé par 20m²
            {
                numericUpDown.BackColor = Color.LightYellow;
                statusLabel.Text = "Attention: Peu d'employés pour la taille de cuisine";
            }
            else
            {
                numericUpDown.BackColor = Color.White;
                statusLabel.Text = "Prêt";
            }
        }

        private void ValidateEquipmentValue(object sender, EventArgs e)
        {
            var numericUpDown = sender as NumericUpDown;
            var kitchenSize = (double)numKitchenSize.Value;
            var valuePerSqm = (double)numericUpDown.Value / kitchenSize;
            
            if (valuePerSqm > 5000) // Plus de 5000$ par m²
            {
                numericUpDown.BackColor = Color.LightYellow;
                statusLabel.Text = "Attention: Équipement très coûteux (> 5000$/m²)";
            }
            else if (valuePerSqm < 500) // Moins de 500$ par m²
            {
                numericUpDown.BackColor = Color.LightYellow;
                statusLabel.Text = "Attention: Équipement peu coûteux (< 500$/m²)";
            }
            else
            {
                numericUpDown.BackColor = Color.White;
                statusLabel.Text = "Prêt";
            }
        }

        private void ValidateRentPerSqm(object sender, EventArgs e)
        {
            var numericUpDown = sender as NumericUpDown;
            
            if (numericUpDown.Value > 100)
            {
                numericUpDown.BackColor = Color.LightYellow;
                statusLabel.Text = "Attention: Loyer très élevé (> 100$/m²)";
            }
            else if (numericUpDown.Value < 10)
            {
                numericUpDown.BackColor = Color.LightYellow;
                statusLabel.Text = "Attention: Loyer très bas (< 10$/m²)";
            }
            else
            {
                numericUpDown.BackColor = Color.White;
                statusLabel.Text = "Prêt";
            }
        }

        private void ValidateComboBoxSelection(object sender, EventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox.SelectedIndex == -1)
            {
                comboBox.BackColor = Color.LightPink;
                statusLabel.Text = "Veuillez sélectionner une option";
            }
            else
            {
                comboBox.BackColor = Color.White;
                statusLabel.Text = "Prêt";
            }
        }

        private bool ValidateAllInputs()
        {
            var isValid = true;
            var errors = new List<string>();

            // Validation du nom de session
            if (string.IsNullOrWhiteSpace(txtSessionName.Text))
            {
                errors.Add("Le nom de session est requis");
                isValid = false;
            }
            else if (txtSessionName.Text.Length < 3)
            {
                errors.Add("Le nom de session doit contenir au moins 3 caractères");
                isValid = false;
            }

            // Validation des ComboBox
            if (cmbTheme.SelectedIndex == -1)
            {
                errors.Add("Veuillez sélectionner un thème de restaurant");
                isValid = false;
            }

            if (cmbRevenueSize.SelectedIndex == -1)
            {
                errors.Add("Veuillez sélectionner une taille de revenus");
                isValid = false;
            }

            if (cmbExperience.SelectedIndex == -1)
            {
                errors.Add("Veuillez sélectionner un niveau d'expérience");
                isValid = false;
            }

            if (cmbEquipmentCondition.SelectedIndex == -1)
            {
                errors.Add("Veuillez sélectionner l'état de l'équipement");
                isValid = false;
            }

            // Validation des règles métier
            var workstationsPerSqm = (double)numWorkstations.Value / (double)numKitchenSize.Value;
            if (workstationsPerSqm > 0.3)
            {
                errors.Add("Trop de postes de travail pour la taille de cuisine");
                isValid = false;
            }

            var capacityPerStaff = (double)numCapacity.Value / (double)numStaffCount.Value;
            if (capacityPerStaff > 100)
            {
                errors.Add("Capacité trop élevée par rapport au nombre d'employés");
                isValid = false;
            }

            if (!isValid)
            {
                var errorMessage = "Erreurs de validation:\n" + string.Join("\n", errors);
                MessageBox.Show(errorMessage, "Validation des données", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return isValid;
        }

        #endregion

        #region Gestion d'erreurs avancée

        /// <summary>
        /// Affiche une erreur de manière conviviale pour l'utilisateur
        /// </summary>
        private void ShowUserFriendlyError(string category, string title, string technicalMessage, string userGuidance)
        {
            var errorDialog = new ErrorDialog(category, title, technicalMessage, userGuidance);
            errorDialog.ShowDialog(this);
        }

        /// <summary>
        /// Affiche un résumé des erreurs de validation
        /// </summary>
        private void ShowValidationErrorSummary()
        {
            var errors = GetCurrentValidationErrors();
            if (errors.Count == 0) return;

            var message = "Veuillez corriger les erreurs suivantes:\n\n";
            for (int i = 0; i < errors.Count; i++)
            {
                message += $"{i + 1}. {errors[i]}\n";
            }

            MessageBox.Show(message, "Erreurs de validation", 
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// Obtient la liste des erreurs de validation actuelles
        /// </summary>
        private List<string> GetCurrentValidationErrors()
        {
            var errors = new List<string>();

            // Validation du nom de session
            if (string.IsNullOrWhiteSpace(txtSessionName.Text))
                errors.Add("Le nom de session est requis");
            else if (txtSessionName.Text.Length < 3)
                errors.Add("Le nom de session doit contenir au moins 3 caractères");

            // Validation des ComboBox
            if (cmbTheme.SelectedIndex == -1)
                errors.Add("Veuillez sélectionner un thème de restaurant");
            if (cmbRevenueSize.SelectedIndex == -1)
                errors.Add("Veuillez sélectionner une taille de revenus");
            if (cmbExperience.SelectedIndex == -1)
                errors.Add("Veuillez sélectionner un niveau d'expérience");
            if (cmbEquipmentCondition.SelectedIndex == -1)
                errors.Add("Veuillez sélectionner l'état de l'équipement");

            return errors;
        }

        /// <summary>
        /// Valide l'intégrité des données d'entrée
        /// </summary>
        private bool ValidateInputDataIntegrity(Dictionary<string, object> inputData)
        {
            try
            {
                var kitchenSize = (double)inputData["kitchen_size_sqm"];
                var workstations = (int)inputData["kitchen_workstations"];
                var capacity = (int)inputData["daily_capacity"];
                var staffCount = (int)inputData["staff_count"];
                var equipmentValue = (double)inputData["equipment_value"];

                // Vérifications de cohérence
                if (workstations / kitchenSize > 0.5)
                    return false; // Trop de postes pour la taille
                
                if (capacity / staffCount > 100)
                    return false; // Capacité irréaliste par employé
                
                if (equipmentValue / kitchenSize < 100)
                    return false; // Équipement sous-évalué

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Configure l'interface pendant le calcul
        /// </summary>
        private void SetCalculationInProgress(bool inProgress)
        {
            btnCalculate.Enabled = !inProgress;
            btnReset.Enabled = !inProgress;
            btnSave.Enabled = !inProgress;
            btnLoad.Enabled = !inProgress;
            
            progressBar.Visible = inProgress;
            progressBar.Style = inProgress ? ProgressBarStyle.Marquee : ProgressBarStyle.Blocks;
            
            if (inProgress)
            {
                statusLabel.Text = "Calcul en cours...";
                statusLabel.ForeColor = Color.FromArgb(0, 123, 255);
            }
        }

        /// <summary>
        /// Animation de succès
        /// </summary>
        private async Task ShowSuccessAnimation()
        {
            var originalColor = statusLabel.ForeColor;
            
            for (int i = 0; i < 3; i++)
            {
                statusLabel.ForeColor = Color.FromArgb(40, 167, 69);
                await Task.Delay(200);
                statusLabel.ForeColor = originalColor;
                await Task.Delay(200);
            }
        }

        /// <summary>
        /// Enregistre les informations de diagnostic
        /// </summary>
        private void LogDiagnosticInfo(Dictionary<string, object> diagnosticInfo)
        {
            var logMessage = $"[DIAGNOSTIC] Python: {diagnosticInfo.GetValueOrDefault("python_available", false)}, " +
                           $"Fichiers: {diagnosticInfo.GetValueOrDefault("files_valid", false)}";
            
            // Log dans la console de debug
            System.Diagnostics.Debug.WriteLine(logMessage);
        }

        /// <summary>
        /// Enregistre les erreurs
        /// </summary>
        private void LogError(Exception ex)
        {
            var logMessage = $"[ERROR] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {ex.GetType().Name}: {ex.Message}";
            if (ex.InnerException != null)
            {
                logMessage += $" | Inner: {ex.InnerException.Message}";
            }
            
            // Log dans la console de debug
            System.Diagnostics.Debug.WriteLine(logMessage);
            
            // Optionnel: Log dans un fichier
            try
            {
                var logFile = Path.Combine(_projectRoot, "logs", "errors.log");
                Directory.CreateDirectory(Path.GetDirectoryName(logFile));
                File.AppendAllText(logFile, logMessage + Environment.NewLine);
            }
            catch
            {
                // Ignorer les erreurs de logging
            }
        }

        #endregion

        /// <summary>
        /// Applique le thème moderne à l'interface
        /// </summary>
        private void ApplyModernTheme()
        {
            // Couleurs du thème moderne
            var primaryColor = Color.FromArgb(52, 58, 64);
            var secondaryColor = Color.FromArgb(108, 117, 125);
            var accentColor = Color.FromArgb(0, 123, 255);
            var backgroundColor = Color.FromArgb(248, 249, 250);
            var cardColor = Color.White;

            // Style du formulaire principal
            this.BackColor = backgroundColor;
            this.Font = new Font("Segoe UI", 9F, FontStyle.Regular);

            // Style des onglets
            if (tabControl != null)
            {
                tabControl.BackColor = backgroundColor;
                tabControl.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
                
                foreach (TabPage tab in tabControl.TabPages)
                {
                    tab.BackColor = cardColor;
                    tab.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
                }
            }

            // Style des boutons avec effet hover
            ApplyButtonStyles();
            
            // Style des contrôles d'entrée
            ApplyInputStyles();
            
            // Style de la barre de statut
            if (statusLabel != null)
            {
                statusLabel.BackColor = primaryColor;
                statusLabel.ForeColor = Color.White;
                statusLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            }
        }

        /// <summary>
        /// Applique les styles aux boutons avec effets hover
        /// </summary>
        private void ApplyButtonStyles()
        {
            var buttons = new[] { btnCalculate, btnReset, btnSave, btnLoad, btnExportPDF, btnExportCSV };
            
            foreach (var button in buttons.Where(b => b != null))
            {
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderSize = 0;
                button.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                button.Cursor = Cursors.Hand;
                
                // Ajouter effets hover
                button.MouseEnter += (s, e) =>
                {
                    var btn = s as Button;
                    btn.FlatAppearance.BorderSize = 1;
                    btn.FlatAppearance.BorderColor = Color.FromArgb(0, 123, 255);
                };
                
                button.MouseLeave += (s, e) =>
                {
                    var btn = s as Button;
                    btn.FlatAppearance.BorderSize = 0;
                };
            }
        }

        /// <summary>
        /// Applique les styles aux contrôles d'entrée
        /// </summary>
        private void ApplyInputStyles()
        {
            var textBoxes = new[] { txtSessionName };
            var comboBoxes = new[] { cmbTheme, cmbRevenueSize, cmbExperience, cmbEquipmentCondition };
            var numericControls = new[] { numKitchenSize, numWorkstations, numCapacity, numStaffCount, 
                                         numTrainingHours, numEquipmentAge, numEquipmentValue, numRentPerSqm };

            // Style des TextBox
            foreach (var textBox in textBoxes.Where(t => t != null))
            {
                textBox.BorderStyle = BorderStyle.FixedSingle;
                textBox.Font = new Font("Segoe UI", 9F);
                textBox.BackColor = Color.White;
            }

            // Style des ComboBox
            foreach (var comboBox in comboBoxes.Where(c => c != null))
            {
                comboBox.FlatStyle = FlatStyle.Flat;
                comboBox.Font = new Font("Segoe UI", 9F);
                comboBox.BackColor = Color.White;
            }

            // Style des NumericUpDown
            foreach (var numeric in numericControls.Where(n => n != null))
            {
                numeric.BorderStyle = BorderStyle.FixedSingle;
                numeric.Font = new Font("Segoe UI", 9F);
                numeric.BackColor = Color.White;
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

    /// <summary>
    /// Dialogue d'erreur convivial pour l'utilisateur
    /// </summary>
    public partial class ErrorDialog : Form
    {
        public ErrorDialog(string category, string title, string technicalMessage, string userGuidance)
        {
            InitializeComponent();
            SetupErrorDialog(category, title, technicalMessage, userGuidance);
        }

        private void SetupErrorDialog(string category, string title, string technicalMessage, string userGuidance)
        {
            this.Text = $"Erreur - {category}";
            this.Size = new Size(500, 300);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 4,
                ColumnCount = 2,
                Padding = new Padding(20)
            };

            // Icône d'erreur
            var iconLabel = new Label
            {
                Text = "⚠️",
                Font = new Font("Segoe UI", 24F),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };

            // Titre
            var titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 53, 69),
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill
            };

            // Message utilisateur
            var userMessageLabel = new Label
            {
                Text = userGuidance,
                Font = new Font("Segoe UI", 10F),
                TextAlign = ContentAlignment.TopLeft,
                Dock = DockStyle.Fill,
                AutoSize = false
            };

            // Détails techniques (masqués par défaut)
            var detailsButton = new Button
            {
                Text = "Afficher les détails",
                Size = new Size(120, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White
            };

            var detailsTextBox = new TextBox
            {
                Text = technicalMessage,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Dock = DockStyle.Fill,
                Visible = false,
                Font = new Font("Consolas", 9F)
            };

            detailsButton.Click += (s, e) =>
            {
                detailsTextBox.Visible = !detailsTextBox.Visible;
                detailsButton.Text = detailsTextBox.Visible ? "Masquer les détails" : "Afficher les détails";
                this.Height = detailsTextBox.Visible ? 400 : 300;
            };

            // Bouton OK
            var okButton = new Button
            {
                Text = "OK",
                Size = new Size(80, 30),
                DialogResult = DialogResult.OK,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White
            };

            // Agencement
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));

            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F));
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            mainPanel.Controls.Add(iconLabel, 0, 0);
            mainPanel.Controls.Add(titleLabel, 1, 0);
            mainPanel.Controls.Add(userMessageLabel, 1, 1);
            mainPanel.Controls.Add(detailsTextBox, 1, 2);
            
            var buttonPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.RightToLeft,
                Dock = DockStyle.Fill
            };
            buttonPanel.Controls.Add(okButton);
            buttonPanel.Controls.Add(detailsButton);
            
            mainPanel.Controls.Add(buttonPanel, 1, 3);

            this.Controls.Add(mainPanel);
            this.AcceptButton = okButton;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(500, 300);
            this.Name = "ErrorDialog";
            this.ResumeLayout(false);
        }
    }
}