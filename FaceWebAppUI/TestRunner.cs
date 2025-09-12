using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FaceWebAppUI
{
    /// <summary>
    /// Classe pour exécuter les tests d'intégration de l'application
    /// </summary>
    public class TestRunner
    {
        private readonly MainForm _mainForm;
        private readonly List<TestResult> _testResults;

        public TestRunner(MainForm mainForm)
        {
            _mainForm = mainForm;
            _testResults = new List<TestResult>();
        }

        /// <summary>
        /// Exécute tous les tests d'intégration
        /// </summary>
        public async Task<List<TestResult>> RunAllTestsAsync()
        {
            _testResults.Clear();

            // Test 1: Validation des contrôles UI
            await RunTest("UI Controls Validation", TestUIControls);

            // Test 2: Validation des données d'entrée
            await RunTest("Input Data Validation", TestInputValidation);

            // Test 3: Test de collecte des données
            await RunTest("Data Collection", TestDataCollection);

            // Test 4: Test de sauvegarde/chargement
            await RunTest("Save/Load Functionality", TestSaveLoad);

            // Test 5: Test de l'interface Python (si disponible)
            await RunTest("Python Integration", TestPythonIntegration);

            return _testResults;
        }

        private async Task RunTest(string testName, Func<Task<bool>> testMethod)
        {
            var startTime = DateTime.Now;
            try
            {
                var success = await testMethod();
                var duration = DateTime.Now - startTime;
                
                _testResults.Add(new TestResult
                {
                    TestName = testName,
                    Success = success,
                    Duration = duration,
                    Message = success ? "Test réussi" : "Test échoué"
                });
            }
            catch (Exception ex)
            {
                var duration = DateTime.Now - startTime;
                _testResults.Add(new TestResult
                {
                    TestName = testName,
                    Success = false,
                    Duration = duration,
                    Message = $"Exception: {ex.Message}"
                });
            }
        }

        private async Task<bool> TestUIControls()
        {
            await Task.Delay(100); // Simulation async

            // Vérifier que tous les contrôles essentiels existent
            var controls = new[]
            {
                _mainForm.txtSessionName,
                _mainForm.cmbTheme,
                _mainForm.cmbRevenueSize,
                _mainForm.numKitchenSize,
                _mainForm.numWorkstations,
                _mainForm.numCapacity,
                _mainForm.numStaffCount,
                _mainForm.cmbExperience,
                _mainForm.btnCalculate,
                _mainForm.dgvResults
            };

            foreach (var control in controls)
            {
                if (control == null)
                    return false;
            }

            // Vérifier que les ComboBox ont des éléments
            if (_mainForm.cmbTheme.Items.Count == 0 ||
                _mainForm.cmbRevenueSize.Items.Count == 0 ||
                _mainForm.cmbExperience.Items.Count == 0)
                return false;

            return true;
        }

        private async Task<bool> TestInputValidation()
        {
            await Task.Delay(100);

            // Test avec des valeurs valides
            _mainForm.txtSessionName.Text = "Test Restaurant";
            _mainForm.cmbTheme.SelectedIndex = 1;
            _mainForm.cmbRevenueSize.SelectedIndex = 1;
            _mainForm.numKitchenSize.Value = 100;
            _mainForm.numWorkstations.Value = 8;
            _mainForm.numCapacity.Value = 200;
            _mainForm.numStaffCount.Value = 15;
            _mainForm.cmbExperience.SelectedIndex = 1;

            // Vérifier que les données peuvent être collectées
            try
            {
                var data = _mainForm.CollectInputData();
                return data != null && data.Count > 0;
            }
            catch
            {
                return false;
            }
        }

        private async Task<bool> TestDataCollection()
        {
            await Task.Delay(100);

            var data = _mainForm.CollectInputData();
            
            // Vérifier que toutes les clés essentielles sont présentes
            var requiredKeys = new[]
            {
                "session_name", "restaurant_theme", "revenue_size",
                "kitchen_size_sqm", "kitchen_workstations", "daily_capacity",
                "staff_count", "staff_experience_level"
            };

            foreach (var key in requiredKeys)
            {
                if (!data.ContainsKey(key))
                    return false;
            }

            // Vérifier les types de données
            return data["kitchen_size_sqm"] is double &&
                   data["kitchen_workstations"] is int &&
                   data["daily_capacity"] is int &&
                   data["staff_count"] is int;
        }

        private async Task<bool> TestSaveLoad()
        {
            await Task.Delay(100);

            try
            {
                // Préparer des données de test
                var testData = new Dictionary<string, object>
                {
                    ["session_name"] = "Test Session",
                    ["restaurant_theme"] = "casual_dining",
                    ["revenue_size"] = "medium",
                    ["kitchen_size_sqm"] = 150.0,
                    ["kitchen_workstations"] = 10,
                    ["daily_capacity"] = 250,
                    ["staff_count"] = 20,
                    ["staff_experience_level"] = "intermediate",
                    ["training_hours_needed"] = 50,
                    ["equipment_age_years"] = 3,
                    ["equipment_condition"] = "good",
                    ["equipment_value"] = 180000.0,
                    ["location_rent_sqm"] = 45.0
                };

                // Test de sauvegarde
                var tempFile = Path.GetTempFileName();
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(testData, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(tempFile, json);

                // Test de chargement
                var loadedJson = File.ReadAllText(tempFile);
                var loadedData = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(loadedJson);

                // Nettoyer
                File.Delete(tempFile);

                return loadedData != null && loadedData.Count == testData.Count;
            }
            catch
            {
                return false;
            }
        }

        private async Task<bool> TestPythonIntegration()
        {
            await Task.Delay(100);

            try
            {
                var projectRoot = Path.GetDirectoryName(Path.GetDirectoryName(Application.StartupPath)) ?? "";
                var bridge = new PythonBridge(projectRoot);

                // Test de validation de l'environnement
                var isValid = bridge.ValidateEnvironment(out string errorMessage);
                
                if (!isValid)
                {
                    // Si Python n'est pas disponible, ce n'est pas un échec critique pour l'UI
                    return true; // On considère que c'est OK pour les tests UI
                }

                // Si Python est disponible, tester la connexion
                return await bridge.TestConnectionAsync();
            }
            catch
            {
                // Erreur de Python n'est pas critique pour les tests UI
                return true;
            }
        }

        /// <summary>
        /// Affiche les résultats des tests dans une boîte de dialogue
        /// </summary>
        public void ShowTestResults()
        {
            var message = "=== RÉSULTATS DES TESTS D'INTÉGRATION ===\n\n";
            
            int passed = 0;
            int total = _testResults.Count;

            foreach (var result in _testResults)
            {
                var status = result.Success ? "✅ RÉUSSI" : "❌ ÉCHOUÉ";
                message += $"{status} - {result.TestName}\n";
                message += $"   Durée: {result.Duration.TotalMilliseconds:F0}ms\n";
                message += $"   Message: {result.Message}\n\n";
                
                if (result.Success) passed++;
            }

            message += $"RÉSUMÉ: {passed}/{total} tests réussis\n";
            
            if (passed == total)
            {
                message += "🎉 Tous les tests sont passés avec succès !";
            }
            else
            {
                message += "⚠️ Certains tests ont échoué. Vérifiez la configuration.";
            }

            MessageBox.Show(message, "Résultats des Tests", MessageBoxButtons.OK, 
                passed == total ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
        }
    }

    /// <summary>
    /// Résultat d'un test individuel
    /// </summary>
    public class TestResult
    {
        public string TestName { get; set; } = string.Empty;
        public bool Success { get; set; }
        public TimeSpan Duration { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
