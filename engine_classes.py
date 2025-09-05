# ==========================
# /engine_classes.py
# ==========================

"""
Module du moteur de calcul des coûts pour Track-A-FACE
Contient les classes et formules pour calculer tous les types de coûts
"""

from typing import Dict, Any, List, Optional
from dataclasses import dataclass
import logging
from sql import DatabaseManager, get_all_daos
from input_handler import RestaurantInputs
from config import BUSINESS_CONSTANTS

logger = logging.getLogger(__name__)


@dataclass
class CostBreakdown:
    """Structure pour détailler un calcul de coût"""
    category: str
    subcategory: str
    amount: float
    formula: str
    details: Dict[str, Any]


@dataclass
class TotalCostSummary:
    """Résumé complet des coûts calculés"""
    session_id: int
    session_name: str
    staff_costs: float
    equipment_costs: float
    location_costs: float
    operational_costs: float
    total_cost: float
    cost_breakdowns: List[CostBreakdown]
    
    def get_cost_by_category(self, category: str) -> float:
        """Retourne le coût total d'une catégorie"""
        category_map = {
            'staff': self.staff_costs,
            'equipment': self.equipment_costs,
            'location': self.location_costs,
            'operations': self.operational_costs
        }
        return category_map.get(category, 0.0)


class StaffCostCalculator:
    """Calculateur des coûts de personnel"""
    
    def __init__(self, db_manager: DatabaseManager):
        self.db_manager = db_manager
        self.daos = get_all_daos(db_manager)
    
    def calculate_training_cost(self, inputs: RestaurantInputs) -> CostBreakdown:
        """
        Calcule le coût de formation du personnel avec formules pondérées avancées
        
        Formule: training_hours × staff_count × hourly_rate × complexity_factor × size_factor × experience_factor
        """
        # Récupérer les facteurs de base de la DB
        base_hourly_rate = self.daos['cost_factors'].get_factor_value(
            'training_rate_per_hour', 
            inputs.restaurant_theme
        ) or 30.0
        
        # Facteur de complexité selon le thème
        complexity_factor = self.daos['cost_factors'].get_factor_value(
            'training_complexity_factor',
            inputs.restaurant_theme
        ) or self._get_theme_complexity_factor(inputs.restaurant_theme)
        
        # Facteur de taille d'entreprise
        size_factor = self.daos['cost_factors'].get_factor_value(
            'training_size_factor',
            revenue_size=inputs.revenue_size
        ) or self._get_size_training_factor(inputs.revenue_size)
        
        # Facteur d'expérience (plus d'employés = économies d'échelle)
        experience_factor = max(0.7, 1.0 - (inputs.staff_count - 5) * 0.02)  # Réduction jusqu'à 30%
        
        # Calcul pondéré avancé
        weighted_hourly_rate = base_hourly_rate * complexity_factor * size_factor
        training_cost = (
            inputs.training_hours_needed * 
            inputs.staff_count * 
            weighted_hourly_rate * 
            experience_factor
        )
        
        formula = f"{inputs.training_hours_needed}h × {inputs.staff_count} × {weighted_hourly_rate:.2f} CAD$/h × {experience_factor:.2f}"
        
        details = {
            'training_hours': inputs.training_hours_needed,
            'staff_count': inputs.staff_count,
            'base_hourly_rate': base_hourly_rate,
            'complexity_factor': complexity_factor,
            'size_factor': size_factor,
            'experience_factor': experience_factor,
            'weighted_hourly_rate': weighted_hourly_rate,
            'theme': inputs.restaurant_theme,
            'revenue_size': inputs.revenue_size
        }
        
        return CostBreakdown(
            category='staff',
            subcategory='formation_initiale',
            amount=training_cost,
            formula=formula,
            details=details
        )
    
    def calculate_salary_costs(self, inputs: RestaurantInputs) -> CostBreakdown:
        """
        Calcule les coûts salariaux annuels avec pondération dynamique
        
        Formule: staff_count × base_salary × theme_mult × revenue_mult × location_mult × performance_mult
        """
        base_annual_salary = BUSINESS_CONSTANTS["default_staff_hourly_rate"] * 40 * 52
        
        # Récupérer les multiplicateurs depuis la DB
        theme_mult = self.daos['cost_factors'].get_factor_value(
            'salary_theme_multiplier',
            inputs.restaurant_theme
        ) or self._get_theme_salary_multiplier(inputs.restaurant_theme)
        
        revenue_mult = self.daos['cost_factors'].get_factor_value(
            'salary_revenue_multiplier',
            revenue_size=inputs.revenue_size
        ) or self._get_revenue_salary_multiplier(inputs.revenue_size)
        
        # Facteur de localisation (basé sur la capacité comme proxy de la zone)
        location_mult = self._calculate_location_multiplier(inputs.daily_capacity, inputs.kitchen_size_sqm)
        
        # Facteur de performance (efficacité opérationnelle)
        performance_mult = self._calculate_performance_multiplier(inputs)
        
        # Calcul pondéré complet
        weighted_salary = base_annual_salary * theme_mult * revenue_mult * location_mult * performance_mult
        annual_salary_cost = inputs.staff_count * weighted_salary
        
        formula = f"{inputs.staff_count} × {base_annual_salary:,.0f} × {theme_mult:.2f} × {revenue_mult:.2f} × {location_mult:.2f} × {performance_mult:.2f}"
        
        details = {
            'staff_count': inputs.staff_count,
            'base_annual_salary': base_annual_salary,
            'theme_multiplier': theme_mult,
            'revenue_multiplier': revenue_mult,
            'location_multiplier': location_mult,
            'performance_multiplier': performance_mult,
            'weighted_salary': weighted_salary,
            'theme': inputs.restaurant_theme,
            'revenue_size': inputs.revenue_size
        }
        
        return CostBreakdown(
            category='staff',
            subcategory='salaires_annuels',
            amount=annual_salary_cost,
            formula=formula,
            details=details
        )
    
    def _get_theme_complexity_factor(self, theme: str) -> float:
        """Facteurs de complexité de formation par thème"""
        factors = {
            'fast_food': 0.8,      # Formation simple
            'casual_dining': 1.0,   # Formation standard
            'fine_dining': 1.6,     # Formation complexe
            'cloud_kitchen': 0.9,   # Formation technique
            'food_truck': 0.7       # Formation basique
        }
        return factors.get(theme, 1.0)
    
    def _get_size_training_factor(self, revenue_size: str) -> float:
        """Facteurs de formation selon la taille"""
        factors = {
            'small': 1.1,      # Plus de formation individuelle
            'medium': 1.0,     # Formation standard
            'large': 0.9,      # Économies d'échelle
            'enterprise': 0.8   # Formation structurée
        }
        return factors.get(revenue_size, 1.0)
    
    def _get_theme_salary_multiplier(self, theme: str) -> float:
        """Multiplicateurs salariaux par thème"""
        multipliers = {
            'fast_food': 0.85,
            'casual_dining': 1.0,
            'fine_dining': 1.45,
            'cloud_kitchen': 0.95,
            'food_truck': 0.80
        }
        return multipliers.get(theme, 1.0)
    
    def _get_revenue_salary_multiplier(self, revenue_size: str) -> float:
        """Multiplicateurs salariaux par taille de revenus"""
        multipliers = {
            'small': 0.88,
            'medium': 1.0,
            'large': 1.18,
            'enterprise': 1.35
        }
        return multipliers.get(revenue_size, 1.0)
    
    def _calculate_location_multiplier(self, daily_capacity: int, kitchen_size: float) -> float:
        """Calcule le multiplicateur de localisation basé sur la densité"""
        # Densité = capacité / taille cuisine (proxy pour zone urbaine)
        if kitchen_size > 0:
            density = daily_capacity / kitchen_size
            # Plus dense = zone urbaine = salaires plus élevés
            return min(1.3, 0.9 + density * 0.01)
        return 1.0
    
    def _calculate_performance_multiplier(self, inputs: RestaurantInputs) -> float:
        """Calcule le multiplicateur de performance opérationnelle"""
        # Ratio efficacité = capacité / nombre d'employés
        if inputs.staff_count > 0:
            efficiency_ratio = inputs.daily_capacity / inputs.staff_count
            # Meilleure efficacité = bonus salarial pour retenir le personnel
            return min(1.2, max(0.9, 0.95 + efficiency_ratio * 0.005))
        return 1.0
    
    def calculate_total_staff_costs(self, inputs: RestaurantInputs) -> List[CostBreakdown]:
        """Calcule tous les coûts de personnel avec formules pondérées"""
        return [
            self.calculate_training_cost(inputs),
            self.calculate_salary_costs(inputs)
        ]


