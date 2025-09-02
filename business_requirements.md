# Track-A-FACE - Analyse des besoins métier

## Objectif principal
Calculer les coûts d'exploitation d'un restaurant selon différents paramètres et scénarios.

## Variables d'entrée identifiées

### 1. **Thème du restaurant**
- Fast Food
- Casual Dining  
- Fine Dining
- Cloud Kitchen
- Food Truck

### 2. **Taille des revenus**
- Petit (< 500k CAD$/an)
- Moyen (500k - 2M CAD$/an)
- Grand (2M - 10M CAD$/an)
- Enterprise (> 10M CAD$/an)

### 3. **Taille de la cuisine**
- Surface en m²
- Nombre de postes de travail
- Capacité de production (couverts/jour)

### 4. **Besoin de formation du personnel**
- Niveau actuel du personnel (débutant/expérimenté)
- Nombre d'employés à former
- Type de formation requis

### 5. **État de l'équipement**
- Âge de l'équipement
- État général (excellent/bon/moyen/mauvais)
- Valeur de remplacement

## Formules de calcul identifiées

### **Coût de formation**
```
coût_formation = besoin_formation × taille_équipe × facteur_complexité × taux_horaire
```

### **Dépréciation équipement**
```
dépréciation = état_équipement × taux_remplacement × valeur_équipement
```

### **Coûts opérationnels**
```
coûts_ops = (loyer_cuisine + utilities + maintenance) × facteur_thème
```

## Sorties attendues

### **Tableau de coûts par catégorie**
- Coûts de personnel (formation, salaires)
- Coûts d'équipement (achat, maintenance, dépréciation)
- Coûts immobiliers (loyer, utilities)
- Coûts opérationnels (matières premières, marketing)

### **Comparaisons de scénarios**
- Scénario A vs Scénario B
- Impact des changements de paramètres
- Recommandations d'optimisation

### **Exports**
- Rapports PDF formatés
- Fichiers CSV pour analyse
- Graphiques de comparaison

## Contraintes techniques
- Interface utilisateur intuitive
- Calculs en temps réel
- Sauvegarde des sessions
- Installation simple sur Windows
