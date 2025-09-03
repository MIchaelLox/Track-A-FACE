# ==========================
# /input_handler.py
# ==========================

"""
Module de gestion des entrées utilisateur pour Track-A-FACE
Gère la collecte, validation et sanitisation des paramètres d'entrée
"""

import re
from typing import Dict, Any, List, Optional, Union
from dataclasses import dataclass, field
from config import RESTAURANT_THEMES, REVENUE_SIZES, BUSINESS_CONSTANTS
import logging

logger = logging.getLogger(__name__)


@dataclass
class RestaurantInputs:
    """Structure de données pour les entrées de restaurant"""
    session_name: str
    restaurant_theme: str
    revenue_size: str
    kitchen_size_sqm: float
    kitchen_workstations: int
    daily_capacity: int
    staff_count: int
    staff_experience_level: str
    training_hours_needed: int = 0
    equipment_age_years: int = 0
    equipment_condition: str = "good"
    equipment_value: float = 0.0
    location_rent_sqm: float = 0.0
    
    def to_dict(self) -> Dict[str, Any]:
        """Convertit en dictionnaire pour la base de données"""
        return {
            'session_name': self.session_name,
            'restaurant_theme': self.restaurant_theme,
            'revenue_size': self.revenue_size,
            'kitchen_size_sqm': self.kitchen_size_sqm,
            'kitchen_workstations': self.kitchen_workstations,
            'daily_capacity': self.daily_capacity,
            'staff_count': self.staff_count,
            'staff_experience_level': self.staff_experience_level,
            'training_hours_needed': self.training_hours_needed,
            'equipment_age_years': self.equipment_age_years,
            'equipment_condition': self.equipment_condition,
            'equipment_value': self.equipment_value,
            'location_rent_sqm': self.location_rent_sqm
        }


class ValidationError(Exception):
    """Exception personnalisée pour les erreurs de validation"""
    pass