class EquipmentCostCalculator:
    """Calculateur des coûts d'équipement"""
    
    def __init__(self, db_manager: DatabaseManager):
        self.db_manager = db_manager
        self.daos = get_all_daos(db_manager)
    
    def calculate_depreciation_cost(self, inputs: RestaurantInputs) -> CostBreakdown:
        """
        Calcule la dépréciation annuelle avec formules pondérées avancées
        
        Formule: equipment_value × condition_factor × depreciation_rate × usage_factor × theme_factor
        """
        # Facteurs de condition améliorés
        condition_factor = self._get_enhanced_condition_factor(inputs.equipment_condition, inputs.equipment_age_years)
        
        # Taux de dépréciation depuis la DB ou calculé
        base_depreciation_rate = self.daos['cost_factors'].get_factor_value(
            'depreciation_rate',
            inputs.restaurant_theme
        ) or 0.20
        
        # Facteur d'usage intensif
        usage_factor = self._calculate_usage_intensity_factor(inputs)
        
        # Facteur thématique (certains thèmes usent plus l'équipement)
        theme_factor = self.daos['cost_factors'].get_factor_value(
            'equipment_theme_factor',
            inputs.restaurant_theme
        ) or self._get_theme_equipment_factor(inputs.restaurant_theme)
        
        # Facteur de technologie (plus moderne = dépréciation différente)
        tech_factor = self._calculate_technology_factor(inputs.equipment_age_years)
        
        # Calcul pondéré complet
        effective_depreciation_rate = base_depreciation_rate * usage_factor * theme_factor * tech_factor
        annual_depreciation = inputs.equipment_value * condition_factor * effective_depreciation_rate
        
        formula = f"{inputs.equipment_value:,.0f} × {condition_factor:.3f} × {effective_depreciation_rate:.3f}"
        
        details = {
            'equipment_value': inputs.equipment_value,
            'equipment_condition': inputs.equipment_condition,
            'equipment_age': inputs.equipment_age_years,
            'condition_factor': condition_factor,
            'base_depreciation_rate': base_depreciation_rate,
            'usage_factor': usage_factor,
            'theme_factor': theme_factor,
            'tech_factor': tech_factor,
            'effective_depreciation_rate': effective_depreciation_rate
        }
        
        return CostBreakdown(
            category='equipment',
            subcategory='depreciation_annuelle',
            amount=annual_depreciation,
            formula=formula,
            details=details
        )
    
    def calculate_maintenance_cost(self, inputs: RestaurantInputs) -> CostBreakdown:
        """
        Calcule les coûts de maintenance avec pondération dynamique
        
        Formule: equipment_value × maintenance_rate × age_factor × usage_factor × complexity_factor × preventive_factor
        """
        # Taux de maintenance de base depuis la DB
        base_maintenance_rate = self.daos['cost_factors'].get_factor_value(
            'maintenance_rate',
            inputs.restaurant_theme
        ) or 0.08
        
        # Facteur d'âge non-linéaire (accélération après 5 ans)
        age_factor = self._calculate_age_maintenance_factor(inputs.equipment_age_years)
        
        # Facteur d'usage intensif amélioré
        usage_factor = self._calculate_maintenance_usage_factor(inputs)
        
        # Facteur de complexité selon le thème
        complexity_factor = self.daos['cost_factors'].get_factor_value(
            'maintenance_complexity_factor',
            inputs.restaurant_theme
        ) or self._get_maintenance_complexity_factor(inputs.restaurant_theme)
        
        # Facteur de maintenance préventive (meilleur état = moins de réparations)
        preventive_factor = self._get_preventive_maintenance_factor(inputs.equipment_condition)
        
        # Facteur de taille (grandes opérations = contrats de maintenance)
        scale_factor = self._get_maintenance_scale_factor(inputs.revenue_size)
        
        # Calcul pondéré complet
        effective_maintenance_rate = (
            base_maintenance_rate * 
            complexity_factor * 
            preventive_factor * 
            scale_factor
        )
        
        annual_maintenance = (
            inputs.equipment_value * 
            effective_maintenance_rate * 
            age_factor * 
            usage_factor
        )
        
        formula = f"{inputs.equipment_value:,.0f} × {effective_maintenance_rate:.3f} × {age_factor:.2f} × {usage_factor:.2f}"
        
        details = {
            'equipment_value': inputs.equipment_value,
            'base_maintenance_rate': base_maintenance_rate,
            'age_factor': age_factor,
            'usage_factor': usage_factor,
            'complexity_factor': complexity_factor,
            'preventive_factor': preventive_factor,
            'scale_factor': scale_factor,
            'effective_maintenance_rate': effective_maintenance_rate,
            'daily_capacity': inputs.daily_capacity
        }
        
        return CostBreakdown(
            category='equipment',
            subcategory='maintenance_annuelle',
            amount=annual_maintenance,
            formula=formula,
            details=details
        )
    
    def _get_enhanced_condition_factor(self, condition: str, age_years: int) -> float:
        """Facteur de condition amélioré tenant compte de l'âge"""
        base_factors = {
            'excellent': 0.03,
            'good': 0.12,
            'fair': 0.22,
            'poor': 0.35
        }
        base_factor = base_factors.get(condition, 0.20)
        
        # Ajustement selon l'âge
        age_adjustment = min(0.15, age_years * 0.01)  # +1% par an, max 15%
        return base_factor + age_adjustment
    
    def _calculate_usage_intensity_factor(self, inputs: RestaurantInputs) -> float:
        """Calcule l'intensité d'usage de l'équipement"""
        # Ratio capacité/taille cuisine comme proxy d'intensité
        if inputs.kitchen_size_sqm > 0:
            intensity = inputs.daily_capacity / inputs.kitchen_size_sqm
            return min(1.5, max(0.8, 0.9 + intensity * 0.02))
        return 1.0
    
    def _get_theme_equipment_factor(self, theme: str) -> float:
        """Facteurs d'usure par thème"""
        factors = {
            'fast_food': 1.3,      # Usage intensif
            'casual_dining': 1.0,   # Usage normal
            'fine_dining': 0.8,     # Usage plus soigné
            'cloud_kitchen': 1.2,   # Usage continu
            'food_truck': 1.4       # Conditions difficiles
        }
        return factors.get(theme, 1.0)
    
    def _calculate_technology_factor(self, age_years: int) -> float:
        """Facteur technologique (plus vieux = dépréciation accélérée)"""
        if age_years <= 2:
            return 0.9  # Équipement récent
        elif age_years <= 5:
            return 1.0  # Équipement standard
        else:
            return min(1.4, 1.0 + (age_years - 5) * 0.08)  # Obsolétude
    
    def _calculate_age_maintenance_factor(self, age_years: int) -> float:
        """Facteur d'âge non-linéaire pour maintenance"""
        if age_years <= 3:
            return 1.0 + age_years * 0.03  # Croissance lente
        else:
            return 1.09 + (age_years - 3) * 0.08  # Accélération
    
    def _calculate_maintenance_usage_factor(self, inputs: RestaurantInputs) -> float:
        """Facteur d'usage pour maintenance"""
        # Combinaison capacité et heures d'opération
        base_usage = min(2.0, inputs.daily_capacity / 100)
        
        # Ajustement selon le nombre d'employés (proxy pour heures d'opération)
        if inputs.staff_count > 0:
            hours_proxy = min(1.5, inputs.staff_count / 10)
            return base_usage * hours_proxy
        return base_usage
    
    def _get_maintenance_complexity_factor(self, theme: str) -> float:
        """Complexité de maintenance par thème"""
        factors = {
            'fast_food': 0.9,       # Équipement standardisé
            'casual_dining': 1.0,    # Complexité moyenne
            'fine_dining': 1.3,      # Équipement spécialisé
            'cloud_kitchen': 1.1,    # Technologie avancée
            'food_truck': 1.2        # Accès difficile
        }
        return factors.get(theme, 1.0)
    
    def _get_preventive_maintenance_factor(self, condition: str) -> float:
        """Facteur de maintenance préventive"""
        factors = {
            'excellent': 0.85,  # Moins de réparations
            'good': 0.95,
            'fair': 1.05,
            'poor': 1.25        # Plus de réparations
        }
        return factors.get(condition, 1.0)
    
    def _get_maintenance_scale_factor(self, revenue_size: str) -> float:
        """Facteur d'échelle pour maintenance"""
        factors = {
            'small': 1.1,       # Coûts unitaires plus élevés
            'medium': 1.0,
            'large': 0.9,       # Contrats de maintenance
            'enterprise': 0.8   # Économies d'échelle
        }
        return factors.get(revenue_size, 1.0)
    
    def calculate_total_equipment_costs(self, inputs: RestaurantInputs) -> List[CostBreakdown]:
        """Calcule tous les coûts d'équipement avec formules pondérées"""
        costs = []
        
        if inputs.equipment_value > 0:
            costs.extend([
                self.calculate_depreciation_cost(inputs),
                self.calculate_maintenance_cost(inputs)
            ])
        
        return costs


