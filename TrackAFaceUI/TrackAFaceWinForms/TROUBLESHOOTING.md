# 🔧 Troubleshooting - Track-A-FACE

## 🚨 ERREURS DE COMPILATION COURANTES

### **ERREUR 1: "BouncyCastle ou iTextSharp introuvable"**

**Message:**
```
Le type ou le nom d'espace de noms 'BouncyCastle' est introuvable
Le type ou le nom d'espace de noms 'iTextSharp' est introuvable
```

**Cause:** Packages NuGet non restaurés

**Solution:**
```
1. Clic droit sur Solution
2. "Restaurer les packages NuGet"
3. Attendre la fin
4. Recompiler (Ctrl+Shift+B)
```

**Solution Alternative:**
```
1. Tools → NuGet Package Manager → Package Manager Console
2. Taper: Update-Package -reinstall
3. Attendre
4. Recompiler
```

---

### **ERREUR 2: "TrackAFaceWinForms.Dialogs introuvable"**

**Message:**
```
Le type ou le nom d'espace de noms 'Dialogs' n'existe pas dans l'espace de noms 'TrackAFaceWinForms'
```

**Cause:** Fichiers Dialogs non inclus dans projet

**Solution:**
```
1. Dans Explorateur de solutions
2. Vérifier dossier "Dialogs" existe
3. Doit contenir:
   - AboutDialog.cs
   - LoadSessionDialog.cs
   - ProgressDialog.cs
4. Si absent: Clic droit Projet → Ajouter → Dossier existant → Dialogs/
```

---

### **ERREUR 3: "btnLoad n'est pas déclaré"**

**Message:**
```
Le nom 'btnLoad' n'existe pas dans le contexte actuel
```

**Cause:** Correction btnLoad non appliquée dans Designer

**Solution:**
```
1. Vérifier InputForm.Designer.cs ligne 318:
   private System.Windows.Forms.Button btnLoad;
   
2. Si absent, fichier InputForm.Designer.cs corrompu
3. Solution: Regénérer depuis commit
```

---

### **ERREUR 4: "ProgressDialog n'a pas de constructeur sans paramètres"**

**Message:**
```
'ProgressDialog' does not contain a constructor that takes 0 arguments
```

**Cause:** InitializeComponent() manquant

**Solution:**
```
Vérifier ProgressDialog.cs contient:
private void InitializeComponent()
{
    this.SuspendLayout();
    // ...
    this.ResumeLayout(false);
}
```

---

### **ERREUR 5: Avertissements Sécurité BouncyCastle**

**Message:**
```
NU1902: Le package 'BouncyCastle' 1.8.9 présente une vulnérabilité de gravité moderate
```

**Cause:** Version 1.8.9 a des vulnérabilités connues (NORMAL)

**Solution:**
```
✅ ACCEPTER - Ce n'est qu'un avertissement
✅ Gravité: MODERATE (pas CRITICAL)
✅ Impact: Faible pour application desktop locale
✅ Alternative: Aucune version compatible iTextSharp

Action: IGNORER et continuer
```

---

## 🚨 ERREURS D'EXÉCUTION (RUNTIME)

### **ERREUR 6: "Python.exe introuvable"**

**Message:**
```
Erreur lors du calcul:
Le système ne peut pas trouver le fichier spécifié
```

**Cause:** Backend Python absent ou chemin incorrect

**Solution:**
```
1. Vérifier backend Python existe:
   Track-A-FACE/backend/main.py
   
2. Vérifier PythonBridge.cs:
   - Chemin Python correct
   - Script main.py accessible
   
3. Options:
   a) Commenter test diagnostic MainForm.cs ligne 20-22
   b) Configurer chemin Python dans config
```

---

### **ERREUR 7: "Fichier session introuvable"**

**Message:**
```
Erreur lors du chargement:
Le chemin d'accès spécifié est introuvable
```

**Cause:** Dossier sessions/ n'existe pas

**Solution:**
```
1. Créer manuellement:
   C:\Users\hp\Documents\Track-A-FACE\sessions\
   
2. OU: Sauvegarder première session
   → Dossier créé automatiquement
```

---

### **ERREUR 8: "Export PDF erreur"**

**Message:**
```
Erreur d'export:
Impossible de charger le fichier ou l'assembly 'BouncyCastle.Crypto'
```

**Cause:** BouncyCastle.Crypto.dll manquant dans bin/

**Solution:**
```
1. Vérifier packages.config ligne 3:
   <package id="BouncyCastle" version="1.8.9" ...
   
2. Restaurer packages NuGet
3. Vérifier fichier existe:
   packages/BouncyCastle.1.8.9/lib/BouncyCastle.Crypto.dll
   
4. Recompiler (Ctrl+Shift+B)
5. Vérifier copié dans bin/Debug/BouncyCastle.Crypto.dll
```

---

### **ERREUR 9: "LoadSessionDialog liste vide"**

**Message:**
Aucun (liste vide normale)

**Cause:** Aucune session sauvegardée encore

**Solution:**
```
✅ NORMAL - Première utilisation
Action:
1. Fermer dialogue
2. Remplir formulaire
3. Ctrl+S (Sauvegarder)
4. Réessayer Ctrl+O (Charger)
```

---

### **ERREUR 10: "ProgressDialog freeze l'application"**

**Symptôme:** Application ne répond plus pendant calcul

**Cause:** Appel synchrone au lieu d'async

**Solution:**
```
Vérifier InputForm.cs btnCalculate_Click():
- Ligne 70: private async void btnCalculate_Click(...)
- Ligne 102: var results = await bridge.CalculateAsync(inputs);

Si "await" absent:
1. Ajouter "await" avant bridge.CalculateAsync
2. Ajouter "async" dans signature méthode
3. Recompiler
```

---

## 🔍 DIAGNOSTIC RAPIDE

### **CHECKLIST AVANT LANCEMENT:**

```
□ Visual Studio 2019+ installé
□ .NET Framework 4.7.2+ installé
□ Solution TrackAFaceWinForms.sln ouverte
□ Packages NuGet restaurés (BouncyCastle, iTextSharp)
□ 0 erreurs de compilation (Ctrl+Shift+B)
□ Backend Python présent (optionnel)
```

---

### **COMMANDES DIAGNOSTIC:**

**Dans Package Manager Console:**
```powershell
# Vérifier packages installés
Get-Package

# Réinstaller tous packages
Update-Package -reinstall

# Nettoyer et recompiler
dotnet clean
dotnet build
```

---

## 📞 SUPPORT

Si aucune solution ne fonctionne:

1. **Copier message d'erreur complet**
2. **Vérifier ligne exacte de l'erreur**
3. **Partager contexte:**
   - Version Visual Studio
   - Message erreur complet
   - Fichier concerné
   - Ligne de code

---

## ✅ TESTS POST-CORRECTION

Après correction erreur:

```
1. Nettoyer solution (Générer → Nettoyer la solution)
2. Recompiler (Ctrl+Shift+B)
3. Vérifier: 0 erreurs
4. Lancer (F5)
5. Tester fonctionnalité corrigée
```

---

**Version:** Track-A-FACE v1.0.0  
**Dernière mise à jour:** 2025-10-02