class InputValidator:
    """Classe de validation des entrées utilisateur"""
    
    # Constantes de validation
    MIN_KITCHEN_SIZE = 10.0  # m²
    MAX_KITCHEN_SIZE = 1000.0  # m²
    MIN_WORKSTATIONS = 1
    MAX_WORKSTATIONS = 50
    MIN_DAILY_CAPACITY = 10
    MAX_DAILY_CAPACITY = 2000
    MIN_STAFF_COUNT = 1
    MAX_STAFF_COUNT = 200
    MAX_EQUIPMENT_AGE = 30  # années
    MAX_EQUIPMENT_VALUE = 1000000.0  # CAD$
    MAX_RENT_PER_SQM = 200.0  # CAD$/m²
    
    EXPERIENCE_LEVELS = ['beginner', 'intermediate', 'experienced', 'expert']
    EQUIPMENT_CONDITIONS = ['excellent', 'good', 'fair', 'poor']
    
    @staticmethod
    def validate_session_name(name: str) -> str:
        """Valide et nettoie le nom de session"""
        if not name or not name.strip():
            raise ValidationError("Le nom de session est requis")
        
        # Nettoyer et limiter la longueur
        clean_name = re.sub(r'[^\w\s\-_]', '', name.strip())
        if len(clean_name) > 100:
            clean_name = clean_name[:100]
        
        if len(clean_name) < 3:
            raise ValidationError("Le nom de session doit contenir au moins 3 caractères")
        
        return clean_name
    
    @staticmethod
    def validate_restaurant_theme(theme: str) -> str:
        """Valide le thème de restaurant"""
        if theme not in RESTAURANT_THEMES:
            raise ValidationError(f"Thème invalide. Choix possibles: {', '.join(RESTAURANT_THEMES)}")
        return theme
    
    @staticmethod
    def validate_revenue_size(size: str) -> str:
        """Valide la taille de revenus"""
        if size not in REVENUE_SIZES:
            raise ValidationError(f"Taille de revenus invalide. Choix possibles: {', '.join(REVENUE_SIZES)}")
        return size
    
    @classmethod
    def validate_kitchen_size(cls, size: float) -> float:
        """Valide la taille de cuisine"""
        if not isinstance(size, (int, float)) or size <= 0:
            raise ValidationError("La taille de cuisine doit être un nombre positif")
        
        if size < cls.MIN_KITCHEN_SIZE:
            raise ValidationError(f"Taille de cuisine trop petite (minimum: {cls.MIN_KITCHEN_SIZE} m²)")
        
        if size > cls.MAX_KITCHEN_SIZE:
            raise ValidationError(f"Taille de cuisine trop grande (maximum: {cls.MAX_KITCHEN_SIZE} m²)")
        
        return float(size)
    
    @classmethod
    def validate_workstations(cls, count: int) -> int:
        """Valide le nombre de postes de travail"""
        if not isinstance(count, int) or count <= 0:
            raise ValidationError("Le nombre de postes doit être un entier positif")
        
        if count < cls.MIN_WORKSTATIONS or count > cls.MAX_WORKSTATIONS:
            raise ValidationError(f"Nombre de postes invalide ({cls.MIN_WORKSTATIONS}-{cls.MAX_WORKSTATIONS})")
        
        return count
    
    @classmethod
    def validate_daily_capacity(cls, capacity: int) -> int:
        """Valide la capacité quotidienne"""
        if not isinstance(capacity, int) or capacity <= 0:
            raise ValidationError("La capacité quotidienne doit être un entier positif")
        
        if capacity < cls.MIN_DAILY_CAPACITY or capacity > cls.MAX_DAILY_CAPACITY:
            raise ValidationError(f"Capacité invalide ({cls.MIN_DAILY_CAPACITY}-{cls.MAX_DAILY_CAPACITY})")
        
        return capacity
    
    @classmethod
    def validate_staff_count(cls, count: int) -> int:
        """Valide le nombre d'employés"""
        if not isinstance(count, int) or count <= 0:
            raise ValidationError("Le nombre d'employés doit être un entier positif")
        
        if count < cls.MIN_STAFF_COUNT or count > cls.MAX_STAFF_COUNT:
            raise ValidationError(f"Nombre d'employés invalide ({cls.MIN_STAFF_COUNT}-{cls.MAX_STAFF_COUNT})")
        
        return count
    
    @classmethod
    def validate_experience_level(cls, level: str) -> str:
        """Valide le niveau d'expérience"""
        if level not in cls.EXPERIENCE_LEVELS:
            raise ValidationError(f"Niveau d'expérience invalide. Choix: {', '.join(cls.EXPERIENCE_LEVELS)}")
        return level
    
    @classmethod
    def validate_equipment_condition(cls, condition: str) -> str:
        """Valide l'état de l'équipement"""
        if condition not in cls.EQUIPMENT_CONDITIONS:
            raise ValidationError(f"État d'équipement invalide. Choix: {', '.join(cls.EQUIPMENT_CONDITIONS)}")
        return condition
    
    @classmethod
    def validate_equipment_age(cls, age: int) -> int:
        """Valide l'âge de l'équipement"""
        if not isinstance(age, int) or age < 0:
            raise ValidationError("L'âge de l'équipement doit être un entier positif ou zéro")
        
        if age > cls.MAX_EQUIPMENT_AGE:
            raise ValidationError(f"Âge d'équipement trop élevé (maximum: {cls.MAX_EQUIPMENT_AGE} ans)")
        
        return age
    
    @classmethod
    def validate_equipment_value(cls, value: float) -> float:
        """Valide la valeur de l'équipement"""
        if not isinstance(value, (int, float)) or value < 0:
            raise ValidationError("La valeur d'équipement doit être un nombre positif ou zéro")
        
        if value > cls.MAX_EQUIPMENT_VALUE:
            raise ValidationError(f"Valeur d'équipement trop élevée (maximum: {cls.MAX_EQUIPMENT_VALUE:,.0f} CAD$)")
        
        return float(value)
    
    @classmethod
    def validate_rent_per_sqm(cls, rent: float) -> float:
        """Valide le loyer par m²"""
        if not isinstance(rent, (int, float)) or rent < 0:
            raise ValidationError("Le loyer doit être un nombre positif ou zéro")
        
        if rent > cls.MAX_RENT_PER_SQM:
            raise ValidationError(f"Loyer trop élevé (maximum: {cls.MAX_RENT_PER_SQM} CAD$/m²)")
        
        return float(rent)


