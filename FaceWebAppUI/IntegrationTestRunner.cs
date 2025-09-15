using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace FaceWebAppUI
{
    /// <summary>
    /// Exécuteur de tests d'intégration complets pour Track-A-FACE
    /// </summary>
    public class IntegrationTestRunner
    {
        private readonly MainForm _mainForm;
        private readonly List<TestResult> _testResults;
        private readonly string _projectRoot;

        public IntegrationTestRunner(MainForm mainForm)
        {
            _mainForm = mainForm ?? throw new ArgumentNullException(nameof(mainForm));
            _testResults = new List<TestResult>();
            _projectRoot = Path.GetDirectoryName(Application.ExecutablePath);
            
            // Remonter au répertoire racine du projet
            while (_projectRoot != null && !File.Exists(Path.Combine(_projectRoot, "engine.py")))
            {
                _projectRoot = Directory.GetParent(_projectRoot)?.FullName;
            }
        }

        /// <summary>
        /// Exécute tous les tests d'intégration
        /// </summary>
        public async Task<List<TestResult>> RunAllIntegrationTestsAsync()
        {
            _testResults.Clear();
            
            await RunTest("1. Validation de l'environnement Python", TestPythonEnvironment);
            await RunTest("2. Validation des fichiers backend", TestBackendFiles);
            await RunTest("3. Test de connectivité Python-C#", TestPythonConnectivity);
            await RunTest("4. Test de calcul simple", TestSimpleCalculation);
            await RunTest("5. Test de gestion d'erreurs", TestErrorHandling);
            await RunTest("6. Test de validation des données", TestDataValidation);
            await RunTest("7. Test des fonctionnalités UI", TestUIFunctionality);
            await RunTest("8. Test end-to-end complet", TestEndToEndWorkflow);
            
            return _testResults;
        }

        /// <summary>
        /// Teste l'environnement Python
        /// </summary>
        private async Task<bool> TestPythonEnvironment()
        {
            try
            {
                var bridge = new PythonBridge(_projectRoot);
                var diagnosticInfo = bridge.GetDiagnosticInfo();
                
                var pythonAvailable = (bool)diagnosticInfo.GetValueOrDefault("python_available", false);
                var filesValid = (bool)diagnosticInfo.GetValueOrDefault("files_valid", false);
                
                if (!pythonAvailable)
                {
                    throw new Exception($"Python non disponible: {diagnosticInfo.GetValueOrDefault("python_error", "Erreur inconnue")}");
                }
                
                if (!filesValid)
                {
                    var missingFiles = diagnosticInfo.GetValueOrDefault("missing_files", new List<string>()) as List<string>;
                    throw new Exception($"Fichiers manquants: {string.Join(", ", missingFiles ?? new List<string>())}");
                }
                
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Environnement Python invalide: {ex.Message}");
            }
        }

        /// <summary>
        /// Teste la présence des fichiers backend
        /// </summary>
        private async Task<bool> TestBackendFiles()
        {
            var requiredFiles = new[]
            {
                "engine.py", "engine_api.py", "engine_classes.py",
                "input_handler.py", "sql.py", "config.py"
            };

            var missingFiles = new List<string>();
            
            foreach (var file in requiredFiles)
            {
                var filePath = Path.Combine(_projectRoot, file);
                if (!File.Exists(filePath))
                {
                    missingFiles.Add(file);
                }
            }

            if (missingFiles.Count > 0)
            {
                throw new Exception($"Fichiers backend manquants: {string.Join(", ", missingFiles)}");
            }

            return true;
        }

        /// <summary>
        /// Teste la connectivité Python-C#
        /// </summary>
        private async Task<bool> TestPythonConnectivity()
        {
            var bridge = new PythonBridge(_projectRoot);
            
            if (!bridge.ValidateEnvironment(out string errorMessage))
            {
                throw new Exception($"Validation environnement échouée: {errorMessage}");
            }

            var isConnected = await bridge.TestConnectionAsync();
            if (!isConnected)
            {
                throw new Exception("Test de connexion Python échoué");
            }

            return true;
        }

        /// <summary>
        /// Teste un calcul simple
        /// </summary>
        private async Task<bool> TestSimpleCalculation()
        {
            var bridge = new PythonBridge(_projectRoot);
            
            var testData = new Dictionary<string, object>
            {
                ["session_name"] = "Test Integration",
                ["restaurant_theme"] = "casual_dining",
                ["revenue_size"] = "medium",
                ["kitchen_size_sqm"] = 100.0,
                ["kitchen_workstations"] = 8,
                ["daily_capacity"] = 200,
                ["staff_count"] = 15,
                ["staff_experience_level"] = "intermediate",
                ["training_hours_needed"] = 40,
                ["equipment_age_years"] = 2,
                ["equipment_condition"] = "good",
                ["equipment_value"] = 150000.0,
                ["location_rent_sqm"] = 40.0
            };

            var result = await bridge.ExecuteCalculationAsync(testData);
            
            if (result == null)
            {
                throw new Exception("Aucun résultat retourné");
            }

            if (result.TotalCost <= 0)
            {
                throw new Exception($"Coût total invalide: {result.TotalCost}");
            }

            if (result.CostBreakdowns == null || result.CostBreakdowns.Count == 0)
            {
                throw new Exception("Détails des coûts manquants");
            }

            // Vérifier la cohérence des totaux
            var calculatedTotal = result.StaffCosts + result.EquipmentCosts + 
                                result.LocationCosts + result.OperationalCosts;
            
            var tolerance = Math.Abs(result.TotalCost * 0.01); // 1% de tolérance
            if (Math.Abs(calculatedTotal - result.TotalCost) > tolerance)
            {
                throw new Exception($"Incohérence dans les totaux: {calculatedTotal} vs {result.TotalCost}");
            }

            return true;
        }

        /// <summary>
        /// Teste la gestion d'erreurs
        /// </summary>
        private async Task<bool> TestErrorHandling()
        {
            var bridge = new PythonBridge(_projectRoot);
            
            // Test avec des données invalides
            var invalidData = new Dictionary<string, object>
            {
                ["session_name"] = "",
                ["restaurant_theme"] = "invalid_theme",
                ["revenue_size"] = "invalid_size",
                ["kitchen_size_sqm"] = -100.0, // Valeur négative
                ["kitchen_workstations"] = 0,
                ["daily_capacity"] = -50,
                ["staff_count"] = 0,
                ["staff_experience_level"] = "invalid",
                ["training_hours_needed"] = -10,
                ["equipment_age_years"] = -5,
                ["equipment_condition"] = "invalid",
                ["equipment_value"] = -1000.0,
                ["location_rent_sqm"] = -20.0
            };

            try
            {
                var result = await bridge.ExecuteCalculationAsync(invalidData);
                throw new Exception("Le calcul aurait dû échouer avec des données invalides");
            }
            catch (PythonExecutionException)
            {
                // Attendu - c'est une bonne chose
                return true;
            }
            catch (PythonCalculationException)
            {
                // Attendu - c'est une bonne chose
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Type d'erreur inattendu: {ex.GetType().Name} - {ex.Message}");
            }
        }

        /// <summary>
        /// Teste la validation des données
        /// </summary>
        private async Task<bool> TestDataValidation()
        {
            // Simuler la validation côté UI
            var testCases = new[]
            {
                new { Name = "", Valid = false, Error = "Nom requis" },
                new { Name = "AB", Valid = false, Error = "Nom trop court" },
                new { Name = "Test Session", Valid = true, Error = "" }
            };

            foreach (var testCase in testCases)
            {
                var isValid = !string.IsNullOrWhiteSpace(testCase.Name) && testCase.Name.Length >= 3;
                
                if (isValid != testCase.Valid)
                {
                    throw new Exception($"Validation échouée pour '{testCase.Name}': attendu {testCase.Valid}, obtenu {isValid}");
                }
            }

            return true;
        }

        /// <summary>
        /// Teste les fonctionnalités UI
        /// </summary>
        private async Task<bool> TestUIFunctionality()
        {
            // Vérifier que les contrôles principaux existent
            var requiredControls = new[]
            {
                "txtSessionName", "cmbTheme", "cmbRevenueSize", "cmbExperience", 
                "cmbEquipmentCondition", "numKitchenSize", "numWorkstations", 
                "numCapacity", "numStaffCount", "numTrainingHours", 
                "numEquipmentAge", "numEquipmentValue", "numRentPerSqm"
            };

            foreach (var controlName in requiredControls)
            {
                var control = FindControlByName(_mainForm, controlName);
                if (control == null)
                {
                    throw new Exception($"Contrôle manquant: {controlName}");
                }
            }

            return true;
        }

        /// <summary>
        /// Teste le workflow end-to-end complet
        /// </summary>
        private async Task<bool> TestEndToEndWorkflow()
        {
            try
            {
                // 1. Collecter les données d'entrée
                var inputData = _mainForm.CollectInputData();
                
                if (inputData == null || inputData.Count == 0)
                {
                    throw new Exception("Échec de collecte des données d'entrée");
                }

                // 2. Exécuter le calcul via l'interface
                var bridge = new PythonBridge(_projectRoot);
                var result = await bridge.ExecuteCalculationWithRetryAsync(inputData, 2);
                
                if (result == null)
                {
                    throw new Exception("Échec du calcul end-to-end");
                }

                // 3. Vérifier que les résultats sont cohérents
                if (result.TotalCost <= 0 || result.CostBreakdowns == null)
                {
                    throw new Exception("Résultats incohérents");
                }

                // 4. Test d'export (simulation)
                var tempDir = Path.GetTempPath();
                var testExportPath = Path.Combine(tempDir, $"test_export_{DateTime.Now:yyyyMMdd_HHmmss}.json");
                
                try
                {
                    var exportData = new
                    {
                        session = inputData,
                        results = result,
                        timestamp = DateTime.Now
                    };
                    
                    File.WriteAllText(testExportPath, Newtonsoft.Json.JsonConvert.SerializeObject(exportData, Newtonsoft.Json.Formatting.Indented));
                    
                    if (!File.Exists(testExportPath))
                    {
                        throw new Exception("Échec de l'export de test");
                    }
                }
                finally
                {
                    if (File.Exists(testExportPath))
                    {
                        File.Delete(testExportPath);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Workflow end-to-end échoué: {ex.Message}");
            }
        }

        /// <summary>
        /// Exécute un test individuel
        /// </summary>
        private async Task RunTest(string testName, Func<Task<bool>> testMethod)
        {
            var result = new TestResult { TestName = testName, StartTime = DateTime.Now };
            
            try
            {
                var success = await testMethod();
                result.Success = success;
                result.Message = success ? "✅ Réussi" : "❌ Échec";
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
        /// Trouve un contrôle par son nom
        /// </summary>
        private Control FindControlByName(Control parent, string name)
        {
            if (parent.Name == name)
                return parent;

            foreach (Control child in parent.Controls)
            {
                var found = FindControlByName(child, name);
                if (found != null)
                    return found;
            }

            return null;
        }

        /// <summary>
        /// Affiche les résultats des tests
        /// </summary>
        public void ShowTestResults()
        {
            var summary = new System.Text.StringBuilder();
            summary.AppendLine("=== RÉSULTATS DES TESTS D'INTÉGRATION ===\n");
            
            int passed = 0, failed = 0;
            
            foreach (var result in _testResults)
            {
                summary.AppendLine($"{result.TestName}");
                summary.AppendLine($"  Status: {result.Message}");
                summary.AppendLine($"  Durée: {result.Duration.TotalMilliseconds:F0}ms");
                
                if (result.Success)
                    passed++;
                else
                    failed++;
                
                summary.AppendLine();
            }
            
            summary.AppendLine($"RÉSUMÉ: {passed} réussis, {failed} échoués");
            summary.AppendLine($"Taux de réussite: {(passed * 100.0 / _testResults.Count):F1}%");
            
            var dialog = new Form
            {
                Text = "Résultats des tests d'intégration",
                Size = new System.Drawing.Size(600, 500),
                StartPosition = FormStartPosition.CenterParent
            };
            
            var textBox = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                ReadOnly = true,
                Dock = DockStyle.Fill,
                Text = summary.ToString(),
                Font = new System.Drawing.Font("Consolas", 9F)
            };
            
            dialog.Controls.Add(textBox);
            dialog.ShowDialog();
        }
    }

    /// <summary>
    /// Résultat d'un test individuel
    /// </summary>
    public class TestResult
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
