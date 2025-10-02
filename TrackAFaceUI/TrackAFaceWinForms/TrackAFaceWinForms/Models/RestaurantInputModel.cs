using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackAFaceWinForms.Models
{
    /// <summary>
    /// Modèle représentant les entrées utilisateur pour le calcul des coûts de restaurant
    /// Compatible avec le backend Python engine_api.py
    /// </summary>
    public class RestaurantInputModel
    {
        #region Propriétés de Base

        /// <summary>
        /// Nom de la session de calcul
        /// </summary>
        [JsonProperty("session_name")]
        public string SessionName { get; set; }

        /// <summary>
        /// Thème du restaurant
        /// Valeurs possibles: fast_food, casual_dining, fine_dining, cloud_kitchen, food_truck
        /// </summary>
        [JsonProperty("restaurant_theme")]
        public string Theme { get; set; }

        /// <summary>
        /// Taille des revenus
        /// Valeurs possibles: small, medium, large, enterprise
        /// </summary>
        [JsonProperty("revenue_size")]
        public string RevenueSize { get; set; }

        #endregion

        #region Coûts Personnel

        /// <summary>
        /// Nombre d'employés (1-500)
        /// </summary>
        [JsonProperty("staff_count")]
        public int StaffCount { get; set; }

        /// <summary>
        /// Heures de formation nécessaires (0-200)
        /// </summary>
        [JsonProperty("training_hours_needed")]
        public int RetrainingNeedHours { get; set; }

        #endregion

        #region Coûts Immobilier

        /// <summary>
        /// Taille de la cuisine en mètres carrés (10-1000)
        /// </summary>
        [JsonProperty("kitchen_size_sqm")]
        public double KitchenSizeSqm { get; set; }

        /// <summary>
        /// Loyer mensuel en CAD$ (0-50000)
        /// </summary>
        [JsonProperty("rent_monthly")]
        public double RentMonthly { get; set; }

        /// <summary>
        /// Type de localisation
        /// Valeurs possibles: urban, suburban, rural
        /// </summary>
        [JsonProperty("location_type")]
        public string LocationType { get; set; }

        /// <summary>
        /// Coûts mensuels des utilities en CAD$ (0-10000)
        /// </summary>
        [JsonProperty("utility_cost_monthly")]
        public double UtilityCostMonthly { get; set; }

        #endregion

        #region Coûts Équipement

        /// <summary>
        /// Valeur de l'équipement en CAD$ (0-5000000)
        /// </summary>
        [JsonProperty("equipment_value")]
        public double EquipmentValue { get; set; }

        /// <summary>
        /// Condition de l'équipement (0.0-1.0, où 1.0 = excellent)
        /// </summary>
        [JsonIgnore]
        public double EquipmentCondition { get; set; }

        /// <summary>
        /// Âge de l'équipement en années (0-20)
        /// </summary>
        [JsonProperty("equipment_age_years")]
        public int EquipmentAgeYears { get; set; }

        #endregion

        #region Coûts Opérationnels

        /// <summary>
        /// Capacité opérationnelle (nombre de couverts/jour) (10-500)
        /// </summary>
        [JsonProperty("daily_capacity")]
        public int OperationalCapacity { get; set; }

        #endregion

        #region Champs Calculés Automatiquement

        /// <summary>
        /// Nombre de postes de travail dans la cuisine (calculé automatiquement)
        /// </summary>
        [JsonProperty("kitchen_workstations")]
        public int KitchenWorkstations => Math.Max(2, (int)(KitchenSizeSqm / 12.5));

        /// <summary>
        /// Niveau d'expérience du personnel (par défaut: intermediate)
        /// </summary>
        [JsonProperty("staff_experience_level")]
        public string StaffExperienceLevel => "intermediate";

        /// <summary>
        /// Loyer par m² (calculé depuis RentMonthly et KitchenSizeSqm)
        /// </summary>
        [JsonProperty("location_rent_sqm")]
        public double LocationRentSqm => KitchenSizeSqm > 0 ? RentMonthly / KitchenSizeSqm : 0;

        /// <summary>
        /// Condition de l'équipement au format string pour Python
        /// </summary>
        [JsonProperty("equipment_condition")]
        public string EquipmentConditionString
        {
            get
            {
                if (EquipmentCondition >= 0.9) return "excellent";
                if (EquipmentCondition >= 0.7) return "good";
                if (EquipmentCondition >= 0.5) return "fair";
                return "poor";
            }
        }

        #endregion

        #region Constructeurs

        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        public RestaurantInputModel()
        {
            SessionName = string.Empty;
            Theme = "casual_dining";
            RevenueSize = "medium";
            LocationType = "urban";

            // Valeurs par défaut raisonnables
            StaffCount = 10;
            RetrainingNeedHours = 40;
            KitchenSizeSqm = 100;
            RentMonthly = 5000;
            UtilityCostMonthly = 1200;
            EquipmentValue = 50000;
            EquipmentCondition = 0.8;
            EquipmentAgeYears = 2;
            OperationalCapacity = 150;
        }

        /// <summary>
        /// Constructeur avec paramètres complets
        /// </summary>
        public RestaurantInputModel(
            string sessionName,
            string theme,
            string revenueSize,
            int staffCount,
            int retrainingNeedHours,
            double kitchenSizeSqm,
            double rentMonthly,
            string locationType,
            double utilityCostMonthly,
            double equipmentValue,
            double equipmentCondition,
            int equipmentAgeYears,
            int operationalCapacity)
        {
            SessionName = sessionName;
            Theme = theme;
            RevenueSize = revenueSize;
            StaffCount = staffCount;
            RetrainingNeedHours = retrainingNeedHours;
            KitchenSizeSqm = kitchenSizeSqm;
            RentMonthly = rentMonthly;
            LocationType = locationType;
            UtilityCostMonthly = utilityCostMonthly;
            EquipmentValue = equipmentValue;
            EquipmentCondition = equipmentCondition;
            EquipmentAgeYears = equipmentAgeYears;
            OperationalCapacity = operationalCapacity;
        }

        #endregion

        #region Méthodes

        /// <summary>
        /// Valide les données d'entrée (validation de base)
        /// </summary>
        /// <returns>True si valide, False sinon</returns>
        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(SessionName) &&
                   !string.IsNullOrWhiteSpace(Theme) &&
                   !string.IsNullOrWhiteSpace(RevenueSize) &&
                   StaffCount > 0 &&
                   KitchenSizeSqm > 0 &&
                   EquipmentValue >= 0 &&
                   OperationalCapacity > 0;
        }

        /// <summary>
        /// Retourne une représentation JSON des données
        /// </summary>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Crée un modèle depuis une chaîne JSON
        /// </summary>
        public static RestaurantInputModel FromJson(string json)
        {
            return JsonConvert.DeserializeObject<RestaurantInputModel>(json);
        }

        /// <summary>
        /// Clone le modèle
        /// </summary>
        public RestaurantInputModel Clone()
        {
            return (RestaurantInputModel)this.MemberwiseClone();
        }

        /// <summary>
        /// Retourne une représentation textuelle
        /// </summary>
        public override string ToString()
        {
            return $"Session: {SessionName} | Theme: {Theme} | Revenue: {RevenueSize} | Staff: {StaffCount}";
        }

        #endregion
    }
}
