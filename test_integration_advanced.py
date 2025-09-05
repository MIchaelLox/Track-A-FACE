#!/usr/bin/env python3
# -*- coding: utf-8 -*-

"""
Tests d'intégration avancés pour Track-A-FACE
Valide les formules pondérées, l'intégration DB et les performances
"""

import unittest
import tempfile
import os
import time
from pathlib import Path

from sql import DatabaseManager, DatabaseInitializer, AdvancedCostFactorsManager, get_all_daos
from engine_classes import CostCalculator, StaffCostCalculator, EquipmentCostCalculator, OperationalCostCalculator
from input_handler import RestaurantInputs, create_sample_inputs
from engine import CalculationEngine
from parallel_analysis import ParallelAnalysis


class TestAdvancedIntegration(unittest.TestCase):
    """Tests d'intégration avancés pour toutes les fonctionnalités"""
    
    def setUp(self):
        """Configuration des tests avec base temporaire"""
        self.temp_dir = tempfile.mkdtemp()
        self.db_path = Path(self.temp_dir) / "test_advanced.db"
        
        # Initialiser la base de données
        self.db_manager = DatabaseManager(str(self.db_path))
        self.initializer = DatabaseInitializer()
        self.initializer.db_manager = self.db_manager
        self.initializer.initialize_database()
        
        # Créer les calculateurs
        self.calculator = CostCalculator(self.db_manager)
        self.engine = CalculationEngine(str(self.db_path))
        
        # Données de test
        self.test_inputs = create_sample_inputs()
    
    def tearDown(self):
        """Nettoyage après tests"""
        if self.db_manager.connection:
            self.db_manager.disconnect()
        
        # Supprimer le fichier de base temporaire
        if self.db_path.exists():
            self.db_path.unlink()
    
    def test_advanced_cost_factors_integration(self):
        """Test de l'intégration avancée des facteurs de coût"""
        print("\n🧪 Test intégration facteurs de coût avancés...")
        
        # Tester le gestionnaire avancé
        advanced_manager = AdvancedCostFactorsManager(self.db_manager)
        
        # Test récupération avec fallback
        training_rate = advanced_manager.get_factor_with_fallback(
            'training_rate_per_hour', 
            'casual_dining'
        )
        self.assertGreater(training_rate, 0)
        self.assertEqual(training_rate, 30.0)  # Valeur attendue pour casual_dining
        
        # Test fallback pour facteur inexistant
        unknown_factor = advanced_manager.get_factor_with_fallback(
            'unknown_factor', 
            'casual_dining', 
            default_value=5.0
        )
        self.assertEqual(unknown_factor, 5.0)
        
        # Test récupération en lot
        all_factors = advanced_manager.get_all_factors_for_calculation(
            'fine_dining', 
            'large'
        )
        self.assertIsInstance(all_factors, dict)
        self.assertIn('training_rate_per_hour', all_factors)
        self.assertIn('food_cost_per_cover', all_factors)
        
        print("✅ Facteurs de coût avancés validés")
    
    def test_weighted_formulas_accuracy(self):
        """Test de précision des formules pondérées"""
        print("\n🧪 Test précision formules pondérées...")
        
        # Test avec différents thèmes
        themes_to_test = ['fast_food', 'casual_dining', 'fine_dining', 'cloud_kitchen']
        
        for theme in themes_to_test:
            test_input = RestaurantInputs(
                session_name=f"Test {theme}",
                restaurant_theme=theme,
                revenue_size='medium',
                staff_count=8,
                training_hours_needed=40,
                daily_capacity=150,
                kitchen_size_sqm=80.0,
                equipment_value=120000,
                equipment_condition='good',
                equipment_age_years=3,
                location_rent_sqm=50.0
            )
            
            # Calculer avec formules pondérées
            summary = self.calculator.calculate_all_costs(test_input)
            
            # Vérifications de cohérence
            self.assertGreater(summary.total_cost, 0)
            self.assertGreater(summary.staff_costs, 0)
            self.assertGreater(summary.equipment_costs, 0)
            self.assertGreater(summary.operational_costs, 0)
            
            # Vérifier que les coûts varient selon le thème
            if theme == 'fine_dining':
                # Fine dining devrait avoir des coûts plus élevés
                self.assertGreater(summary.staff_costs, 50000)
                self.assertGreater(summary.operational_costs, 100000)
            elif theme == 'fast_food':
                # Fast food devrait avoir des coûts plus bas
                self.assertLess(summary.staff_costs, 80000)
        
        print("✅ Formules pondérées validées pour tous les thèmes")
    
    def test_database_performance_optimization(self):
        """Test des optimisations de performance DB"""
        print("\n🧪 Test optimisations performance DB...")
        
        advanced_manager = AdvancedCostFactorsManager(self.db_manager)
        
        # Test de performance avec cache
        start_time = time.time()
        
        # Premier appel (sans cache)
        for _ in range(10):
            factor = advanced_manager.get_factor_with_fallback(
                'training_rate_per_hour', 
                'casual_dining'
            )
        
        first_call_time = time.time() - start_time
        
        # Deuxième série d'appels (avec cache)
        start_time = time.time()
        
        for _ in range(10):
            factor = advanced_manager.get_factor_with_fallback(
                'training_rate_per_hour', 
                'casual_dining'
            )
        
        cached_call_time = time.time() - start_time
        
        # Le cache devrait améliorer les performances
        self.assertLess(cached_call_time, first_call_time * 0.8)
        
        print(f"✅ Performance améliorée: {first_call_time:.4f}s → {cached_call_time:.4f}s")
    
    def test_complete_calculation_workflow(self):
        """Test du workflow complet de calcul"""
        print("\n🧪 Test workflow complet de calcul...")
        
        # Test avec l'engine principal
        summary = self.engine.calculate_restaurant_costs(self.test_inputs)
        
        # Vérifications détaillées
        self.assertIsNotNone(summary)
        self.assertGreater(summary.total_cost, 0)
        self.assertGreater(len(summary.cost_breakdowns), 5)
        
        # Vérifier que tous les détails sont présents
        for breakdown in summary.cost_breakdowns:
            self.assertIsNotNone(breakdown.formula)
            self.assertIsInstance(breakdown.details, dict)
            self.assertGreater(breakdown.amount, 0)
        
        # Test de génération de rapport
        report = self.engine.generate_calculation_report(summary)
        self.assertIn("RAPPORT DE CALCUL", report)
        self.assertIn("CAD$", report)
        
        print("✅ Workflow complet validé")
    
    def test_scenario_comparison_integration(self):
        """Test d'intégration de la comparaison de scénarios"""
        print("\n🧪 Test intégration comparaison scénarios...")
        
        # Créer une analyse parallèle
        analysis = ParallelAnalysis(self.engine)
        
        # Ajouter plusieurs scénarios
        scenarios = {
            'Fast Food': RestaurantInputs(
                session_name="Fast Food Scenario",
                restaurant_theme='fast_food',
                revenue_size='small',
                staff_count=5,
                training_hours_needed=20,
                daily_capacity=200,
                kitchen_size_sqm=60.0,
                equipment_value=80000,
                equipment_condition='good',
                equipment_age_years=2,
                location_rent_sqm=35.0
            ),
            'Fine Dining': RestaurantInputs(
                session_name="Fine Dining Scenario",
                restaurant_theme='fine_dining',
                revenue_size='large',
                staff_count=15,
                training_hours_needed=80,
                daily_capacity=80,
                kitchen_size_sqm=120.0,
                equipment_value=300000,
                equipment_condition='excellent',
                equipment_age_years=1,
                location_rent_sqm=85.0
            )
        }
        
        for name, inputs in scenarios.items():
            analysis.add_scenario(name, inputs)
        
        # Calculer tous les scénarios
        results = analysis.calculate_all_scenarios()
        
        # Vérifications
        self.assertEqual(len(results), 2)
        self.assertIn('Fast Food', results)
        self.assertIn('Fine Dining', results)
        
        # Le fine dining devrait coûter plus cher
        self.assertGreater(
            results['Fine Dining'].total_cost,
            results['Fast Food'].total_cost
        )
        
        # Générer rapport de comparaison
        comparison_report = analysis.generate_comparison_report()
        self.assertIn("COMPARAISON", comparison_report)
        self.assertIn("Fast Food", comparison_report)
        self.assertIn("Fine Dining", comparison_report)
        
        print("✅ Comparaison de scénarios validée")
    
    def test_batch_processing_performance(self):
        """Test de performance du traitement en lot"""
        print("\n🧪 Test performance traitement en lot...")
        
        # Créer plusieurs inputs de test
        batch_inputs = []
        for i in range(5):
            inputs = RestaurantInputs(
                session_name=f"Batch Test {i+1}",
                restaurant_theme=['fast_food', 'casual_dining', 'fine_dining'][i % 3],
                revenue_size=['small', 'medium', 'large'][i % 3],
                staff_count=5 + i * 2,
                training_hours_needed=20 + i * 10,
                daily_capacity=100 + i * 50,
                kitchen_size_sqm=60.0 + i * 20,
                equipment_value=80000 + i * 40000,
                equipment_condition='good',
                equipment_age_years=1 + i,
                location_rent_sqm=40.0 + i * 10
            )
            batch_inputs.append(inputs)
        
        # Test traitement en lot
        start_time = time.time()
        batch_results = self.engine.batch_calculate(batch_inputs)
        batch_time = time.time() - start_time
        
        # Vérifications
        self.assertEqual(len(batch_results), 5)
        for result in batch_results:
            self.assertGreater(result.total_cost, 0)
        
        # Test traitement individuel pour comparaison
        start_time = time.time()
        individual_results = []
        for inputs in batch_inputs:
            result = self.engine.calculate_restaurant_costs(inputs)
            individual_results.append(result)
        individual_time = time.time() - start_time
        
        print(f"✅ Traitement en lot: {batch_time:.3f}s vs individuel: {individual_time:.3f}s")
    
    def test_data_consistency_validation(self):
        """Test de validation de cohérence des données"""
        print("\n🧪 Test validation cohérence données...")
        
        # Test avec données cohérentes
        valid_inputs = create_sample_inputs()
        summary = self.calculator.calculate_all_costs(valid_inputs)
        
        # Vérifier la cohérence interne
        calculated_total = (
            summary.staff_costs + 
            summary.equipment_costs + 
            summary.location_costs + 
            summary.operational_costs
        )
        
        self.assertAlmostEqual(
            summary.total_cost, 
            calculated_total, 
            places=2
        )
        
        # Test reproductibilité
        summary2 = self.calculator.calculate_all_costs(valid_inputs)
        self.assertEqual(summary.total_cost, summary2.total_cost)
        
        print("✅ Cohérence des données validée")
    
    def test_error_handling_robustness(self):
        """Test de robustesse de la gestion d'erreurs"""
        print("\n🧪 Test robustesse gestion d'erreurs...")
        
        # Test avec données invalides
        invalid_inputs = RestaurantInputs(
            session_name="Test Invalid",
            restaurant_theme='invalid_theme',
            revenue_size='invalid_size',
            staff_count=-1,  # Invalide
            training_hours_needed=0,
            daily_capacity=0,  # Invalide
            kitchen_size_sqm=0,
            equipment_value=-1000,  # Invalide
            equipment_condition='unknown',
            equipment_age_years=-5,  # Invalide
            location_rent_sqm=0
        )
        
        # Le système devrait gérer les erreurs gracieusement
        try:
            summary = self.calculator.calculate_all_costs(invalid_inputs)
            # Même avec des données invalides, on devrait avoir un résultat
            self.assertIsNotNone(summary)
            self.assertGreaterEqual(summary.total_cost, 0)
        except Exception as e:
            self.fail(f"Le système devrait gérer les données invalides: {e}")
        
        print("✅ Gestion d'erreurs robuste validée")


def run_integration_tests():
    """Lance tous les tests d'intégration"""
    print("🚀 LANCEMENT DES TESTS D'INTÉGRATION AVANCÉS")
    print("=" * 60)
    
    # Créer la suite de tests
    suite = unittest.TestLoader().loadTestsFromTestCase(TestAdvancedIntegration)
    
    # Lancer les tests avec rapport détaillé
    runner = unittest.TextTestRunner(verbosity=2)
    result = runner.run(suite)
    
    print("\n" + "=" * 60)
    if result.wasSuccessful():
        print("✅ TOUS LES TESTS D'INTÉGRATION RÉUSSIS")
        print(f"📊 {result.testsRun} tests exécutés avec succès")
    else:
        print("❌ CERTAINS TESTS ONT ÉCHOUÉ")
        print(f"📊 {len(result.failures)} échecs, {len(result.errors)} erreurs")
    
    return result.wasSuccessful()


if __name__ == "__main__":
    success = run_integration_tests()
    exit(0 if success else 1)
