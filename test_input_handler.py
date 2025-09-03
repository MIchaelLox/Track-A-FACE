# ==========================
# /test_input_handler.py
# ==========================

"""
Tests unitaires pour le module input_handler
Teste toutes les fonctions de validation et gestion des entrées
"""

import unittest
from input_handler import (
    InputHandler, InputValidator, RestaurantInputs, 
    ValidationError, create_sample_inputs
)


class TestInputValidator(unittest.TestCase):
    """Tests pour la classe InputValidator"""
    
    def setUp(self):
        self.validator = InputValidator()
    
    def test_validate_session_name_valid(self):
        """Test validation nom de session valide"""
        valid_names = [
            "Restaurant Test",
            "Fast Food Montreal",
            "Fine_Dining-Toronto"
        ]
        
        for name in valid_names:
            result = self.validator.validate_session_name(name)
            self.assertIsInstance(result, str)
            self.assertGreaterEqual(len(result), 3)
    
    def test_validate_session_name_invalid(self):
        """Test validation nom de session invalide"""
        invalid_names = ["", "  ", "ab", None]
        
        for name in invalid_names:
            with self.assertRaises(ValidationError):
                self.validator.validate_session_name(name)
    
    def test_validate_restaurant_theme_valid(self):
        """Test validation thème valide"""
        valid_themes = ["fast_food", "casual_dining", "fine_dining", "cloud_kitchen", "food_truck"]
        
        for theme in valid_themes:
            result = self.validator.validate_restaurant_theme(theme)
            self.assertEqual(result, theme)
    
    def test_validate_restaurant_theme_invalid(self):
        """Test validation thème invalide"""
        invalid_themes = ["invalid", "restaurant", ""]
        
        for theme in invalid_themes:
            with self.assertRaises(ValidationError):
                self.validator.validate_restaurant_theme(theme)
    
    def test_validate_kitchen_size_valid(self):
        """Test validation taille cuisine valide"""
        valid_sizes = [15.0, 50, 100.5, 500]
        
        for size in valid_sizes:
            result = self.validator.validate_kitchen_size(size)
            self.assertIsInstance(result, float)
            self.assertGreaterEqual(result, self.validator.MIN_KITCHEN_SIZE)
            self.assertLessEqual(result, self.validator.MAX_KITCHEN_SIZE)
    
    def test_validate_kitchen_size_invalid(self):
        """Test validation taille cuisine invalide"""
        invalid_sizes = [0, -10, 5.0, 1500, "abc"]
        
        for size in invalid_sizes:
            with self.assertRaises(ValidationError):
                self.validator.validate_kitchen_size(size)
    
    def test_validate_workstations_valid(self):
        """Test validation postes de travail valides"""
        valid_counts = [1, 5, 10, 25, 50]
        
        for count in valid_counts:
            result = self.validator.validate_workstations(count)
            self.assertEqual(result, count)
    
    def test_validate_workstations_invalid(self):
        """Test validation postes de travail invalides"""
        invalid_counts = [0, -1, 51, 100, 1.5]
        
        for count in invalid_counts:
            with self.assertRaises(ValidationError):
                self.validator.validate_workstations(count)
    
    def test_validate_staff_count_valid(self):
        """Test validation nombre employés valide"""
        valid_counts = [1, 10, 50, 100, 200]
        
        for count in valid_counts:
            result = self.validator.validate_staff_count(count)
            self.assertEqual(result, count)
    
    def test_validate_equipment_condition_valid(self):
        """Test validation état équipement valide"""
        valid_conditions = ["excellent", "good", "fair", "poor"]
        
        for condition in valid_conditions:
            result = self.validator.validate_equipment_condition(condition)
            self.assertEqual(result, condition)
    
    def test_validate_equipment_value_valid(self):
        """Test validation valeur équipement valide"""
        valid_values = [0, 1000.0, 50000, 500000.50]
        
        for value in valid_values:
            result = self.validator.validate_equipment_value(value)
            self.assertIsInstance(result, float)
            self.assertGreaterEqual(result, 0)


