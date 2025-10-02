# 🧪 Plan de Tests - Track-A-FACE

## ✅ CHECKLIST DE TESTS (30-45 min)

### **1. COMPILATION (5 min) ⚠️ PRIORITÉ CRITIQUE**

```powershell
# Dans Visual Studio:
1. Ouvrir TrackAFaceWinForms.sln
2. Ctrl+Shift+B (Générer la solution)
3. Vérifier: 0 erreurs de compilation

Attendu: ✅ Génération réussie: 1 réussite, 0 échec
Erreurs possibles: Packages NuGet non restaurés
```

---

### **2. DÉMARRAGE APPLICATION (2 min)**

```
1. F5 (Démarrer avec débogage)
2. MainForm s'ouvre
3. Vérifier:
   ✅ Titre: "Track-A-FACE - Calculateur de Coûts de Restaurant"
   ✅ Menu visible: Fichier, Aide
   ✅ Fenêtre maximisée
   ✅ Aucune erreur console
```

---

### **3. MENU PRINCIPAL (5 min)**

**TEST 3.1: Menu Fichier → Nouvelle Session**
```
1. Cliquer: Fichier → Nouvelle Session (ou Ctrl+N)
2. Vérifier: Message "Fonctionnalité à venir"
✅ Attendu: MessageBox informatif
```

**TEST 3.2: Menu Fichier → Ouvrir Session**
```
1. Cliquer: Fichier → Ouvrir Session... (ou Ctrl+O)
2. LoadSessionDialog s'ouvre
3. Vérifier: Liste sessions (vide si première fois)
4. Cliquer: Annuler
✅ Attendu: Dialogue fonctionne, se ferme correctement
```

**TEST 3.3: Menu Aide → À Propos**
```
1. Cliquer: Aide → À Propos... (ou F1)
2. AboutDialog s'ouvre
3. Vérifier:
   ✅ En-tête vert
   ✅ "Track-A-FACE" titre
   ✅ "Version 1.0.0"
   ✅ Description complète
   ✅ Copyright 2025
4. Cliquer: OK
✅ Attendu: Dialogue se ferme
```

**TEST 3.4: Menu Fichier → Quitter**
```
1. Cliquer: Fichier → Quitter (ou Alt+F4)
2. Confirmation demandée
3. Cliquer: Non
4. Application reste ouverte
✅ Attendu: Pas de fermeture intempestive
```

---

### **4. INPUTFORM - SAISIE DONNÉES (10 min)**

**TEST 4.1: Remplissage Formulaire**
```
1. InputForm visible (ou ouvrir depuis MainForm)
2. Remplir tous les champs:
   - Nom session: "Test_Restaurant_1"
   - Thème: "Casual Dining"
   - Taille revenus: Medium (sélectionné)
   - Staff Count: 15
   - Retraining Hours: 50
   - Kitchen Size: 120
   - Rent Monthly: 6000
   - Location Type: Urban
   - Utilities: 1500
   - Equipment Value: 60000
   - Equipment Condition: 85%
   - Equipment Age: 1
   - Capacity: 180

✅ Attendu: Tous les champs acceptent valeurs
```

**TEST 4.2: Réinitialisation**
```
1. Après remplissage
2. Cliquer: Réinitialiser (ou Ctrl+R)
3. Vérifier: Tous champs reviennent aux valeurs par défaut
✅ Attendu: Formulaire réinitialisé
```

---

### **5. SESSION MANAGER (10 min)**

**TEST 5.1: Sauvegarde Session**
```
1. Remplir formulaire (voir TEST 4.1)
2. Cliquer: Sauvegarder (ou Ctrl+S)
3. Vérifier:
   ✅ Message "Session sauvegardée avec succès"
   ✅ Nom fichier affiché
4. Vérifier fichier créé:
   C:\Users\hp\Documents\Track-A-FACE\sessions\Test_Restaurant_1_*.json
✅ Attendu: Fichier JSON créé avec timestamp
```

**TEST 5.2: Charger Session (LoadSessionDialog)**
```
1. Réinitialiser formulaire (Ctrl+R)
2. Cliquer: Charger (ou Ctrl+O)
3. LoadSessionDialog s'ouvre
4. Vérifier:
   ✅ "Test_Restaurant_1" apparaît dans liste
   ✅ Date création affichée
   ✅ Date modification affichée
5. Cliquer sur session
6. Vérifier: Panel détails rempli
7. Cliquer: Charger
8. Vérifier:
   ✅ Message "Session chargée avec succès"
   ✅ Tous les champs remplis correctement
✅ Attendu: Données restaurées complètement
```

**TEST 5.3: Supprimer Session**
```
1. Cliquer: Charger (Ctrl+O)
2. Sélectionner "Test_Restaurant_1"
3. Cliquer: Supprimer (bouton rouge)
4. Confirmation demandée
5. Cliquer: Oui
6. Vérifier:
   ✅ Message "Session supprimée"
   ✅ Session disparaît de la liste
7. Cliquer: Annuler
8. Vérifier fichier supprimé du dossier sessions/
✅ Attendu: Session supprimée définitivement
```

---

### **6. CALCUL ET PROGRESSDIALOG (5 min)**