class InputHandler:
    """Gestionnaire principal des entrées utilisateur"""
    
    def __init__(self):
        self.validator = InputValidator()
        self.current_inputs = None
    
    def create_inputs_from_dict(self, data: Dict[str, Any]) -> RestaurantInputs:
        """
        Crée et valide des entrées à partir d'un dictionnaire
        
        Args:
            data: Dictionnaire avec les données d'entrée
            
        Returns:
            RestaurantInputs validé
            
        Raises:
            ValidationError: Si les données sont invalides
        """
        try:
            # Validation et nettoyage de chaque champ
            validated_data = {
                'session_name': self.validator.validate_session_name(data['session_name']),
                'restaurant_theme': self.validator.validate_restaurant_theme(data['restaurant_theme']),
                'revenue_size': self.validator.validate_revenue_size(data['revenue_size']),
                'kitchen_size_sqm': self.validator.validate_kitchen_size(data['kitchen_size_sqm']),
                'kitchen_workstations': self.validator.validate_workstations(data['kitchen_workstations']),
                'daily_capacity': self.validator.validate_daily_capacity(data['daily_capacity']),
                'staff_count': self.validator.validate_staff_count(data['staff_count']),
                'staff_experience_level': self.validator.validate_experience_level(data['staff_experience_level']),
                'equipment_condition': self.validator.validate_equipment_condition(data['equipment_condition']),
            }
            
            # Champs optionnels avec valeurs par défaut
            validated_data['training_hours_needed'] = data.get('training_hours_needed', 0)
            validated_data['equipment_age_years'] = self.validator.validate_equipment_age(
                data.get('equipment_age_years', 0)
            )
            validated_data['equipment_value'] = self.validator.validate_equipment_value(
                data.get('equipment_value', 0.0)
            )
            validated_data['location_rent_sqm'] = self.validator.validate_rent_per_sqm(
                data.get('location_rent_sqm', 0.0)
            )
            
            # Créer l'objet RestaurantInputs
            inputs = RestaurantInputs(**validated_data)
            
            # Validations croisées
            self._validate_cross_references(inputs)
            
            self.current_inputs = inputs
            logger.info(f"Entrées validées pour la session: {inputs.session_name}")
            
            return inputs
            
        except KeyError as e:
            raise ValidationError(f"Champ requis manquant: {e}")
        except Exception as e:
            logger.error(f"Erreur validation entrées: {e}")
            raise ValidationError(f"Erreur de validation: {e}")
    
    def _validate_cross_references(self, inputs: RestaurantInputs):
        """Valide les cohérences entre les différents champs"""
        
        # Vérifier cohérence taille cuisine / postes de travail
        min_sqm_per_station = 8.0  # m² minimum par poste
        max_sqm_per_station = 50.0  # m² maximum par poste
        
        sqm_per_station = inputs.kitchen_size_sqm / inputs.kitchen_workstations
        if sqm_per_station < min_sqm_per_station:
            raise ValidationError(f"Trop de postes pour la taille de cuisine (min: {min_sqm_per_station} m²/poste)")
        if sqm_per_station > max_sqm_per_station:
            raise ValidationError(f"Pas assez de postes pour la taille de cuisine (max: {max_sqm_per_station} m²/poste)")
        
        # Vérifier cohérence capacité / personnel
        min_capacity_per_staff = 5   # couverts minimum par employé
        max_capacity_per_staff = 100  # couverts maximum par employé
        
        capacity_per_staff = inputs.daily_capacity / inputs.staff_count
        if capacity_per_staff < min_capacity_per_staff:
            raise ValidationError(f"Trop de personnel pour la capacité (min: {min_capacity_per_staff} couverts/employé)")
        if capacity_per_staff > max_capacity_per_staff:
            raise ValidationError(f"Pas assez de personnel pour la capacité (max: {max_capacity_per_staff} couverts/employé)")
        
        # Ajuster automatiquement les heures de formation selon l'expérience
        if inputs.training_hours_needed == 0:
            inputs.training_hours_needed = self._calculate_default_training_hours(inputs)
    
    def _calculate_default_training_hours(self, inputs: RestaurantInputs) -> int:
        """Calcule les heures de formation par défaut selon le contexte"""
        base_hours = BUSINESS_CONSTANTS["training_hours_per_employee"]
        
        # Ajustement selon l'expérience
        experience_multipliers = {
            'beginner': 1.5,
            'intermediate': 1.0,
            'experienced': 0.7,
            'expert': 0.4
        }
        
        # Ajustement selon le thème
        theme_multipliers = {
            'fast_food': 0.8,
            'casual_dining': 1.0,
            'fine_dining': 1.8,
            'cloud_kitchen': 0.9,
            'food_truck': 0.7
        }
        
        calculated_hours = (
            base_hours * 
            experience_multipliers.get(inputs.staff_experience_level, 1.0) *
            theme_multipliers.get(inputs.restaurant_theme, 1.0)
        )
        
        return max(10, int(calculated_hours))  # Minimum 10 heures
    
    def get_input_summary(self, inputs: RestaurantInputs = None) -> Dict[str, Any]:
        """
        Génère un résumé des entrées pour affichage
        
        Args:
            inputs: Entrées à résumer (utilise current_inputs si None)
            
        Returns:
            Dictionnaire avec le résumé formaté
        """
        if inputs is None:
            inputs = self.current_inputs
        
        if inputs is None:
            return {"error": "Aucune entrée disponible"}
        
        return {
            "session": inputs.session_name,
            "type": f"{inputs.restaurant_theme.replace('_', ' ').title()} ({inputs.revenue_size})",
            "cuisine": f"{inputs.kitchen_size_sqm} m² avec {inputs.kitchen_workstations} postes",
            "capacite": f"{inputs.daily_capacity} couverts/jour",
            "personnel": f"{inputs.staff_count} employés ({inputs.staff_experience_level})",
            "formation": f"{inputs.training_hours_needed} heures requises",
            "equipement": f"{inputs.equipment_condition} (âge: {inputs.equipment_age_years} ans, valeur: {inputs.equipment_value:,.0f} CAD$)",
            "loyer": f"{inputs.location_rent_sqm} CAD$/m²" if inputs.location_rent_sqm > 0 else "Pas de loyer fixe"
        }
    
    def validate_inputs(self, data: Dict[str, Any]) -> bool:
        """
        Valide des entrées sans les créer (validation rapide)
        
        Args:
            data: Données à valider
            
        Returns:
            True si valide, False sinon
        """
        try:
            self.create_inputs_from_dict(data)
            return True
        except ValidationError:
            return False
    
    def get_validation_errors(self, data: Dict[str, Any]) -> List[str]:
        """
        Retourne la liste des erreurs de validation
        
        Args:
            data: Données à valider
            
        Returns:
            Liste des messages d'erreur
        """
        errors = []
        try:
            self.create_inputs_from_dict(data)
        except ValidationError as e:
            errors.append(str(e))
        except Exception as e:
            errors.append(f"Erreur inattendue: {e}")
        
        return errors