class LocationCostCalculator:
    """Calculateur des coûts immobiliers"""
    
    def __init__(self, db_manager: DatabaseManager):
        self.db_manager = db_manager
        self.daos = get_all_daos(db_manager)
    
    def calculate_rent_cost(self, inputs: RestaurantInputs) -> CostBreakdown:
        """
        Calcule le coût de loyer annuel
        
        Formule: kitchen_size × rent_per_sqm × 12 × location_factor
        """
        if inputs.location_rent_sqm <= 0:
            return CostBreakdown(
                category='location',
                subcategory='loyer_annuel',
                amount=0.0,
                formula="Pas de loyer fixe",
                details={'note': 'Food truck ou propriété'}
            )
        
        # Facteur de localisation selon le thème
        location_factor = self.daos['cost_factors'].get_factor_value(
            'rent_factor',
            inputs.restaurant_theme
        ) or 1.0
        
        annual_rent = (
            inputs.kitchen_size_sqm * 
            inputs.location_rent_sqm * 
            12 * 
            location_factor
        )
        
        formula = f"{inputs.kitchen_size_sqm} m² × {inputs.location_rent_sqm} CAD$/m² × 12 mois × {location_factor}"
        
        details = {
            'kitchen_size_sqm': inputs.kitchen_size_sqm,
            'rent_per_sqm': inputs.location_rent_sqm,
            'location_factor': location_factor,
            'theme': inputs.restaurant_theme
        }
        
        return CostBreakdown(
            category='location',
            subcategory='loyer_annuel',
            amount=annual_rent,
            formula=formula,
            details=details
        )
    
    def calculate_utilities_cost(self, inputs: RestaurantInputs) -> CostBreakdown:
        """
        Calcule les coûts d'utilities annuels
        
        Formule: kitchen_size × base_utility_rate × capacity_factor
        """
        base_utility_rate = 15.0  # CAD$/m²/mois pour utilities
        
        # Facteur selon la capacité (plus de couverts = plus d'utilities)
        capacity_factor = 1.0 + (inputs.daily_capacity / 1000)  # +0.1 par 100 couverts
        
        annual_utilities = (
            inputs.kitchen_size_sqm * 
            base_utility_rate * 
            12 * 
            capacity_factor
        )
        
        formula = f"{inputs.kitchen_size_sqm} m² × {base_utility_rate} CAD$/m²/mois × 12 × {capacity_factor:.2f}"
        
        details = {
            'kitchen_size_sqm': inputs.kitchen_size_sqm,
            'base_utility_rate': base_utility_rate,
            'capacity_factor': capacity_factor,
            'daily_capacity': inputs.daily_capacity
        }
        
        return CostBreakdown(
            category='location',
            subcategory='utilities_annuelles',
            amount=annual_utilities,
            formula=formula,
            details=details
        )
    
    def calculate_total_location_costs(self, inputs: RestaurantInputs) -> List[CostBreakdown]:
        """Calcule tous les coûts immobiliers"""
        return [
            self.calculate_rent_cost(inputs),
            self.calculate_utilities_cost(inputs)
        ]


