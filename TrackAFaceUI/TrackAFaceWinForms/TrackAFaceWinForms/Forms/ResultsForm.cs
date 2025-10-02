using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackAFaceWinForms.Models;

namespace TrackAFaceWinForms.Forms
{
    public partial class ResultsForm : Form
    {
        private CalculationResultModel _results;

        public ResultsForm()
        {
            InitializeComponent();
            InitializeDataGridViews();
        }

        /// <summary>
        /// Initialise les colonnes et le style des DataGridView
        /// </summary>
        private void InitializeDataGridViews()
        {
            // Configuration commune pour tous les DataGridView
            foreach (var dgv in new[] { dgvStaff, dgvEquipment, dgvLocation, dgvOperational })
            {
                dgv.AutoGenerateColumns = false;
                dgv.ReadOnly = true;
                dgv.AllowUserToAddRows = false;
                dgv.AllowUserToDeleteRows = false;
                dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgv.MultiSelect = false;
                dgv.RowHeadersVisible = false;

                // Colonnes
                dgv.Columns.Add(new DataGridViewTextBoxColumn
                {
                    HeaderText = "Sous-catégorie",
                    DataPropertyName = "Subcategory",
                    Width = 200,
                    DefaultCellStyle = new DataGridViewCellStyle
                    {
                        Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular)
                    }
                });

                dgv.Columns.Add(new DataGridViewTextBoxColumn
                {
                    HeaderText = "Montant (CAD$)",
                    DataPropertyName = "Amount",
                    Width = 150,
                    DefaultCellStyle = new DataGridViewCellStyle
                    {
                        Format = "N2",
                        Alignment = DataGridViewContentAlignment.MiddleRight,
                        Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold)
                    }
                });

                dgv.Columns.Add(new DataGridViewTextBoxColumn
                {
                    HeaderText = "Formule",
                    DataPropertyName = "Formula",
                    Width = 150,
                    DefaultCellStyle = new DataGridViewCellStyle
                    {
                        Font = new Font("Consolas", 8F, FontStyle.Regular)
                    }
                });

                dgv.Columns.Add(new DataGridViewTextBoxColumn
                {
                    HeaderText = "Détails",
                    DataPropertyName = "Details",
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                    DefaultCellStyle = new DataGridViewCellStyle
                    {
                        Font = new Font("Microsoft Sans Serif", 8F, FontStyle.Italic),
                        ForeColor = Color.Gray
                    }
                });
            }

            // Couleurs spécifiques par catégorie
            ApplyColors();
        }

        /// <summary>
        /// Applique les couleurs par catégorie
        /// </summary>
        private void ApplyColors()
        {
            dgvStaff.DefaultCellStyle.BackColor = Color.FromArgb(220, 76, 175, 80);        // Vert clair
            dgvStaff.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 76, 175, 80);

            dgvEquipment.DefaultCellStyle.BackColor = Color.FromArgb(220, 33, 150, 243);   // Bleu clair
            dgvEquipment.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 33, 150, 243);

            dgvLocation.DefaultCellStyle.BackColor = Color.FromArgb(220, 255, 152, 0);     // Orange clair
            dgvLocation.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 255, 152, 0);

            dgvOperational.DefaultCellStyle.BackColor = Color.FromArgb(220, 156, 39, 176); // Violet clair
            dgvOperational.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 156, 39, 176);
        }

        /// <summary>
        /// Affiche les résultats dans le formulaire
        /// </summary>
        public void DisplayResults(CalculationResultModel results)
        {
            _results = results;

            // Vérifier si erreur
            if (results.HasError)
            {
                MessageBox.Show(
                    $"Erreur lors du calcul:\n\n{results.ErrorMessage}\n\nDétails:\n{results.ErrorDetails}",
                    "Erreur de Calcul",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            // Afficher le coût total
            lblTotalCost.Text = $"{results.TotalCost:N2} CAD$";

            // Remplir les DataGridView
            dgvStaff.DataSource = results.GetStaffBreakdowns();
            dgvEquipment.DataSource = results.GetEquipmentBreakdowns();
            dgvLocation.DataSource = results.GetLocationBreakdowns();
            dgvOperational.DataSource = results.GetOperationalBreakdowns();

            // Ajuster la hauteur des lignes
            AdjustRowHeights();
        }

        /// <summary>
        /// Ajuste la hauteur des lignes pour un meilleur affichage
        /// </summary>
        private void AdjustRowHeights()
        {
            foreach (var dgv in new[] { dgvStaff, dgvEquipment, dgvLocation, dgvOperational })
            {
                if (dgv.Rows.Count > 0)
                {
                    dgv.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCells);
                }
            }
        }

        /// <summary>
        /// Bouton Nouvelle Analyse - Ferme le formulaire
        /// </summary>
        private void btnNewAnalysis_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Retourne le modèle de résultats actuel
        /// </summary>
        public CalculationResultModel GetResults()
        {
            return _results;
        }
    }
}
