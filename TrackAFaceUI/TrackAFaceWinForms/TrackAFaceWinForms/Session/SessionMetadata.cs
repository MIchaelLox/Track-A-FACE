using System;
using Newtonsoft.Json;

namespace TrackAFaceWinForms.Session
{
    /// <summary>
    /// Métadonnées d'une session sauvegardée
    /// </summary>
    public class SessionMetadata
    {
        /// <summary>
        /// Nom de la session
        /// </summary>
        [JsonProperty("session_name")]
        public string SessionName { get; set; }

        /// <summary>
        /// Date de création
        /// </summary>
        [JsonProperty("created_date")]
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Date de dernière modification
        /// </summary>
        [JsonProperty("last_modified")]
        public DateTime LastModified { get; set; }

        /// <summary>
        /// Version du format de session
        /// </summary>
        [JsonProperty("version")]
        public string Version { get; set; }

        /// <summary>
        /// Description optionnelle
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Chemin du fichier (non sauvegardé dans JSON)
        /// </summary>
        [JsonIgnore]
        public string FilePath { get; set; }

        /// <summary>
        /// Nom du fichier uniquement
        /// </summary>
        [JsonIgnore]
        public string FileName => System.IO.Path.GetFileName(FilePath);

        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        public SessionMetadata()
        {
            CreatedDate = DateTime.Now;
            LastModified = DateTime.Now;
            Version = "1.0";
            Description = string.Empty;
        }

        /// <summary>
        /// Représentation textuelle
        /// </summary>
        public override string ToString()
        {
            return $"{SessionName} - {LastModified:yyyy-MM-dd HH:mm}";
        }
    }
}
