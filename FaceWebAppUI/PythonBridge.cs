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
            _projectRoot = projectRoot ?? throw new ArgumentNullException(nameof(projectRoot));
            _pythonScriptPath = Path.Combine(projectRoot, "engine_api.py");
            
            // Valider le chemin du projet
            if (!Directory.Exists(_projectRoot))
            {
                throw new DirectoryNotFoundException($"Répertoire projet non trouvé: {_projectRoot}");
            }
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
                        CreateNoWindow = true,
                        StandardOutputEncoding = System.Text.Encoding.UTF8,
                        StandardErrorEncoding = System.Text.Encoding.UTF8
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
                        var errorDetails = ParsePythonError(error, output);
                        throw new PythonExecutionException($"Erreur Python (Code {process.ExitCode})", process.ExitCode, errorDetails, error);
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

                    // Vérifier si c'est une erreur JSON
                    if (resultJson.TrimStart().StartsWith("{") && resultJson.Contains("\"error\""))
                    {
                        var errorResult = JsonConvert.DeserializeObject<PythonErrorResponse>(resultJson);
                        throw new PythonCalculationException(errorResult.Message, errorResult.Error, errorResult.Details);
                    }
                    
                    // Désérialiser le résultat
                    var result = JsonConvert.DeserializeObject<CalculationResult>(resultJson);
                    if (result == null)
                    {
                        throw new Exception("Résultat de calcul invalide - désérialisation échouée");
                    }
                    
                    // Valider la cohérence des résultats
                    ValidateCalculationResult(result);
                    
                    return result;
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
        
        /// <summary>
        /// Parse les erreurs Python pour extraire des informations utiles
        /// </summary>
        private PythonErrorDetails ParsePythonError(string error, string output)
        {
            var details = new PythonErrorDetails
            {
                RawError = error,
                RawOutput = output,
                ErrorType = "UnknownError",
                UserFriendlyMessage = "Une erreur s'est produite lors du calcul"
            };
            
            // Parser les erreurs communes
            if (error.Contains("ModuleNotFoundError"))
            {
                details.ErrorType = "ModuleNotFound";
                details.UserFriendlyMessage = "Module Python manquant. Vérifiez l'installation des dépendances.";
            }
            else if (error.Contains("ValidationError"))
            {
                details.ErrorType = "ValidationError";
                details.UserFriendlyMessage = "Données d'entrée invalides. Vérifiez vos paramètres.";
            }
            else if (error.Contains("FileNotFoundError"))
            {
                details.ErrorType = "FileNotFound";
                details.UserFriendlyMessage = "Fichier requis manquant. Vérifiez l'installation.";
            }
            else if (error.Contains("sqlite3.OperationalError"))
            {
                details.ErrorType = "DatabaseError";
                details.UserFriendlyMessage = "Erreur de base de données. La base peut être corrompue.";
            }
            
            return details;
        }
        
        /// <summary>
        /// Valide la cohérence d'un résultat de calcul
        /// </summary>
        private void ValidateCalculationResult(CalculationResult result)
        {
            if (result.TotalCost < 0)
            {
                throw new InvalidOperationException("Le coût total ne peut pas être négatif");
            }
            
            var calculatedTotal = result.StaffCosts + result.EquipmentCosts + 
                                result.LocationCosts + result.OperationalCosts;
            
            // Tolérance de 1% pour les erreurs d'arrondi
            var tolerance = Math.Max(1.0, calculatedTotal * 0.01);
            if (Math.Abs(result.TotalCost - calculatedTotal) > tolerance)
            {
                throw new InvalidOperationException(
                    $"Incohérence dans les totaux: {result.TotalCost:F2} vs {calculatedTotal:F2}");
            }
        }
        
        /// <summary>
        /// Exécute un calcul avec retry automatique en cas d'échec temporaire
        /// </summary>
        public async Task<CalculationResult> ExecuteCalculationWithRetryAsync(Dictionary<string, object> inputData, int maxRetries = 3)
        {
            Exception lastException = null;
            
            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    return await ExecuteCalculationAsync(inputData);
                }
                catch (PythonExecutionException ex) when (ex.ExitCode == -1 && attempt < maxRetries)
                {
                    // Retry pour les erreurs temporaires
                    lastException = ex;
                    await Task.Delay(1000 * attempt); // Délai progressif
                }
                catch (Exception ex)
                {
                    // Ne pas retry pour les autres erreurs
                    throw;
                }
            }
            
            throw lastException ?? new Exception("Échec après plusieurs tentatives");
        }
    }
    
    /// <summary>
    /// Exception spécialisée pour les erreurs d'exécution Python
    /// </summary>
    public class PythonExecutionException : Exception
    {
        public int ExitCode { get; }
        public PythonErrorDetails ErrorDetails { get; }
        public string RawError { get; }
        
        public PythonExecutionException(string message, int exitCode, PythonErrorDetails errorDetails, string rawError) 
            : base(message)
        {
            ExitCode = exitCode;
            ErrorDetails = errorDetails;
            RawError = rawError;
        }
    }
    
    /// <summary>
    /// Exception pour les erreurs de calcul Python
    /// </summary>
    public class PythonCalculationException : Exception
    {
        public string ErrorType { get; }
        public string Details { get; }
        
        public PythonCalculationException(string message, string errorType, string details) : base(message)
        {
            ErrorType = errorType;
            Details = details;
        }
    }
    
    /// <summary>
    /// Détails d'une erreur Python
    /// </summary>
    public class PythonErrorDetails
    {
        public string ErrorType { get; set; } = string.Empty;
        public string UserFriendlyMessage { get; set; } = string.Empty;
        public string RawError { get; set; } = string.Empty;
        public string RawOutput { get; set; } = string.Empty;
    }
    
    /// <summary>
    /// Réponse d'erreur du Python
    /// </summary>
    public class PythonErrorResponse
    {
        [JsonProperty("error")]
        public string Error { get; set; } = string.Empty;
        
        [JsonProperty("message")]
        public string Message { get; set; } = string.Empty;
        
        [JsonProperty("details")]
        public string Details { get; set; } = string.Empty;
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
