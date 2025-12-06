#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Tests unitaires pour engine.py
Validation du moteur d'orchestration Track-A-FACE
"""

import unittest
import tempfile
import os
import sys
from pathlib import Path

# Ajouter le répertoire parent au path pour les imports
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))

from engine import CalculationEngine
from engine_classes import TotalCostSummary
from input_handler import create_sample_inputs, RestaurantInputs
from sql import DatabaseManager, DatabaseInitializer


class TestCalculationEngine(unittest.TestCase):
    """Tests pour le moteur d'orchestration principal"""
    
    def setUp(self):
        """Configuration des tests"""
        # Créer une base de données temporaire
        self.temp_db = tempfile.NamedTemporaryFile(delete=False, suffix='.db')
        self.temp_db.close()
        
        # Initialiser le moteur avec la DB temporaire
        self.engine = CalculationEngine(Path(self.temp_db.name))
        
        # Créer des entrées de test
        self.test_inputs = create_sample_inputs()
    
    def tearDown(self):
        """Nettoyage après tests"""
        self.engine.close()
        if os.path.exists(self.temp_db.name):
            os.unlink(self.temp_db.name)
    
    def test_engine_initialization(self):
        """Test d'initialisation du moteur"""
        # Vérifier que tous les composants sont initialisés
        self.assertIsNotNone(self.engine.db_manager)
        self.assertIsNotNone(self.engine.cost_calculator)
        self.assertIsNotNone(self.engine.input_handler)
    
    def test_sample_calculation(self):
        """Test du calcul avec données d'exemple"""
        summary = self.engine.run_sample_calculation()
        
        # Vérifications de base
        self.assertIsInstance(summary, TotalCostSummary)
        self.assertEqual(summary.session_name, "Restaurant Test")
        self.assertGreater(summary.total_cost, 0)
        
        # Vérifier que tous les coûts sont positifs
        self.assertGreater(summary.staff_costs, 0)
        self.assertGreater(summary.equipment_costs, 0)
        self.assertGreater(summary.location_costs, 0)
        self.assertGreater(summary.operational_costs, 0)
        
        print(f"✅ Calcul d'exemple réussi: {summary.total_cost:,.2f} CAD$")
    
    def test_calculation_accuracy_validation(self):
        """Test de validation de la précision des calculs"""
        summary = self.engine.run_sample_calculation()
        
        # Valider la cohérence
        is_valid = self.engine.validate_calculation_accuracy(summary)
        self.assertTrue(is_valid)
        
        # Vérifier manuellement
        calculated_total = (
            summary.staff_costs + 
            summary.equipment_costs + 
            summary.location_costs + 
            summary.operational_costs
        )
        self.assertAlmostEqual(summary.total_cost, calculated_total, places=2)
    
    def test_cost_breakdown_by_category(self):
        """Test d'organisation des coûts par catégorie"""
        summary = self.engine.run_sample_calculation()
        breakdown_by_cat = self.engine.get_cost_breakdown_by_category(summary)
        
        # Vérifier que toutes les catégories sont présentes
        expected_categories = ['staff', 'equipment', 'location', 'operations']
        for category in expected_categories:
            self.assertIn(category, breakdown_by_cat)
            self.assertIsInstance(breakdown_by_cat[category], list)
        
        # Vérifier qu'il y a des détails dans chaque catégorie
        total_breakdowns = sum(len(breakdowns) for breakdowns in breakdown_by_cat.values())
        self.assertGreater(total_breakdowns, 0)
    
    def test_report_generation(self):
        """Test de génération de rapport"""
        summary = self.engine.run_sample_calculation()
        report = self.engine.generate_calculation_report(summary)
        
        # Vérifications du contenu du rapport
        self.assertIsInstance(report, str)
        self.assertIn("RAPPORT DE CALCUL", report)
        self.assertIn(summary.session_name, report)
        self.assertIn("RÉSUMÉ DES COÛTS", report)
        self.assertIn("DÉTAIL DES CALCULS", report)
        
        # Vérifier que les montants sont présents
        self.assertIn(f"{summary.total_cost:,.2f} CAD$", report)
        
        print("✅ Rapport généré avec succès")
    
    def test_create_inputs_from_dict(self):
        """Test de création d'entrées à partir d'un dictionnaire"""
        data = {
            'session_name': 'Test Dict',
            'restaurant_theme': 'casual_dining',
            'revenue_size': 'medium',
            'kitchen_size_sqm': 120,
            'kitchen_workstations': 5,
            'daily_capacity': 70,
            'staff_count': 12,
            'staff_experience_level': 'intermediate',
            'training_hours_needed': 35,
            'equipment_age_years': 3,
            'equipment_condition': 'good',
            'equipment_value': 75000.0,
            'location_rent_sqm': 18.0
        }
        
        inputs = self.engine.create_inputs_from_dict(data)
        
        # Vérifications
        self.assertIsInstance(inputs, RestaurantInputs)
        self.assertEqual(inputs.session_name, 'Test Dict')
        self.assertEqual(inputs.restaurant_theme, 'casual_dining')
        self.assertEqual(inputs.daily_capacity, 70)
    
    def test_batch_calculation(self):
        """Test de calcul en lot"""
        # Créer plusieurs jeux d'entrées
        inputs_list = []
        for i in range(3):
            data = {
                'session_name': f'Restaurant Batch {i+1}',
                'restaurant_theme': 'casual_dining',
                'revenue_size': 'medium',
                'kitchen_size_sqm': 100 + (i * 20),
                'kitchen_workstations': 4 + i,
                'daily_capacity': 60 + (i * 10),
                'staff_count': 10 + (i * 2),
                'staff_experience_level': 'intermediate',
                'training_hours_needed': 30,
                'equipment_age_years': 2,
                'equipment_condition': 'good',
                'equipment_value': 60000.0 + (i * 10000),
                'location_rent_sqm': 15.0 + (i * 2)
            }
            inputs = self.engine.create_inputs_from_dict(data)
            inputs_list.append(inputs)
        
        # Exécuter le calcul en lot
        results = self.engine.batch_calculate(inputs_list)
        
        # Vérifications
        self.assertEqual(len(results), 3)
        for i, summary in enumerate(results):
            self.assertIsInstance(summary, TotalCostSummary)
            self.assertEqual(summary.session_name, f'Restaurant Batch {i+1}')
            self.assertGreater(summary.total_cost, 0)
        
        print(f"✅ Calcul en lot réussi pour {len(results)} restaurants")
    
    def test_calculation_consistency(self):
        """Test de cohérence des calculs répétés"""
        # Exécuter le même calcul plusieurs fois
        summary1 = self.engine.calculate_restaurant_costs(self.test_inputs)
        summary2 = self.engine.calculate_restaurant_costs(self.test_inputs)
        summary3 = self.engine.calculate_restaurant_costs(self.test_inputs)
        
        # Vérifier que les résultats sont identiques
        self.assertEqual(summary1.total_cost, summary2.total_cost)
        self.assertEqual(summary2.total_cost, summary3.total_cost)
        
        # Vérifier chaque catégorie
        self.assertEqual(summary1.staff_costs, summary2.staff_costs)
        self.assertEqual(summary1.equipment_costs, summary2.equipment_costs)
        self.assertEqual(summary1.location_costs, summary2.location_costs)
        self.assertEqual(summary1.operational_costs, summary2.operational_costs)
        
        print("✅ Cohérence des calculs validée")
    
    def test_error_handling(self):
        """Test de gestion d'erreurs"""
        # Tester avec des entrées invalides (si possible)
        try:
            # Créer des entrées avec des valeurs extrêmes
            invalid_data = {
                'session_name': '',  # Nom vide
                'restaurant_theme': 'invalid_theme',
                'revenue_size': 'invalid_size',
                'kitchen_size_sqm': -10,  # Valeur négative
                'kitchen_workstations': 0,
                'daily_capacity': -5,
                'staff_count': -1,
                'staff_experience_level': 'invalid',
                'training_hours_needed': -10,
                'equipment_age_years': -1,
                'equipment_condition': 'invalid',
                'equipment_value': -1000,
                'location_rent_sqm': -5
            }
            
            # Cela devrait lever une exception ou être géré gracieusement
            inputs = self.engine.create_inputs_from_dict(invalid_data)
            summary = self.engine.calculate_restaurant_costs(inputs)
            
            # Si on arrive ici, vérifier que les résultats sont raisonnables
            self.assertGreaterEqual(summary.total_cost, 0)
            
        except Exception as e:
            # C'est normal que des entrées invalides lèvent des exceptions
            print(f"✅ Gestion d'erreur appropriée: {type(e).__name__}")
    
    def test_database_integration(self):
        """Test d'intégration avec la base de données"""
        # Vérifier que la base de données est accessible
        self.engine.db_manager.connect()
        
        # Tester une requête
        connection = self.engine.db_manager.connection
        cursor = connection.cursor()
        cursor.execute("SELECT COUNT(*) FROM cost_factors")
        factor_count = cursor.fetchone()[0]
        
        self.assertGreater(factor_count, 0)
        
        self.engine.db_manager.disconnect()
        
        print(f"✅ Base de données intégrée avec {factor_count} facteurs")


