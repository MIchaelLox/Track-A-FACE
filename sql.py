# ==========================
# /sql.py
# ==========================

"""
Module de gestion de la base de données SQLite pour Track-A-FACE
Gère la création des tables, connexions, et opérations CRUD
"""

import sqlite3
import logging
from pathlib import Path
from typing import Dict, List, Any, Optional
from config import DATABASE_CONFIG, LOGGING_CONFIG

# Configuration du logging
logging.basicConfig(
    level=getattr(logging, LOGGING_CONFIG["level"]),
    format=LOGGING_CONFIG["format"]
)
logger = logging.getLogger(__name__)


class DatabaseManager:
    """Gestionnaire principal de la base de données SQLite"""
    
    def __init__(self, db_path: Optional[Path] = None):
        """
        Initialise la connexion à la base de données
        
        Args:
            db_path: Chemin vers le fichier SQLite (optionnel)
        """
        self.db_path = db_path or DATABASE_CONFIG["path"]
        self.connection = None
        self._ensure_data_directory()
    
    def _ensure_data_directory(self):
        """Crée le dossier data s'il n'existe pas"""
        self.db_path.parent.mkdir(parents=True, exist_ok=True)
    
    def connect(self) -> sqlite3.Connection:
        """
        Établit la connexion à la base de données
        
        Returns:
            Connection SQLite
        """
        try:
            self.connection = sqlite3.connect(str(self.db_path))
            self.connection.row_factory = sqlite3.Row  # Pour accès par nom de colonne
            logger.info(f"Connexion établie à la base de données: {self.db_path}")
            return self.connection
        except sqlite3.Error as e:
            logger.error(f"Erreur de connexion à la base de données: {e}")
            raise
    
    def disconnect(self):
        """Ferme la connexion à la base de données"""
        if self.connection:
            self.connection.close()
            self.connection = None
            logger.info("Connexion fermée")
    
    def execute_sql_file(self, sql_file_path: Path):
        """
        Exécute un fichier SQL complet
        
        Args:
            sql_file_path: Chemin vers le fichier SQL
        """
        if not self.connection:
            self.connect()
        
        try:
            with open(sql_file_path, 'r', encoding='utf-8') as f:
                sql_script = f.read()
            
            # Exécuter le script complet
            self.connection.executescript(sql_script)
            self.connection.commit()
            logger.info(f"Script SQL exécuté avec succès: {sql_file_path}")
            
        except (sqlite3.Error, FileNotFoundError) as e:
            logger.error(f"Erreur lors de l'exécution du script SQL: {e}")
            raise
    
    def create_tables(self):
        """Crée toutes les tables du schéma"""
        schema_file = Path(__file__).parent / "database_schema.sql"
        self.execute_sql_file(schema_file)
    
    def execute_query(self, query: str, params: tuple = ()) -> List[sqlite3.Row]:
        """
        Exécute une requête SELECT
        
        Args:
            query: Requête SQL
            params: Paramètres de la requête
            
        Returns:
            Liste des résultats
        """
        if not self.connection:
            self.connect()
        
        try:
            cursor = self.connection.execute(query, params)
            return cursor.fetchall()
        except sqlite3.Error as e:
            logger.error(f"Erreur lors de l'exécution de la requête: {e}")
            raise
    
    def execute_insert(self, query: str, params: tuple = ()) -> int:
        """
        Exécute une requête INSERT
        
        Args:
            query: Requête SQL INSERT
            params: Paramètres de la requête
            
        Returns:
            ID de l'enregistrement inséré
        """
        if not self.connection:
            self.connect()
        
        try:
            cursor = self.connection.execute(query, params)
            self.connection.commit()
            logger.info(f"Enregistrement inséré avec ID: {cursor.lastrowid}")
            return cursor.lastrowid
        except sqlite3.Error as e:
            logger.error(f"Erreur lors de l'insertion: {e}")
            raise
    
    def execute_update(self, query: str, params: tuple = ()) -> int:
        """
        Exécute une requête UPDATE
        
        Args:
            query: Requête SQL UPDATE
            params: Paramètres de la requête
            
        Returns:
            Nombre de lignes affectées
        """
        if not self.connection:
            self.connect()
        
        try:
            cursor = self.connection.execute(query, params)
            self.connection.commit()
            rows_affected = cursor.rowcount
            logger.info(f"Mise à jour effectuée: {rows_affected} lignes affectées")
            return rows_affected
        except sqlite3.Error as e:
            logger.error(f"Erreur lors de la mise à jour: {e}")
            raise


