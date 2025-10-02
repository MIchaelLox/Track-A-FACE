# ⌨️ Raccourcis Clavier - Track-A-FACE

## 📋 MainForm (Menu Principal)

| Raccourci | Action | Description |
|-----------|--------|-------------|
| **Ctrl+N** | Nouvelle Session | Réinitialise le formulaire pour une nouvelle session |
| **Ctrl+O** | Ouvrir Session | Ouvre le dialogue LoadSessionDialog pour charger une session |
| **Alt+F4** | Quitter | Quitte l'application (avec confirmation) |
| **F1** | À Propos | Affiche le dialogue AboutDialog |

---

## 📝 InputForm (Formulaire de Saisie)

| Raccourci | Action | Description |
|-----------|--------|-------------|
| **Ctrl+S** | Sauvegarder | Sauvegarde la session actuelle |
| **Ctrl+O** | Charger | Ouvre LoadSessionDialog pour charger une session |
| **F5** | Calculer | Lance le calcul des coûts (avec ProgressDialog) |
| **Ctrl+R** | Réinitialiser | Réinitialise tous les champs aux valeurs par défaut |

---

## 📊 Navigation Générale

| Raccourci | Action | Description |
|-----------|--------|-------------|
| **Tab** | Champ suivant | Navigue vers le champ suivant |
| **Shift+Tab** | Champ précédent | Navigue vers le champ précédent |
| **Enter** | Valider | Valide le champ actuel (ou bouton par défaut) |
| **Esc** | Annuler | Ferme les dialogues |

---

## 🎯 Conseils d'Utilisation

### **Workflow Rapide:**
```
1. Ctrl+N → Nouvelle session
2. Remplir les champs (Tab pour naviguer)
3. F5 → Calculer
4. Ctrl+S → Sauvegarder
```

### **Reprendre une Session:**
```
1. Ctrl+O → Charger session
2. Sélectionner dans la liste
3. Modifier si nécessaire
4. F5 → Recalculer
```

### **Workflow Complet:**
```
1. Ctrl+N → Nouvelle session
2. Saisir Nom de session
3. Choisir Thème (combo)
4. Sélectionner Taille revenus (radio)
5. Remplir tous les champs
6. F5 → Calculer
7. Voir résultats
8. Export CSV/PDF depuis ResultsForm
9. Ctrl+S → Sauvegarder si modifications
```

---

## 🔥 Raccourcis Experts

### **Dans LoadSessionDialog:**
- **↑↓** = Naviguer dans la liste
- **Enter** = Charger la session sélectionnée
- **Delete** = Supprimer la session (avec confirmation)
- **Esc** = Annuler

### **Dans les Dialogues:**
- **Enter** = Bouton par défaut (OK, Charger, etc.)
- **Esc** = Annuler/Fermer
- **Alt+O** = OK (souligné)
- **Alt+C** = Cancel (souligné)

---

## 💡 Astuces Productivité

### **1. Saisie Rapide:**
```
- Utilisez Tab pour naviguer sans souris
- NumericUpDown: ↑↓ pour ajuster, ou taper directement
- ComboBox: ↑↓ pour sélectionner, ou taper première lettre
- RadioButtons: ↑↓ pour sélectionner
```

### **2. Calculs Multiples:**
```
F5 → Voir résultats → Esc (fermer) → Modifier → F5 → ...
```

### **3. Sessions:**
```
Ctrl+S fréquemment pour ne pas perdre de données
Ctrl+O pour comparer différentes configurations
```

---

## 🛠️ Pour les Développeurs

### **Implémentation des Raccourcis:**

**InputForm.cs:**
```csharp
protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
{
    if (keyData == (Keys.Control | Keys.S))
    {
        btnSave_Click(this, EventArgs.Empty);
        return true;
    }
    // ... autres raccourcis
    return base.ProcessCmdKey(ref msg, keyData);
}
```

**MainForm.Designer.cs (MenuStrip):**
```csharp
menuFichierOuvrir.ShortcutKeys = Keys.Control | Keys.O;
menuAideAPropos.ShortcutKeys = Keys.F1;
```

### **Ajouter un Nouveau Raccourci:**
1. Dans `ProcessCmdKey()`, ajouter condition:
   ```csharp
   if (keyData == (Keys.Control | Keys.X)) { ... }
   ```
2. Mettre à jour cette documentation
3. Tester le raccourci

---

## 📝 Personnalisation

Les raccourcis peuvent être modifiés dans:
- `InputForm.cs` → Méthode `ProcessCmdKey()`
- `MainForm.Designer.cs` → Propriétés `ShortcutKeys` des MenuItems

**Note:** Éviter les conflits avec raccourcis Windows standard.

---

**Version:** Phase 3 - Track-A-FACE v1.0.0  
**Dernière mise à jour:** 2025-10-02
