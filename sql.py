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
    
    def __init__(self, db_path: str = None):
        """
        Initialise le gestionnaire de base de données
        
        Args:
            db_path: Chemin vers le fichier SQLite (optionnel)
        """
        self.db_path = Path(db_path) if db_path else DATABASE_CONFIG["path"]
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
        """Initialise la base de données avec le schéma et les données de base"""
        try:
            self.db_manager.connect()
            
            # Créer les tables depuis le schéma
            self._create_tables_from_schema()
            
            # Insérer les facteurs de coût avancés
            self._insert_advanced_cost_factors()
            
            # Insérer des données d'exemple
            self._insert_sample_data()
            
            print("✅ Base de données initialisée avec succès")
            
        except Exception as e:
            print(f"❌ Erreur lors de l'initialisation: {e}")
            raise
        finally:
            self.db_manager.disconnect()
    
    def _create_tables_from_schema(self):
        """Crée les tables depuis le fichier de schéma"""
        schema_path = "database_schema.sql"
        try:
            with open(schema_path, 'r', encoding='utf-8') as f:
                schema_sql = f.read()
            
            # Exécuter le script complet
            self.db_manager.connection.executescript(schema_sql)
            self.db_manager.connection.commit()
            print("📋 Tables créées depuis le schéma")
            
        except FileNotFoundError:
            print("⚠️ Fichier schema non trouvé, création manuelle des tables")
            self._create_tables_manually()
    
    def _create_tables_manually(self):
        """Crée les tables manuellement si le schéma n'est pas disponible"""
        tables = [
            """
            CREATE TABLE IF NOT EXISTS inputs (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                session_name VARCHAR(100) NOT NULL,
                restaurant_theme VARCHAR(50) NOT NULL,
                revenue_size VARCHAR(20) NOT NULL,
                staff_count INTEGER NOT NULL,
                training_hours_needed INTEGER,
                daily_capacity INTEGER NOT NULL,
                kitchen_size_sqm DECIMAL(8,2),
                equipment_value DECIMAL(12,2),
                equipment_condition VARCHAR(20),
                equipment_age_years INTEGER,
                location_rent_sqm DECIMAL(8,2),
                created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
            )
            """,
            """
            CREATE TABLE IF NOT EXISTS cost_factors (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                factor_name VARCHAR(100) NOT NULL,
                factor_category VARCHAR(50) NOT NULL,
                restaurant_theme VARCHAR(50),
                revenue_size VARCHAR(20),
                multiplier DECIMAL(8,4) NOT NULL DEFAULT 1.0,
                base_cost DECIMAL(10,2) DEFAULT 0,
                description TEXT,
                is_active BOOLEAN DEFAULT 1,
                created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
            )
            """,
            """
            CREATE TABLE IF NOT EXISTS calculation_results (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                input_session_id INTEGER NOT NULL,
                cost_category VARCHAR(50) NOT NULL,
                cost_subcategory VARCHAR(50),
                calculated_amount DECIMAL(12,2) NOT NULL,
                formula_used TEXT,
                calculation_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                FOREIGN KEY (input_session_id) REFERENCES inputs(id) ON DELETE CASCADE
            )
            """,
            """
            CREATE TABLE IF NOT EXISTS scenarios (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                comparison_name VARCHAR(100) NOT NULL,
                scenario_name VARCHAR(100) NOT NULL,
                input_session_id INTEGER NOT NULL,
                total_cost DECIMAL(12,2),
                created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                FOREIGN KEY (input_session_id) REFERENCES inputs(id) ON DELETE CASCADE
            )
            """
        ]
        
        for table_sql in tables:
            self.db_manager.connection.execute(table_sql)
        
        self.db_manager.connection.commit()
    
    def _insert_advanced_cost_factors(self):
        """Insère les facteurs de coût avancés pour les formules pondérées"""
        
        # Facteurs de formation
        training_factors = [
            ('training_rate_per_hour', 'training', 'fast_food', None, 1.0, 25.0, 'Taux horaire formation fast food'),
            ('training_rate_per_hour', 'training', 'casual_dining', None, 1.0, 30.0, 'Taux horaire formation casual dining'),
            ('training_rate_per_hour', 'training', 'fine_dining', None, 1.0, 45.0, 'Taux horaire formation fine dining'),
            ('training_rate_per_hour', 'training', 'cloud_kitchen', None, 1.0, 28.0, 'Taux horaire formation cloud kitchen'),
            ('training_rate_per_hour', 'training', 'food_truck', None, 1.0, 22.0, 'Taux horaire formation food truck'),
            
            ('training_complexity_factor', 'training', 'fast_food', None, 0.8, 0, 'Facteur complexité formation fast food'),
            ('training_complexity_factor', 'training', 'casual_dining', None, 1.0, 0, 'Facteur complexité formation casual dining'),
            ('training_complexity_factor', 'training', 'fine_dining', None, 1.6, 0, 'Facteur complexité formation fine dining'),
            ('training_complexity_factor', 'training', 'cloud_kitchen', None, 0.9, 0, 'Facteur complexité formation cloud kitchen'),
            ('training_complexity_factor', 'training', 'food_truck', None, 0.7, 0, 'Facteur complexité formation food truck'),
            
            ('training_size_factor', 'training', None, 'small', 1.1, 0, 'Facteur taille formation small'),
            ('training_size_factor', 'training', None, 'medium', 1.0, 0, 'Facteur taille formation medium'),
            ('training_size_factor', 'training', None, 'large', 0.9, 0, 'Facteur taille formation large'),
            ('training_size_factor', 'training', None, 'enterprise', 0.8, 0, 'Facteur taille formation enterprise'),
        ]
        
        # Facteurs salariaux
        salary_factors = [
            ('salary_theme_multiplier', 'salary', 'fast_food', None, 0.85, 0, 'Multiplicateur salarial fast food'),
            ('salary_theme_multiplier', 'salary', 'casual_dining', None, 1.0, 0, 'Multiplicateur salarial casual dining'),
            ('salary_theme_multiplier', 'salary', 'fine_dining', None, 1.45, 0, 'Multiplicateur salarial fine dining'),
            ('salary_theme_multiplier', 'salary', 'cloud_kitchen', None, 0.95, 0, 'Multiplicateur salarial cloud kitchen'),
            ('salary_theme_multiplier', 'salary', 'food_truck', None, 0.80, 0, 'Multiplicateur salarial food truck'),
            
            ('salary_revenue_multiplier', 'salary', None, 'small', 0.88, 0, 'Multiplicateur salarial small'),
            ('salary_revenue_multiplier', 'salary', None, 'medium', 1.0, 0, 'Multiplicateur salarial medium'),
            ('salary_revenue_multiplier', 'salary', None, 'large', 1.18, 0, 'Multiplicateur salarial large'),
            ('salary_revenue_multiplier', 'salary', None, 'enterprise', 1.35, 0, 'Multiplicateur salarial enterprise'),
        ]
        
        # Facteurs d'équipement
        equipment_factors = [
            ('depreciation_rate', 'equipment', 'fast_food', None, 0.22, 0, 'Taux dépréciation fast food'),
            ('depreciation_rate', 'equipment', 'casual_dining', None, 0.20, 0, 'Taux dépréciation casual dining'),
            ('depreciation_rate', 'equipment', 'fine_dining', None, 0.18, 0, 'Taux dépréciation fine dining'),
            ('depreciation_rate', 'equipment', 'cloud_kitchen', None, 0.25, 0, 'Taux dépréciation cloud kitchen'),
            ('depreciation_rate', 'equipment', 'food_truck', None, 0.28, 0, 'Taux dépréciation food truck'),
            
            ('equipment_theme_factor', 'equipment', 'fast_food', None, 1.3, 0, 'Facteur usure équipement fast food'),
            ('equipment_theme_factor', 'equipment', 'casual_dining', None, 1.0, 0, 'Facteur usure équipement casual dining'),
            ('equipment_theme_factor', 'equipment', 'fine_dining', None, 0.8, 0, 'Facteur usure équipement fine dining'),
            ('equipment_theme_factor', 'equipment', 'cloud_kitchen', None, 1.2, 0, 'Facteur usure équipement cloud kitchen'),
            ('equipment_theme_factor', 'equipment', 'food_truck', None, 1.4, 0, 'Facteur usure équipement food truck'),
            
            ('maintenance_rate', 'equipment', 'fast_food', None, 0.09, 0, 'Taux maintenance fast food'),
            ('maintenance_rate', 'equipment', 'casual_dining', None, 0.08, 0, 'Taux maintenance casual dining'),
            ('maintenance_rate', 'equipment', 'fine_dining', None, 0.07, 0, 'Taux maintenance fine dining'),
            ('maintenance_rate', 'equipment', 'cloud_kitchen', None, 0.10, 0, 'Taux maintenance cloud kitchen'),
            ('maintenance_rate', 'equipment', 'food_truck', None, 0.12, 0, 'Taux maintenance food truck'),
            
            ('maintenance_complexity_factor', 'equipment', 'fast_food', None, 0.9, 0, 'Complexité maintenance fast food'),
            ('maintenance_complexity_factor', 'equipment', 'casual_dining', None, 1.0, 0, 'Complexité maintenance casual dining'),
            ('maintenance_complexity_factor', 'equipment', 'fine_dining', None, 1.3, 0, 'Complexité maintenance fine dining'),
            ('maintenance_complexity_factor', 'equipment', 'cloud_kitchen', None, 1.1, 0, 'Complexité maintenance cloud kitchen'),
            ('maintenance_complexity_factor', 'equipment', 'food_truck', None, 1.2, 0, 'Complexité maintenance food truck'),
        ]
        
        # Facteurs opérationnels
        operational_factors = [
            ('food_cost_per_cover', 'operations', 'fast_food', None, 1.0, 3.2, 'Coût alimentaire par couvert fast food'),
            ('food_cost_per_cover', 'operations', 'casual_dining', None, 1.0, 7.5, 'Coût alimentaire par couvert casual dining'),
            ('food_cost_per_cover', 'operations', 'fine_dining', None, 1.0, 22.0, 'Coût alimentaire par couvert fine dining'),
            ('food_cost_per_cover', 'operations', 'cloud_kitchen', None, 1.0, 4.8, 'Coût alimentaire par couvert cloud kitchen'),
            ('food_cost_per_cover', 'operations', 'food_truck', None, 1.0, 3.8, 'Coût alimentaire par couvert food truck'),
            
            ('food_efficiency_factor', 'operations', None, 'small', 1.05, 0, 'Efficacité alimentaire small'),
            ('food_efficiency_factor', 'operations', None, 'medium', 1.0, 0, 'Efficacité alimentaire medium'),
            ('food_efficiency_factor', 'operations', None, 'large', 0.92, 0, 'Efficacité alimentaire large'),
            ('food_efficiency_factor', 'operations', None, 'enterprise', 0.85, 0, 'Efficacité alimentaire enterprise'),
            
            ('marketing_budget_base', 'operations', None, 'small', 1.0, 12000, 'Budget marketing base small'),
            ('marketing_budget_base', 'operations', None, 'medium', 1.0, 32000, 'Budget marketing base medium'),
            ('marketing_budget_base', 'operations', None, 'large', 1.0, 72000, 'Budget marketing base large'),
            ('marketing_budget_base', 'operations', None, 'enterprise', 1.0, 145000, 'Budget marketing base enterprise'),
            
            ('marketing_theme_factor', 'operations', 'fast_food', None, 1.25, 0, 'Facteur marketing fast food'),
            ('marketing_theme_factor', 'operations', 'casual_dining', None, 1.0, 0, 'Facteur marketing casual dining'),
            ('marketing_theme_factor', 'operations', 'fine_dining', None, 0.75, 0, 'Facteur marketing fine dining'),
            ('marketing_theme_factor', 'operations', 'cloud_kitchen', None, 1.6, 0, 'Facteur marketing cloud kitchen'),
            ('marketing_theme_factor', 'operations', 'food_truck', None, 0.55, 0, 'Facteur marketing food truck'),
            
            ('seasonal_price_factor', 'operations', None, None, 1.05, 0, 'Facteur variations saisonnières prix'),
        ]
        
        # Facteurs immobiliers
        location_factors = [
            ('rent_factor', 'location', 'fast_food', None, 1.1, 0, 'Facteur loyer fast food'),
            ('rent_factor', 'location', 'casual_dining', None, 1.0, 0, 'Facteur loyer casual dining'),
            ('rent_factor', 'location', 'fine_dining', None, 1.3, 0, 'Facteur loyer fine dining'),
            ('rent_factor', 'location', 'cloud_kitchen', None, 0.8, 0, 'Facteur loyer cloud kitchen'),
            ('rent_factor', 'location', 'food_truck', None, 0.0, 0, 'Facteur loyer food truck'),
        ]
        
        # Combiner tous les facteurs
        all_factors = training_factors + salary_factors + equipment_factors + operational_factors + location_factors
        
        # Insérer les facteurs
        insert_query = """
        INSERT OR REPLACE INTO cost_factors 
        (factor_name, factor_category, restaurant_theme, revenue_size, multiplier, base_cost, description)
        VALUES (?, ?, ?, ?, ?, ?, ?)
        """
        
        for factor in all_factors:
            self.db_manager.connection.execute(insert_query, factor)
        
        self.db_manager.connection.commit()
        print(f"📊 {len(all_factors)} facteurs de coût avancés insérés")

    def _insert_sample_data(self):
        """Insert sample input sessions for quick tests and demos.

        Note: inputs.kitchen_workstations and staff_experience_level are NOT NULL in the current schema,
        so sample data must provide values for these columns to avoid NOT NULL constraint errors.
        """
        
        sample_inputs = [
            (
                'Restaurant Test 1',  # session_name
                'casual_dining',  # restaurant_theme
                'medium',  # revenue_size
                85.0,  # kitchen_size_sqm
                8,  # kitchen_workstations
                120,  # daily_capacity
                10,  # staff_count
                'intermediate',  # staff_experience_level
                40,  # training_hours_needed
                3,  # equipment_age_years
                'good',  # equipment_condition
                150000.0,  # equipment_value
                45.0  # location_rent_sqm
            ),
            (
                'Fast Food Test',
                'fast_food',
                'small',
                60.0,
                5,
                200,
                7,
                'beginner',
                20,
                2,
                'fair',
                80000.0,
                35.0
            ),
            (
                'Fine Dining Test',
                'fine_dining',
                'large',
                120.0,
                10,
                80,
                20,
                'experienced',
                80,
                1,
                'excellent',
                300000.0,
                85.0
            ),
        ]

        insert_query = """
        INSERT OR REPLACE INTO inputs (
            session_name,
            restaurant_theme,
            revenue_size,
            kitchen_size_sqm,
            kitchen_workstations,
            daily_capacity,
            staff_count,
            staff_experience_level,
            training_hours_needed,
            equipment_age_years,
            equipment_condition,
            equipment_value,
            location_rent_sqm
        ) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
        """

        for sample in sample_inputs:
            self.db_manager.connection.execute(insert_query, sample)

        self.db_manager.connection.commit()
        print(f"🧪 {len(sample_inputs)} sessions d'exemple insérées")


