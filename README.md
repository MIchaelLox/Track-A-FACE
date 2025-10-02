# Track-A-FACE - Financial Analysis Calculation Engine

**Calculateur Professionnel de Coûts de Restaurant**

[![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.7.2-512BD4?logo=.net)](https://dotnet.microsoft.com/)
[![Python](https://img.shields.io/badge/Python-3.x-3776AB?logo=python&logoColor=white)](https://www.python.org/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![Status](https://img.shields.io/badge/Status-Production%20Ready-success)](https://github.com/MIchaelLox/Track-A-FACE)

---

## Vue d'ensemble

**Track-A-FACE** est une solution professionnelle d'estimation des coûts de démarrage et d'exploitation pour restaurants. L'application combine un **moteur de calcul Python** avancé avec une **interface utilisateur C# WinForms** moderne.

### Fonctionnalités Principales:

Calculs Précis: Moteur Python avec formules pondérées  
5 Thèmes: Fast Food, Casual Dining, Fine Dining, Cloud Kitchen, Food Truck  
4 Tailles: Small, Medium, Large, Enterprise  
Sessions: Sauvegarde/Chargement JSON  
Export: CSV et PDF professionnels  
UX Moderne: Dialogues personnalisés, raccourcis clavier, feedback temps réel  

---

## Architecture

Track-A-FACE/
├── backend/              # Moteur Python
│   ├── engine.py         # Calcul principal
│   ├── sql.py            # Base SQLite
│   └── input_handler.py  # Validation
│
├── TrackAFaceUI/         # Interface C#
│   └── TrackAFaceWinForms/
│       ├── Forms/        # 3 formulaires
│       ├── Dialogs/      # 3 dialogues UX
│       ├── Helpers/      # Utilitaires
│       └── Services/     # PythonBridge
│
└── Documentation/        # Guides complets

**Communication:**  
`C# WinForms` → `Process.Start()` → `Python Engine` → `JSON Results` → `C# Display`

---

## Installation

### Prérequis:

- Windows 10/11
- Visual Studio 2019+ (.NET Framework 4.7.2)
- Python 3.8+ (optionnel)

### Démarrage Rapide:

```bash
# 1. Cloner
git clone https://github.com/MIchaelLox/Track-A-FACE.git
cd Track-A-FACE

# 2. Backend Python (optionnel)
cd backend
pip install -r requirements.txt

# 3. Frontend C#
cd ../TrackAFaceUI/TrackAFaceWinForms
# Ouvrir TrackAFaceWinForms.sln
# Restaurer packages NuGet
# F5 (Démarrer)
```

---

## Utilisation

### Workflow:

1. **Saisie** (`InputForm`)  
   - Nom session, Thème, Revenus  
   - Personnel, Immobilier, Équipement  
   - `F5` = Calculer

2. **Résultats** (`ResultsForm`)  
   - Tableaux colorés par catégorie  
   - Export CSV/PDF

3. **Sessions**  
   - `Ctrl+S` = Sauvegarder  
   - `Ctrl+O` = Charger  
   - Format JSON dans `Documents/Track-A-FACE/sessions/`

### Raccourcis Clavier:

| Touche | Action |
|--------|--------|
| `F5` | Calculer |
| `Ctrl+S` | Sauvegarder |
| `Ctrl+O` | Charger session |
| `Ctrl+R` | Réinitialiser |
| `F1` | À propos |

---

## Documentation

**Guides Utilisateur:**
- [PLAN_TESTS.md](TrackAFaceUI/TrackAFaceWinForms/PLAN_TESTS.md) - 20 tests (30-45min)
- [RACCOURCIS_CLAVIER.md](TrackAFaceUI/TrackAFaceWinForms/RACCOURCIS_CLAVIER.md) - Guide complet

**Guides Développeur:**
- [TROUBLESHOOTING.md](TrackAFaceUI/TrackAFaceWinForms/TROUBLESHOOTING.md) - 10 erreurs + solutions
- [STRUCTURE_PROJET.md](TrackAFaceUI/TrackAFaceWinForms/STRUCTURE_PROJET.md) - Architecture
- [AUDIT_SEMAINE3.md](TrackAFaceUI/TrackAFaceWinForms/AUDIT_SEMAINE3.md) - Audit 125%

---

## Technologies

**Backend:** Python 3.8+, SQLite, JSON  
**Frontend:** .NET Framework 4.7.2, C# WinForms  
**Packages:** BouncyCastle 1.8.9, iTextSharp 5.5.13.3  
**Communication:** Process + JSON Async  

---

## Roadmap

**v1.0 (Actuelle):**
- [x] Moteur Python complet
- [x] Interface C# moderne
- [x] Sessions JSON
- [x] Export CSV/PDF
- [x] Dialogues UX

**v1.1 (Futur):**
- [ ] Graphiques résultats
- [ ] Thème sombre
- [ ] Multi-langue (FR/EN)
- [ ] Comparaison scénarios

**v2.0 (Vision):**
- [ ] API REST
- [ ] Application web
- [ ] Cloud deployment

---

## Licence

Ce projet est sous licence **MIT**. Voir [LICENSE](LICENSE).

---

## Contribution

1. Fork le projet
2. Créer une branche (`feature/AmazingFeature`)
3. Commit (`git commit -m 'Add AmazingFeature'`)
4. Push (`git push origin feature/AmazingFeature`)
5. Pull Request

---

## Support

- **Issues:** [Créer une issue](https://github.com/MIchaelLox/Track-A-FACE/issues)
- **Documentation:** `/TrackAFaceUI/TrackAFaceWinForms/`
- **Troubleshooting:** [Guide dépannage](TrackAFaceUI/TrackAFaceWinForms/TROUBLESHOOTING.md)

---

<div align="center">

**⭐ Si ce projet vous a aidé, donnez-lui une étoile! ⭐**

[![Télécharger](https://github.com/MIchaelLox/Track-A-FACE/releases) • [![Docs](TrackAFaceUI/TrackAFaceWinForms/)] • [![Bug](https://github.com/MIchaelLox/Track-A-FACE/issues)]

**Version 1.0.0** • **Production Ready** ✅

</div>
