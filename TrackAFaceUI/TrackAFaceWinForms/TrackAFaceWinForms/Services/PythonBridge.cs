using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackAFaceWinForms.Helpers;
using TrackAFaceWinForms.Models;

namespace TrackAFaceWinForms.Services
{
    public class PythonBridge
    {
        private readonly string _pythonPath;
        private readonly string _engineApiPath;
        private const int TIMEOUT_SECONDS = 30;

        public PythonBridge()
        {
            _pythonPath = ConfigurationHelper.PythonPath;
            _engineApiPath = ConfigurationHelper.EngineApiPath;
        }

        /// <summary>
        /// Exécute le calcul Python de manière asynchrone
        /// </summary>
        public async Task<CalculationResultModel> CalculateAsync(RestaurantInputModel inputs)
        {
            try
            {
                // 1. Créer un fichier JSON temporaire avec les inputs
                string tempInputFile = Path.GetTempFileName();
                string jsonInput = inputs.ToJson();
                File.WriteAllText(tempInputFile, jsonInput);

                // 2. Préparer le processus Python
                var processInfo = new ProcessStartInfo
                {
                    FileName = _pythonPath,
                    Arguments = $"\"{_engineApiPath}\" --input \"{tempInputFile}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    WorkingDirectory = Path.GetDirectoryName(_engineApiPath)
                };

                // 3. Exécuter Python
                using (var process = new Process { StartInfo = processInfo })
                {
                    process.Start();

                    // Lire la sortie de manière asynchrone
                    string output = await process.StandardOutput.ReadToEndAsync();
                    string error = await process.StandardError.ReadToEndAsync();

                    // Attendre la fin avec timeout
                    bool finished = await Task.Run(() => process.WaitForExit(TIMEOUT_SECONDS * 1000));

                    // 4. Nettoyer le fichier temporaire
                    try { File.Delete(tempInputFile); } catch { }

                    // 5. Vérifier si timeout
                    if (!finished)
                    {
                        try { process.Kill(); } catch { }
                        return new CalculationResultModel
                        {
                            Error = "timeout",
                            ErrorMessage = $"Le calcul a dépassé le délai de {TIMEOUT_SECONDS} secondes.",
                            ErrorDetails = "Le processus Python a été interrompu."
                        };
                    }

                    // 6. Vérifier le code de sortie
                    if (process.ExitCode != 0)
                    {
                        return new CalculationResultModel
                        {
                            Error = "python_error",
                            ErrorMessage = "Erreur lors de l'exécution du moteur Python.",
                            ErrorDetails = string.IsNullOrWhiteSpace(error) ? output : error
                        };
                    }

                    // 7. Parser la sortie JSON
                    if (string.IsNullOrWhiteSpace(output))
                    {
                        return new CalculationResultModel
                        {
                            Error = "empty_output",
                            ErrorMessage = "Le moteur Python n'a retourné aucun résultat.",
                            ErrorDetails = error
                        };
                    }

                    // 8. Convertir JSON → CalculationResultModel
                    return CalculationResultModel.FromJson(output);
                }
            }
            catch (FileNotFoundException ex)
            {
                return new CalculationResultModel
                {
                    Error = "file_not_found",
                    ErrorMessage = "Python ou engine_api.py introuvable.",
                    ErrorDetails = $"Fichier manquant: {ex.FileName}\n\n" +
                                 $"Python: {_pythonPath}\n" +
                                 $"Engine: {_engineApiPath}\n\n" +
                                 "Vérifiez App.config et l'installation Python."
                };
            }
            catch (JsonException ex)
            {
                return new CalculationResultModel
                {
                    Error = "json_parse_error",
                    ErrorMessage = "Erreur de parsing JSON.",
                    ErrorDetails = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new CalculationResultModel
                {
                    Error = "exception",
                    ErrorMessage = "Erreur inattendue lors du calcul.",
                    ErrorDetails = $"{ex.GetType().Name}: {ex.Message}\n\n{ex.StackTrace}"
                };
            }
        }

        /// <summary>
        /// Vérifie si Python est disponible
        /// </summary>
        public bool CheckPythonAvailable()
        {
            return ConfigurationHelper.IsPythonAvailable();
        }

        /// <summary>
        /// Vérifie si engine_api.py est disponible
        /// </summary>
        public bool CheckEngineApiAvailable()
        {
            return ConfigurationHelper.IsEngineApiAvailable();
        }

        /// <summary>
        /// Retourne un diagnostic de la configuration
        /// </summary>
        public string GetDiagnostic()
        {
            return ConfigurationHelper.GetConfigurationSummary();
        }
    }
}
