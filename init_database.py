#!/usr/bin/env python3
"""
Script d'initialisation de la base de données Track-A-FACE
Crée les tables et insère les données de base
"""

import sys
import os

# Ajouter le répertoire courant au path
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))

from sql import DatabaseInitializer

def main():
    print("=" * 60)
    print("Initialisation de la base de données Track-A-FACE")
    print("=" * 60)
    
    try:
        initializer = DatabaseInitializer()
        print("\n🔄 Initialisation en cours...")
        initializer.initialize_database()
        print("✅ Base de données initialisée avec succès!")
        print(f"📁 Fichier: dataface_engine.db")
        print("\n✨ Vous pouvez maintenant utiliser l'application C#!")
        
    except Exception as e:
        print(f"❌ Erreur lors de l'initialisation: {e}")
        import traceback
        traceback.print_exc()
        return 1
    
    return 0

if __name__ == "__main__":
    sys.exit(main())