class InputsDAO:
    """Data Access Object pour la table inputs"""
    
    def __init__(self, db_manager: DatabaseManager):
        self.db = db_manager
    
    def create_input_session(self, session_data: Dict[str, Any]) -> int:
        """
        Crée une nouvelle session d'inputs
        
        Args:
            session_data: Dictionnaire avec les données de session
            
        Returns:
            ID de la session créée
        """
        query = """
        INSERT INTO inputs (
            session_name, restaurant_theme, revenue_size, kitchen_size_sqm,
            kitchen_workstations, daily_capacity, staff_count, staff_experience_level,
            training_hours_needed, equipment_age_years, equipment_condition,
            equipment_value, location_rent_sqm
        ) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
        """
        
        params = (
            session_data['session_name'],
            session_data['restaurant_theme'],
            session_data['revenue_size'],
            session_data['kitchen_size_sqm'],
            session_data['kitchen_workstations'],
            session_data['daily_capacity'],
            session_data['staff_count'],
            session_data['staff_experience_level'],
            session_data.get('training_hours_needed', 0),
            session_data.get('equipment_age_years', 0),
            session_data['equipment_condition'],
            session_data['equipment_value'],
            session_data['location_rent_sqm']
        )
        
        return self.db.execute_insert(query, params)
    
    def get_input_session(self, session_id: int) -> Optional[sqlite3.Row]:
        """Récupère une session par ID"""
        query = "SELECT * FROM inputs WHERE id = ?"
        results = self.db.execute_query(query, (session_id,))
        return results[0] if results else None
    
    def list_sessions(self) -> List[sqlite3.Row]:
        """Liste toutes les sessions"""
        query = "SELECT * FROM inputs ORDER BY created_at DESC"
        return self.db.execute_query(query)


class CostFactorsDAO:
    """Data Access Object pour la table cost_factors"""
    
    def __init__(self, db_manager: DatabaseManager):
        self.db = db_manager
    
    def get_factors_by_category(self, category: str, theme: str = None, revenue_size: str = None) -> List[sqlite3.Row]:
        """
        Récupère les facteurs de coût par catégorie
        
        Args:
            category: Catégorie de facteur
            theme: Thème de restaurant (optionnel)
            revenue_size: Taille de revenus (optionnel)
            
        Returns:
            Liste des facteurs correspondants
        """
        query = """
        SELECT * FROM cost_factors 
        WHERE factor_category = ? 
        AND is_active = 1
        AND (restaurant_theme IS NULL OR restaurant_theme = ?)
        AND (revenue_size IS NULL OR revenue_size = ?)
        ORDER BY factor_name
        """
        
        return self.db.execute_query(query, (category, theme, revenue_size))
    
    def get_factor_value(self, factor_name: str, theme: str = None, revenue_size: str = None) -> Optional[float]:
        """
        Récupère la valeur d'un facteur spécifique
        
        Args:
            factor_name: Nom du facteur
            theme: Thème de restaurant (optionnel)
            revenue_size: Taille de revenus (optionnel)
            
        Returns:
            Valeur du facteur ou None
        """
        query = """
        SELECT multiplier, base_cost FROM cost_factors 
        WHERE factor_name = ?
        AND is_active = 1
        AND (restaurant_theme IS NULL OR restaurant_theme = ?)
        AND (revenue_size IS NULL OR revenue_size = ?)
        ORDER BY restaurant_theme DESC, revenue_size DESC
        LIMIT 1
        """
        
        results = self.db.execute_query(query, (factor_name, theme, revenue_size))
        if results:
            row = results[0]
            return row['base_cost'] if row['base_cost'] > 0 else row['multiplier']
        return None


