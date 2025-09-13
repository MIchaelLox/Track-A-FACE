using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace FaceWebAppUI
{
    /// <summary>
    /// Classe utilitaire pour la validation avancée des entrées utilisateur
    /// </summary>
    public static class InputValidationHelper
    {
        /// <summary>
        /// Valide un nom de session selon les règles métier
        /// </summary>
        public static ValidationResult ValidateSessionName(string sessionName)
        {
            if (string.IsNullOrWhiteSpace(sessionName))
            {
                return new ValidationResult(false, "Le nom de session est requis", ValidationSeverity.Error);
            }

            if (sessionName.Length < 3)
            {
                return new ValidationResult(false, "Le nom de session doit contenir au moins 3 caractères", ValidationSeverity.Error);
            }

            if (sessionName.Length > 50)
            {
                return new ValidationResult(false, "Le nom de session ne peut pas dépasser 50 caractères", ValidationSeverity.Error);
            }

            // Vérifier les caractères interdits
            var invalidChars = new[] { '<', '>', ':', '"', '|', '?', '*', '\\', '/' };
            if (sessionName.Any(c => invalidChars.Contains(c)))
            {
                return new ValidationResult(false, "Le nom de session contient des caractères interdits", ValidationSeverity.Error);
            }

            return new ValidationResult(true, "Nom de session valide", ValidationSeverity.Success);
        }

        /// <summary>
        /// Valide la cohérence entre la taille de cuisine et le nombre de postes de travail
        /// </summary>
        public static ValidationResult ValidateKitchenWorkstationRatio(decimal kitchenSize, int workstations)
        {
            var ratio = (double)workstations / (double)kitchenSize;

            if (ratio > 0.3)
            {
                return new ValidationResult(false, 
                    $"Trop de postes de travail ({workstations}) pour une cuisine de {kitchenSize}m² (ratio: {ratio:F2})", 
                    ValidationSeverity.Error);
            }

            if (ratio > 0.2)
            {
                return new ValidationResult(true, 
                    $"Beaucoup de postes pour la taille de cuisine (ratio: {ratio:F2})", 
                    ValidationSeverity.Warning);
            }

            if (ratio < 0.05)
            {
                return new ValidationResult(true, 
                    $"Peu de postes pour la taille de cuisine (ratio: {ratio:F2})", 
                    ValidationSeverity.Warning);
            }

            return new ValidationResult(true, "Ratio postes/cuisine optimal", ValidationSeverity.Success);
        }

        /// <summary>
        /// Valide la cohérence entre la capacité et le nombre d'employés
        /// </summary>
        public static ValidationResult ValidateCapacityStaffRatio(int capacity, int staffCount)
        {
            if (staffCount == 0)
            {
                return new ValidationResult(false, "Le nombre d'employés ne peut pas être zéro", ValidationSeverity.Error);
            }

            var ratio = (double)capacity / staffCount;

            if (ratio > 100)
            {
                return new ValidationResult(false, 
                    $"Capacité trop élevée ({capacity}) pour {staffCount} employés (ratio: {ratio:F1})", 
                    ValidationSeverity.Error);
            }

            if (ratio > 50)
            {
                return new ValidationResult(true, 
                    $"Capacité élevée par employé (ratio: {ratio:F1} clients/employé)", 
                    ValidationSeverity.Warning);
            }

            if (ratio < 5)
            {
                return new ValidationResult(true, 
                    $"Capacité faible par employé (ratio: {ratio:F1} clients/employé)", 
                    ValidationSeverity.Warning);
            }

            return new ValidationResult(true, "Ratio capacité/employés optimal", ValidationSeverity.Success);
        }

        /// <summary>
        /// Valide la densité d'employés dans la cuisine
        /// </summary>
        public static ValidationResult ValidateStaffDensity(decimal kitchenSize, int staffCount)
        {
            var density = (double)staffCount / (double)kitchenSize;

            if (density > 0.5)
            {
                return new ValidationResult(false, 
                    $"Trop d'employés ({staffCount}) pour une cuisine de {kitchenSize}m² (densité: {density:F2})", 
                    ValidationSeverity.Error);
            }

            if (density > 0.3)
            {
                return new ValidationResult(true, 
                    $"Beaucoup d'employés pour la taille de cuisine (densité: {density:F2})", 
                    ValidationSeverity.Warning);
            }

            if (density < 0.05)
            {
                return new ValidationResult(true, 
                    $"Peu d'employés pour la taille de cuisine (densité: {density:F2})", 
                    ValidationSeverity.Warning);
            }

            return new ValidationResult(true, "Densité d'employés optimale", ValidationSeverity.Success);
        }

        /// <summary>
        /// Valide la valeur d'équipement par rapport à la taille de cuisine
        /// </summary>
        public static ValidationResult ValidateEquipmentValue(decimal kitchenSize, decimal equipmentValue)
        {
            var valuePerSqm = (double)equipmentValue / (double)kitchenSize;

            if (valuePerSqm > 10000)
            {
                return new ValidationResult(false, 
                    $"Valeur d'équipement excessive ({equipmentValue:C}) pour {kitchenSize}m² ({valuePerSqm:C}/m²)", 
                    ValidationSeverity.Error);
            }

            if (valuePerSqm > 5000)
            {
                return new ValidationResult(true, 
                    $"Équipement très coûteux ({valuePerSqm:C}/m²)", 
                    ValidationSeverity.Warning);
            }

            if (valuePerSqm < 500)
            {
                return new ValidationResult(true, 
                    $"Équipement peu coûteux ({valuePerSqm:C}/m²)", 
                    ValidationSeverity.Warning);
            }

            return new ValidationResult(true, "Valeur d'équipement appropriée", ValidationSeverity.Success);
        }

        /// <summary>
        /// Valide le loyer par mètre carré selon les standards du marché
        /// </summary>
        public static ValidationResult ValidateRentPerSqm(decimal rentPerSqm)
        {
            if (rentPerSqm > 150)
            {
                return new ValidationResult(false, 
                    $"Loyer excessif ({rentPerSqm:C}/m²)", 
                    ValidationSeverity.Error);
            }

            if (rentPerSqm > 100)
            {
                return new ValidationResult(true, 
                    $"Loyer très élevé ({rentPerSqm:C}/m²)", 
                    ValidationSeverity.Warning);
            }

            if (rentPerSqm < 10)
            {
                return new ValidationResult(true, 
                    $"Loyer très bas ({rentPerSqm:C}/m²)", 
                    ValidationSeverity.Warning);
            }

            return new ValidationResult(true, "Loyer dans la norme", ValidationSeverity.Success);
        }

        /// <summary>
        /// Valide la cohérence entre l'âge et l'état de l'équipement
        /// </summary>
        public static ValidationResult ValidateEquipmentAgeCondition(int equipmentAge, string condition)
        {
            switch (condition?.ToLower())
            {
                case "excellent":
                    if (equipmentAge > 2)
                        return new ValidationResult(true, 
                            "Équipement ancien mais en excellent état - vérifiez la cohérence", 
                            ValidationSeverity.Warning);
                    break;

                case "good":
                    if (equipmentAge > 8)
                        return new ValidationResult(true, 
                            "Équipement ancien en bon état - maintenance importante", 
                            ValidationSeverity.Warning);
                    break;

                case "fair":
                    if (equipmentAge < 3)
                        return new ValidationResult(true, 
                            "Équipement récent en état moyen - problème potentiel", 
                            ValidationSeverity.Warning);
                    break;

                case "poor":
                    if (equipmentAge < 5)
                        return new ValidationResult(false, 
                            "Équipement récent en mauvais état - incohérence", 
                            ValidationSeverity.Error);
                    break;
            }

            return new ValidationResult(true, "Cohérence âge/état acceptable", ValidationSeverity.Success);
        }

        /// <summary>
        /// Applique le style visuel selon le résultat de validation
        /// </summary>
        public static void ApplyValidationStyle(Control control, ValidationResult result)
        {
            switch (result.Severity)
            {
                case ValidationSeverity.Error:
                    control.BackColor = Color.LightPink;
                    break;
                case ValidationSeverity.Warning:
                    control.BackColor = Color.LightYellow;
                    break;
                case ValidationSeverity.Success:
                    control.BackColor = Color.White;
                    break;
            }

            // Ajouter tooltip si pas déjà présent
            var tooltip = new ToolTip();
            tooltip.SetToolTip(control, result.Message);
        }

        /// <summary>
        /// Valide un ensemble complet de données d'entrée
        /// </summary>
        public static List<ValidationResult> ValidateCompleteInput(Dictionary<string, object> inputData)
        {
            var results = new List<ValidationResult>();

            try
            {
                // Extraire les valeurs
                var sessionName = inputData["session_name"]?.ToString() ?? "";
                var kitchenSize = Convert.ToDecimal(inputData["kitchen_size_sqm"]);
                var workstations = Convert.ToInt32(inputData["kitchen_workstations"]);
                var capacity = Convert.ToInt32(inputData["daily_capacity"]);
                var staffCount = Convert.ToInt32(inputData["staff_count"]);
                var equipmentValue = Convert.ToDecimal(inputData["equipment_value"]);
                var equipmentAge = Convert.ToInt32(inputData["equipment_age_years"]);
                var equipmentCondition = inputData["equipment_condition"]?.ToString() ?? "";
                var rentPerSqm = Convert.ToDecimal(inputData["location_rent_sqm"]);

                // Validations individuelles
                results.Add(ValidateSessionName(sessionName));
                results.Add(ValidateRentPerSqm(rentPerSqm));

                // Validations croisées
                results.Add(ValidateKitchenWorkstationRatio(kitchenSize, workstations));
                results.Add(ValidateCapacityStaffRatio(capacity, staffCount));
                results.Add(ValidateStaffDensity(kitchenSize, staffCount));
                results.Add(ValidateEquipmentValue(kitchenSize, equipmentValue));
                results.Add(ValidateEquipmentAgeCondition(equipmentAge, equipmentCondition));
            }
            catch (Exception ex)
            {
                results.Add(new ValidationResult(false, $"Erreur de validation: {ex.Message}", ValidationSeverity.Error));
            }

            return results;
        }
    }

    /// <summary>
    /// Résultat d'une validation
    /// </summary>
    public class ValidationResult
    {
        public bool IsValid { get; }
        public string Message { get; }
        public ValidationSeverity Severity { get; }

        public ValidationResult(bool isValid, string message, ValidationSeverity severity)
        {
            IsValid = isValid;
            Message = message;
            Severity = severity;
        }
    }

    /// <summary>
    /// Niveaux de sévérité pour la validation
    /// </summary>
    public enum ValidationSeverity
    {
        Success,
        Warning,
        Error
    }
}
