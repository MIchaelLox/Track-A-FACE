# Track-A-FACE - Interface Utilisateur C#

## Vue d'ensemble

Interface utilisateur Windows Forms pour le moteur de calcul financier Track-A-FACE. Cette application permet de saisir les paramètres d'un restaurant et d'obtenir une analyse détaillée des coûts d'exploitation.

## Fonctionnalités

### 🏪 Paramètres d'entrée
- **Informations générales** : Nom de session, thème de restaurant
- **Taille d'entreprise** : Revenus (small/medium/large/enterprise)
- **Configuration cuisine** : Superficie, postes de travail, capacité quotidienne
- **Personnel** : Nombre d'employés, niveau d'expérience, heures de formation
- **Équipement** : Âge, état, valeur de remplacement
- **Immobilier** : Loyer par m²

### 📊 Calculs avancés
- **Coûts de personnel** : Formation, salaires avec facteurs pondérés
- **Coûts d'équipement** : Dépréciation, maintenance selon l'usage
- **Coûts immobiliers** : Loyer, utilities ajustés par thème
- **Coûts opérationnels** : Matières premières, marketing stratégique

### 📈 Affichage des résultats
- **Résumé visuel** : Totaux par catégorie avec codes couleur
- **Détails complets** : Tableau avec formules de calcul
- **Export** : CSV pour analyse, PDF (à implémenter)

### 💾 Gestion des sessions
- **Sauvegarde/Chargement** : Format JSON pour réutilisation
- **Validation** : Contrôles de cohérence automatiques
- **Interface intuitive** : Onglets organisés, navigation fluide

## Architecture technique

### Frontend C# (.NET 8)
- **Windows Forms** : Interface native Windows
- **Newtonsoft.Json** : Sérialisation des données
- **PythonBridge** : Communication avec le backend

### Backend Python
- **engine_api.py** : Interface de communication
- **Moteur complet** : Calculs avec formules pondérées
- **Base de données SQLite** : Facteurs de coût configurables

## Installation et utilisation

### Prérequis
1. **Python 3.8+** installé et dans le PATH
2. **Dépendances Python** : `pip install -r requirements.txt`
3. **.NET 8 Runtime** pour Windows

### Compilation
```bash
cd FaceWebAppUI
dotnet build
dotnet run
```

### Utilisation
1. **Saisie** : Remplir les paramètres dans l'onglet "Paramètres d'entrée"
2. **Calcul** : Cliquer sur "🧮 Calculer" pour lancer l'analyse
3. **Résultats** : Consulter les résultats dans l'onglet "Résultats"
4. **Export** : Sauvegarder en CSV ou PDF selon les besoins

## Thèmes supportés

- **Fast Food** : Restauration rapide, usage intensif
- **Casual Dining** : Restaurant familial standard
- **Fine Dining** : Haute gastronomie, équipement spécialisé
- **Cloud Kitchen** : Cuisine virtuelle, livraison uniquement
- **Food Truck** : Mobile, contraintes spécifiques

## Tailles d'entreprise

- **Small** : < 500k CAD$/an
- **Medium** : 500k - 2M CAD$/an  
- **Large** : 2M - 10M CAD$/an
- **Enterprise** : > 10M CAD$/an

## Formules de calcul

### Personnel
```
Formation = heures × employés × taux_horaire × facteur_complexité × facteur_taille
Salaires = employés × salaire_base × mult_thème × mult_revenus × mult_localisation
```

### Équipement
```
Dépréciation = valeur × facteur_état × taux_base × facteur_usage × facteur_thème
Maintenance = valeur × taux_maintenance × facteur_âge × facteur_usage
```

### Opérationnel
```
Matières premières = capacité × jours × coût_couvert × facteurs_qualité
Marketing = budget_base × facteurs_thème × facteurs_compétition
```

## Structure des fichiers

```
FaceWebAppUI/
├── FaceWebAppUI.csproj      # Configuration projet
├── Program.cs               # Point d'entrée
├── Form1.cs                 # Interface principale (MainForm)
├── Form1.Designer.cs        # Définition des contrôles
├── PythonBridge.cs          # Communication Python
└── README.md               # Cette documentation
```

## Développement

### Ajout de nouvelles fonctionnalités
1. **Nouveaux paramètres** : Modifier `CollectInputData()` et l'interface
2. **Nouveaux calculs** : Étendre le backend Python
3. **Nouveaux exports** : Implémenter dans les handlers d'export

### Débogage
- **Logs Python** : Vérifier les erreurs dans la console
- **Validation** : Utiliser les messages d'erreur intégrés
- **Tests** : Utiliser les données d'exemple fournies

## Support et maintenance

### Problèmes courants
- **Python non trouvé** : Vérifier l'installation et le PATH
- **Erreurs de calcul** : Contrôler la cohérence des données
- **Performance** : Optimiser pour les gros volumes de données

### Évolutions futures
- **Comparaison de scénarios** : Onglet dédié à implémenter
- **Export PDF avancé** : Rapports formatés avec graphiques
- **Base de données** : Interface de gestion des facteurs de coût
- **API REST** : Service web pour intégration externe

---

**Version** : 1.0.0  
**Dernière mise à jour** : Septembre 2025  
**Développé pour** : Track-A-FACE Financial Analysis Engine