class TestInputHandler(unittest.TestCase):
    """Tests pour la classe InputHandler"""
    
    def setUp(self):
        self.handler = InputHandler()
        self.valid_data = {
            'session_name': 'Test Restaurant',
            'restaurant_theme': 'casual_dining',
            'revenue_size': 'medium',
            'kitchen_size_sqm': 100.0,
            'kitchen_workstations': 8,
            'daily_capacity': 200,
            'staff_count': 15,
            'staff_experience_level': 'intermediate',
            'equipment_condition': 'good',
            'equipment_value': 120000.0,
            'location_rent_sqm': 40.0
        }
    
    def test_create_inputs_from_dict_valid(self):
        """Test création entrées à partir de données valides"""
        inputs = self.handler.create_inputs_from_dict(self.valid_data)
        
        self.assertIsInstance(inputs, RestaurantInputs)
        self.assertEqual(inputs.session_name, 'Test Restaurant')
        self.assertEqual(inputs.restaurant_theme, 'casual_dining')
        self.assertEqual(inputs.kitchen_size_sqm, 100.0)
        self.assertGreater(inputs.training_hours_needed, 0)  # Calculé automatiquement
    
    def test_create_inputs_missing_required_field(self):
        """Test création entrées avec champ requis manquant"""
        incomplete_data = self.valid_data.copy()
        del incomplete_data['session_name']
        
        with self.assertRaises(ValidationError):
            self.handler.create_inputs_from_dict(incomplete_data)
    
    def test_validate_cross_references_kitchen_workstations(self):
        """Test validation croisée cuisine/postes"""
        # Trop de postes pour la taille
        invalid_data = self.valid_data.copy()
        invalid_data['kitchen_size_sqm'] = 20.0  # Petite cuisine
        invalid_data['kitchen_workstations'] = 10  # Trop de postes
        
        with self.assertRaises(ValidationError):
            self.handler.create_inputs_from_dict(invalid_data)
    
    def test_validate_cross_references_capacity_staff(self):
        """Test validation croisée capacité/personnel"""
        # Trop de personnel pour la capacité
        invalid_data = self.valid_data.copy()
        invalid_data['daily_capacity'] = 50  # Petite capacité
        invalid_data['staff_count'] = 20     # Trop de personnel
        
        with self.assertRaises(ValidationError):
            self.handler.create_inputs_from_dict(invalid_data)
    
    def test_calculate_default_training_hours(self):
        """Test calcul automatique heures formation"""
        # Test avec différents niveaux d'expérience
        test_cases = [
            ('beginner', 'fine_dining', 108),  # 40 * 1.5 * 1.8
            ('expert', 'fast_food', 12),       # 40 * 0.4 * 0.8 = 12.8 -> 12
            ('intermediate', 'casual_dining', 40)  # 40 * 1.0 * 1.0
        ]
        
        for experience, theme, expected_min in test_cases:
            data = self.valid_data.copy()
            data['staff_experience_level'] = experience
            data['restaurant_theme'] = theme
            data['training_hours_needed'] = 0  # Force le calcul automatique
            
            inputs = self.handler.create_inputs_from_dict(data)
            self.assertGreaterEqual(inputs.training_hours_needed, 10)  # Minimum
            # Vérifier que c'est dans la bonne fourchette
            if expected_min <= 50:
                self.assertLessEqual(inputs.training_hours_needed, 80)
    
    def test_get_input_summary(self):
        """Test génération résumé entrées"""
        inputs = self.handler.create_inputs_from_dict(self.valid_data)
        summary = self.handler.get_input_summary(inputs)
        
        self.assertIsInstance(summary, dict)
        self.assertIn('session', summary)
        self.assertIn('type', summary)
        self.assertIn('cuisine', summary)
        self.assertIn('personnel', summary)
        
        # Vérifier le formatage
        self.assertEqual(summary['session'], 'Test Restaurant')
        self.assertIn('Casual Dining', summary['type'])
        self.assertIn('100.0 m²', summary['cuisine'])
    
    def test_validate_inputs_quick(self):
        """Test validation rapide sans création"""
        # Données valides
        self.assertTrue(self.handler.validate_inputs(self.valid_data))
        
        # Données invalides
        invalid_data = {'session_name': ''}
        self.assertFalse(self.handler.validate_inputs(invalid_data))
    
    def test_get_validation_errors(self):
        """Test récupération erreurs de validation"""
        invalid_data = {
            'session_name': '',  # Invalide
            'restaurant_theme': 'invalid',  # Invalide
            'kitchen_size_sqm': -10  # Invalide
        }
        
        errors = self.handler.get_validation_errors(invalid_data)
        self.assertIsInstance(errors, list)
        self.assertGreater(len(errors), 0)


