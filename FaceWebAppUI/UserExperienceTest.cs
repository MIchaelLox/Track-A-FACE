using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FaceWebAppUI
{
    /// <summary>
    /// Système de tests d'expérience utilisateur pour Track-A-FACE
    /// </summary>
    public class UserExperienceTest
    {
        private readonly MainForm _mainForm;
        private readonly List<UserTestResult> _testResults;
        private readonly Stopwatch _testTimer;

        public UserExperienceTest(MainForm mainForm)
        {
            _mainForm = mainForm ?? throw new ArgumentNullException(nameof(mainForm));
            _testResults = new List<UserTestResult>();
            _testTimer = new Stopwatch();
        }

        /// <summary>
        /// Lance tous les tests d'expérience utilisateur
        /// </summary>
        public async Task<List<UserTestResult>> RunAllUserExperienceTestsAsync()
        {
            _testResults.Clear();
            
            await RunTest("1. Test de navigation intuitive", TestNavigationIntuitive);
            await RunTest("2. Test de workflow guidé", TestWorkflowGuidance);
            await RunTest("3. Test de validation en temps réel", TestRealTimeValidation);
            await RunTest("4. Test de feedback utilisateur", TestUserFeedback);
            await RunTest("5. Test de performance interface", TestInterfacePerformance);
            await RunTest("6. Test d'accessibilité", TestAccessibility);
            await RunTest("7. Test de gestion d'erreurs UX", TestErrorHandlingUX);
            await RunTest("8. Test de sauvegarde/chargement UX", TestSessionManagementUX);
            
            return _testResults;
        }

        /// <summary>
        /// Test de navigation intuitive
        /// </summary>
        private async Task<bool> TestNavigationIntuitive()
        {
            var issues = new List<string>();
            
            // Vérifier la présence des boutons de navigation
            var navigationButtons = FindNavigationButtons();
            if (navigationButtons.Count < 2)
            {
                issues.Add("Boutons de navigation manquants dans la barre de statut");
            }

            // Vérifier les onglets avec icônes
            var tabControl = FindControl<TabControl>(_mainForm, "tabControl");
            if (tabControl != null)
            {
                foreach (TabPage tab in tabControl.TabPages)
                {
                    if (!tab.Text.Contains("📝") && !tab.Text.Contains("🔄") && !tab.Text.Contains("📊"))
                    {
                        issues.Add($"Onglet '{tab.Text}' sans icône visuelle");
                    }
                }
            }
            else
            {
                issues.Add("TabControl principal non trouvé");
            }

            // Vérifier les tooltips
            var controlsWithoutTooltips = CountControlsWithoutTooltips();
            if (controlsWithoutTooltips > 5)
            {
                issues.Add($"{controlsWithoutTooltips} contrôles sans tooltip d'aide");
            }

            return issues.Count == 0;
        }

        /// <summary>
        /// Test du workflow guidé
        /// </summary>
        private async Task<bool> TestWorkflowGuidance()
        {
            var issues = new List<string>();

            // Vérifier la présence de l'assistant workflow
            var workflowTab = FindControl<TabPage>(_mainForm, "tabWorkflow");
            if (workflowTab == null)
            {
                issues.Add("Onglet Assistant workflow manquant");
                return false;
            }

            // Vérifier la barre de progression
            var progressBar = FindControlByName(_mainForm, "pbWorkflowProgress");
            if (progressBar == null)
            {
                issues.Add("Barre de progression du workflow manquante");
            }

            // Vérifier les étapes du workflow
            var progressLabel = FindControlByName(_mainForm, "lblWorkflowProgress");
            if (progressLabel == null)
            {
                issues.Add("Label de progression du workflow manquant");
            }

            // Vérifier les boutons de navigation workflow
            var btnNext = FindControlByName(_mainForm, "btnWorkflowNext");
            var btnPrevious = FindControlByName(_mainForm, "btnWorkflowPrevious");
            
            if (btnNext == null || btnPrevious == null)
            {
                issues.Add("Boutons de navigation workflow manquants");
            }

            return issues.Count == 0;
        }

        /// <summary>
        /// Test de validation en temps réel
        /// </summary>
        private async Task<bool> TestRealTimeValidation()
        {
            var issues = new List<string>();

            // Simuler la saisie dans le champ nom de session
            var txtSessionName = FindControl<TextBox>(_mainForm, "txtSessionName");
            if (txtSessionName != null)
            {
                var originalText = txtSessionName.Text;
                var originalColor = txtSessionName.BackColor;

                // Test avec texte vide
                txtSessionName.Text = "";
                txtSessionName.Focus();
                await Task.Delay(100);
                
                if (txtSessionName.BackColor == originalColor)
                {
                    issues.Add("Validation visuelle manquante pour champ nom de session vide");
                }

                // Restaurer
                txtSessionName.Text = originalText;
            }
            else
            {
                issues.Add("Champ nom de session non trouvé");
            }

            // Vérifier les ComboBox
            var comboBoxes = FindAllControls<ComboBox>(_mainForm);
            foreach (var combo in comboBoxes)
            {
                if (combo.SelectedIndex == -1)
                {
                    var originalColor = combo.BackColor;
                    combo.Focus();
                    await Task.Delay(50);
                    
                    if (combo.BackColor == originalColor && combo.BackColor != Color.LightPink)
                    {
                        issues.Add($"ComboBox '{combo.Name}' sans validation visuelle");
                    }
                }
            }

            return issues.Count <= 2; // Tolérance pour quelques contrôles
        }

        /// <summary>
        /// Test de feedback utilisateur
        /// </summary>
        private async Task<bool> TestUserFeedback()
        {
            var issues = new List<string>();

            // Vérifier la barre de statut
            var statusLabel = FindControl<ToolStripStatusLabel>(_mainForm, "statusLabel");
            if (statusLabel == null)
            {
                issues.Add("Barre de statut manquante");
            }
            else if (string.IsNullOrWhiteSpace(statusLabel.Text))
            {
                issues.Add("Barre de statut sans message initial");
            }

            // Vérifier la barre de progression
            var progressBar = FindControl<ToolStripProgressBar>(_mainForm, "progressBar");
            if (progressBar == null)
            {
                issues.Add("Barre de progression manquante");
            }

            // Vérifier les messages d'aide/conseils dans le workflow
            var workflowContent = FindControlByName(_mainForm, "pnlWorkflowContent");
            if (workflowContent != null)
            {
                var hasHelpText = HasHelpTextInWorkflow(workflowContent);
                if (!hasHelpText)
                {
                    issues.Add("Messages d'aide manquants dans le workflow");
                }
            }

            return issues.Count == 0;
        }

        /// <summary>
        /// Test de performance de l'interface
        /// </summary>
        private async Task<bool> TestInterfacePerformance()
        {
            var issues = new List<string>();
            
            _testTimer.Restart();

            // Test de temps de réponse des onglets
            var tabControl = FindControl<TabControl>(_mainForm, "tabControl");
            if (tabControl != null)
            {
                var originalTab = tabControl.SelectedIndex;
                
                for (int i = 0; i < tabControl.TabPages.Count; i++)
                {
                    _testTimer.Restart();
                    tabControl.SelectedIndex = i;
                    Application.DoEvents();
                    _testTimer.Stop();
                    
                    if (_testTimer.ElapsedMilliseconds > 500)
                    {
                        issues.Add($"Changement d'onglet lent: {_testTimer.ElapsedMilliseconds}ms");
                    }
                }
                
                tabControl.SelectedIndex = originalTab;
            }

            // Test de temps de réponse des contrôles
            var numericControls = FindAllControls<NumericUpDown>(_mainForm);
            foreach (var numeric in numericControls.Take(3)) // Tester seulement les 3 premiers
            {
                _testTimer.Restart();
                var originalValue = numeric.Value;
                numeric.Value = originalValue + 1;
                Application.DoEvents();
                numeric.Value = originalValue;
                _testTimer.Stop();
                
                if (_testTimer.ElapsedMilliseconds > 200)
                {
                    issues.Add($"Contrôle numérique lent: {_testTimer.ElapsedMilliseconds}ms");
                }
            }

            return issues.Count <= 1; // Tolérance pour un contrôle lent
        }

        /// <summary>
        /// Test d'accessibilité
        /// </summary>
        private async Task<bool> TestAccessibility()
        {
            var issues = new List<string>();

            // Vérifier les TabIndex
            var controlsWithoutTabIndex = CountControlsWithoutTabIndex();
            if (controlsWithoutTabIndex > 10)
            {
                issues.Add($"{controlsWithoutTabIndex} contrôles sans TabIndex défini");
            }

            // Vérifier les raccourcis clavier
            var buttonsWithoutShortcuts = CountButtonsWithoutShortcuts();
            if (buttonsWithoutShortcuts > 5)
            {
                issues.Add($"{buttonsWithoutShortcuts} boutons sans raccourci clavier");
            }

            // Vérifier les couleurs de contraste
            var lowContrastControls = CountLowContrastControls();
            if (lowContrastControls > 3)
            {
                issues.Add($"{lowContrastControls} contrôles avec faible contraste");
            }

            // Vérifier les tailles de police
            var smallFontControls = CountSmallFontControls();
            if (smallFontControls > 5)
            {
                issues.Add($"{smallFontControls} contrôles avec police trop petite");
            }

            return issues.Count <= 2; // Tolérance pour quelques problèmes d'accessibilité
        }

        /// <summary>
        /// Test de gestion d'erreurs UX
        /// </summary>
        private async Task<bool> TestErrorHandlingUX()
        {
            var issues = new List<string>();

            // Vérifier que les erreurs sont affichées de manière conviviale
            try
            {
                // Simuler une erreur de validation
                var txtSessionName = FindControl<TextBox>(_mainForm, "txtSessionName");
                if (txtSessionName != null)
                {
                    var originalText = txtSessionName.Text;
                    txtSessionName.Text = ""; // Déclencher une erreur de validation
                    
                    // Vérifier si une indication visuelle apparaît
                    await Task.Delay(100);
                    
                    if (txtSessionName.BackColor == Color.White)
                    {
                        issues.Add("Pas d'indication visuelle pour erreur de validation");
                    }
                    
                    txtSessionName.Text = originalText;
                }
            }
            catch (Exception ex)
            {
                issues.Add($"Exception lors du test d'erreur: {ex.Message}");
            }

            return issues.Count == 0;
        }

        /// <summary>
        /// Test de sauvegarde/chargement UX
        /// </summary>
        private async Task<bool> TestSessionManagementUX()
        {
            var issues = new List<string>();

            // Vérifier la présence des boutons de session
            var btnSave = FindButtonByText(_mainForm, "Sauvegarder");
            var btnLoad = FindButtonByText(_mainForm, "Charger");
            
            if (btnSave == null && btnLoad == null)
            {
                // Chercher dans le menu
                var menuItems = FindMenuItems(_mainForm);
                var hasSaveMenu = menuItems.Any(m => m.Text.Contains("Sauvegarder"));
                var hasLoadMenu = menuItems.Any(m => m.Text.Contains("Charger"));
                
                if (!hasSaveMenu || !hasLoadMenu)
                {
                    issues.Add("Options de sauvegarde/chargement non accessibles");
                }
            }

            return issues.Count == 0;
        }

        /// <summary>
        /// Exécute un test individuel
        /// </summary>
        private async Task RunTest(string testName, Func<Task<bool>> testMethod)
        {
            var result = new UserTestResult { TestName = testName, StartTime = DateTime.Now };
            
            try
            {
                var success = await testMethod();
                result.Success = success;
                result.Message = success ? "✅ Réussi" : "⚠️ Améliorations possibles";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"❌ Erreur: {ex.Message}";
                result.Exception = ex;
            }
            finally
            {
                result.EndTime = DateTime.Now;
                result.Duration = result.EndTime - result.StartTime;
                _testResults.Add(result);
            }
        }

        /// <summary>
        /// Affiche les résultats des tests UX
        /// </summary>
        public void ShowTestResults()
        {
            var summary = new System.Text.StringBuilder();
            summary.AppendLine("=== RÉSULTATS DES TESTS D'EXPÉRIENCE UTILISATEUR ===\n");
            
            int passed = 0, warnings = 0, failed = 0;
            
            foreach (var result in _testResults)
            {
                summary.AppendLine($"{result.TestName}");
                summary.AppendLine($"  Status: {result.Message}");
                summary.AppendLine($"  Durée: {result.Duration.TotalMilliseconds:F0}ms");
                
                if (result.Success)
                    passed++;
                else if (result.Message.Contains("Améliorations"))
                    warnings++;
                else
                    failed++;
                
                summary.AppendLine();
            }
            
            summary.AppendLine($"RÉSUMÉ: {passed} réussis, {warnings} avec améliorations possibles, {failed} échoués");
            
            var score = (passed * 100.0 / _testResults.Count);
            summary.AppendLine($"Score UX: {score:F1}%");
            
            if (score >= 90)
                summary.AppendLine("🏆 Excellente expérience utilisateur!");
            else if (score >= 75)
                summary.AppendLine("👍 Bonne expérience utilisateur");
            else if (score >= 60)
                summary.AppendLine("⚠️ Expérience utilisateur acceptable, améliorations recommandées");
            else
                summary.AppendLine("❌ Expérience utilisateur nécessite des améliorations importantes");
            
            var dialog = new Form
            {
                Text = "Résultats des tests d'expérience utilisateur",
                Size = new Size(700, 600),
                StartPosition = FormStartPosition.CenterParent
            };
            
            var textBox = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                ReadOnly = true,
                Dock = DockStyle.Fill,
                Text = summary.ToString(),
                Font = new Font("Consolas", 9F)
            };
            
            dialog.Controls.Add(textBox);
            dialog.ShowDialog();
        }

        #region Méthodes utilitaires

        private T FindControl<T>(Control parent, string name) where T : Control
        {
            return parent.Controls.Find(name, true).OfType<T>().FirstOrDefault();
        }

        private Control FindControlByName(Control parent, string name)
        {
            return parent.Controls.Find(name, true).FirstOrDefault();
        }

        private List<T> FindAllControls<T>(Control parent) where T : Control
        {
            var controls = new List<T>();
            FindControlsRecursive(parent, controls);
            return controls;
        }

        private void FindControlsRecursive<T>(Control parent, List<T> controls) where T : Control
        {
            foreach (Control control in parent.Controls)
            {
                if (control is T t)
                    controls.Add(t);
                
                FindControlsRecursive(control, controls);
            }
        }

        private Button FindButtonByText(Control parent, string text)
        {
            var buttons = FindAllControls<Button>(parent);
            return buttons.FirstOrDefault(b => b.Text.Contains(text));
        }

        private List<ToolStripMenuItem> FindMenuItems(Form form)
        {
            var items = new List<ToolStripMenuItem>();
            
            if (form.MainMenuStrip != null)
            {
                foreach (ToolStripItem item in form.MainMenuStrip.Items)
                {
                    if (item is ToolStripMenuItem menuItem)
                    {
                        items.Add(menuItem);
                        AddSubMenuItems(menuItem, items);
                    }
                }
            }
            
            return items;
        }

        private void AddSubMenuItems(ToolStripMenuItem parent, List<ToolStripMenuItem> items)
        {
            foreach (ToolStripItem item in parent.DropDownItems)
            {
                if (item is ToolStripMenuItem menuItem)
                {
                    items.Add(menuItem);
                    AddSubMenuItems(menuItem, items);
                }
            }
        }

        private List<Button> FindNavigationButtons()
        {
            var buttons = FindAllControls<Button>(_mainForm);
            return buttons.Where(b => b.Text.Contains("Précédent") || b.Text.Contains("Suivant")).ToList();
        }

        private int CountControlsWithoutTooltips()
        {
            var allControls = FindAllControls<Control>(_mainForm);
            return allControls.Count(c => c is Button || c is TextBox || c is ComboBox || c is NumericUpDown);
        }

        private int CountControlsWithoutTabIndex()
        {
            var allControls = FindAllControls<Control>(_mainForm);
            return allControls.Count(c => c.TabIndex == 0 && c.TabStop);
        }

        private int CountButtonsWithoutShortcuts()
        {
            var buttons = FindAllControls<Button>(_mainForm);
            return buttons.Count(b => !b.Text.Contains("&"));
        }

        private int CountLowContrastControls()
        {
            var allControls = FindAllControls<Control>(_mainForm);
            return allControls.Count(c => IsLowContrast(c.ForeColor, c.BackColor));
        }

        private int CountSmallFontControls()
        {
            var allControls = FindAllControls<Control>(_mainForm);
            return allControls.Count(c => c.Font.Size < 8);
        }

        private bool IsLowContrast(Color foreground, Color background)
        {
            var brightness1 = (foreground.R * 299 + foreground.G * 587 + foreground.B * 114) / 1000;
            var brightness2 = (background.R * 299 + background.G * 587 + background.B * 114) / 1000;
            return Math.Abs(brightness1 - brightness2) < 125;
        }

        private bool HasHelpTextInWorkflow(Control workflowContent)
        {
            var labels = FindAllControls<Label>(workflowContent);
            return labels.Any(l => l.Text.Contains("💡") || l.Text.Contains("Conseil"));
        }

        #endregion
    }

    /// <summary>
    /// Résultat d'un test d'expérience utilisateur
    /// </summary>
    public class UserTestResult
    {
        public string TestName { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Duration { get; set; }
        public Exception Exception { get; set; }
    }
}
