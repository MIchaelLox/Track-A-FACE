using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Newtonsoft.Json;

namespace FaceWebAppUI
{
    /// <summary>
    /// Gestionnaire d'affichage et d'export des résultats de calcul Track-A-FACE
    /// Implémente l'affichage en tableaux avec mise en forme et couleurs
    /// </summary>
    public class OutputPipe
    {
        private readonly MainForm _parentForm;
        private DataGridView _resultsGrid;
        private Panel _summaryPanel;
        private Dictionary<string, Color> _categoryColors;

        public OutputPipe(MainForm parentForm)
        {
            _parentForm = parentForm ?? throw new ArgumentNullException(nameof(parentForm));
            InitializeCategoryColors();
        }

        /// <summary>
        /// Initialise les couleurs par catégorie pour un affichage cohérent
        /// </summary>
        private void InitializeCategoryColors()
        {
            _categoryColors = new Dictionary<string, Color>
            {
                ["staff"] = Color.FromArgb(0, 123, 255),      // Bleu
                ["equipment"] = Color.FromArgb(40, 167, 69),   // Vert
                ["location"] = Color.FromArgb(255, 193, 7),    // Jaune/Orange
                ["operations"] = Color.FromArgb(108, 117, 125), // Gris
                ["total"] = Color.FromArgb(220, 53, 69),       // Rouge
                ["training"] = Color.FromArgb(102, 16, 242),   // Violet
                ["salary"] = Color.FromArgb(32, 201, 151),     // Turquoise
                ["maintenance"] = Color.FromArgb(255, 99, 132) // Rose
            };
        }

        /// <summary>
        /// Configure et stylise le DataGridView pour l'affichage des résultats
        /// </summary>
        public void ConfigureResultsDataGrid(DataGridView dataGridView)
        {
            _resultsGrid = dataGridView;
            
            // Configuration générale
            _resultsGrid.AutoGenerateColumns = false;
            _resultsGrid.AllowUserToAddRows = false;
            _resultsGrid.AllowUserToDeleteRows = false;
            _resultsGrid.ReadOnly = true;
            _resultsGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _resultsGrid.MultiSelect = false;
            _resultsGrid.RowHeadersVisible = false;
            _resultsGrid.EnableHeadersVisualStyles = false;
            _resultsGrid.GridColor = Color.FromArgb(230, 230, 230);
            _resultsGrid.BackgroundColor = Color.White;
            _resultsGrid.BorderStyle = BorderStyle.None;

            // Style des en-têtes
            _resultsGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 58, 64);
            _resultsGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            _resultsGrid.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F, FontStyle.Bold);
            _resultsGrid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            _resultsGrid.ColumnHeadersHeight = 35;