class OperationalCostCalculator:
    """Calculateur des coûts opérationnels"""
    
    def __init__(self, db_manager: DatabaseManager):
        self.db_manager = db_manager
        self.daos = get_all_daos(db_manager)
    
    def calculate_food_cost(self, inputs: RestaurantInputs) -> CostBreakdown:
        """
        Calcule les coûts de matières premières avec pondération dynamique
        
        Formule: daily_capacity × days_per_year × cost_per_cover × efficiency_factor × quality_factor × volume_factor × seasonal_factor
        """
        # Jours d'opération ajustés selon le thème
        days_per_year = self._get_operating_days_per_year(inputs.restaurant_theme)
        
        # Coût de base par couvert depuis la DB ou calculé
        base_cost_per_cover = self.daos['cost_factors'].get_factor_value(
            'food_cost_per_cover',
            inputs.restaurant_theme
        ) or self._get_theme_food_cost_per_cover(inputs.restaurant_theme)
        
        # Facteur d'efficacité opérationnelle
        efficiency_factor = self.daos['cost_factors'].get_factor_value(
            'food_efficiency_factor',
            revenue_size=inputs.revenue_size
        ) or self._get_food_efficiency_factor(inputs.revenue_size)
        
        # Facteur de qualité des ingrédients
        quality_factor = self._get_ingredient_quality_factor(inputs.restaurant_theme)
        
        # Facteur de volume (achats en gros)
        volume_factor = self._calculate_volume_discount_factor(inputs)
        
        # Facteur saisonnier (variations des prix)
        seasonal_factor = self.daos['cost_factors'].get_factor_value(
            'seasonal_price_factor'
        ) or 1.05  # +5% pour variations saisonnières
        
        # Facteur de gaspillage
        waste_factor = self._calculate_waste_factor(inputs)
        
        # Calcul pondéré complet
        effective_cost_per_cover = (
            base_cost_per_cover * 
            quality_factor * 
            seasonal_factor * 
            waste_factor
        )
        
        annual_food_cost = (
            inputs.daily_capacity * 
            days_per_year * 
            effective_cost_per_cover * 
            efficiency_factor * 
            volume_factor
        )
        
        formula = f"{inputs.daily_capacity} × {days_per_year} × {effective_cost_per_cover:.2f} × {efficiency_factor:.2f} × {volume_factor:.2f}"
        
        details = {
            'daily_capacity': inputs.daily_capacity,
            'days_per_year': days_per_year,
            'base_cost_per_cover': base_cost_per_cover,
            'effective_cost_per_cover': effective_cost_per_cover,
            'efficiency_factor': efficiency_factor,
            'quality_factor': quality_factor,
            'volume_factor': volume_factor,
            'seasonal_factor': seasonal_factor,
            'waste_factor': waste_factor,
            'theme': inputs.restaurant_theme,
            'revenue_size': inputs.revenue_size
        }
        
        return CostBreakdown(
            category='operations',
            subcategory='matieres_premieres',
            amount=annual_food_cost,
            formula=formula,
            details=details
        )
    
    def calculate_marketing_cost(self, inputs: RestaurantInputs) -> CostBreakdown:
        """
        Calcule les coûts marketing avec stratégie pondérée
        
        Formule: base_marketing × theme_factor × competition_factor × digital_factor × maturity_factor
        """
        # Budget marketing de base depuis la DB ou calculé
        base_marketing_budget = self.daos['cost_factors'].get_factor_value(
            'marketing_budget_base',
            revenue_size=inputs.revenue_size
        ) or self._get_base_marketing_budget(inputs.revenue_size)
        
        # Facteur thématique avancé
        theme_factor = self.daos['cost_factors'].get_factor_value(
            'marketing_theme_factor',
            inputs.restaurant_theme
        ) or self._get_marketing_theme_factor(inputs.restaurant_theme)
        
        # Facteur de compétition (basé sur la densité du marché)
        competition_factor = self._calculate_competition_factor(inputs)
        
        # Facteur digital (importance du marketing en ligne)
        digital_factor = self._get_digital_marketing_factor(inputs.restaurant_theme)
        
        # Facteur de maturité (nouveaux restaurants = plus de marketing)
        maturity_factor = self._calculate_business_maturity_factor(inputs)
        
        # Facteur de localisation (zones urbaines = plus cher)
        location_factor = self._calculate_marketing_location_factor(inputs)
        
        # Calcul pondéré sophistiqué
        effective_marketing_rate = (
            theme_factor * 
            competition_factor * 
            digital_factor * 
            maturity_factor * 
            location_factor
        )
        
        annual_marketing = base_marketing_budget * effective_marketing_rate
        
        formula = f"{base_marketing_budget:,.0f} × {effective_marketing_rate:.3f}"
        
        details = {
            'base_marketing_budget': base_marketing_budget,
            'theme_factor': theme_factor,
            'competition_factor': competition_factor,
            'digital_factor': digital_factor,
            'maturity_factor': maturity_factor,
            'location_factor': location_factor,
            'effective_marketing_rate': effective_marketing_rate,
            'revenue_size': inputs.revenue_size,
            'theme': inputs.restaurant_theme
        }
        
        return CostBreakdown(
            category='operations',
            subcategory='marketing_annuel',
            amount=annual_marketing,
            formula=formula,
            details=details
        )
    
    def _get_operating_days_per_year(self, theme: str) -> int:
        """Jours d'opération par thème"""
        days = {
            'fast_food': 360,      # Presque tous les jours
            'casual_dining': 300,   # Fermé certains jours
            'fine_dining': 280,     # Fermé plus souvent
            'cloud_kitchen': 350,   # Opération continue
            'food_truck': 250       # Dépendant de la saison/événements
        }
        return days.get(theme, 300)
    
    def _get_theme_food_cost_per_cover(self, theme: str) -> float:
        """Coût alimentaire par couvert selon le thème"""
        costs = {
            'fast_food': 3.2,
            'casual_dining': 7.5,
            'fine_dining': 22.0,
            'cloud_kitchen': 4.8,
            'food_truck': 3.8
        }
        return costs.get(theme, 8.0)
    
    def _get_food_efficiency_factor(self, revenue_size: str) -> float:
        """Facteur d'efficacité alimentaire par taille"""
        factors = {
            'small': 1.05,      # Moins d'efficacité
            'medium': 1.0,
            'large': 0.92,      # Achats en volume
            'enterprise': 0.85   # Optimisation maximale
        }
        return factors.get(revenue_size, 1.0)
    
    def _get_ingredient_quality_factor(self, theme: str) -> float:
        """Facteur de qualité des ingrédients"""
        factors = {
            'fast_food': 0.85,      # Ingrédients économiques
            'casual_dining': 1.0,    # Qualité standard
            'fine_dining': 1.6,      # Ingrédients premium
            'cloud_kitchen': 0.95,   # Optimisé pour livraison
            'food_truck': 0.9        # Contraintes d'espace
        }
        return factors.get(theme, 1.0)
    
    def _calculate_volume_discount_factor(self, inputs: RestaurantInputs) -> float:
        """Facteur de remise sur volume"""
        # Basé sur la capacité quotidienne comme proxy du volume d'achat
        if inputs.daily_capacity >= 500:
            return 0.88  # Grosse remise
        elif inputs.daily_capacity >= 200:
            return 0.93  # Remise moyenne
        elif inputs.daily_capacity >= 100:
            return 0.97  # Petite remise
        else:
            return 1.02  # Surcoût petit volume
    
    def _calculate_waste_factor(self, inputs: RestaurantInputs) -> float:
        """Facteur de gaspillage alimentaire"""
        # Gaspillage selon l'efficacité opérationnelle
        if inputs.staff_count > 0:
            efficiency_ratio = inputs.daily_capacity / inputs.staff_count
            # Meilleure efficacité = moins de gaspillage
            return max(1.02, min(1.15, 1.25 - efficiency_ratio * 0.01))
        return 1.08  # Gaspillage moyen de 8%
    
    def _get_base_marketing_budget(self, revenue_size: str) -> float:
        """Budget marketing de base par taille"""
        budgets = {
            'small': 12000,
            'medium': 32000,
            'large': 72000,
            'enterprise': 145000
        }
        return budgets.get(revenue_size, 32000)
    
    def _get_marketing_theme_factor(self, theme: str) -> float:
        """Facteur marketing par thème"""
        factors = {
            'fast_food': 1.25,      # Marketing intensif
            'casual_dining': 1.0,    # Marketing standard
            'fine_dining': 0.75,     # Bouche-à-oreille
            'cloud_kitchen': 1.6,    # Marketing digital crucial
            'food_truck': 0.55       # Marketing local/social
        }
        return factors.get(theme, 1.0)
    
    def _calculate_competition_factor(self, inputs: RestaurantInputs) -> float:
        """Facteur de compétition basé sur la densité"""
        # Proxy: plus de capacité dans petit espace = marché compétitif
        if inputs.kitchen_size_sqm > 0:
            market_density = inputs.daily_capacity / inputs.kitchen_size_sqm
            return min(1.4, max(0.9, 1.0 + market_density * 0.008))
        return 1.0
    
    def _get_digital_marketing_factor(self, theme: str) -> float:
        """Importance du marketing digital"""
        factors = {
            'fast_food': 1.1,
            'casual_dining': 1.0,
            'fine_dining': 0.9,
            'cloud_kitchen': 1.3,    # Crucial pour visibilité
            'food_truck': 1.2        # Réseaux sociaux importants
        }
        return factors.get(theme, 1.0)
    
    def _calculate_business_maturity_factor(self, inputs: RestaurantInputs) -> float:
        """Facteur de maturité du business"""
        # Proxy: plus d'employés = business plus mature = moins de marketing d'acquisition
        if inputs.staff_count <= 3:
            return 1.3  # Nouveau business
        elif inputs.staff_count <= 8:
            return 1.1  # En croissance
        else:
            return 0.95  # Mature
    
    def _calculate_marketing_location_factor(self, inputs: RestaurantInputs) -> float:
        """Facteur de localisation pour marketing"""
        # Zones denses = marketing plus cher mais plus efficace
        if inputs.kitchen_size_sqm > 0:
            density = inputs.daily_capacity / inputs.kitchen_size_sqm
            return min(1.25, max(0.85, 0.9 + density * 0.007))
        return 1.0
    
    def calculate_total_operational_costs(self, inputs: RestaurantInputs) -> List[CostBreakdown]:
        """Calcule tous les coûts opérationnels avec formules pondérées"""
        return [
            self.calculate_food_cost(inputs),
            self.calculate_marketing_cost(inputs)
        ]


