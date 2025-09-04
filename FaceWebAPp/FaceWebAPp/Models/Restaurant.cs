namespace FaceWebAPp.Models
{
    public class Restaurant
    {
        public int Id { get; set; } // Identifiant unique pour la base de données

        // --- VARIABLES D'ENTRÉE (décrites dans input_handler.py) ---
        public string Theme { get; set; }         // Ex: "FastFood", "Cafe", "Sushi"
        public string TailleChiffreAffaires { get; set; } // "Petit", "Moyen", "Grand"
        public double TailleCuisineM2 { get; set; }       // Surface en mètres carrés
        public bool BesoinRecyclage { get; set; }         // true si besoin de recyclage
        public int EtatEquipement { get; set; }           // 1 (mauvais) à 5 (excellent)

        // --- AUTRES PROPRIÉTÉS POSSIBLES POUR LES CALCULS ---
        public int NombreEmployes { get; set; }
        public string TypeCuisine { get; set; }   // Ex: "Traditionnelle", "Moderne"
        public int AnneesExperience { get; set; } // Ancienneté du restaurant

        // --- COÛTS CALCULÉS (seront remplis par le moteur de calcul) ---
        public decimal CoutFormation { get; set; }
        public CoutAmortissement Amortissement { get; set; } // Ajout du type 'CoutAmortissement'
        public decimal CoutRecyclage { get; set; } // Ajout du type 'decimal'
        public decimal CoutTotal { get; set; }
    }

    // Classe pour représenter le coût d'amortissement (optionnel)
    public class CoutAmortissement
    {
        public decimal CoutRemplacement { get; set; }
        public decimal TauxAmortissement { get; set; }
    }
}