            // Style des cellules par défaut
            _resultsGrid.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F);
            _resultsGrid.DefaultCellStyle.BackColor = Color.White;
            _resultsGrid.DefaultCellStyle.ForeColor = Color.FromArgb(33, 37, 41);
            _resultsGrid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 123, 255);
            _resultsGrid.DefaultCellStyle.SelectionForeColor = Color.White;
            _resultsGrid.RowTemplate.Height = 28;

            // Style des lignes alternées
            _resultsGrid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);

            // Créer les colonnes avec style
            CreateStyledColumns();

            // Événements pour le style dynamique
            _resultsGrid.CellFormatting += OnCellFormatting;
            _resultsGrid.RowPrePaint += OnRowPrePaint;
        }

        /// <summary>
        /// Crée les colonnes stylisées pour le DataGridView
        /// </summary>
        private void CreateStyledColumns()
        {
            _resultsGrid.Columns.Clear();

            // Colonne Catégorie
            var categoryColumn = new DataGridViewTextBoxColumn
            {
                Name = "Category",
                HeaderText = "Catégorie",
                DataPropertyName = "Category",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new System.Drawing.Font("Segoe UI", 9F, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleLeft
                }
            };
            _resultsGrid.Columns.Add(categoryColumn);

            // Colonne Sous-catégorie
            var subcategoryColumn = new DataGridViewTextBoxColumn
            {
                Name = "Subcategory",
                HeaderText = "Sous-catégorie",
                DataPropertyName = "Subcategory",
                Width = 180,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleLeft,
                    Padding = new Padding(15, 0, 0, 0)
                }
            };
            _resultsGrid.Columns.Add(subcategoryColumn);

            // Colonne Montant avec style monétaire
            var amountColumn = new DataGridViewTextBoxColumn
            {
                Name = "Amount",
                HeaderText = "Montant (CAD$)",
                DataPropertyName = "Amount",
                Width = 140,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "N2",
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Font = new System.Drawing.Font("Segoe UI", 9F, FontStyle.Bold)
                }
            };
            _resultsGrid.Columns.Add(amountColumn);

            // Colonne Pourcentage
            var percentageColumn = new DataGridViewTextBoxColumn
            {
                Name = "Percentage",
                HeaderText = "% du Total",
                DataPropertyName = "Percentage",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "P1",
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            };
            _resultsGrid.Columns.Add(percentageColumn);

            // Colonne Formule
            var formulaColumn = new DataGridViewTextBoxColumn
            {
                Name = "Formula",
                HeaderText = "Formule de calcul",
                DataPropertyName = "Formula",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new System.Drawing.Font("Consolas", 8F),
                    ForeColor = Color.FromArgb(108, 117, 125),
                    WrapMode = DataGridViewTriState.True
                }
            };
            _resultsGrid.Columns.Add(formulaColumn);
        }

        /// <summary>
        /// Affiche les résultats de calcul avec formatage avancé
        /// </summary>
        public void DisplayCalculationResults(dynamic calculationResult)
        {
            try
            {
                var displayData = PrepareDisplayData(calculationResult);
                _resultsGrid.DataSource = displayData;
                
                // Appliquer le formatage spécial après liaison des données
                ApplyAdvancedFormatting();
                
                // Mettre à jour le panneau de résumé
                UpdateSummaryPanel(calculationResult);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'affichage des résultats: {ex.Message}", 
                    "Erreur d'affichage", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Prépare les données pour l'affichage avec calcul des pourcentages
        /// </summary>
        private List<ResultDisplayItem> PrepareDisplayData(dynamic result)
        {
            var displayItems = new List<ResultDisplayItem>();
            double totalCost = (double)result.total_cost;

            // Traiter chaque breakdown
            foreach (var breakdown in result.cost_breakdowns)
            {
                var item = new ResultDisplayItem
                {
                    Category = GetCategoryDisplayName(breakdown.category.ToString()),
                    Subcategory = breakdown.subcategory.ToString(),
                    Amount = (double)breakdown.amount,
                    Percentage = totalCost > 0 ? (double)breakdown.amount / totalCost : 0,
                    Formula = breakdown.formula?.ToString() ?? "",
                    CategoryKey = breakdown.category.ToString()
                };
                displayItems.Add(item);
            }

            // Trier par catégorie puis par montant décroissant
            return displayItems
                .OrderBy(x => GetCategoryOrder(x.CategoryKey))
                .ThenByDescending(x => x.Amount)
                .ToList();
        }

        /// <summary>
        /// Applique le formatage avancé aux cellules
        /// </summary>
        private void ApplyAdvancedFormatting()
        {
            foreach (DataGridViewRow row in _resultsGrid.Rows)
            {
                if (row.DataBoundItem is ResultDisplayItem item)
                {
                    // Couleur de catégorie
                    if (_categoryColors.ContainsKey(item.CategoryKey))
                    {
                        var categoryColor = _categoryColors[item.CategoryKey];
                        row.Cells["Category"].Style.ForeColor = categoryColor;
                        
                        // Barre de couleur à gauche
                        row.Cells["Category"].Style.BackColor = Color.FromArgb(20, categoryColor);
                    }

                    // Mise en évidence des montants élevés
                    if (item.Percentage > 0.15) // Plus de 15% du total
                    {
                        row.Cells["Amount"].Style.BackColor = Color.FromArgb(255, 243, 205);
                        row.Cells["Amount"].Style.ForeColor = Color.FromArgb(133, 100, 4);
                    }

                    // Style spécial pour les totaux
                    if (item.Subcategory.ToLower().Contains("total"))
                    {
                        row.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F, FontStyle.Bold);
                        row.DefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
                    }
                }
            }
        }

        /// <summary>
        /// Met à jour le panneau de résumé avec graphiques en barres
        /// </summary>
        private void UpdateSummaryPanel(dynamic result)
        {
            // Cette méthode sera appelée par le formulaire principal
            // pour mettre à jour les labels de résumé
        }

        /// <summary>
        /// Gestion du formatage des cellules en temps réel
        /// </summary>
        private void OnCellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (_resultsGrid.Columns[e.ColumnIndex].Name == "Amount" && e.Value != null)
            {
                if (double.TryParse(e.Value.ToString(), out double amount))
                {
                    // Formatage monétaire avec couleur selon le montant
                    e.Value = $"{amount:N2} $";
                    e.FormattingApplied = true;

                    if (amount > 50000)
                    {
                        e.CellStyle.ForeColor = Color.FromArgb(220, 53, 69); // Rouge pour montants élevés
                    }
                    else if (amount > 20000)
                    {
                        e.CellStyle.ForeColor = Color.FromArgb(255, 193, 7); // Orange pour montants moyens
                    }
                    else
                    {
                        e.CellStyle.ForeColor = Color.FromArgb(40, 167, 69); // Vert pour montants faibles
                    }
                }
            }
        }

        /// <summary>
        /// Gestion du pré-rendu des lignes pour effets visuels
        /// </summary>
        private void OnRowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            // Ajouter une bordure subtile entre les catégories
            if (e.RowIndex > 0)
            {
                var currentRow = _resultsGrid.Rows[e.RowIndex];
                var previousRow = _resultsGrid.Rows[e.RowIndex - 1];

                if (currentRow.DataBoundItem is ResultDisplayItem currentItem &&
                    previousRow.DataBoundItem is ResultDisplayItem previousItem)
                {
                    if (currentItem.CategoryKey != previousItem.CategoryKey)
                    {
                        // Dessiner une ligne de séparation
                        using (var pen = new Pen(Color.FromArgb(200, 200, 200), 1))
                        {
                            var y = e.RowBounds.Top - 1;
                            e.Graphics.DrawLine(pen, e.RowBounds.Left, y, e.RowBounds.Right, y);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Exporte les résultats en CSV avec formatage
        /// </summary>
        public void ExportToCSV(string filePath)
        {
            try
            {
                using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
                {
                    // En-têtes
                    writer.WriteLine("Catégorie,Sous-catégorie,Montant (CAD$),Pourcentage,Formule");

                    // Données
                    foreach (DataGridViewRow row in _resultsGrid.Rows)
                    {
                        if (row.DataBoundItem is ResultDisplayItem item)
                        {
                            var csvLine = $"\"{item.Category}\",\"{item.Subcategory}\"," +
                                        $"{item.Amount:F2},{item.Percentage:P2},\"{item.Formula.Replace("\"", "\"\"")}\"";
                            writer.WriteLine(csvLine);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de l'export CSV: {ex.Message}");
            }
        }

        /// <summary>
        /// Exporte les résultats en PDF avec mise en forme professionnelle
        /// </summary>
        public void ExportToPDF(string filePath, string sessionName)
        {
            try
            {
                using (var document = new Document(PageSize.A4, 50, 50, 50, 50))
                {
                    PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
                    document.Open();

                    // En-tête du document
                    var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, BaseColor.DARK_GRAY);
                    var title = new Paragraph($"Rapport de Calcul - {sessionName}", titleFont);
                    title.Alignment = Element.ALIGN_CENTER;
                    title.SpacingAfter = 20;
                    document.Add(title);

                    // Date de génération
                    var dateFont = FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.GRAY);
                    var dateP = new Paragraph($"Généré le: {DateTime.Now:dd/MM/yyyy HH:mm}", dateFont);
                    dateP.Alignment = Element.ALIGN_RIGHT;
                    dateP.SpacingAfter = 30;
                    document.Add(dateP);

                    // Tableau des résultats
                    var table = new PdfPTable(4) { WidthPercentage = 100 };
                    table.SetWidths(new float[] { 20f, 35f, 20f, 25f });

                    // En-têtes du tableau
                    var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.WHITE);
                    var headerCells = new[] { "Catégorie", "Sous-catégorie", "Montant (CAD$)", "% du Total" };
                    
                    foreach (var header in headerCells)
                    {
                        var cell = new PdfPCell(new Phrase(header, headerFont));
                        cell.BackgroundColor = new BaseColor(52, 58, 64);
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.Padding = 8;
                        table.AddCell(cell);
                    }

                    // Données du tableau
                    var dataFont = FontFactory.GetFont(FontFactory.HELVETICA, 9);
                    foreach (DataGridViewRow row in _resultsGrid.Rows)
                    {
                        if (row.DataBoundItem is ResultDisplayItem item)
                        {
                            table.AddCell(new PdfPCell(new Phrase(item.Category, dataFont)) { Padding = 5 });
                            table.AddCell(new PdfPCell(new Phrase(item.Subcategory, dataFont)) { Padding = 5 });
                            table.AddCell(new PdfPCell(new Phrase($"{item.Amount:N2} $", dataFont)) 
                            { 
                                Padding = 5, 
                                HorizontalAlignment = Element.ALIGN_RIGHT 
                            });
                            table.AddCell(new PdfPCell(new Phrase($"{item.Percentage:P1}", dataFont)) 
                            { 
                                Padding = 5, 
                                HorizontalAlignment = Element.ALIGN_CENTER 
                            });
                        }
                    }

                    document.Add(table);
                    document.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de l'export PDF: {ex.Message}");
            }
        }

        /// <summary>
        /// Crée un graphique en barres pour visualiser les coûts par catégorie
        /// </summary>
        public Panel CreateCostChart(dynamic result)
        {
            var chartPanel = new Panel
            {
                Size = new Size(400, 200),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            chartPanel.Paint += (sender, e) =>
            {
                DrawCostChart(e.Graphics, chartPanel.ClientRectangle, result);
            };

            return chartPanel;
        }

        /// <summary>
        /// Dessine un graphique en barres simple
        /// </summary>
        private void DrawCostChart(Graphics g, Rectangle bounds, dynamic result)
        {
            var categories = new[]
            {
                new { Name = "Personnel", Value = (double)result.staff_costs, Color = _categoryColors["staff"] },
                new { Name = "Équipement", Value = (double)result.equipment_costs, Color = _categoryColors["equipment"] },
                new { Name = "Immobilier", Value = (double)result.location_costs, Color = _categoryColors["location"] },
                new { Name = "Opérationnel", Value = (double)result.operational_costs, Color = _categoryColors["operations"] }
            };

            var maxValue = categories.Max(c => c.Value);
            var barWidth = bounds.Width / categories.Length - 20;
            var maxBarHeight = bounds.Height - 60;

            for (int i = 0; i < categories.Length; i++)
            {
                var category = categories[i];
                var barHeight = maxValue > 0 ? (int)(category.Value / maxValue * maxBarHeight) : 0;
                var x = i * (barWidth + 20) + 10;
                var y = bounds.Height - barHeight - 30;

                // Dessiner la barre
                using (var brush = new SolidBrush(category.Color))
                {
                    g.FillRectangle(brush, x, y, barWidth, barHeight);
                }

                // Dessiner le label
                using (var font = new System.Drawing.Font("Segoe UI", 8))
                using (var brush = new SolidBrush(Color.Black))
                {
                    var labelSize = g.MeasureString(category.Name, font);
                    g.DrawString(category.Name, font, brush, 
                        x + (barWidth - labelSize.Width) / 2, bounds.Height - 25);
                }
            }
        }

        #region Méthodes utilitaires

        private string GetCategoryDisplayName(string categoryKey)
        {
            return categoryKey switch
            {
                "staff" => "👥 Personnel",
                "equipment" => "🔧 Équipement",
                "location" => "🏢 Immobilier",
                "operations" => "⚙️ Opérationnel",
                "training" => "📚 Formation",
                "salary" => "💰 Salaires",
                "maintenance" => "🔧 Maintenance",
                _ => categoryKey
            };
        }

        private int GetCategoryOrder(string categoryKey)
        {
            return categoryKey switch
            {
                "staff" => 1,
                "training" => 2,
                "salary" => 3,
                "equipment" => 4,
                "maintenance" => 5,
                "location" => 6,
                "operations" => 7,
                _ => 99
            };
        }

        #endregion
    }

    /// <summary>
    /// Classe pour représenter un élément d'affichage des résultats
    /// </summary>
    public class ResultDisplayItem
    {
        public string Category { get; set; }
        public string Subcategory { get; set; }
        public double Amount { get; set; }
        public double Percentage { get; set; }
        public string Formula { get; set; }
        public string CategoryKey { get; set; }
    }

    /// <summary>
    /// Extensions pour améliorer l'affichage des DataGridView
    /// </summary>
    public static class DataGridViewExtensions
    {
        public static void ApplyModernStyle(this DataGridView dgv)
        {
            dgv.BorderStyle = BorderStyle.None;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(238, 239, 249);
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.DefaultCellStyle.SelectionBackColor = Color.DarkTurquoise;
            dgv.DefaultCellStyle.SelectionForeColor = Color.WhiteSmoke;
            dgv.BackgroundColor = Color.White;
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(20, 25, 72);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        }
    }
}