# Fonctions utilitaires
def create_sample_inputs() -> RestaurantInputs:
    """Crée des entrées d'exemple pour les tests"""
    sample_data = {
        'session_name': 'Restaurant Test',
        'restaurant_theme': 'casual_dining',
        'revenue_size': 'medium',
        'kitchen_size_sqm': 100.0,
        'kitchen_workstations': 8,
        'daily_capacity': 200,
        'staff_count': 15,
        'staff_experience_level': 'intermediate',
        'training_hours_needed': 35,
        'equipment_age_years': 2,
        'equipment_condition': 'good',
        'equipment_value': 120000.0,
        'location_rent_sqm': 40.0
    }
    
    handler = InputHandler()
    return handler.create_inputs_from_dict(sample_data)


if __name__ == "__main__":
    # Test du module
    print("🧪 Test du module input_handler")
    
    try:
        # Test avec des données valides
        sample = create_sample_inputs()
        print("✅ Création d'entrées d'exemple réussie")
        
        handler = InputHandler()
        summary = handler.get_input_summary(sample)
        print("📋 Résumé des entrées:")
        for key, value in summary.items():
            print(f"  {key}: {value}")
        
        # Test de validation d'erreur
        invalid_data = {'session_name': '', 'restaurant_theme': 'invalid'}
        errors = handler.get_validation_errors(invalid_data)
        if errors:
            print(f"✅ Validation d'erreurs fonctionne: {len(errors)} erreurs détectées")
        
    except Exception as e:
        print(f"❌ Erreur de test: {e}")
