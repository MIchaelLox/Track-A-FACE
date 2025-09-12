#!/usr/bin/env python3
# ==========================
# /engine_api.py
# ==========================

"""
API wrapper pour le moteur Track-A-FACE
Interface de communication avec le frontend C#
"""

import sys
import json
import argparse
import logging
from pathlib import Path
from typing import Dict, Any

# Import des modules du moteur
from engine import CalculationEngine
from input_handler import InputHandler, ValidationError

# Configuration du logging
logging.basicConfig(
    level=logging.INFO,
    format='%(asctime)s - %(name)s - %(levelname)s - %(message)s'
)
logger = logging.getLogger(__name__)


class EngineAPI:
    """API wrapper pour le moteur de calcul Track-A-FACE"""
    
    def __init__(self):
        self.engine = CalculationEngine()
        self.input_handler = InputHandler()
    
    def process_calculation(self, input_data: Dict[str, Any]) -> Dict[str, Any]:
        """
        Traite une demande de calcul et retourne les résultats
        
        Args:
            input_data: Dictionnaire avec les paramètres d'entrée
            
        Returns:
            Dictionnaire avec les résultats de calcul
        """
        try:
            # Valider et créer les entrées
            logger.info(f"Traitement du calcul pour: {input_data.get('session_name', 'Session inconnue')}")
            
            # Créer les entrées validées
            inputs = self.input_handler.create_inputs_from_dict(input_data)
            
            # Exécuter le calcul
            summary = self.engine.calculate_restaurant_costs(inputs)
            
            # Valider les résultats
            is_valid = self.engine.validate_calculation_accuracy(summary)
            if not is_valid:
                logger.warning("Incohérence détectée dans les calculs")
            
            # Convertir en format JSON-friendly
            result = {
                "session_id": summary.session_id,
                "session_name": summary.session_name,
                "staff_costs": summary.staff_costs,
                "equipment_costs": summary.equipment_costs,
                "location_costs": summary.location_costs,
                "operational_costs": summary.operational_costs,
                "total_cost": summary.total_cost,
                "cost_breakdowns": [
                    {
                        "category": breakdown.category,
                        "subcategory": breakdown.subcategory,
                        "amount": breakdown.amount,
                        "formula": breakdown.formula,
                        "details": breakdown.details
                    }
                    for breakdown in summary.cost_breakdowns
                ],
                "validation_passed": is_valid,
                "calculation_timestamp": summary.session_id  # Utiliser l'ID comme timestamp
            }
            
            logger.info(f"Calcul terminé - Total: {summary.total_cost:,.2f} CAD$")
            return result
            
        except ValidationError as e:
            logger.error(f"Erreur de validation: {e}")
            return {
                "error": "validation_error",
                "message": str(e),
                "details": "Les données d'entrée ne sont pas valides"
            }
        
        except Exception as e:
            logger.error(f"Erreur lors du calcul: {e}")
            return {
                "error": "calculation_error",
                "message": str(e),
                "details": "Erreur interne du moteur de calcul"
            }
    
    def get_input_summary(self, input_data: Dict[str, Any]) -> Dict[str, Any]:
        """
        Génère un résumé des entrées pour validation
        
        Args:
            input_data: Dictionnaire avec les paramètres d'entrée
            
        Returns:
            Résumé formaté des entrées
        """
        try:
            inputs = self.input_handler.create_inputs_from_dict(input_data)
            return self.input_handler.get_input_summary(inputs)
        except Exception as e:
            return {"error": str(e)}
    
    def validate_inputs(self, input_data: Dict[str, Any]) -> Dict[str, Any]:
        """
        Valide les entrées sans exécuter le calcul
        
        Args:
            input_data: Dictionnaire avec les paramètres d'entrée
            
        Returns:
            Résultat de validation
        """
        try:
            is_valid = self.input_handler.validate_inputs(input_data)
            errors = [] if is_valid else self.input_handler.get_validation_errors(input_data)
            
            return {
                "is_valid": is_valid,
                "errors": errors,
                "summary": self.get_input_summary(input_data) if is_valid else None
            }
        except Exception as e:
            return {
                "is_valid": False,
                "errors": [str(e)],
                "summary": None
            }


def main():
    """Point d'entrée principal pour l'API"""
    parser = argparse.ArgumentParser(description="Track-A-FACE Engine API")
    parser.add_argument("--input", "-i", help="Fichier JSON d'entrée")
    parser.add_argument("--output", "-o", help="Fichier JSON de sortie")
    parser.add_argument("--validate-only", action="store_true", help="Valider seulement, sans calculer")
    parser.add_argument("--summary-only", action="store_true", help="Résumé seulement, sans calculer")
    
    args = parser.parse_args()
    
    try:
        # Initialiser l'API
        api = EngineAPI()
        
        # Lire les données d'entrée
        if args.input:
            with open(args.input, 'r', encoding='utf-8') as f:
                input_data = json.load(f)
        else:
            # Lire depuis stdin si pas de fichier spécifié
            input_data = json.load(sys.stdin)
        
        # Traiter selon le mode demandé
        if args.validate_only:
            result = api.validate_inputs(input_data)
        elif args.summary_only:
            result = api.get_input_summary(input_data)
        else:
            result = api.process_calculation(input_data)
        
        # Écrire le résultat
        result_json = json.dumps(result, ensure_ascii=False, indent=2)
        
        if args.output:
            with open(args.output, 'w', encoding='utf-8') as f:
                f.write(result_json)
        else:
            print(result_json)
        
        # Code de sortie selon le succès
        if "error" in result:
            sys.exit(1)
        else:
            sys.exit(0)
            
    except FileNotFoundError as e:
        error_result = {
            "error": "file_not_found",
            "message": f"Fichier non trouvé: {e}",
            "details": "Vérifiez le chemin du fichier d'entrée"
        }
        print(json.dumps(error_result, ensure_ascii=False, indent=2))
        sys.exit(1)
        
    except json.JSONDecodeError as e:
        error_result = {
            "error": "json_decode_error",
            "message": f"Erreur de format JSON: {e}",
            "details": "Le fichier d'entrée n'est pas un JSON valide"
        }
        print(json.dumps(error_result, ensure_ascii=False, indent=2))
        sys.exit(1)
        
    except Exception as e:
        error_result = {
            "error": "unexpected_error",
            "message": str(e),
            "details": "Erreur inattendue du système"
        }
        print(json.dumps(error_result, ensure_ascii=False, indent=2))
        sys.exit(1)


if __name__ == "__main__":
    main()
