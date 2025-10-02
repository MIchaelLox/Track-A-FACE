using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackAFaceWinForms.Helpers;
using TrackAFaceWinForms.Models;
using TrackAFaceWinForms.Services;

namespace TrackAFaceWinForms.Forms
{
    public partial class InputForm : Form
    {
        public InputForm()
        {
            InitializeComponent();
            InitializeDefaults();
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
        // ✅ APRÈS (avec async)
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
            btnCalculate.Text = "Calcul en cours...";
            this.Cursor = Cursors.WaitCursor;

            try
            {
                // Appeler Python via PythonBridge
                var bridge = new PythonBridge();
                var results = await bridge.CalculateAsync(inputs);

                // Afficher les résultats
                var resultsForm = new ResultsForm();
                resultsForm.DisplayResults(results);
                resultsForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erreur lors du calcul:\n\n{ex.Message}",
                    "Erreur",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                // Réactiver le bouton
                btnCalculate.Enabled = true;
                btnCalculate.Text = "Calculer";
                this.Cursor = Cursors.Default;
            }
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

