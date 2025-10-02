using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using TrackAFaceWinForms.Models;
using TrackAFaceWinForms.Helpers;

namespace TrackAFaceWinForms.Session
{
    /// <summary>
    /// Gestionnaire de sessions pour sauvegarder/charger les analyses
    /// </summary>
    public class SessionManager
    {
        private readonly string _sessionsDirectory;

        public SessionManager()
        {
            _sessionsDirectory = ConfigurationHelper.SessionsDirectory;
            EnsureDirectoryExists();
        }

        /// <summary>
        /// Crée le répertoire des sessions s'il n'existe pas
        /// </summary>
        private void EnsureDirectoryExists()
        {
            if (!Directory.Exists(_sessionsDirectory))
            {
                Directory.CreateDirectory(_sessionsDirectory);
            }
        }

        /// <summary>
        /// Sauvegarde une session dans un fichier JSON
        /// </summary>
        public string SaveSession(RestaurantInputModel inputs, string fileName = null)
        {
            try
            {
                // Générer un nom de fichier si non fourni
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    string safeName = SanitizeFileName(inputs.SessionName);
                    string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    fileName = $"{safeName}_{timestamp}.json";
                }

                // Ajouter .json si absent
                if (!fileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                {
                    fileName += ".json";
                }

                string fullPath = Path.Combine(_sessionsDirectory, fileName);

                // Créer les métadonnées
                var sessionData = new SessionData
                {
                    Metadata = new SessionMetadata
                    {
                        SessionName = inputs.SessionName,
                        CreatedDate = DateTime.Now,
                        LastModified = DateTime.Now,
                        Version = "1.0",
                        Description = $"Session pour restaurant {inputs.Theme}"
                    },
                    Inputs = inputs
                };

                // Sauvegarder en JSON
                string json = JsonConvert.SerializeObject(sessionData, Formatting.Indented);
                File.WriteAllText(fullPath, json);

                return fullPath;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la sauvegarde de la session: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Charge une session depuis un fichier JSON
        /// </summary>
        public RestaurantInputModel LoadSession(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"Fichier de session introuvable: {filePath}");
                }

                string json = File.ReadAllText(filePath);
                var sessionData = JsonConvert.DeserializeObject<SessionData>(json);

                // Mettre à jour la date de dernière modification
                sessionData.Metadata.LastModified = DateTime.Now;

                return sessionData.Inputs;
            }
            catch (JsonException ex)
            {
                throw new Exception($"Erreur de format JSON: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors du chargement de la session: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Liste toutes les sessions disponibles
        /// </summary>
        public List<SessionMetadata> ListSessions()
        {
            var sessions = new List<SessionMetadata>();

            if (!Directory.Exists(_sessionsDirectory))
            {
                return sessions;
            }

            var files = Directory.GetFiles(_sessionsDirectory, "*.json");

            foreach (var file in files)
            {
                try
                {
                    string json = File.ReadAllText(file);
                    var sessionData = JsonConvert.DeserializeObject<SessionData>(json);
                    
                    if (sessionData?.Metadata != null)
                    {
                        sessionData.Metadata.FilePath = file;
                        sessions.Add(sessionData.Metadata);
                    }
                }
                catch
                {
                    // Ignorer les fichiers corrompus
                    continue;
                }
            }

            return sessions.OrderByDescending(s => s.LastModified).ToList();
        }

        /// <summary>
        /// Supprime une session
        /// </summary>
        public void DeleteSession(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        /// <summary>
        /// Nettoie le nom de fichier des caractères invalides
        /// </summary>
        private string SanitizeFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return "Session";
            }

            // Remplacer les caractères invalides
            char[] invalidChars = Path.GetInvalidFileNameChars();
            string safe = new string(fileName.Select(c => invalidChars.Contains(c) ? '_' : c).ToArray());

            // Limiter la longueur
            if (safe.Length > 50)
            {
                safe = safe.Substring(0, 50);
            }

            return safe;
        }

        /// <summary>
        /// Exporte une session vers un dossier spécifique
        /// </summary>
        public string ExportSession(RestaurantInputModel inputs, string exportPath)
        {
            string fileName = Path.GetFileName(exportPath);
            string directory = Path.GetDirectoryName(exportPath);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var sessionData = new SessionData
            {
                Metadata = new SessionMetadata
                {
                    SessionName = inputs.SessionName,
                    CreatedDate = DateTime.Now,
                    LastModified = DateTime.Now,
                    Version = "1.0"
                },
                Inputs = inputs
            };

            string json = JsonConvert.SerializeObject(sessionData, Formatting.Indented);
            File.WriteAllText(exportPath, json);

            return exportPath;
        }

        /// <summary>
        /// Retourne le nombre total de sessions
        /// </summary>
        public int GetSessionCount()
        {
            if (!Directory.Exists(_sessionsDirectory))
            {
                return 0;
            }

            return Directory.GetFiles(_sessionsDirectory, "*.json").Length;
        }
    }

    /// <summary>
    /// Données complètes d'une session
    /// </summary>
    public class SessionData
    {
        [JsonProperty("metadata")]
        public SessionMetadata Metadata { get; set; }

        [JsonProperty("inputs")]
        public RestaurantInputModel Inputs { get; set; }
    }
}
