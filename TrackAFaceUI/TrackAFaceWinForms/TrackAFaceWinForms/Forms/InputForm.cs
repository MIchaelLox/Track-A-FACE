using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackAFaceWinForms.Dialogs;
using TrackAFaceWinForms.Helpers;
using TrackAFaceWinForms.Models;
using TrackAFaceWinForms.Services;
using TrackAFaceWinForms.Session;

namespace TrackAFaceWinForms.Forms
{
    public partial class InputForm : Form
    {
        public InputForm()
        {
            InitializeComponent();
            InitializeDefaults();
            
            // Activer la capture des touches
            this.KeyPreview = true;
        }

        /// <summary>
        /// Gestion des raccourcis clavier
        /// </summary>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Ctrl+S = Sauvegarder
            if (keyData == (Keys.Control | Keys.S))
            {
                btnSave_Click(this, EventArgs.Empty);
                return true;
            }

            // Ctrl+O = Charger Session
            if (keyData == (Keys.Control | Keys.O))
            {
                btnLoad_Click(this, EventArgs.Empty);
                return true;
            }

            // F5 = Calculer
            if (keyData == Keys.F5)
            {
                btnCalculate_Click(this, EventArgs.Empty);
                return true;
            }

            // Ctrl+R = Réinitialiser
            if (keyData == (Keys.Control | Keys.R))
            {
                btnReset_Click(this, EventArgs.Empty);
                return true;
            }

