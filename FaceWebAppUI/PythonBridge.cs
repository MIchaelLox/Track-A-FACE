using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FaceWebAppUI
{
    /// <summary>
    /// Pont de communication entre l'interface C# et le backend Python
    /// </summary>
    public class PythonBridge
    {
        private readonly string _pythonScriptPath;
        private readonly string _projectRoot;

        public PythonBridge(string projectRoot)
        {
            _projectRoot = projectRoot;
            _pythonScriptPath = Path.Combine(projectRoot, "engine.py");
        }

        /// <summary>
        /// Exécute le calcul Python avec les données d'entrée
        /// </summary>
        public async Task<CalculationResult> ExecuteCalculationAsync(Dictionary<string, object> inputData)
        {
            return await Task.Run(() =>
            {
                try
                {
                    // Créer un fichier temporaire avec les données JSON
                    var jsonInput = JsonConvert.SerializeObject(inputData, Formatting.Indented);
                    var tempInputFile = Path.GetTempFileName();
                    var tempOutputFile = Path.GetTempFileName();
                    
                    File.WriteAllText(tempInputFile, jsonInput);

                    // Configuration du processus Python
                    var startInfo = new ProcessStartInfo
                    {
                        FileName = "python",
                        Arguments = $"\"{_pythonScriptPath}\" --input \"{tempInputFile}\" --output \"{tempOutputFile}\"",
                        WorkingDirectory = _projectRoot,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    using var process = Process.Start(startInfo);
                    if (process == null)
                        throw new Exception("Impossible de démarrer le processus Python");

                    var output = process.StandardOutput.ReadToEnd();
                    var error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    // Nettoyer les fichiers temporaires
                    try
                    {
                        File.Delete(tempInputFile);
                    }
                    catch { }

                    if (process.ExitCode != 0)
                    {
                        throw new Exception($"Erreur Python (Code {process.ExitCode}): {error}");
                    }

                    // Lire le résultat depuis le fichier de sortie ou stdout
                    string resultJson;
                    if (File.Exists(tempOutputFile) && new FileInfo(tempOutputFile).Length > 0)
                    {
                        resultJson = File.ReadAllText(tempOutputFile);
                        File.Delete(tempOutputFile);
                    }
                    else
                    {
                        resultJson = output;
                    }

                    if (string.IsNullOrWhiteSpace(resultJson))
                    {
                        throw new Exception("Aucun résultat retourné par Python");
                    }

                    // Désérialiser le résultat
                    var result = JsonConvert.DeserializeObject<CalculationResult>(resultJson);
                    return result ?? throw new Exception("Résultat de calcul invalide");
                }
                catch (Exception ex)
                {
                    throw new Exception($"Erreur d'exécution Python: {ex.Message}", ex);
                }
            });
        }

        /// <summary>
        /// Vérifie si Python et le script engine.py sont disponibles
        /// </summary>
        public bool ValidateEnvironment(out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                // Vérifier si Python est installé
                var pythonCheck = new ProcessStartInfo
                {
                    FileName = "python",
                    Arguments = "--version",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = Process.Start(pythonCheck);
                if (process == null)
                {
                    errorMessage = "Python n'est pas installé ou n'est pas dans le PATH";
                    return false;
                }

                process.WaitForExit();
                if (process.ExitCode != 0)
                {
                    errorMessage = "Python n'est pas accessible";
                    return false;
                }

                // Vérifier si le script engine.py existe
                if (!File.Exists(_pythonScriptPath))
                {
                    errorMessage = $"Script Python non trouvé: {_pythonScriptPath}";
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                errorMessage = $"Erreur de validation: {ex.Message}";
                return false;
            }
        }

        /// <summary>
        /// Teste la connexion avec le backend Python
        /// </summary>
        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                var testData = new Dictionary<string, object>
                {
                    ["session_name"] = "Test Connection",
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

                var result = await ExecuteCalculationAsync(testData);
                return result != null;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Valide que tous les fichiers Python requis sont présents
        /// </summary>
        public bool ValidatePythonFiles(out List<string> missingFiles)
        {
            missingFiles = new List<string>();
            
            var requiredFiles = new[]
            {
                "engine_api.py",
                "engine.py", 
                "engine_classes.py",
                "input_handler.py",
                "sql.py",
                "config.py"
            };

            foreach (var file in requiredFiles)
            {
                var filePath = Path.Combine(_projectRoot, file);
                if (!File.Exists(filePath))
                {
                    missingFiles.Add(file);
                }
            }

            return missingFiles.Count == 0;
        }

        /// <summary>
        /// Obtient des informations de diagnostic sur l'environnement
        /// </summary>
        public Dictionary<string, object> GetDiagnosticInfo()
        {
            var info = new Dictionary<string, object>();
            
            try
            {
                // Vérifier Python
                var pythonProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "python",
                        Arguments = "--version",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                pythonProcess.Start();
                var pythonVersion = pythonProcess.StandardOutput.ReadToEnd().Trim();
                pythonProcess.WaitForExit();
                
                info["python_available"] = pythonProcess.ExitCode == 0;
                info["python_version"] = pythonVersion;
            }
            catch (Exception ex)
            {
                info["python_available"] = false;
                info["python_error"] = ex.Message;
            }

            // Vérifier les fichiers
            info["project_root"] = _projectRoot;
            info["files_valid"] = ValidatePythonFiles(out var missingFiles);
            info["missing_files"] = missingFiles;

            return info;
        }
    }

    /// <summary>
    /// Structure pour les résultats de calcul
    /// </summary>
    public class CalculationResult
    {
        [JsonProperty("session_id")]
        public int SessionId { get; set; }

        [JsonProperty("session_name")]
        public string SessionName { get; set; } = string.Empty;

        [JsonProperty("staff_costs")]
        public double StaffCosts { get; set; }

        [JsonProperty("equipment_costs")]
        public double EquipmentCosts { get; set; }

        [JsonProperty("location_costs")]
        public double LocationCosts { get; set; }

        [JsonProperty("operational_costs")]
        public double OperationalCosts { get; set; }

        [JsonProperty("total_cost")]
        public double TotalCost { get; set; }

        [JsonProperty("cost_breakdowns")]
        public List<CostBreakdown> CostBreakdowns { get; set; } = new List<CostBreakdown>();
    }

    /// <summary>
    /// Structure pour le détail des coûts
    /// </summary>
    public class CostBreakdown
    {
        [JsonProperty("category")]
        public string Category { get; set; } = string.Empty;

        [JsonProperty("subcategory")]
        public string Subcategory { get; set; } = string.Empty;

        [JsonProperty("amount")]
        public double Amount { get; set; }

        [JsonProperty("formula")]
        public string Formula { get; set; } = string.Empty;

        [JsonProperty("details")]
        public Dictionary<string, object> Details { get; set; } = new Dictionary<string, object>();
    }
}
