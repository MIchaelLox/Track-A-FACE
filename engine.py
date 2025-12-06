# ==========================
# /engine.py
# ==========================

"""
Module d'orchestration principal pour Track-A-FACE
Coordonne les calculs de coûts et fournit une API unifiée
"""

import logging
from typing import Dict, List, Optional, Tuple, Any
from datetime import datetime
from pathlib import Path

from engine_classes import CostCalculator, TotalCostSummary, CostBreakdown
from input_handler import RestaurantInputs, InputHandler, create_sample_inputs
from sql import DatabaseManager, DatabaseInitializer
from config import DATABASE_CONFIG

logger = logging.getLogger(__name__)


class CalculationEngine:
    """
    Moteur principal d'orchestration des calculs Track-A-FACE
    Coordonne tous les modules pour fournir une interface unifiée
    """
    
    def __init__(self, db_path: Optional[Path] = None):
        """
        Initialise le moteur de calcul
        
        Args:
            db_path: Chemin vers la base de données (optionnel)
        """
        self.db_manager = DatabaseManager(db_path)
        self.cost_calculator = CostCalculator(self.db_manager)
        self.input_handler = InputHandler()
        self._ensure_database_ready()
        
        logger.info("Moteur de calcul Track-A-FACE initialisé")

    def _ensure_database_ready(self):
        """Vérifie que la base est initialisée et contient les facteurs nécessaires"""
        try:
            # Vérifier si la table cost_factors existe et contient des données
            self.db_manager.connect()
            connection = self.db_manager.connection
            cursor = connection.cursor()
            cursor.execute("SELECT COUNT(*) FROM cost_factors")
            factor_count = cursor.fetchone()[0]

            if factor_count == 0:
                logger.warning("Base de données vide, initialisation.")
                db_init = DatabaseInitializer()
                db_init.db_manager = self.db_manager
                db_init.initialize_database()

            logger.info(f"Base de données prête avec {factor_count} facteurs de coût")

        except Exception as e:
            logger.error(f"Erreur d'initialisation de la base: {e}")

            # Fermer proprement la connexion ouverte avant de réinitialiser
            if self.db_manager.connection:
                self.db_manager.disconnect()

            # Initialiser une nouvelle base
            db_init = DatabaseInitializer()
            db_init.db_manager = self.db_manager
            db_init.initialize_database()

        finally:
            # S'assurer qu'aucune connexion ne reste ouverte
            if self.db_manager.connection:
                self.db_manager.disconnect()

    def calculate_restaurant_costs(self, inputs: RestaurantInputs) -> TotalCostSummary:
        """
        Calcule tous les coûts pour un restaurant
        
        Args:
            inputs: Paramètres d'entrée du restaurant
            
        Returns:
            Résumé complet des coûts calculés
        """
        logger.info(f"Début du calcul pour: {inputs.session_name}")
        
        try:
            # Connecter à la base
            self.db_manager.connect()
            
            # Exécuter les calculs
            summary = self.cost_calculator.calculate_all_costs(inputs)
            
            # Déconnecter
            self.db_manager.disconnect()
            
            logger.info(f"Calculs terminés - Total: {summary.total_cost:,.2f} CAD$")
            return summary
            
        except Exception as e:
            logger.error(f"Erreur lors du calcul: {e}")
            if self.db_manager.connection:
                self.db_manager.disconnect()
            raise
    
    def create_inputs_from_dict(self, data: Dict[str, Any]) -> RestaurantInputs:
        """
        Crée des entrées validées à partir d'un dictionnaire

        Args:
            data: Dictionnaire avec les paramètres du restaurant

        Returns:
            Objet RestaurantInputs validé
        """
        return self.input_handler.create_inputs_from_dict(data)

    def run_sample_calculation(self) -> TotalCostSummary:
        """
        Exécute un calcul complet en utilisant un jeu de données d'exemple.

        Cette méthode est un raccourci pratique pour les tests unitaires,
        la démonstration du moteur et la documentation. Elle génère des
        entrées factices via ``create_sample_inputs()`` (session
        « Restaurant Test ») puis appelle la même pipeline que pour un
        calcul réel via :meth:`calculate_restaurant_costs`.

        Aucun identifiant de session n’est fourni, donc aucun résultat
        n’est persisté en base ; seule la lecture des facteurs de coûts
        est effectuée.

        Returns:
            Résumé des coûts calculés pour le scénario d'exemple.
        """
        logger.info("Exécution d'un calcul d'exemple")

        # Créer des entrées d'exemple
        sample_inputs = create_sample_inputs()

        # Calculer
        return self.calculate_restaurant_costs(sample_inputs)

    def batch_calculate(self, inputs_list: List[RestaurantInputs]) -> List[TotalCostSummary]:
        """
        Calcule les coûts pour plusieurs restaurants en lot
        
        Args:
            inputs_list: Liste des paramètres de restaurants
            
        Returns:
            Liste des résumés de coûts
        """
        logger.info(f"Calcul en lot pour {len(inputs_list)} restaurants")
        
        results = []
        self.db_manager.connect()
        
        try:
            for inputs in inputs_list:
                summary = self.cost_calculator.calculate_all_costs(inputs)
                results.append(summary)
                logger.info(f"Calculé: {inputs.session_name} - {summary.total_cost:,.2f} CAD$")
        
        finally:
            self.db_manager.disconnect()
        
        return results
    
    def get_cost_breakdown_by_category(self, summary: TotalCostSummary) -> Dict[str, List[CostBreakdown]]:
        """
        Organise les détails de coûts par catégorie
        
        Args:
            summary: Résumé des coûts
            
        Returns:
            Dictionnaire organisé par catégorie
        """
        breakdown_by_category = {
            'staff': [],
            'equipment': [],
            'location': [],
            'operations': []
        }
        
        for breakdown in summary.cost_breakdowns:
            if breakdown.category in breakdown_by_category:
                breakdown_by_category[breakdown.category].append(breakdown)
        
        return breakdown_by_category
    
    def generate_calculation_report(self, summary: TotalCostSummary) -> str:
        """
        Génère un rapport détaillé des calculs
        
        Args:
            summary: Résumé des coûts
            
        Returns:
            Rapport formaté en texte
        """
        report_lines = [
            "=" * 60,
            f"RAPPORT DE CALCUL - {summary.session_name}",
            f"Généré le: {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}",
            "=" * 60,
            "",
            "📊 RÉSUMÉ DES COÛTS:",
            f"  Personnel:     {summary.staff_costs:>12,.2f} CAD$",
            f"  Équipement:    {summary.equipment_costs:>12,.2f} CAD$",
            f"  Immobilier:    {summary.location_costs:>12,.2f} CAD$",
            f"  Opérationnel:  {summary.operational_costs:>12,.2f} CAD$",
            f"  {'─' * 35}",
            f"  TOTAL:         {summary.total_cost:>12,.2f} CAD$",
            "",
            "📋 DÉTAIL DES CALCULS:"
        ]
        
        # Organiser par catégorie
        breakdown_by_cat = self.get_cost_breakdown_by_category(summary)
        
        for category, breakdowns in breakdown_by_cat.items():
            if breakdowns:
                category_names = {
                    'staff': 'PERSONNEL',
                    'equipment': 'ÉQUIPEMENT', 
                    'location': 'IMMOBILIER',
                    'operations': 'OPÉRATIONNEL'
                }
                
                report_lines.append(f"\n  {category_names[category]}:")
                for breakdown in breakdowns:
                    report_lines.append(f"    {breakdown.subcategory}: {breakdown.amount:,.2f} CAD$")
                    if breakdown.formula:
                        report_lines.append(f"      Formule: {breakdown.formula}")
        
        report_lines.extend(["", "=" * 60])
        
        return "\n".join(report_lines)
    
    def validate_calculation_accuracy(self, summary: TotalCostSummary) -> bool:
        """
        Valide la cohérence des calculs
        
        Args:
            summary: Résumé à valider
            
        Returns:
            True si les calculs sont cohérents
        """
        # Vérifier que le total correspond à la somme des catégories
        calculated_total = (
            summary.staff_costs + 
            summary.equipment_costs + 
            summary.location_costs + 
            summary.operational_costs
        )
        
        # Tolérance de 0.01 CAD pour les erreurs d'arrondi
        is_valid = abs(summary.total_cost - calculated_total) < 0.01
        
        if not is_valid:
            logger.warning(f"Incohérence détectée: Total={summary.total_cost}, Calculé={calculated_total}")
        
        return is_valid
    
    def close(self):
        """Ferme proprement les connexions"""
        if self.db_manager.connection:
            self.db_manager.disconnect()
        logger.info("Moteur de calcul fermé")


def run_engine_demo():
    """Démonstration du moteur de calcul"""
    print("🚀 Démonstration du moteur Track-A-FACE")
    print("=" * 50)
    
    try:
        # Initialiser le moteur
        engine = CalculationEngine()
        
        # Exécuter un calcul d'exemple
        print("\n📊 Calcul avec données d'exemple...")
        summary = engine.run_sample_calculation()
        
        # Valider les résultats
        is_valid = engine.validate_calculation_accuracy(summary)
        print(f"✅ Validation: {'Réussie' if is_valid else 'Échouée'}")
        
        # Générer le rapport
        print("\n📋 Rapport détaillé:")
        report = engine.generate_calculation_report(summary)
        print(report)
        
        # Fermer proprement
        engine.close()
        
        print("\n✅ Démonstration terminée avec succès!")
        
    except Exception as e:
        print(f"❌ Erreur lors de la démonstration: {e}")
        logger.error(f"Erreur démonstration: {e}")


if __name__ == "__main__":
    # Configuration du logging pour le test
    logging.basicConfig(
        level=logging.INFO,
        format='%(asctime)s - %(name)s - %(levelname)s - %(message)s'
    )
    
    # Lancer la démonstration
    run_engine_demo()
