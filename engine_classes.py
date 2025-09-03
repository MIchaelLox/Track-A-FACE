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
        Calcule le coût de formation du personnel
        
        Formule: training_hours × staff_count × hourly_rate × complexity_factor
        """
        # Récupérer les facteurs de la DB
        hourly_rate = self.daos['cost_factors'].get_factor_value(
            'training_rate_per_hour', 
            inputs.restaurant_theme
        ) or 30.0
        
        complexity_factor = self.daos['cost_factors'].get_factor_value(
            'complexity_factor',
            inputs.restaurant_theme
        ) or 1.0
        
        # Calcul principal
        training_cost = (
            inputs.training_hours_needed * 
            inputs.staff_count * 
            hourly_rate * 
            complexity_factor
        )
        
        formula = f"{inputs.training_hours_needed}h × {inputs.staff_count} employés × {hourly_rate} CAD$/h × {complexity_factor}"
        
        details = {
            'training_hours': inputs.training_hours_needed,
            'staff_count': inputs.staff_count,
            'hourly_rate': hourly_rate,
            'complexity_factor': complexity_factor,
            'theme': inputs.restaurant_theme
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
        Calcule les coûts salariaux annuels estimés
        
        Formule: staff_count × base_salary × theme_multiplier × revenue_multiplier
        """
        base_annual_salary = BUSINESS_CONSTANTS["default_staff_hourly_rate"] * 40 * 52  # 40h/semaine, 52 semaines
        
        # Facteurs d'ajustement
        theme_multipliers = {
            'fast_food': 0.9,
            'casual_dining': 1.0,
            'fine_dining': 1.4,
            'cloud_kitchen': 0.95,
            'food_truck': 0.85
        }
        
        revenue_multipliers = {
            'small': 0.9,
            'medium': 1.0,
            'large': 1.15,
            'enterprise': 1.3
        }
        
        theme_mult = theme_multipliers.get(inputs.restaurant_theme, 1.0)
        revenue_mult = revenue_multipliers.get(inputs.revenue_size, 1.0)
        
        annual_salary_cost = (
            inputs.staff_count * 
            base_annual_salary * 
            theme_mult * 
            revenue_mult
        )
        
        formula = f"{inputs.staff_count} × {base_annual_salary:,.0f} × {theme_mult} × {revenue_mult}"
        
        details = {
            'staff_count': inputs.staff_count,
            'base_annual_salary': base_annual_salary,
            'theme_multiplier': theme_mult,
            'revenue_multiplier': revenue_mult
        }
        
        return CostBreakdown(
            category='staff',
            subcategory='salaires_annuels',
            amount=annual_salary_cost,
            formula=formula,
            details=details
        )
    
    def calculate_total_staff_costs(self, inputs: RestaurantInputs) -> List[CostBreakdown]:
        """Calcule tous les coûts de personnel"""
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
        Calcule la dépréciation annuelle de l'équipement
        
        Formule: equipment_value × condition_factor × depreciation_rate
        """
        # Facteurs de condition depuis la DB
        condition_factors = {
            'excellent': 0.05,
            'good': 0.15,
            'fair': 0.25,
            'poor': 0.40
        }
        
        condition_factor = condition_factors.get(inputs.equipment_condition, 0.20)
        depreciation_rate = self.daos['cost_factors'].get_factor_value('depreciation_rate') or 0.20
        
        annual_depreciation = inputs.equipment_value * condition_factor * depreciation_rate
        
        formula = f"{inputs.equipment_value:,.0f} CAD$ × {condition_factor} × {depreciation_rate}"
        
        details = {
            'equipment_value': inputs.equipment_value,
            'equipment_condition': inputs.equipment_condition,
            'condition_factor': condition_factor,
            'depreciation_rate': depreciation_rate,
            'equipment_age': inputs.equipment_age_years
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
        Calcule les coûts de maintenance annuels
        
        Formule: equipment_value × maintenance_rate × age_factor × usage_factor
        """
        base_maintenance_rate = 0.08  # 8% de la valeur par an
        
        # Facteur d'âge (plus vieux = plus de maintenance)
        age_factor = 1.0 + (inputs.equipment_age_years * 0.05)  # +5% par année
        
        # Facteur d'utilisation selon la capacité
        usage_factor = min(2.0, inputs.daily_capacity / 100)  # Normalisé sur 100 couverts
        
        annual_maintenance = (
            inputs.equipment_value * 
            base_maintenance_rate * 
            age_factor * 
            usage_factor
        )
        
        formula = f"{inputs.equipment_value:,.0f} × {base_maintenance_rate} × {age_factor:.2f} × {usage_factor:.2f}"
        
        details = {
            'equipment_value': inputs.equipment_value,
            'base_maintenance_rate': base_maintenance_rate,
            'age_factor': age_factor,
            'usage_factor': usage_factor,
            'daily_capacity': inputs.daily_capacity
        }
        
        return CostBreakdown(
            category='equipment',
            subcategory='maintenance_annuelle',
            amount=annual_maintenance,
            formula=formula,
            details=details
        )
    
    def calculate_total_equipment_costs(self, inputs: RestaurantInputs) -> List[CostBreakdown]:
        """Calcule tous les coûts d'équipement"""
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
        Calcule les coûts de matières premières annuels
        
        Formule: daily_capacity × days_per_year × cost_per_cover × theme_factor
        """
        days_per_year = 300  # Jours d'opération typiques
        
        # Coût par couvert selon le thème
        cost_per_cover = {
            'fast_food': 3.5,
            'casual_dining': 8.0,
            'fine_dining': 18.0,
            'cloud_kitchen': 4.5,
            'food_truck': 4.0
        }
        
        cover_cost = cost_per_cover.get(inputs.restaurant_theme, 8.0)
        
        # Facteur d'efficacité selon la taille
        efficiency_factor = self.daos['cost_factors'].get_factor_value(
            'efficiency_factor',
            revenue_size=inputs.revenue_size
        ) or 1.0
        
        annual_food_cost = (
            inputs.daily_capacity * 
            days_per_year * 
            cover_cost * 
            efficiency_factor
        )
        
        formula = f"{inputs.daily_capacity} × {days_per_year} × {cover_cost} CAD$ × {efficiency_factor}"
        
        details = {
            'daily_capacity': inputs.daily_capacity,
            'days_per_year': days_per_year,
            'cost_per_cover': cover_cost,
            'efficiency_factor': efficiency_factor,
            'theme': inputs.restaurant_theme
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
        Calcule les coûts marketing annuels
        
        Formule: base_marketing × revenue_factor × theme_factor
        """
        # Budget marketing de base selon la taille
        base_marketing = {
            'small': 15000,
            'medium': 35000,
            'large': 75000,
            'enterprise': 150000
        }
        
        marketing_budget = base_marketing.get(inputs.revenue_size, 35000)
        
        # Ajustement selon le thème
        theme_factors = {
            'fast_food': 1.2,  # Plus de marketing
            'casual_dining': 1.0,
            'fine_dining': 0.8,  # Moins de marketing, plus de bouche-à-oreille
            'cloud_kitchen': 1.5,  # Marketing digital important
            'food_truck': 0.6   # Marketing local/social
        }
        
        theme_factor = theme_factors.get(inputs.restaurant_theme, 1.0)
        
        annual_marketing = marketing_budget * theme_factor
        
        formula = f"{marketing_budget:,.0f} CAD$ × {theme_factor}"
        
        details = {
            'base_marketing_budget': marketing_budget,
            'theme_factor': theme_factor,
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
    
    def calculate_total_operational_costs(self, inputs: RestaurantInputs) -> List[CostBreakdown]:
        """Calcule tous les coûts opérationnels"""
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
