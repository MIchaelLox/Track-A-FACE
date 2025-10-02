# 📂 Dialogs - Dialogues Personnalisés Track-A-FACE

## 📋 Vue d'ensemble

Ce dossier contient tous les dialogues personnalisés de l'application Track-A-FACE.

---

## 🎨 AboutDialog.cs

**Dialogue "À Propos"** - Affiche les informations sur l'application.

### Fonctionnalités:
- En-tête coloré avec logo Track-A-FACE (vert #4CAF50)
- Version de l'application (1.0.0)
- Description détaillée des fonctionnalités
- Copyright et informations légales
- Bouton OK pour fermer

### Utilisation:
```csharp
using (var dialog = new AboutDialog())
{
    dialog.ShowDialog();
}
```

### Design:
- Taille: 500x400 px
- Couleurs: Vert Track-A-FACE (#4CAF50)
- Police: Segoe UI

---

## 📂 LoadSessionDialog.cs

**Dialogue "Charger une Session"** - Permet de sélectionner et charger une session sauvegardée.

### Fonctionnalités:
- Liste toutes les sessions disponibles (triées par date de modification)
- Affichage personnalisé (Owner-drawn ListBox):
  - Nom de session (gras)
  - Date de création
  - Date de modification
  - Fond coloré pour sélection (vert clair)
- Panel de détails affichant les informations complètes
- Bouton **Charger** (vert) - Retourne le chemin de la session
- Bouton **Supprimer** (rouge) - Supprime la session avec confirmation
- Bouton **Annuler** (gris) - Ferme le dialogue

### Utilisation:
```csharp
using (var dialog = new LoadSessionDialog())
{
    if (dialog.ShowDialog() == DialogResult.OK)
    {
        string sessionPath = dialog.SelectedSessionPath;
        // Charger la session depuis sessionPath
    }
}
```

### Propriétés publiques:
- `SelectedSessionPath` (string) - Chemin de la session sélectionnée

### Design:
- Taille: 600x500 px
- Couleurs: Bleu en-tête (#2196F3), Vert bouton (#4CAF50), Rouge suppression (#F44336)
- ListBox personnalisé: 60px par item

---

## ⏳ ProgressDialog.cs

**Dialogue "Progression"** - Affiche une barre de progression pendant le calcul Python.

### Fonctionnalités:
- Barre de progression animée (Marquee style)
- Message principal personnalisable
- Statut secondaire personnalisable
- Mode déterminé/indéterminé
- Pas de bouton fermer (ControlBox = false)
- Thread-safe (utilise Invoke)

### Utilisation:
```csharp
var progress = new ProgressDialog();
progress.Show();

// Mettre à jour le message
progress.UpdateMessage("Calcul en cours...");

// Mettre à jour le statut
progress.UpdateStatus("Traitement des données...");

// Mode déterminé
progress.SetProgressMode(false);
progress.SetProgress(50); // 0-100

// Fermer quand terminé
progress.Close();
```

### Méthodes publiques:
- `UpdateMessage(string message)` - Change le message principal
- `UpdateStatus(string status)` - Change le texte de statut
- `SetProgressMode(bool indeterminate)` - Mode déterminé/indéterminé
- `SetProgress(int value)` - Valeur 0-100 (mode déterminé uniquement)

### Design:
- Taille: 450x200 px
- Couleurs: Blanc fond, texte gris foncé
- Barre de progression: 360x30 px, animation 30ms

---

## 🎯 Conventions de Code

### Nomenclature:
- Tous les dialogues héritent de `Form`
- Suffixe `Dialog` pour tous les fichiers
- Namespace: `TrackAFaceWinForms.Dialogs`

### Style:
- Polices: Segoe UI
- Couleurs Track-A-FACE:
  - Vert principal: #4CAF50 (76, 175, 80)
  - Bleu: #2196F3 (33, 150, 243)
  - Rouge: #F44336 (244, 67, 54)
  - Gris: #9E9E9E (158, 158, 158)

### Disposition:
- `FormBorderStyle.FixedDialog`
- `StartPosition.CenterParent`
- `MaximizeBox = false`
- `MinimizeBox = false`

---

## 🧪 Tests

### Tests manuels requis:

**AboutDialog:**
1. Ouvrir le dialogue
2. Vérifier l'affichage du titre et version
3. Cliquer OK → Fermeture

**LoadSessionDialog:**
1. Ouvrir avec sessions existantes
2. Sélectionner une session → Détails affichés
3. Boutons activés/désactivés selon sélection
4. Supprimer une session → Confirmation demandée
5. Charger une session → DialogResult.OK retourné

**ProgressDialog:**
1. Ouvrir en mode indéterminé
2. Changer messages dynamiquement
3. Passer en mode déterminé
4. Mettre à jour progression 0→100
5. Fermer le dialogue

---

## 📦 Dépendances

- System.Windows.Forms
- System.Drawing
- TrackAFaceWinForms.Session (LoadSessionDialog uniquement)

---

## 🚀 Améliorations Futures

- [ ] AboutDialog: Ajouter lien vers documentation
- [ ] LoadSessionDialog: Filtrage/recherche de sessions
- [ ] LoadSessionDialog: Aperçu des données de session
- [ ] ProgressDialog: Bouton annuler avec callback
- [ ] ProgressDialog: Estimation temps restant
- [ ] Dialogues: Support thèmes sombre/clair

---

**Dernière mise à jour:** Phase 2 - Dialogues UX (2025-10-02)
