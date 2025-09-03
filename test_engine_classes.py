#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Tests unitaires pour engine_classes.py
Validation du moteur de calcul des coûts pour restaurants
"""

import unittest
import tempfile
import os
import sys
from pathlib import Path

# Ajouter le répertoire parent au path pour les imports
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))

from engine_classes import CostBreakdown, TotalCostSummary, CostCalculator
from input_handler import create_sample_inputs
from sql import DatabaseManager, DatabaseInitializer


class TestCostBreakdown(unittest.TestCase):
    """Tests pour la structure CostBreakdown"""
    
    def test_cost_breakdown_creation(self):
        """Test création d'un breakdown de coût"""
        breakdown = CostBreakdown(
            category="test",
            subcategory="sous_test",
            amount=1000.0,
            formula="test * 1000",
            details={"param": "valeur"}
        )
        
        self.assertEqual(breakdown.category, "test")
        self.assertEqual(breakdown.subcategory, "sous_test")
        self.assertEqual(breakdown.amount, 1000.0)
        self.assertEqual(breakdown.formula, "test * 1000")
        self.assertEqual(breakdown.details["param"], "valeur")


class TestTotalCostSummary(unittest.TestCase):
    """Tests pour la structure TotalCostSummary"""
    
    def test_total_cost_summary_creation(self):
        """Test création d'un résumé de coûts totaux"""
        summary = TotalCostSummary(
            session_id=1,
            session_name="Test",
            staff_costs=100000.0,
            equipment_costs=20000.0,
            location_costs=50000.0,
            operational_costs=200000.0,
            total_cost=370000.0,
            cost_breakdowns=[]
        )
        
        self.assertEqual(summary.staff_costs, 100000.0)
        self.assertEqual(summary.equipment_costs, 20000.0)
        self.assertEqual(summary.location_costs, 50000.0)
        self.assertEqual(summary.operational_costs, 200000.0)
        self.assertEqual(summary.total_cost, 370000.0)


class TestCostCalculatorIntegration(unittest.TestCase):
    """Tests d'intégration pour le calculateur principal"""
    
    def setUp(self):
        """Configuration des tests d'intégration"""
        # Créer une vraie base de données temporaire
        self.temp_db = tempfile.NamedTemporaryFile(delete=False, suffix='.db')
        self.temp_db.close()
        
        # Initialiser la base de données
        self.db_manager = DatabaseManager(Path(self.temp_db.name))
        
        # Utiliser DatabaseInitializer pour initialiser la DB
        db_initializer = DatabaseInitializer()
        db_initializer.db_manager = self.db_manager
        db_initializer.initialize_database()
        
        # Créer le calculateur principal
        self.calculator = CostCalculator(self.db_manager)
        
        # Créer des entrées de test valides
        self.test_inputs = create_sample_inputs()
    
    def tearDown(self):
        """Nettoyage après tests d'intégration"""
        if hasattr(self.db_manager, 'disconnect'):
            self.db_manager.disconnect()
        if os.path.exists(self.temp_db.name):
            os.unlink(self.temp_db.name)
    
    def test_full_calculation_workflow(self):
        """Test du workflow complet de calcul"""
        # Exécuter le calcul complet
        summary = self.calculator.calculate_all_costs(self.test_inputs)
        
        # Vérifications de base
        self.assertIsInstance(summary, TotalCostSummary)
        self.assertGreater(len(summary.cost_breakdowns), 0)
        
        # Vérifier que tous les coûts sont positifs
        self.assertGreater(summary.staff_costs, 0)
        self.assertGreater(summary.equipment_costs, 0)
        self.assertGreater(summary.location_costs, 0)
        self.assertGreater(summary.operational_costs, 0)
        self.assertGreater(summary.total_cost, 0)
        
        # Vérifier la cohérence du total
        calculated_total = (summary.staff_costs + summary.equipment_costs + 
                          summary.location_costs + summary.operational_costs)
        self.assertAlmostEqual(summary.total_cost, calculated_total, places=2)
        
        # Vérifier que tous les breakdowns ont des montants positifs
        for breakdown in summary.cost_breakdowns:
            self.assertGreater(breakdown.amount, 0)
            self.assertIsNotNone(breakdown.category)
            self.assertIsNotNone(breakdown.subcategory)
        
        print(f"✅ Test réussi - Coût total: {summary.total_cost:,.2f} CAD$")
    
    def test_calculation_consistency(self):
        """Test de cohérence des calculs"""
        # Exécuter le calcul deux fois avec les mêmes entrées
        summary1 = self.calculator.calculate_all_costs(self.test_inputs)
        summary2 = self.calculator.calculate_all_costs(self.test_inputs)
        
        # Les résultats doivent être identiques
        self.assertEqual(summary1.total_cost, summary2.total_cost)
        self.assertEqual(summary1.staff_costs, summary2.staff_costs)
        self.assertEqual(summary1.equipment_costs, summary2.equipment_costs)
        self.assertEqual(summary1.location_costs, summary2.location_costs)
        self.assertEqual(summary1.operational_costs, summary2.operational_costs)
        
        print(f"✅ Test de cohérence réussi")


def run_all_tests():
    """Exécuter tous les tests unitaires"""
    print("🧪 Exécution des tests unitaires pour engine_classes.py")
    print("=" * 60)
    
    # Créer la suite de tests
    test_suite = unittest.TestSuite()
    
    # Ajouter tous les tests
    test_classes = [
        TestCostBreakdown,
        TestTotalCostSummary,
        TestCostCalculatorIntegration
    ]
    
    for test_class in test_classes:
        tests = unittest.TestLoader().loadTestsFromTestCase(test_class)
        test_suite.addTests(tests)
    
    # Exécuter les tests
    runner = unittest.TextTestRunner(verbosity=2)
    result = runner.run(test_suite)
    
    # Résumé
    print("\n" + "=" * 60)
    if result.wasSuccessful():
        print("✅ Tous les tests sont passés avec succès!")
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
