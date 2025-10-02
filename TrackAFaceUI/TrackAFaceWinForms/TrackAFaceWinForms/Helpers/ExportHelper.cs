using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using TrackAFaceWinForms.Models;

namespace TrackAFaceWinForms.Helpers
{
    /// <summary>
    /// Helper pour exporter les résultats vers différents formats
    /// </summary>
    public static class ExportHelper
    {
        /// <summary>
        /// Exporte les résultats vers un fichier CSV
        /// </summary>
        public static void ExportToCsv(CalculationResultModel results, string filePath)
        {
            try
            {
                var csv = new StringBuilder();

                // En-tête du rapport
                csv.AppendLine("===========================================");
                csv.AppendLine("Track-A-FACE - Rapport de Calcul des Coûts");
                csv.AppendLine("===========================================");
                csv.AppendLine();
                csv.AppendLine($"Date du rapport,{DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                csv.AppendLine($"Nom de la session,{EscapeCsvValue(results.SessionName)}");
                csv.AppendLine($"Date de calcul,{results.CalculationTimestamp}");
                csv.AppendLine();

                // Résumé des coûts
                csv.AppendLine("===========================================");
                csv.AppendLine("RÉSUMÉ DES COÛTS");
                csv.AppendLine("===========================================");
                csv.AppendLine("Catégorie,Montant (CAD$)");
                csv.AppendLine($"Personnel,{results.StaffCosts:N2}");
                csv.AppendLine($"Équipement,{results.EquipmentCosts:N2}");
                csv.AppendLine($"Immobilier,{results.LocationCosts:N2}");
                csv.AppendLine($"Opérationnel,{results.OperationalCosts:N2}");
                csv.AppendLine($"TOTAL,{results.TotalCost:N2}");
                csv.AppendLine();

                // Détails par catégorie
                csv.AppendLine("===========================================");
                csv.AppendLine("DÉTAILS DES COÛTS");
                csv.AppendLine("===========================================");
                csv.AppendLine("Catégorie,Sous-catégorie,Montant (CAD$),Formule,Détails");

                if (results.CostBreakdowns != null && results.CostBreakdowns.Count > 0)
                {
                    foreach (var breakdown in results.CostBreakdowns)
                    {
                        csv.AppendLine($"{EscapeCsvValue(breakdown.Category)}," +
                                      $"{EscapeCsvValue(breakdown.Subcategory)}," +
                                      $"{breakdown.Amount:N2}," +
                                      $"{EscapeCsvValue(breakdown.Formula)}," +
                                      $"{EscapeCsvValue(breakdown.Details)}");
                    }
                }
                else
                {
                    csv.AppendLine("Aucun détail disponible,,,");
                }

                csv.AppendLine();
                csv.AppendLine("===========================================");
                csv.AppendLine("Généré par Track-A-FACE");
                csv.AppendLine("===========================================");

                // Écrire le fichier
                File.WriteAllText(filePath, csv.ToString(), Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de l'export CSV: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Échappe les valeurs CSV (guillemets, virgules, retours à la ligne)
        /// </summary>
        private static string EscapeCsvValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            // Si la valeur contient des guillemets, virgules ou retours à la ligne
            if (value.Contains("\"") || value.Contains(",") || value.Contains("\n") || value.Contains("\r"))
            {
                // Doubler les guillemets et entourer de guillemets
                return "\"" + value.Replace("\"", "\"\"") + "\"";
            }

            return value;
        }

        /// <summary>
        /// Génère un nom de fichier sûr pour l'export
        /// </summary>
        public static string GenerateSafeFileName(string sessionName, string extension)
        {
            // Nettoyer le nom de session
            string safeName = sessionName;
            
            if (string.IsNullOrWhiteSpace(safeName))
            {
                safeName = "Export";
            }

            // Remplacer les caractères invalides
            char[] invalidChars = Path.GetInvalidFileNameChars();
            foreach (char c in invalidChars)
            {
                safeName = safeName.Replace(c, '_');
            }

            // Limiter la longueur
            if (safeName.Length > 50)
            {
                safeName = safeName.Substring(0, 50);
            }

            // Ajouter timestamp et extension
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            return $"TrackAFACE_{safeName}_{timestamp}.{extension}";
        }

        /// <summary>
        /// Retourne le répertoire par défaut pour les exports
        /// </summary>
        public static string GetDefaultExportDirectory()
        {
            string myDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string exportDir = Path.Combine(myDocuments, "Track-A-FACE", "Exports");

            if (!Directory.Exists(exportDir))
            {
                Directory.CreateDirectory(exportDir);
            }

            return exportDir;
        }

        /// <summary>
        /// Formate un montant en CAD$ pour l'affichage
        /// </summary>
        public static string FormatCurrency(double amount)
        {
            return $"{amount:N2} CAD$";
        }

        /// <summary>
        /// Formate une date pour l'export
        /// </summary>
        public static string FormatDateForExport(DateTime date)
        {
            return date.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