            // Escape = Fermer
            if (keyData == Keys.Escape)
            {
                btnClose_Click(this, EventArgs.Empty);
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void InitializeDefaults()
        {
            // Valeurs par défaut
            if (cmbTheme.Items.Count > 0)
                cmbTheme.SelectedIndex = 1; // casual_dining

            if (cmbLocationType.Items.Count > 0)
                cmbLocationType.SelectedIndex = 0; // urban

            rbMedium.Checked = true;

            numStaffCount.Value = 10;
            numRetrainingHours.Value = 40;
            numKitchenSize.Value = 100;
            numRentMonthly.Value = 5000;
            numUtilities.Value = 1200;
            numEquipmentValue.Value = 50000;
            trkEquipmentCondition.Value = 80;
            numEquipmentAge.Value = 2;
            numCapacity.Value = 150;

            UpdateConditionLabel();
        }

        // ⭐ MÉTHODE REQUISE PAR DESIGNER
        private void trkEquipmentCondition_Scroll(object sender, EventArgs e)
        {
            UpdateConditionLabel();
        }

        private void UpdateConditionLabel()
        {
            lblConditionValue.Text = $"{trkEquipmentCondition.Value}%";
        }

        // ⭐ MÉTHODE REQUISE PAR DESIGNER
        private void btnReset_Click(object sender, EventArgs e)
        {
            txtSessionName.Clear();
            InitializeDefaults();
        }

        // ⭐ MÉTHODE REQUISE PAR DESIGNER
        private void btnClose_Click(object sender, EventArgs e)
        {
            // Demander confirmation si des données ont été saisies
            var sessionName = txtSessionName.Text.Trim();
            if (!string.IsNullOrEmpty(sessionName))
            {
                var result = MessageBox.Show(
                    "Voulez-vous fermer le formulaire sans sauvegarder?",
                    "Confirmer la fermeture",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.No)
                {
                    return; // Ne pas fermer
                }
            }

            this.Close();
        }

        // ⭐ MÉTHODE REQUISE PAR DESIGNER
        // ✅ APRÈS (avec async + ProgressDialog)
        private async void btnCalculate_Click(object sender, EventArgs e)
        {
            var inputs = GetInputData();

            // Validation
            if (!ValidationHelper.ValidateInputs(inputs, out string error))
            {
                ValidationHelper.ShowValidationError(error);
                return;
            }

            // Désactiver le bouton pendant le calcul
            btnCalculate.Enabled = false;

            // Créer et afficher le dialogue de progression
            ProgressDialog progressDialog = null;
            try
            {
                progressDialog = new ProgressDialog();
                progressDialog.UpdateMessage("Calcul des coûts en cours...");
                progressDialog.UpdateStatus("Préparation des données...");
                progressDialog.Show(this);
                
                // Forcer l'affichage du dialogue
                Application.DoEvents();

                // Mettre à jour le statut
                progressDialog.UpdateStatus("Communication avec le moteur Python...");
                Application.DoEvents();

                // Appeler Python via PythonBridge
                var bridge = new PythonBridge();
                var results = await bridge.CalculateAsync(inputs);

                // Mettre à jour le statut
                progressDialog.UpdateStatus("Traitement des résultats...");
                Application.DoEvents();

                // Fermer le dialogue de progression
                progressDialog.Close();
                progressDialog.Dispose();
                progressDialog = null;

                // Afficher les résultats
                var resultsForm = new ResultsForm();
                resultsForm.DisplayResults(results);
                resultsForm.ShowDialog();
            }
            catch (Exception ex)
            {
                // Fermer le dialogue de progression en cas d'erreur
                if (progressDialog != null)
                {
                    progressDialog.Close();
                    progressDialog.Dispose();
                }

                MessageBox.Show(
                    $"Erreur lors du calcul:\n\n{ex.Message}",
                    "Erreur",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                // S'assurer que le dialogue est fermé
                if (progressDialog != null && !progressDialog.IsDisposed)
                {
                    progressDialog.Close();
                    progressDialog.Dispose();
                }

                // Réactiver le bouton
                btnCalculate.Enabled = true;
            }
        }

        // ⭐ MÉTHODE REQUISE PAR DESIGNER
        private void btnSave_Click(object sender, EventArgs e)
        {
            var inputs = GetInputData();

            // Validation avant sauvegarde
            if (!ValidationHelper.ValidateInputs(inputs, out string error))
            {
                MessageBox.Show(
                    $"Impossible de sauvegarder une session invalide:\n\n{error}",
                    "Validation échouée",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            try
            {
                var sessionManager = new SessionManager();
                string filePath = sessionManager.SaveSession(inputs);

                MessageBox.Show(
                    $"Session sauvegardée avec succès!\n\nFichier: {System.IO.Path.GetFileName(filePath)}",
                    "Sauvegarde réussie",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erreur lors de la sauvegarde:\n\n{ex.Message}",
                    "Erreur",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // ⭐ MÉTHODE REQUISE PAR DESIGNER
        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                using (var dialog = new LoadSessionDialog())
                {
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        if (!string.IsNullOrEmpty(dialog.SelectedSessionPath))
                        {
                            LoadSessionFromFile(dialog.SelectedSessionPath);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erreur lors de l'ouverture du dialogue:\n\n{ex.Message}",
                    "Erreur",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Charge une session depuis un fichier spécifique
        /// </summary>
        private void LoadSessionFromFile(string filePath)
        {
            try
            {
                var sessionManager = new SessionManager();
                var inputs = sessionManager.LoadSession(filePath);

                // Charger les données dans le formulaire
                LoadInputsToForm(inputs);

                MessageBox.Show(
                    $"Session '{inputs.SessionName}' chargée avec succès!",
                    "Chargement réussi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erreur lors du chargement:\n\n{ex.Message}",
                    "Erreur",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Charge une session depuis un fichier (ancienne méthode, conservée pour compatibilité)
        /// </summary>
        public void LoadSession()
        {
            var openDialog = new OpenFileDialog
            {
                Title = "Charger une session",
                Filter = "Fichiers JSON (*.json)|*.json|Tous les fichiers (*.*)|*.*",
                InitialDirectory = ConfigurationHelper.SessionsDirectory
            };

            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                LoadSessionFromFile(openDialog.FileName);
            }
        }

        /// <summary>
        /// Charge les données du modèle dans le formulaire
        /// </summary>
        private void LoadInputsToForm(RestaurantInputModel inputs)
        {
            txtSessionName.Text = inputs.SessionName;

            // Theme
            int themeIndex = cmbTheme.Items.IndexOf(inputs.Theme);
            if (themeIndex >= 0)
                cmbTheme.SelectedIndex = themeIndex;

            // Revenue size
            switch (inputs.RevenueSize.ToLower())
            {
                case "small": rbSmall.Checked = true; break;
                case "medium": rbMedium.Checked = true; break;
                case "large": rbLarge.Checked = true; break;
                case "enterprise": rbEnterprise.Checked = true; break;
            }

            // Personnel
            numStaffCount.Value = (decimal)inputs.StaffCount;
            numRetrainingHours.Value = (decimal)inputs.RetrainingNeedHours;

            // Immobilier
            numKitchenSize.Value = (decimal)inputs.KitchenSizeSqm;
            numRentMonthly.Value = (decimal)inputs.RentMonthly;

            int locationIndex = cmbLocationType.Items.IndexOf(inputs.LocationType);
            if (locationIndex >= 0)
                cmbLocationType.SelectedIndex = locationIndex;

            numUtilities.Value = (decimal)inputs.UtilityCostMonthly;

            // Équipement
            numEquipmentValue.Value = (decimal)inputs.EquipmentValue;
            trkEquipmentCondition.Value = (int)(inputs.EquipmentCondition * 100);
            numEquipmentAge.Value = inputs.EquipmentAgeYears;

            // Opérationnel
            numCapacity.Value = (decimal)inputs.OperationalCapacity;

            UpdateConditionLabel();
        }

        // Méthode pour récupérer les données du formulaire
        public RestaurantInputModel GetInputData()
        {
            string revenueSize = rbSmall.Checked ? "small" :
                                 rbMedium.Checked ? "medium" :
                                 rbLarge.Checked ? "large" : "enterprise";

            return new RestaurantInputModel
            {
                SessionName = txtSessionName.Text,
                Theme = cmbTheme.SelectedItem?.ToString() ?? "casual_dining",
                RevenueSize = revenueSize,
                StaffCount = (int)numStaffCount.Value,
                RetrainingNeedHours = (int)numRetrainingHours.Value,
                KitchenSizeSqm = (double)numKitchenSize.Value,
                RentMonthly = (double)numRentMonthly.Value,
                LocationType = cmbLocationType.SelectedItem?.ToString() ?? "urban",
                UtilityCostMonthly = (double)numUtilities.Value,
                EquipmentValue = (double)numEquipmentValue.Value,
                EquipmentCondition = trkEquipmentCondition.Value / 100.0,
                EquipmentAgeYears = (int)numEquipmentAge.Value,
                OperationalCapacity = (int)numCapacity.Value
            };
        }
    }
}