# Classe pour la gestion avancée des facteurs de coût
class AdvancedCostFactorsManager:
    """Gestionnaire avancé pour les facteurs de coût avec cache et optimisations"""
    
    def __init__(self, db_manager: DatabaseManager):
        self.db_manager = db_manager
        self.dao = CostFactorsDAO(db_manager)
        self._cache = {}  # Cache pour les facteurs fréquemment utilisés
        self._cache_timeout = 300  # 5 minutes
        self._last_cache_update = 0
    
    def get_factor_with_fallback(self, factor_name: str, theme: str = None, 
                                revenue_size: str = None, default_value: float = 1.0) -> float:
        """Récupère un facteur avec système de fallback intelligent"""
        import time
        
        # Vérifier le cache
        cache_key = f"{factor_name}_{theme}_{revenue_size}"
        current_time = time.time()
        
        if (cache_key in self._cache and 
            current_time - self._last_cache_update < self._cache_timeout):
            return self._cache[cache_key]
        
        # Essayer de récupérer le facteur spécifique
        value = self.dao.get_factor_value(factor_name, theme, revenue_size)
        
        if value is not None:
            self._cache[cache_key] = value
            return value
        
        # Fallback 1: Essayer sans le revenue_size
        if revenue_size:
            value = self.dao.get_factor_value(factor_name, theme, None)
            if value is not None:
                self._cache[cache_key] = value
                return value
        
        # Fallback 2: Essayer sans le theme
        if theme:
            value = self.dao.get_factor_value(factor_name, None, revenue_size)
            if value is not None:
                self._cache[cache_key] = value
                return value
        
        # Fallback 3: Facteur générique
        value = self.dao.get_factor_value(factor_name, None, None)
        if value is not None:
            self._cache[cache_key] = value
            return value
        
        # Fallback final: valeur par défaut
        self._cache[cache_key] = default_value
        return default_value
    
    def get_all_factors_for_calculation(self, theme: str, revenue_size: str) -> Dict[str, float]:
        """Récupère tous les facteurs nécessaires pour un calcul en une seule fois"""
        factors = {}
        
        # Liste des facteurs essentiels
        essential_factors = [
            'training_rate_per_hour', 'training_complexity_factor', 'training_size_factor',
            'salary_theme_multiplier', 'salary_revenue_multiplier',
            'depreciation_rate', 'equipment_theme_factor', 'maintenance_rate', 'maintenance_complexity_factor',
            'food_cost_per_cover', 'food_efficiency_factor', 'marketing_budget_base', 'marketing_theme_factor',
            'rent_factor', 'seasonal_price_factor'
        ]
        
        for factor_name in essential_factors:
            factors[factor_name] = self.get_factor_with_fallback(factor_name, theme, revenue_size)
        
        return factors
    
    def clear_cache(self):
        """Vide le cache des facteurs"""
        self._cache.clear()
        self._last_cache_update = 0


# Fonction utilitaire pour obtenir tous les DAOs
def get_all_daos(db_manager: DatabaseManager) -> Dict[str, Any]:
    """Retourne un dictionnaire de tous les DAOs disponibles"""
    return {
        'inputs': InputsDAO(db_manager),
        'cost_factors': CostFactorsDAO(db_manager),
        'results': CalculationResultsDAO(db_manager),
        'scenarios': ScenariosDAO(db_manager),
        'advanced_factors': AdvancedCostFactorsManager(db_manager)
    }


if __name__ == "__main__":
    # Test d'initialisation
    try:
        initializer = DatabaseInitializer()
        initializer.initialize_database()
        print("✅ Base de données Track-A-FACE initialisée avec succès")
    except Exception:
        print("❌ Erreur lors de l'initialisation de la base de données")
