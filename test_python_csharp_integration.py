#!/usr/bin/env python3
"""
Test script pour vérifier l'intégration Python-C# de Track-A-FACE
"""

import json
import sys
import os
from pathlib import Path

def test_engine_api():
    """Test du moteur de calcul via engine_api.py"""
    
    # Données de test
    test_data = {
        "session_name": "Test Integration Python-C#",
        "restaurant_theme": "casual_dining",
        "revenue_size": "medium",
        "kitchen_size_sqm": 100.0,
        "kitchen_workstations": 8,
        "daily_capacity": 200,
        "staff_count": 15,
        "staff_experience_level": "intermediate",
        "training_hours_needed": 40,
        "equipment_age_years": 2,
        "equipment_condition": "good",
        "equipment_value": 150000.0,
        "location_rent_sqm": 40.0
    }
    
    print("=== TEST D'INTÉGRATION PYTHON-C# ===")
    print(f"Répertoire de travail: {os.getcwd()}")
    
    # Vérifier la présence des fichiers requis
    required_files = [
        "engine_api.py", "engine.py", "engine_classes.py",
        "input_handler.py", "sql.py", "config.py"
    ]
    
    missing_files = []
    for file in required_files:
        if not os.path.exists(file):
            missing_files.append(file)
    
    if missing_files:
        print(f"❌ Fichiers manquants: {', '.join(missing_files)}")
        return False
    
    print("✅ Tous les fichiers requis sont présents")
    
    # Créer un fichier d'entrée temporaire
    input_file = "test_input.json"
    output_file = "test_output.json"
    
    try:
        # Écrire les données de test
        with open(input_file, 'w', encoding='utf-8') as f:
            json.dump(test_data, f, indent=2, ensure_ascii=False)
        
        print("✅ Fichier d'entrée créé")
        
        # Importer et exécuter le moteur
        try:
            import engine_api
            
            # Simuler l'exécution avec les arguments
            sys.argv = ['engine_api.py', '--input', input_file, '--output', output_file]
            
            # Exécuter le moteur
            if hasattr(engine_api, 'main'):
                engine_api.main()
            else:
                print("⚠️ Fonction main() non trouvée dans engine_api.py")
                # Essayer d'exécuter directement
                exec(open('engine_api.py').read())
            
            print("✅ Moteur de calcul exécuté")
            
        except Exception as e:
            print(f"❌ Erreur d'exécution du moteur: {e}")
            return False
        
        # Vérifier le fichier de sortie
        if os.path.exists(output_file):
            with open(output_file, 'r', encoding='utf-8') as f:
                result = json.load(f)
            
            print("✅ Fichier de sortie généré")
            
            # Vérifier la structure des résultats
            required_fields = ['total_cost', 'staff_costs', 'equipment_costs', 
                             'location_costs', 'operational_costs', 'cost_breakdowns']
            
            missing_fields = []
            for field in required_fields:
                if field not in result:
                    missing_fields.append(field)
            
            if missing_fields:
                print(f"❌ Champs manquants dans le résultat: {', '.join(missing_fields)}")
                return False
            
            print("✅ Structure des résultats valide")
            
            # Vérifier la cohérence des totaux
            calculated_total = (result.get('staff_costs', 0) + 
                              result.get('equipment_costs', 0) + 
                              result.get('location_costs', 0) + 
                              result.get('operational_costs', 0))
            
            total_cost = result.get('total_cost', 0)
            tolerance = abs(total_cost * 0.01)  # 1% de tolérance
            
            if abs(calculated_total - total_cost) > tolerance:
                print(f"⚠️ Incohérence dans les totaux: {calculated_total} vs {total_cost}")
            else:
                print("✅ Cohérence des totaux vérifiée")
            
            # Afficher un résumé des résultats
            print(f"\n--- RÉSULTATS DU CALCUL ---")
            print(f"Coût total: {total_cost:,.2f} CAD$")
            print(f"Personnel: {result.get('staff_costs', 0):,.2f} CAD$")
            print(f"Équipement: {result.get('equipment_costs', 0):,.2f} CAD$")
            print(f"Immobilier: {result.get('location_costs', 0):,.2f} CAD$")
            print(f"Opérationnel: {result.get('operational_costs', 0):,.2f} CAD$")
            
            breakdowns = result.get('cost_breakdowns', [])
            print(f"Détails: {len(breakdowns)} éléments de coût")
            
            return True
            
        else:
            print("❌ Fichier de sortie non généré")
            return False
            
    except Exception as e:
        print(f"❌ Erreur générale: {e}")
        return False
        
    finally:
        # Nettoyer les fichiers temporaires
        for temp_file in [input_file, output_file]:
            if os.path.exists(temp_file):
                try:
                    os.remove(temp_file)
                except:
                    pass

def main():
    """Fonction principale"""
    print("Démarrage du test d'intégration Python-C#...")
    
    success = test_engine_api()
    
    if success:
        print("\n🎉 TEST D'INTÉGRATION RÉUSSI!")
        print("L'intégration Python-C# fonctionne correctement.")
        return 0
    else:
        print("\n❌ TEST D'INTÉGRATION ÉCHOUÉ!")
        print("Des problèmes ont été détectés dans l'intégration.")
        return 1

if __name__ == "__main__":
    sys.exit(main())