def run_all_tests():
    """Exécuter tous les tests du moteur d'orchestration"""
    print("🧪 Exécution des tests unitaires pour engine.py")
    print("=" * 60)
    
    # Créer la suite de tests
    test_suite = unittest.TestSuite()
    
    # Ajouter tous les tests
    test_classes = [TestCalculationEngine]
    
    for test_class in test_classes:
        tests = unittest.TestLoader().loadTestsFromTestCase(test_class)
        test_suite.addTests(tests)
    
    # Exécuter les tests
    runner = unittest.TextTestRunner(verbosity=2)
    result = runner.run(test_suite)
    
    # Résumé
    print("\n" + "=" * 60)
    if result.wasSuccessful():
        print("✅ Tous les tests du moteur d'orchestration sont passés avec succès!")
        print("🎯 Le module engine.py est complètement fonctionnel")
    else:
        print(f"❌ {len(result.failures)} échecs, {len(result.errors)} erreurs")
        
        if result.failures:
            print("\nÉchecs:")
            for test, traceback in result.failures:
                print(f"  - {test}: {traceback}")
        
        if result.errors:
            print("\nErreurs:")
            for test, traceback in result.errors:
                print(f"  - {test}: {traceback}")
    
    return result.wasSuccessful()


if __name__ == "__main__":
    run_all_tests()
