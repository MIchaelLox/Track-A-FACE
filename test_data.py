# ==========================
# /test_data.py
# ==========================

"""
Données de test pour valider la base de données Track-A-FACE
Crée des sessions d'exemple avec différents types de restaurants
"""

from sql import DatabaseManager, get_all_daos
import logging
from typing import List

logger = logging.getLogger(__name__)

# Données de test pour différents scénarios
TEST_SESSIONS = [
    {
        "session_name": "Fast Food Downtown Toronto",
        "restaurant_theme": "fast_food",
        "revenue_size": "medium",
        "kitchen_size_sqm": 80.0,
        "kitchen_workstations": 6,
        "daily_capacity": 300,
        "staff_count": 12,
        "staff_experience_level": "beginner",
        "training_hours_needed": 40,
        "equipment_age_years": 2,
        "equipment_condition": "good",
        "equipment_value": 85000.0,
        "location_rent_sqm": 45.0
    },
    {
        "session_name": "Fine Dining Montreal",
        "restaurant_theme": "fine_dining",
        "revenue_size": "large",
        "kitchen_size_sqm": 150.0,
        "kitchen_workstations": 10,
        "daily_capacity": 120,
        "staff_count": 25,
        "staff_experience_level": "experienced",
        "training_hours_needed": 20,
        "equipment_age_years": 1,
        "equipment_condition": "excellent",
        "equipment_value": 250000.0,
        "location_rent_sqm": 65.0
    },
    {
        "session_name": "Cloud Kitchen Vancouver",
        "restaurant_theme": "cloud_kitchen",
        "revenue_size": "small",
        "kitchen_size_sqm": 40.0,
        "kitchen_workstations": 4,
        "daily_capacity": 150,
        "staff_count": 6,
        "staff_experience_level": "intermediate",
        "training_hours_needed": 30,
        "equipment_age_years": 3,
        "equipment_condition": "fair",
        "equipment_value": 45000.0,
        "location_rent_sqm": 25.0
    },
    {
        "session_name": "Food Truck Calgary",
        "restaurant_theme": "food_truck",
        "revenue_size": "small",
        "kitchen_size_sqm": 15.0,
        "kitchen_workstations": 2,
        "daily_capacity": 80,
        "staff_count": 3,
        "staff_experience_level": "intermediate",
        "training_hours_needed": 25,
        "equipment_age_years": 4,
        "equipment_condition": "good",
        "equipment_value": 35000.0,
        "location_rent_sqm": 0.0  # Pas de loyer fixe pour food truck
    },
    {
        "session_name": "Casual Dining Ottawa",
        "restaurant_theme": "casual_dining",
        "revenue_size": "medium",
        "kitchen_size_sqm": 100.0,
        "kitchen_workstations": 8,
        "daily_capacity": 200,
        "staff_count": 18,
        "staff_experience_level": "intermediate",
        "training_hours_needed": 35,
        "equipment_age_years": 3,
        "equipment_condition": "good",
        "equipment_value": 120000.0,
        "location_rent_sqm": 40.0
    }
]


class TestDataManager:
    """Gestionnaire des données de test"""
    
    def __init__(self):
        self.db_manager = DatabaseManager()
        self.daos = None
    
    def setup(self):
        """Initialise la connexion et les DAOs"""
        self.db_manager.connect()
        self.daos = get_all_daos(self.db_manager)
    
    def teardown(self):
        """Ferme la connexion"""
        self.db_manager.disconnect()
    
    def insert_test_sessions(self) -> List[int]:
        """
        Insère toutes les sessions de test
        
        Returns:
            Liste des IDs des sessions créées
        """
        session_ids = []
        
        for session_data in TEST_SESSIONS:
            try:
                session_id = self.daos['inputs'].create_input_session(session_data)
                session_ids.append(session_id)
                logger.info(f"Session créée: {session_data['session_name']} (ID: {session_id})")
            except Exception as e:
                logger.error(f"Erreur création session {session_data['session_name']}: {e}")
        
        return session_ids
    
    def validate_test_data(self):
        """Valide que les données de test ont été correctement insérées"""
        try:
            # Vérifier le nombre de sessions
            sessions = self.daos['inputs'].list_sessions()
            print(f"📊 Nombre de sessions créées: {len(sessions)}")
            
            # Vérifier les facteurs de coût
            for category in ['staff', 'equipment', 'location', 'operations']:
                factors = self.daos['cost_factors'].get_factors_by_category(category)
                print(f"📋 Facteurs {category}: {len(factors)} entrées")
            
            # Afficher un résumé des sessions
            print("\n📝 Sessions de test créées:")
            for session in sessions:
                print(f"  • {session['session_name']} ({session['restaurant_theme']}, {session['revenue_size']})")
            
            return True
            
        except Exception as e:
            logger.error(f"Erreur validation: {e}")
            return False
    
    def run_full_test(self):
        """Exécute le test complet"""
        try:
            self.setup()
            session_ids = self.insert_test_sessions()
            
            if session_ids:
                print(f"✅ {len(session_ids)} sessions de test créées avec succès")
                self.validate_test_data()
            else:
                print("❌ Aucune session de test créée")
            
        except Exception as e:
            logger.error(f"Erreur test complet: {e}")
            print(f"❌ Erreur: {e}")
        finally:
            self.teardown()


def test_database_operations():
    """Test des opérations de base de données"""
    print("🧪 Test des opérations de base de données...")
    
    db_manager = DatabaseManager()
    try:
        db_manager.connect()
        
        # Test de requête simple
        result = db_manager.execute_query("SELECT COUNT(*) as count FROM cost_factors")
        factor_count = result[0]['count']
        print(f"✅ Facteurs de coût dans la DB: {factor_count}")
        
        # Test de récupération par catégorie
        daos = get_all_daos(db_manager)
        staff_factors = daos['cost_factors'].get_factors_by_category('staff')
        print(f"✅ Facteurs de personnel: {len(staff_factors)}")
        
        return True
        
    except Exception as e:
        print(f"❌ Erreur test DB: {e}")
        return False
    finally:
        db_manager.disconnect()


if __name__ == "__main__":
    print("🚀 Démarrage des tests de données Track-A-FACE")
    
    # Test des opérations de base
    if test_database_operations():
        print("✅ Tests de base réussis")
        
        # Test complet avec données
        test_manager = TestDataManager()
        test_manager.run_full_test()
    else:
        print("❌ Échec des tests de base")