class TestRestaurantInputs(unittest.TestCase):
    """Tests pour la classe RestaurantInputs"""
    
    def test_to_dict(self):
        """Test conversion en dictionnaire"""
        sample = create_sample_inputs()
        data_dict = sample.to_dict()
        
        self.assertIsInstance(data_dict, dict)
        self.assertIn('session_name', data_dict)
        self.assertIn('restaurant_theme', data_dict)
        self.assertIn('kitchen_size_sqm', data_dict)
        
        # Vérifier que toutes les valeurs sont sérialisables
        for key, value in data_dict.items():
            self.assertIsNotNone(value)


class TestIntegration(unittest.TestCase):
    """Tests d'intégration complets"""
    
    def test_full_workflow_valid_restaurant(self):
        """Test workflow complet avec restaurant valide"""
        handler = InputHandler()
        
        # Données d'un restaurant réaliste
        restaurant_data = {
            'session_name': 'Bistro Montreal Downtown',
            'restaurant_theme': 'casual_dining',
            'revenue_size': 'large',
            'kitchen_size_sqm': 150.0,
            'kitchen_workstations': 12,
            'daily_capacity': 300,
            'staff_count': 25,
            'staff_experience_level': 'experienced',
            'equipment_condition': 'good',
            'equipment_age_years': 3,
            'equipment_value': 200000.0,
            'location_rent_sqm': 55.0
        }
        
        # Créer et valider
        inputs = handler.create_inputs_from_dict(restaurant_data)
        self.assertIsNotNone(inputs)
        
        # Vérifier le résumé
        summary = handler.get_input_summary(inputs)
        self.assertIn('Bistro Montreal Downtown', summary['session'])
        
        # Vérifier la conversion
        data_dict = inputs.to_dict()
        self.assertEqual(len(data_dict), 13)  # Tous les champs
    
    def test_edge_cases(self):
        """Test cas limites"""
        handler = InputHandler()
        
        # Restaurant minimal (food truck)
        minimal_data = {
            'session_name': 'Food Truck Minimal',
            'restaurant_theme': 'food_truck',
            'revenue_size': 'small',
            'kitchen_size_sqm': 20.0,  # Ajusté pour 2 postes (min 8m²/poste)
            'kitchen_workstations': 2,
            'daily_capacity': 50,
            'staff_count': 2,
            'staff_experience_level': 'beginner',
            'equipment_condition': 'fair',
            'equipment_value': 0.0,  # Pas d'équipement
            'location_rent_sqm': 0.0  # Pas de loyer
        }
        
        inputs = handler.create_inputs_from_dict(minimal_data)
        self.assertIsNotNone(inputs)
        self.assertGreater(inputs.training_hours_needed, 0)
        
        # Restaurant maximal
        maximal_data = {
            'session_name': 'Fine Dining Luxury',
            'restaurant_theme': 'fine_dining',
            'revenue_size': 'enterprise',
            'kitchen_size_sqm': 500.0,
            'kitchen_workstations': 25,
            'daily_capacity': 400,
            'staff_count': 80,
            'staff_experience_level': 'expert',
            'equipment_condition': 'excellent',
            'equipment_age_years': 0,
            'equipment_value': 800000.0,
            'location_rent_sqm': 120.0
        }
        
        inputs = handler.create_inputs_from_dict(maximal_data)
        self.assertIsNotNone(inputs)


def run_all_tests():
    """Exécute tous les tests et affiche un résumé"""
    print("🧪 Exécution des tests unitaires input_handler")
    print("=" * 50)
    
    # Créer la suite de tests
    test_classes = [
        TestInputValidator,
        TestInputHandler, 
        TestRestaurantInputs,
        TestIntegration
    ]
    
    total_tests = 0
    total_failures = 0
    
    for test_class in test_classes:
        print(f"\n📋 Tests {test_class.__name__}")
        suite = unittest.TestLoader().loadTestsFromTestCase(test_class)
        runner = unittest.TextTestRunner(verbosity=1)
        result = runner.run(suite)
        
        total_tests += result.testsRun
        total_failures += len(result.failures) + len(result.errors)
        
        if result.failures:
            print(f"❌ Échecs: {len(result.failures)}")
        if result.errors:
            print(f"💥 Erreurs: {len(result.errors)}")
    
    print("\n" + "=" * 50)
    print(f"📊 Résumé: {total_tests} tests exécutés")
    
    if total_failures == 0:
        print("✅ Tous les tests sont passés avec succès!")
        return True
    else:
        print(f"❌ {total_failures} tests ont échoué")
        return False


if __name__ == "__main__":
    success = run_all_tests()
    exit(0 if success else 1)