**TEST 6.1: Calcul avec ProgressDialog**
```
PRÉ-REQUIS: Backend Python fonctionnel

1. Remplir formulaire valide
2. Cliquer: Calculer (ou F5)
3. Vérifier:
   ✅ ProgressDialog apparaît immédiatement
   ✅ Titre: "Calcul en cours..."
   ✅ Message: "Calcul des coûts en cours..."
   ✅ Barre progression animée (Marquee)
   ✅ Statut change:
      - "Préparation des données..."
      - "Communication avec le moteur Python..."
      - "Traitement des résultats..."
   ✅ ProgressDialog se ferme automatiquement
   ✅ ResultsForm s'ouvre avec résultats

✅ Attendu: Animation fluide, pas de freeze
❌ Si erreur Python: Message erreur clair
```

---

### **7. EXPORT CSV/PDF (8 min)**

**TEST 7.1: Export CSV**
```
1. Après calcul réussi (ResultsForm ouvert)
2. Cliquer: Export CSV
3. SaveFileDialog s'ouvre
4. Nom par défaut: TrackAFACE_Session_*.csv
5. Sauvegarder
6. Vérifier:
   ✅ Message "Export réussi"
   ✅ Fichier CSV créé
7. Ouvrir fichier dans Excel/Notepad++
8. Vérifier structure:
   ✅ En-tête Track-A-FACE
   ✅ Résumé 4 catégories
   ✅ Détails complets
   ✅ Pas de caractères mal encodés

✅ Attendu: CSV lisible, données correctes
```

**TEST 7.2: Export PDF**
```
PRÉ-REQUIS: BouncyCastle + iTextSharp restaurés

1. ResultsForm ouvert
2. Cliquer: Export PDF
3. SaveFileDialog s'ouvre
4. Nom par défaut: TrackAFACE_Report_*.pdf
5. Sauvegarder
6. Vérifier:
   ✅ Message "Export réussi"
   ✅ Option "Ouvrir le fichier" proposée
7. Cliquer: Oui
8. PDF s'ouvre dans lecteur
9. Vérifier:
   ✅ Titre "Track-A-FACE" (gros, en haut)
   ✅ Date rapport
   ✅ Nom session
   ✅ Coût total en ROUGE (encadré)
   ✅ Résumé coloré (4 catégories):
      - Personnel (VERT)
      - Équipement (BLEU)
      - Immobilier (ORANGE)
      - Opérationnel (VIOLET)
   ✅ Tableau détails (4 colonnes)
   ✅ Pied de page avec timestamp

✅ Attendu: PDF professionnel, couleurs correctes
❌ Si erreur: Vérifier packages NuGet restaurés
```

---

### **8. RACCOURCIS CLAVIER (5 min)**

**TEST 8.1: InputForm Raccourcis**
```
1. Focus sur InputForm
2. Appuyer: Ctrl+S
   ✅ Dialogue sauvegarde (si valide) ou erreur validation
3. Appuyer: Ctrl+O
   ✅ LoadSessionDialog s'ouvre
   Appuyer: Esc pour fermer
4. Appuyer: F5
   ✅ Calcul démarre (si Python OK) ou erreur
5. Appuyer: Ctrl+R
   ✅ Formulaire réinitialisé

✅ Attendu: Tous raccourcis répondent
```

**TEST 8.2: MainForm Raccourcis**
```
1. Focus sur MainForm
2. Appuyer: Ctrl+N
   ✅ Message "Nouvelle Session"
3. Appuyer: Ctrl+O
   ✅ LoadSessionDialog s'ouvre
4. Appuyer: F1
   ✅ AboutDialog s'ouvre

✅ Attendu: Raccourcis menus fonctionnent
```

---

## 📊 RÉSULTATS ATTENDUS

### **✅ SUCCÈS COMPLET:**
```
□ Compilation: 0 erreurs
□ Démarrage: Application s'ouvre
□ Menu: Tous items fonctionnels
□ Saisie: Tous champs validés
□ Sessions: Sauvegarder/Charger/Supprimer OK
□ Calcul: ProgressDialog fluide
□ Export CSV: Fichier correct
□ Export PDF: Format professionnel
□ Raccourcis: Tous répondent
```

### **⚠️ PROBLÈMES POSSIBLES:**

| Erreur | Cause | Solution |
|--------|-------|----------|
| **Compilation échoue** | Packages NuGet | Clic droit Solution → Restaurer packages |
| **PDF erreur BouncyCastle** | Package manquant | Vérifier packages.config ligne 3 |
| **Calcul erreur Python** | Backend absent | Vérifier PythonBridge.cs chemin Python |
| **Sessions vides** | Première utilisation | Normal, créer session test |
| **ProgressDialog freeze** | Async problème | Vérifier await bridge.CalculateAsync() |

---

## 📝 RAPPORT DE TESTS

Après avoir terminé tous les tests, compléter:

**TESTS RÉUSSIS:** [ ] / 20  
**TESTS ÉCHOUÉS:** [ ] / 20  
**BUGS IDENTIFIÉS:** [ ]

**Blocker (empêche utilisation):**
- [ ] Aucun

**Majeur (fonctionnalité cassée):**
- [ ] Aucun

**Mineur (cosmétique):**
- [ ] Aucun

**Prêt pour production:** OUI / NON

---

**Durée totale:** ~45 minutes  
**Date:** 2025-10-02  
**Version:** Track-A-FACE v1.0.0
