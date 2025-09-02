# ==========================
# /config.py
# ==========================

"""
Configuration centrale pour Track-A-FACE
Paramètres de base de données, logging, et constantes métier
"""

import os
from pathlib import Path

# Chemins du projet
PROJECT_ROOT = Path(__file__).parent
DATA_DIR = PROJECT_ROOT / "data"
LOGS_DIR = PROJECT_ROOT / "logs"
EXPORTS_DIR = PROJECT_ROOT / "exports"

# Configuration base de données
DATABASE_CONFIG = {
    "type": "sqlite",
    "path": DATA_DIR / "face_engine.db",
    "backup_path": DATA_DIR / "backups"
}

# Configuration logging
LOGGING_CONFIG = {
    "level": "INFO",
    "format": "%(asctime)s - %(name)s - %(levelname)s - %(message)s",
    "file": LOGS_DIR / "face_engine.log"
}

# Constantes métier (Canada - CAD$)
BUSINESS_CONSTANTS = {
    "default_staff_hourly_rate": 18.0,  # CAD$/heure
    "equipment_depreciation_years": 5,
    "training_hours_per_employee": 40,
    "default_kitchen_sqm_cost": 35.0,  # CAD$/m²
    "currency": "CAD"
}

# Types de restaurants (thèmes)
RESTAURANT_THEMES = [
    "fast_food",
    "casual_dining", 
    "fine_dining",
    "cloud_kitchen",
    "food_truck"
]

# Tailles de revenus
REVENUE_SIZES = [
    "small",    # < 500k
    "medium",   # 500k - 2M
    "large",    # 2M - 10M
    "enterprise" # > 10M
]

# Créer les dossiers nécessaires
def setup_directories():
    """Crée les dossiers nécessaires au projet"""
    for directory in [DATA_DIR, LOGS_DIR, EXPORTS_DIR]:
        directory.mkdir(exist_ok=True)

if __name__ == "__main__":
    setup_directories()
    print("Configuration initialisée avec succès")
