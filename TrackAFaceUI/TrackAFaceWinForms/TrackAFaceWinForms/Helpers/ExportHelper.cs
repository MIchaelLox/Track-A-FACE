using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using TrackAFaceWinForms.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;

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

        /// <summary>
        /// Exporte les résultats vers un fichier PDF
        /// </summary>
        public static void ExportToPdf(CalculationResultModel results, string filePath)
        {
            try
            {
                // Créer le document PDF
                Document document = new Document(PageSize.A4, 50, 50, 50, 50);
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
                document.Open();

                // Polices
                Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20, BaseColor.DARK_GRAY);
                Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, BaseColor.WHITE);
                Font subHeaderFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.BLACK);
                Font normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.BLACK);
                Font smallFont = FontFactory.GetFont(FontFactory.HELVETICA, 8, BaseColor.DARK_GRAY);

                // Titre principal
                Paragraph title = new Paragraph("Track-A-FACE", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                title.SpacingAfter = 5f;
                document.Add(title);

                Paragraph subtitle = new Paragraph("Rapport de Calcul des Coûts de Restaurant", normalFont);
                subtitle.Alignment = Element.ALIGN_CENTER;
                subtitle.SpacingAfter = 20f;
                document.Add(subtitle);

                // Ligne de séparation
                document.Add(new Chunk(new LineSeparator(1f, 100f, BaseColor.GRAY, Element.ALIGN_CENTER, -2)));
                document.Add(new Paragraph(" ")); // Espacement

                // Informations de session
                PdfPTable infoTable = new PdfPTable(2);
                infoTable.WidthPercentage = 100;
                infoTable.SpacingAfter = 20f;

                AddInfoRow(infoTable, "Date du rapport:", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), normalFont);
                AddInfoRow(infoTable, "Nom de la session:", results.SessionName, normalFont);
                AddInfoRow(infoTable, "Date de calcul:", results.CalculationTimestamp, normalFont);

                document.Add(infoTable);

                // Coût total (encadré)
                PdfPTable totalTable = new PdfPTable(1);
                totalTable.WidthPercentage = 100;
                totalTable.SpacingAfter = 20f;

                PdfPCell totalCell = new PdfPCell(new Phrase($"COÛT TOTAL: {results.TotalCost:N2} CAD$", 
                    FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16, BaseColor.WHITE)));
                totalCell.BackgroundColor = new BaseColor(244, 67, 54); // Rouge
                totalCell.HorizontalAlignment = Element.ALIGN_CENTER;
                totalCell.Padding = 15f;
                totalTable.AddCell(totalCell);

                document.Add(totalTable);

                // Résumé par catégorie
                Paragraph summaryTitle = new Paragraph("RÉSUMÉ DES COÛTS", subHeaderFont);
                summaryTitle.SpacingAfter = 10f;
                document.Add(summaryTitle);

                PdfPTable summaryTable = new PdfPTable(2);
                summaryTable.WidthPercentage = 100;
                summaryTable.SetWidths(new float[] { 60f, 40f });
                summaryTable.SpacingAfter = 20f;

                // En-tête
                AddHeaderCell(summaryTable, "Catégorie", headerFont, new BaseColor(76, 175, 80));
                AddHeaderCell(summaryTable, "Montant (CAD$)", headerFont, new BaseColor(76, 175, 80));

                // Données
                AddSummaryRow(summaryTable, "Personnel", results.StaffCosts, normalFont, new BaseColor(220, 237, 200));
                AddSummaryRow(summaryTable, "Équipement", results.EquipmentCosts, normalFont, new BaseColor(187, 222, 251));
                AddSummaryRow(summaryTable, "Immobilier", results.LocationCosts, normalFont, new BaseColor(255, 224, 178));
                AddSummaryRow(summaryTable, "Opérationnel", results.OperationalCosts, normalFont, new BaseColor(225, 190, 231));

                document.Add(summaryTable);

                // Détails par catégorie
                if (results.CostBreakdowns != null && results.CostBreakdowns.Count > 0)
                {
                    Paragraph detailsTitle = new Paragraph("DÉTAILS DES COÛTS", subHeaderFont);
                    detailsTitle.SpacingAfter = 10f;
                    document.Add(detailsTitle);

                    PdfPTable detailsTable = new PdfPTable(4);
                    detailsTable.WidthPercentage = 100;
                    detailsTable.SetWidths(new float[] { 20f, 30f, 20f, 30f });

                    // En-têtes
                    AddHeaderCell(detailsTable, "Catégorie", headerFont, BaseColor.DARK_GRAY);
                    AddHeaderCell(detailsTable, "Sous-catégorie", headerFont, BaseColor.DARK_GRAY);
                    AddHeaderCell(detailsTable, "Montant (CAD$)", headerFont, BaseColor.DARK_GRAY);
                    AddHeaderCell(detailsTable, "Formule", headerFont, BaseColor.DARK_GRAY);

                    // Données
                    foreach (var breakdown in results.CostBreakdowns)
                    {
                        AddDetailCell(detailsTable, breakdown.Category, normalFont);
                        AddDetailCell(detailsTable, breakdown.Subcategory, normalFont);
                        AddDetailCell(detailsTable, $"{breakdown.Amount:N2}", normalFont);
                        AddDetailCell(detailsTable, breakdown.Formula, smallFont);
                    }

                    document.Add(detailsTable);
                }

                // Pied de page
                document.Add(new Paragraph(" ")); // Espacement
                document.Add(new Chunk(new LineSeparator(1f, 100f, BaseColor.GRAY, Element.ALIGN_CENTER, -2)));
                
                Paragraph footer = new Paragraph($"Généré par Track-A-FACE le {DateTime.Now:yyyy-MM-dd à HH:mm:ss}", smallFont);
                footer.Alignment = Element.ALIGN_CENTER;
                footer.SpacingBefore = 10f;
                document.Add(footer);

                // Fermer le document
                document.Close();
                writer.Close();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de l'export PDF: {ex.Message}", ex);
            }
        }

        // Méthodes privées pour la génération PDF

        private static void AddInfoRow(PdfPTable table, string label, string value, Font font)
        {
            PdfPCell labelCell = new PdfPCell(new Phrase(label, font));
            labelCell.Border = Rectangle.NO_BORDER;
            labelCell.HorizontalAlignment = Element.ALIGN_LEFT;
            labelCell.Padding = 5f;
            table.AddCell(labelCell);

            PdfPCell valueCell = new PdfPCell(new Phrase(value, font));
            valueCell.Border = Rectangle.NO_BORDER;
            valueCell.HorizontalAlignment = Element.ALIGN_LEFT;
            valueCell.Padding = 5f;
            table.AddCell(valueCell);
        }

        private static void AddHeaderCell(PdfPTable table, string text, Font font, BaseColor backgroundColor)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, font));
            cell.BackgroundColor = backgroundColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Padding = 8f;
            table.AddCell(cell);
        }

        private static void AddSummaryRow(PdfPTable table, string category, double amount, Font font, BaseColor backgroundColor)
        {
            PdfPCell categoryCell = new PdfPCell(new Phrase(category, font));
            categoryCell.BackgroundColor = backgroundColor;
            categoryCell.Padding = 8f;
            table.AddCell(categoryCell);

            PdfPCell amountCell = new PdfPCell(new Phrase($"{amount:N2}", font));
            amountCell.BackgroundColor = backgroundColor;
            amountCell.HorizontalAlignment = Element.ALIGN_RIGHT;
            amountCell.Padding = 8f;
            table.AddCell(amountCell);
        }

        private static void AddDetailCell(PdfPTable table, string text, Font font)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text ?? "", font));
            cell.Padding = 5f;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);
        }
    }
}
