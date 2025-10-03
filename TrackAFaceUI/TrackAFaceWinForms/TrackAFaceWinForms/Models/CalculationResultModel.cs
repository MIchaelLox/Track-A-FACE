using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackAFaceWinForms.Models
{
    /// <summary>
    /// Modèle représentant les résultats de calcul du backend Python
    /// Compatible avec la sortie de engine_api.py
    /// </summary>
    public class CalculationResultModel
    {
        #region Propriétés Principales

        /// <summary>
        /// Identifiant unique de la session
        /// </summary>
        [JsonProperty("session_id")]
        public string SessionId { get; set; }

        /// <summary>
        /// Nom de la session
        /// </summary>
        [JsonProperty("session_name")]
        public string SessionName { get; set; }

        /// <summary>
        /// Date et heure du calcul
        /// </summary>
        [JsonProperty("calculation_timestamp")]
        public string CalculationTimestamp { get; set; }

        /// <summary>
        /// Indique si la validation a réussi
        /// </summary>
        [JsonProperty("validation_passed")]
        public bool ValidationPassed { get; set; }

        #endregion

        #region Coûts par Catégorie

        /// <summary>
        /// Coûts totaux du personnel en CAD$
        /// </summary>
        [JsonProperty("staff_costs")]
        public double StaffCosts { get; set; }

        /// <summary>
        /// Coûts totaux de l'équipement en CAD$
        /// </summary>
        [JsonProperty("equipment_costs")]
        public double EquipmentCosts { get; set; }

        /// <summary>
        /// Coûts totaux de l'immobilier en CAD$
        /// </summary>
        [JsonProperty("location_costs")]
        public double LocationCosts { get; set; }

        /// <summary>
        /// Coûts totaux opérationnels en CAD$
        /// </summary>
        [JsonProperty("operational_costs")]
        public double OperationalCosts { get; set; }

        /// <summary>
        /// Coût total en CAD$
        /// </summary>
        [JsonProperty("total_cost")]
        public double TotalCost { get; set; }

        #endregion

        #region Détails des Coûts

        /// <summary>
        /// Liste détaillée des coûts par sous-catégorie
        /// </summary>
        [JsonProperty("cost_breakdowns")]
        public List<CostBreakdownItem> CostBreakdowns { get; set; }

        #endregion

        #region Gestion d'Erreurs

        /// <summary>
        /// Type d'erreur si le calcul a échoué
        /// </summary>
        [JsonProperty("error")]
        public string Error { get; set; }

        /// <summary>
        /// Message d'erreur détaillé
        /// </summary>
        [JsonProperty("message")]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Détails supplémentaires sur l'erreur
        /// </summary>
        [JsonProperty("details")]
        public string ErrorDetails { get; set; }

        #endregion

        #region Constructeurs

        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        public CalculationResultModel()
        {
            CostBreakdowns = new List<CostBreakdownItem>();
            SessionId = string.Empty;
            SessionName = string.Empty;
            CalculationTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            ValidationPassed = false;
        }

        #endregion

        #region Propriétés Calculées

        /// <summary>
        /// Indique si le résultat contient une erreur
        /// </summary>
        [JsonIgnore]
        public bool HasError => !string.IsNullOrEmpty(Error);

        /// <summary>
        /// Indique si les résultats sont valides
        /// </summary>
        [JsonIgnore]
        public bool IsValid => !HasError && ValidationPassed && TotalCost > 0;

        /// <summary>
        /// Pourcentage des coûts de personnel par rapport au total
        /// </summary>
        [JsonIgnore]
        public double StaffCostsPercentage => TotalCost > 0 ? (StaffCosts / TotalCost) * 100 : 0;

        /// <summary>
        /// Pourcentage des coûts d'équipement par rapport au total
        /// </summary>
        [JsonIgnore]
        public double EquipmentCostsPercentage => TotalCost > 0 ? (EquipmentCosts / TotalCost) * 100 : 0;

        /// <summary>
        /// Pourcentage des coûts immobiliers par rapport au total
        /// </summary>
        [JsonIgnore]
        public double LocationCostsPercentage => TotalCost > 0 ? (LocationCosts / TotalCost) * 100 : 0;

        /// <summary>
        /// Pourcentage des coûts opérationnels par rapport au total
        /// </summary>
        [JsonIgnore]
        public double OperationalCostsPercentage => TotalCost > 0 ? (OperationalCosts / TotalCost) * 100 : 0;

        #endregion

        #region Méthodes de Requête

        /// <summary>
        /// Récupère les breakdowns d'une catégorie spécifique
        /// </summary>
        public List<CostBreakdownItem> GetBreakdownsByCategory(string category)
        {
            return CostBreakdowns?.Where(b => b.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList()
                   ?? new List<CostBreakdownItem>();
        }

        /// <summary>
        /// Récupère les breakdowns du personnel
        /// </summary>
        public List<CostBreakdownItem> GetStaffBreakdowns() => GetBreakdownsByCategory("Staff");

        /// <summary>
        /// Récupère les breakdowns de l'équipement
        /// </summary>
        public List<CostBreakdownItem> GetEquipmentBreakdowns() => GetBreakdownsByCategory("Equipment");

        /// <summary>
        /// Récupère les breakdowns immobiliers
        /// </summary>
        public List<CostBreakdownItem> GetLocationBreakdowns() => GetBreakdownsByCategory("Location");

        /// <summary>
        /// Récupère les breakdowns opérationnels
        /// </summary>
        public List<CostBreakdownItem> GetOperationalBreakdowns()
        {
            var items = GetBreakdownsByCategory("Operational");
            
            // Si vide et qu'il y a un coût opérationnel total, créer un item générique
            if ((items == null || items.Count == 0) && OperationalCosts > 0)
            {
                items = new List<CostBreakdownItem>
                {
                    new CostBreakdownItem
                    {
                        Category = "Operational",
                        Subcategory = "couts_operationnels_totaux",
                        Amount = OperationalCosts,
                        Formula = $"{OperationalCosts:N2}",
                        Details = "Coûts opérationnels calculés par le moteur Python"
                    }
                };
            }
            
            return items ?? new List<CostBreakdownItem>();
        }

        #endregion

        #region Méthodes de Sérialisation

        /// <summary>
        /// Retourne une représentation JSON des résultats
        /// </summary>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Crée un modèle depuis une chaîne JSON
        /// </summary>
        public static CalculationResultModel FromJson(string json)
        {
            return JsonConvert.DeserializeObject<CalculationResultModel>(json);
        }

        #endregion

        #region Méthodes Utilitaires

        /// <summary>
        /// Retourne un résumé textuel des résultats
        /// </summary>
        public string GetSummary()
        {
            if (HasError)
            {
                return $"Erreur: {ErrorMessage}";
            }

            return $@"Session: {SessionName}
Coût Total: {TotalCost:N2} CAD$
├─ Personnel: {StaffCosts:N2} CAD$ ({StaffCostsPercentage:F1}%)
├─ Équipement: {EquipmentCosts:N2} CAD$ ({EquipmentCostsPercentage:F1}%)
├─ Immobilier: {LocationCosts:N2} CAD$ ({LocationCostsPercentage:F1}%)
└─ Opérationnel: {OperationalCosts:N2} CAD$ ({OperationalCostsPercentage:F1}%)
Validation: {(ValidationPassed ? "Réussie" : "Échouée")}";
        }

        /// <summary>
        /// Retourne une représentation textuelle
        /// </summary>
        public override string ToString()
        {
            return HasError
                ? $"Error: {ErrorMessage}"
                : $"Session: {SessionName} | Total: {TotalCost:N2} CAD$";
        }

        #endregion
    }

    /// <summary>
    /// Représente un élément détaillé de breakdown des coûts
    /// </summary>
    public class CostBreakdownItem
    {
        /// <summary>
        /// Catégorie principale (Staff, Equipment, Location, Operational)
        /// </summary>
        [JsonProperty("category")]
        public string Category { get; set; }

        /// <summary>
        /// Sous-catégorie (ex: "Training", "Depreciation", "Rent")
        /// </summary>
        [JsonProperty("subcategory")]
        public string Subcategory { get; set; }

        /// <summary>
        /// Montant en CAD$
        /// </summary>
        [JsonProperty("amount")]
        public double Amount { get; set; }

        /// <summary>
        /// Formule de calcul utilisée
        /// </summary>
        [JsonProperty("formula")]
        public string Formula { get; set; }

        /// <summary>
        /// Détails supplémentaires du calcul (accepte tout type JSON)
        /// </summary>
        [JsonProperty("details")]
        public JToken DetailsRaw { get; set; }

        /// <summary>
        /// Détails formatés en string pour l'affichage
        /// </summary>
        [JsonIgnore]
        public string Details
        {
            get
            {
                if (DetailsRaw == null) return string.Empty;
                
                // Si c'est une string simple
                if (DetailsRaw.Type == JTokenType.String)
                    return DetailsRaw.ToString();
                
                // Si c'est un objet ou array, formater joliment
                return DetailsRaw.ToString(Formatting.None);
            }
            set 
            { 
                if (value != null)
                    DetailsRaw = JToken.Parse($"\"{value}\""); 
            }
        }

        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        public CostBreakdownItem()
        {
            Category = string.Empty;
            Subcategory = string.Empty;
            Formula = string.Empty;
            DetailsRaw = null;
        }

        /// <summary>
        /// Retourne une représentation textuelle
        /// </summary>
        public override string ToString()
        {
            return $"{Category} > {Subcategory}: {Amount:N2} CAD$";
        }
    }
}