class CostCalculator:
    """Calculateur principal qui orchestre tous les calculs"""
    
    def __init__(self, db_manager: DatabaseManager = None):
        self.db_manager = db_manager or DatabaseManager()
        self.staff_calc = StaffCostCalculator(self.db_manager)
        self.equipment_calc = EquipmentCostCalculator(self.db_manager)
        self.location_calc = LocationCostCalculator(self.db_manager)
        self.operational_calc = OperationalCostCalculator(self.db_manager)
        self.daos = get_all_daos(self.db_manager)
    
    def calculate_all_costs(self, inputs: RestaurantInputs, session_id: int = None) -> TotalCostSummary:
        """
        Calcule tous les coûts pour un restaurant
        
        Args:
            inputs: Paramètres d'entrée du restaurant
            session_id: ID de session (optionnel)
            
        Returns:
            Résumé complet des coûts
        """
        try:
            # S'assurer que la connexion DB est active
            if not self.db_manager.connection:
                self.db_manager.connect()
            
            # Calculer tous les coûts par catégorie
            staff_breakdowns = self.staff_calc.calculate_total_staff_costs(inputs)
            equipment_breakdowns = self.equipment_calc.calculate_total_equipment_costs(inputs)
            location_breakdowns = self.location_calc.calculate_total_location_costs(inputs)
            operational_breakdowns = self.operational_calc.calculate_total_operational_costs(inputs)
            
            # Agréger tous les breakdowns
            all_breakdowns = (
                staff_breakdowns + 
                equipment_breakdowns + 
                location_breakdowns + 
                operational_breakdowns
            )
            
            # Calculer les totaux par catégorie
            staff_total = sum(bd.amount for bd in staff_breakdowns)
            equipment_total = sum(bd.amount for bd in equipment_breakdowns)
            location_total = sum(bd.amount for bd in location_breakdowns)
            operational_total = sum(bd.amount for bd in operational_breakdowns)
            
            total_cost = staff_total + equipment_total + location_total + operational_total
            
            # Sauvegarder les résultats en DB si session_id fourni
            if session_id:
                self._save_calculation_results(session_id, all_breakdowns)
            
            logger.info(f"Calculs terminés pour {inputs.session_name}: {total_cost:,.2f} CAD$")
            
            return TotalCostSummary(
                session_id=session_id or 0,
                session_name=inputs.session_name,
                staff_costs=staff_total,
                equipment_costs=equipment_total,
                location_costs=location_total,
                operational_costs=operational_total,
                total_cost=total_cost,
                cost_breakdowns=all_breakdowns
            )
            
        except Exception as e:
            logger.error(f"Erreur calcul des coûts: {e}")
            raise
    
    def _save_calculation_results(self, session_id: int, breakdowns: List[CostBreakdown]):
        """Sauvegarde les résultats de calcul en base de données"""
        try:
            for breakdown in breakdowns:
                self.daos['results'].save_calculation_result(
                    session_id=session_id,
                    category=breakdown.category,
                    subcategory=breakdown.subcategory,
                    amount=breakdown.amount,
                    formula=breakdown.formula
                )
            logger.info(f"Résultats sauvegardés: {len(breakdowns)} calculs")
        except Exception as e:
            logger.error(f"Erreur sauvegarde résultats: {e}")


