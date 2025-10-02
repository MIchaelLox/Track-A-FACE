using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackAFaceWinForms.Models;

namespace TrackAFaceWinForms.Helpers
{
    public static class ValidationHelper
    {
        public static bool ValidateInputs(RestaurantInputModel model, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(model.SessionName))
            {
                errorMessage = "Le nom de la session est requis.";
                return false;
            }

            if (model.StaffCount < 1 || model.StaffCount > 500)
            {
                errorMessage = "Le nombre d'employés doit être entre 1 et 500.";
                return false;
            }

            if (model.KitchenSizeSqm < 10 || model.KitchenSizeSqm > 1000)
            {
                errorMessage = "La taille de la cuisine doit être entre 10 et 1000 m².";
                return false;
            }

            if (model.EquipmentValue < 0 || model.EquipmentValue > 5000000)
            {
                errorMessage = "La valeur de l'équipement doit être entre 0 et 5,000,000 CAD$.";
                return false;
            }

            if (model.OperationalCapacity < 10 || model.OperationalCapacity > 500)
            {
                errorMessage = "La capacité opérationnelle doit être entre 10 et 500.";
                return false;
            }

            return true;
        }

        public static void ShowValidationError(string message)
        {
            MessageBox.Show(message, "Erreur de validation",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
