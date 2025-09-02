# Track-A-FACE - Modèle de données

## Structure de la base de données

### Table: **inputs**
Stocke les paramètres d'entrée pour chaque calcul

| Colonne | Type | Description |
|---------|------|-------------|
| id | INTEGER PRIMARY KEY | Identifiant unique |
| session_name | VARCHAR(100) | Nom de la session |
| restaurant_theme | VARCHAR(50) | Type de restaurant |
| revenue_size | VARCHAR(20) | Catégorie de revenus |
| kitchen_size_sqm | DECIMAL(10,2) | Surface cuisine en m² |
| kitchen_workstations | INTEGER | Nombre de postes |
| daily_capacity | INTEGER | Couverts/jour |
| staff_count | INTEGER | Nombre d'employés |
| staff_experience_level | VARCHAR(20) | Niveau expérience équipe |
| training_hours_needed | INTEGER | Heures formation requises |
| equipment_age_years | INTEGER | Âge équipement en années |
| equipment_condition | VARCHAR(20) | État équipement |
| equipment_value | DECIMAL(12,2) | Valeur équipement (CAD$) |
| location_rent_sqm | DECIMAL(8,2) | Loyer/m² (CAD$) |
| created_at | TIMESTAMP | Date création |

### Table: **cost_factors**
Coefficients et multiplicateurs pour les calculs

| Colonne | Type | Description |
|---------|------|-------------|
| id | INTEGER PRIMARY KEY | Identifiant |
| factor_category | VARCHAR(50) | Catégorie (staff/equipment/location) |
| factor_name | VARCHAR(100) | Nom du facteur |
| restaurant_theme | VARCHAR(50) | Thème applicable |
| revenue_size | VARCHAR(20) | Taille applicable |
| multiplier | DECIMAL(8,4) | Coefficient multiplicateur |
| base_cost | DECIMAL(10,2) | Coût de base (CAD$) |
| description | TEXT | Description du facteur |

### Table: **calculation_results**
Résultats des calculs pour chaque session

| Colonne | Type | Description |
|---------|------|-------------|
| id | INTEGER PRIMARY KEY | Identifiant |
| input_session_id | INTEGER | Référence vers inputs |
| cost_category | VARCHAR(50) | Catégorie de coût |
| cost_subcategory | VARCHAR(50) | Sous-catégorie |
| calculated_amount | DECIMAL(12,2) | Montant calculé (CAD$) |
| formula_used | TEXT | Formule appliquée |
| calculation_date | TIMESTAMP | Date du calcul |

### Table: **scenarios**
Comparaisons de scénarios multiples

| Colonne | Type | Description |
|---------|------|-------------|
| id | INTEGER PRIMARY KEY | Identifiant |
| comparison_name | VARCHAR(100) | Nom de la comparaison |
| scenario_name | VARCHAR(100) | Nom du scénario |
| input_session_id | INTEGER | Référence vers inputs |
| total_cost | DECIMAL(12,2) | Coût total calculé (CAD$) |
| created_at | TIMESTAMP | Date création |

## Catégories de coûts

### **1. Coûts de personnel**
- Formation initiale
- Salaires et charges
- Formation continue
- Turnover et recrutement

### **2. Coûts d'équipement**
- Achat initial
- Maintenance préventive
- Réparations
- Dépréciation
- Remplacement

### **3. Coûts immobiliers**
- Loyer cuisine
- Utilities (électricité, gaz, eau)
- Assurances
- Taxes foncières

### **4. Coûts opérationnels**
- Matières premières
- Marketing et publicité
- Licences et permis
- Frais administratifs

## Formules de calcul détaillées

### **Formation du personnel**
```
coût_formation = (
    training_hours_needed × 
    staff_count × 
    hourly_training_rate × 
    theme_complexity_factor
)
```

### **Dépréciation équipement**
```
dépréciation_annuelle = (
    equipment_value × 
    condition_factor × 
    (1 / equipment_depreciation_years)
)
```

### **Coûts immobiliers**
```
coût_immobilier_mensuel = (
    kitchen_size_sqm × 
    location_rent_sqm × 
    theme_location_factor
)
```

### **Coûts opérationnels**
```
coût_ops_quotidien = (
    daily_capacity × 
    cost_per_cover × 
    revenue_size_factor
)
```