class CalculationResultsDAO:
    """Data Access Object pour la table calculation_results"""
    
    def __init__(self, db_manager: DatabaseManager):
        self.db = db_manager
    
    def save_calculation_result(self, session_id: int, category: str, subcategory: str, 
                               amount: float, formula: str) -> int:
        """
        Sauvegarde un résultat de calcul
        
        Args:
            session_id: ID de la session
            category: Catégorie de coût
            subcategory: Sous-catégorie
            amount: Montant calculé
            formula: Formule utilisée
            
        Returns:
            ID du résultat créé
        """
        query = """
        INSERT INTO calculation_results (
            input_session_id, cost_category, cost_subcategory,
            calculated_amount, formula_used
        ) VALUES (?, ?, ?, ?, ?)
        """
        
        return self.db.execute_insert(query, (session_id, category, subcategory, amount, formula))
    
    def get_results_by_session(self, session_id: int) -> List[sqlite3.Row]:
        """Récupère tous les résultats d'une session"""
        query = """
        SELECT * FROM calculation_results 
        WHERE input_session_id = ? 
        ORDER BY cost_category, cost_subcategory
        """
        return self.db.execute_query(query, (session_id,))


class ScenariosDAO:
    """Data Access Object pour la table scenarios"""
    
    def __init__(self, db_manager: DatabaseManager):
        self.db = db_manager
    
    def create_scenario(self, comparison_name: str, scenario_name: str, 
                       session_id: int, total_cost: float) -> int:
        """Crée un nouveau scénario"""
        query = """
        INSERT INTO scenarios (comparison_name, scenario_name, input_session_id, total_cost)
        VALUES (?, ?, ?, ?)
        """
        return self.db.execute_insert(query, (comparison_name, scenario_name, session_id, total_cost))
    
    def get_comparison_scenarios(self, comparison_name: str) -> List[sqlite3.Row]:
        """Récupère tous les scénarios d'une comparaison"""
        query = """
        SELECT s.*, i.session_name, i.restaurant_theme 
        FROM scenarios s
        JOIN inputs i ON s.input_session_id = i.id
        WHERE s.comparison_name = ?
        ORDER BY s.created_at
        """
        return self.db.execute_query(query, (comparison_name,))


# Classe principale pour l'initialisation
class DatabaseInitializer:
    """Classe pour initialiser et configurer la base de données"""
    
    def __init__(self):
        self.db_manager = DatabaseManager()
    
    def initialize_database(self):
        """Initialise complètement la base de données"""
        try:
            # Connexion
            self.db_manager.connect()
            
            # Création des tables et insertion des données de base
            self.db_manager.create_tables()
            
            logger.info("Base de données initialisée avec succès")
            return True
            
        except Exception as e:
            logger.error(f"Erreur lors de l'initialisation: {e}")
            return False
        finally:
            self.db_manager.disconnect()
    
    def reset_database(self):
        """Remet à zéro la base de données (ATTENTION: supprime toutes les données)"""
        try:
            if self.db_manager.db_path.exists():
                self.db_manager.db_path.unlink()
                logger.info("Base de données supprimée")
            
            return self.initialize_database()
            
        except Exception as e:
            logger.error(f"Erreur lors de la remise à zéro: {e}")
            return False


# Fonctions utilitaires
def get_database_manager() -> DatabaseManager:
    """Factory function pour obtenir un gestionnaire de DB"""
    return DatabaseManager()

def get_all_daos(db_manager: DatabaseManager) -> Dict[str, Any]:
    """
    Retourne tous les DAOs configurés
    
    Args:
        db_manager: Gestionnaire de base de données
        
    Returns:
        Dictionnaire avec tous les DAOs
    """
    return {
        'inputs': InputsDAO(db_manager),
        'cost_factors': CostFactorsDAO(db_manager),
        'results': CalculationResultsDAO(db_manager),
        'scenarios': ScenariosDAO(db_manager)
    }


if __name__ == "__main__":
    # Test d'initialisation
    initializer = DatabaseInitializer()
    if initializer.initialize_database():
        print("✅ Base de données Track-A-FACE initialisée avec succès")
    else:
        print("❌ Erreur lors de l'initialisation de la base de données")
