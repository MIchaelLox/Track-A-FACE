-- ==========================
-- Track-A-FACE Database Schema
-- SQLite Database Structure
-- ==========================

-- Table des paramètres d'entrée
CREATE TABLE IF NOT EXISTS inputs (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    session_name VARCHAR(100) NOT NULL,
    restaurant_theme VARCHAR(50) NOT NULL CHECK (restaurant_theme IN ('fast_food', 'casual_dining', 'fine_dining', 'cloud_kitchen', 'food_truck')),
    revenue_size VARCHAR(20) NOT NULL CHECK (revenue_size IN ('small', 'medium', 'large', 'enterprise')),
    kitchen_size_sqm DECIMAL(10,2) NOT NULL CHECK (kitchen_size_sqm > 0),
    kitchen_workstations INTEGER NOT NULL CHECK (kitchen_workstations > 0),
    daily_capacity INTEGER NOT NULL CHECK (daily_capacity > 0),
    staff_count INTEGER NOT NULL CHECK (staff_count > 0),
    staff_experience_level VARCHAR(20) NOT NULL CHECK (staff_experience_level IN ('beginner', 'intermediate', 'experienced', 'expert')),
    training_hours_needed INTEGER DEFAULT 0,
    equipment_age_years INTEGER DEFAULT 0 CHECK (equipment_age_years >= 0),
    equipment_condition VARCHAR(20) NOT NULL CHECK (equipment_condition IN ('excellent', 'good', 'fair', 'poor')),
    equipment_value DECIMAL(12,2) NOT NULL CHECK (equipment_value >= 0),
    location_rent_sqm DECIMAL(8,2) NOT NULL CHECK (location_rent_sqm >= 0),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Table des facteurs de coût et multiplicateurs
CREATE TABLE IF NOT EXISTS cost_factors (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    factor_category VARCHAR(50) NOT NULL CHECK (factor_category IN ('staff', 'equipment', 'location', 'operations')),
    factor_name VARCHAR(100) NOT NULL,
    restaurant_theme VARCHAR(50),
    revenue_size VARCHAR(20),
    multiplier DECIMAL(8,4) NOT NULL DEFAULT 1.0,
    base_cost DECIMAL(10,2) DEFAULT 0,
    description TEXT,
    is_active BOOLEAN DEFAULT 1,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Table des résultats de calculs
CREATE TABLE IF NOT EXISTS calculation_results (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    input_session_id INTEGER NOT NULL,
    cost_category VARCHAR(50) NOT NULL,
    cost_subcategory VARCHAR(50),
    calculated_amount DECIMAL(12,2) NOT NULL,
    formula_used TEXT,
    calculation_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (input_session_id) REFERENCES inputs(id) ON DELETE CASCADE
);

-- Table des scénarios de comparaison
CREATE TABLE IF NOT EXISTS scenarios (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    comparison_name VARCHAR(100) NOT NULL,
    scenario_name VARCHAR(100) NOT NULL,
    input_session_id INTEGER NOT NULL,
    total_cost DECIMAL(12,2),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (input_session_id) REFERENCES inputs(id) ON DELETE CASCADE
);

-- Index pour optimiser les performances
CREATE INDEX IF NOT EXISTS idx_inputs_theme ON inputs(restaurant_theme);
CREATE INDEX IF NOT EXISTS idx_inputs_revenue ON inputs(revenue_size);
CREATE INDEX IF NOT EXISTS idx_cost_factors_category ON cost_factors(factor_category);
CREATE INDEX IF NOT EXISTS idx_results_session ON calculation_results(input_session_id);
CREATE INDEX IF NOT EXISTS idx_scenarios_comparison ON scenarios(comparison_name);

-- Données de base pour les facteurs de coût (CAD$)
INSERT OR REPLACE INTO cost_factors (factor_category, factor_name, restaurant_theme, revenue_size, multiplier, base_cost, description) VALUES
-- Facteurs de formation du personnel (CAD$/heure)
('staff', 'training_rate_per_hour', 'fast_food', NULL, 1.0, 28.0, 'Taux horaire formation fast food (CAD$)'),
('staff', 'training_rate_per_hour', 'casual_dining', NULL, 1.2, 35.0, 'Taux horaire formation casual dining (CAD$)'),
('staff', 'training_rate_per_hour', 'fine_dining', NULL, 1.8, 50.0, 'Taux horaire formation fine dining (CAD$)'),
('staff', 'training_rate_per_hour', 'cloud_kitchen', NULL, 0.8, 25.0, 'Taux horaire formation cloud kitchen (CAD$)'),
('staff', 'training_rate_per_hour', 'food_truck', NULL, 0.9, 26.0, 'Taux horaire formation food truck (CAD$)'),

-- Facteurs de complexité par thème
('staff', 'complexity_factor', 'fast_food', NULL, 1.0, 0, 'Facteur complexité fast food'),
('staff', 'complexity_factor', 'casual_dining', NULL, 1.3, 0, 'Facteur complexité casual dining'),
('staff', 'complexity_factor', 'fine_dining', NULL, 2.0, 0, 'Facteur complexité fine dining'),
('staff', 'complexity_factor', 'cloud_kitchen', NULL, 0.8, 0, 'Facteur complexité cloud kitchen'),
('staff', 'complexity_factor', 'food_truck', NULL, 0.9, 0, 'Facteur complexité food truck'),

-- Facteurs de dépréciation équipement
('equipment', 'depreciation_rate', NULL, NULL, 0.2, 0, 'Taux dépréciation annuel standard'),
('equipment', 'condition_excellent', NULL, NULL, 0.05, 0, 'Facteur état excellent'),
('equipment', 'condition_good', NULL, NULL, 0.15, 0, 'Facteur état bon'),
('equipment', 'condition_fair', NULL, NULL, 0.25, 0, 'Facteur état moyen'),
('equipment', 'condition_poor', NULL, NULL, 0.40, 0, 'Facteur état mauvais'),

-- Facteurs immobiliers
('location', 'rent_factor', 'fast_food', NULL, 0.8, 0, 'Facteur loyer fast food'),
('location', 'rent_factor', 'casual_dining', NULL, 1.0, 0, 'Facteur loyer casual dining'),
('location', 'rent_factor', 'fine_dining', NULL, 1.5, 0, 'Facteur loyer fine dining'),
('location', 'rent_factor', 'cloud_kitchen', NULL, 0.6, 0, 'Facteur loyer cloud kitchen'),
('location', 'rent_factor', 'food_truck', NULL, 0.3, 0, 'Facteur loyer food truck'),

-- Facteurs opérationnels par taille
('operations', 'efficiency_factor', NULL, 'small', 1.2, 0, 'Facteur efficacité petite taille'),
('operations', 'efficiency_factor', NULL, 'medium', 1.0, 0, 'Facteur efficacité moyenne taille'),
('operations', 'efficiency_factor', NULL, 'large', 0.85, 0, 'Facteur efficacité grande taille'),
('operations', 'efficiency_factor', NULL, 'enterprise', 0.7, 0, 'Facteur efficacité enterprise');