if __name__ == "__main__":
    # Test du calculateur
    print("🧪 Test du moteur de calcul")
    
    from input_handler import create_sample_inputs
    
    try:
        # Créer des entrées de test
        sample_inputs = create_sample_inputs()
        print(f"✅ Entrées créées: {sample_inputs.session_name}")
        
        # Calculer les coûts
        calculator = CostCalculator()
        calculator.db_manager.connect()
        
        summary = calculator.calculate_all_costs(sample_inputs)
        
        print(f"\n📊 Résumé des coûts pour {summary.session_name}:")
        print(f"  Personnel: {summary.staff_costs:,.2f} CAD$")
        print(f"  Équipement: {summary.equipment_costs:,.2f} CAD$")
        print(f"  Immobilier: {summary.location_costs:,.2f} CAD$")
        print(f"  Opérationnel: {summary.operational_costs:,.2f} CAD$")
        print(f"  TOTAL: {summary.total_cost:,.2f} CAD$")
        
        print(f"\n📋 Détail des calculs ({len(summary.cost_breakdowns)} éléments):")
        for breakdown in summary.cost_breakdowns:
            print(f"  {breakdown.category}/{breakdown.subcategory}: {breakdown.amount:,.2f} CAD$")
        
        calculator.db_manager.disconnect()
        print("✅ Test terminé avec succès")
        
    except Exception as e:
        print(f"❌ Erreur de test: {e}")
