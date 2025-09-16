using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

namespace FaceWebAppUI
{
    /// <summary>
    /// Méthodes pour créer les étapes du workflow
    /// </summary>
    public partial class MainForm
    {
        /// <summary>
        /// Étape 1: Informations générales
        /// </summary>
        private void CreateStep1_GeneralInfo(Panel contentPanel)
        {
            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 4,
                ColumnCount = 2,
                Padding = new Padding(20)
            };

            // Titre de l'étape
            var titleLabel = new Label
            {
                Text = "🏪 Informations générales de votre restaurant",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 58, 64),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };
            mainPanel.Controls.Add(titleLabel, 0, 0);
            mainPanel.SetColumnSpan(titleLabel, 2);

            // Nom de session
            var lblSessionName = new Label
            {
                Text = "Nom de votre projet:",
                Font = new Font("Segoe UI", 10F),
                TextAlign = ContentAlignment.MiddleRight,
                Dock = DockStyle.Fill
            };
            mainPanel.Controls.Add(lblSessionName, 0, 1);
            mainPanel.Controls.Add(txtSessionName, 1, 1);

            // Thème du restaurant
            var lblTheme = new Label
            {
                Text = "Type de restaurant:",
                Font = new Font("Segoe UI", 10F),
                TextAlign = ContentAlignment.MiddleRight,
                Dock = DockStyle.Fill
            };
            mainPanel.Controls.Add(lblTheme, 0, 2);
            mainPanel.Controls.Add(cmbTheme, 1, 2);

            // Taille de revenus
            var lblRevenue = new Label
            {
                Text = "Taille d'entreprise:",
                Font = new Font("Segoe UI", 10F),
                TextAlign = ContentAlignment.MiddleRight,
                Dock = DockStyle.Fill
            };
            mainPanel.Controls.Add(lblRevenue, 0, 3);
            mainPanel.Controls.Add(cmbRevenueSize, 1, 3);

            // Configuration des styles de ligne
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));

            contentPanel.Controls.Add(mainPanel);
        }

        /// <summary>
        /// Étape 2: Configuration de la cuisine
        /// </summary>
        private void CreateStep2_KitchenConfig(Panel contentPanel)
        {
            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 5,
                ColumnCount = 2,
                Padding = new Padding(20)
            };

            // Titre
            var titleLabel = new Label
            {
                Text = "🍳 Configuration de votre cuisine",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 58, 64),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };
            mainPanel.Controls.Add(titleLabel, 0, 0);
            mainPanel.SetColumnSpan(titleLabel, 2);

            // Taille de la cuisine
            var lblKitchenSize = new Label
            {
                Text = "Superficie (m²):",
                Font = new Font("Segoe UI", 10F),
                TextAlign = ContentAlignment.MiddleRight,
                Dock = DockStyle.Fill
            };
            mainPanel.Controls.Add(lblKitchenSize, 0, 1);
            mainPanel.Controls.Add(numKitchenSize, 1, 1);

            // Postes de travail
            var lblWorkstations = new Label
            {
                Text = "Postes de travail:",
                Font = new Font("Segoe UI", 10F),
                TextAlign = ContentAlignment.MiddleRight,
                Dock = DockStyle.Fill
            };
            mainPanel.Controls.Add(lblWorkstations, 0, 2);
            mainPanel.Controls.Add(numWorkstations, 1, 2);

            // Capacité journalière
            var lblCapacity = new Label
            {
                Text = "Capacité journalière (clients):",
                Font = new Font("Segoe UI", 10F),
                TextAlign = ContentAlignment.MiddleRight,
                Dock = DockStyle.Fill
            };
            mainPanel.Controls.Add(lblCapacity, 0, 3);
            mainPanel.Controls.Add(numCapacity, 1, 3);

            // Conseils
            var tipsLabel = new Label
            {
                Text = "💡 Conseil: Prévoyez environ 1 poste de travail pour 10-15m² de cuisine",
                Font = new Font("Segoe UI", 9F, FontStyle.Italic),
                ForeColor = Color.FromArgb(108, 117, 125),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.TopLeft
            };
            mainPanel.Controls.Add(tipsLabel, 0, 4);
            mainPanel.SetColumnSpan(tipsLabel, 2);

            // Configuration des styles
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));

            contentPanel.Controls.Add(mainPanel);
        }

        /// <summary>
        /// Étape 3: Personnel et formation
        /// </summary>
        private void CreateStep3_StaffTraining(Panel contentPanel)
        {
            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 5,
                ColumnCount = 2,
                Padding = new Padding(20)
            };

            // Titre
            var titleLabel = new Label
            {
                Text = "👥 Personnel et formation",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 58, 64),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };
            mainPanel.Controls.Add(titleLabel, 0, 0);
            mainPanel.SetColumnSpan(titleLabel, 2);

            // Nombre d'employés
            var lblStaffCount = new Label
            {
                Text = "Nombre d'employés:",
                Font = new Font("Segoe UI", 10F),
                TextAlign = ContentAlignment.MiddleRight,
                Dock = DockStyle.Fill
            };
            mainPanel.Controls.Add(lblStaffCount, 0, 1);
            mainPanel.Controls.Add(numStaffCount, 1, 1);

            // Niveau d'expérience
            var lblExperience = new Label
            {
                Text = "Niveau d'expérience:",
                Font = new Font("Segoe UI", 10F),
                TextAlign = ContentAlignment.MiddleRight,
                Dock = DockStyle.Fill
            };
            mainPanel.Controls.Add(lblExperience, 0, 2);
            mainPanel.Controls.Add(cmbExperience, 1, 2);

            // Heures de formation
            var lblTraining = new Label
            {
                Text = "Heures de formation nécessaires:",
                Font = new Font("Segoe UI", 10F),
                TextAlign = ContentAlignment.MiddleRight,
                Dock = DockStyle.Fill
            };
            mainPanel.Controls.Add(lblTraining, 0, 3);
            mainPanel.Controls.Add(numTrainingHours, 1, 3);

            // Conseils
            var tipsLabel = new Label
            {
                Text = "💡 Conseil: Un personnel expérimenté réduit les coûts de formation mais augmente les salaires",
                Font = new Font("Segoe UI", 9F, FontStyle.Italic),
                ForeColor = Color.FromArgb(108, 117, 125),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.TopLeft
            };
            mainPanel.Controls.Add(tipsLabel, 0, 4);
            mainPanel.SetColumnSpan(tipsLabel, 2);

            // Configuration des styles
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));

            contentPanel.Controls.Add(mainPanel);
        }

        /// <summary>
        /// Étape 4: Équipement
        /// </summary>
        private void CreateStep4_Equipment(Panel contentPanel)
        {
            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 5,
                ColumnCount = 2,
                Padding = new Padding(20)
            };

            // Titre
            var titleLabel = new Label
            {
                Text = "🔧 Équipement de cuisine",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 58, 64),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };
            mainPanel.Controls.Add(titleLabel, 0, 0);
            mainPanel.SetColumnSpan(titleLabel, 2);

            // Âge de l'équipement
            var lblAge = new Label
            {
                Text = "Âge de l'équipement (années):",
                Font = new Font("Segoe UI", 10F),
                TextAlign = ContentAlignment.MiddleRight,
                Dock = DockStyle.Fill
            };
            mainPanel.Controls.Add(lblAge, 0, 1);
            mainPanel.Controls.Add(numEquipmentAge, 1, 1);

            // État de l'équipement
            var lblCondition = new Label
            {
                Text = "État de l'équipement:",
                Font = new Font("Segoe UI", 10F),
                TextAlign = ContentAlignment.MiddleRight,
                Dock = DockStyle.Fill
            };
            mainPanel.Controls.Add(lblCondition, 0, 2);
            mainPanel.Controls.Add(cmbEquipmentCondition, 1, 2);

            // Valeur de l'équipement
            var lblValue = new Label
            {
                Text = "Valeur de l'équipement (CAD$):",
                Font = new Font("Segoe UI", 10F),
                TextAlign = ContentAlignment.MiddleRight,
                Dock = DockStyle.Fill
            };
            mainPanel.Controls.Add(lblValue, 0, 3);
            mainPanel.Controls.Add(numEquipmentValue, 1, 3);

            // Conseils
            var tipsLabel = new Label
            {
                Text = "💡 Conseil: Un équipement neuf coûte plus cher mais nécessite moins de maintenance",
                Font = new Font("Segoe UI", 9F, FontStyle.Italic),
                ForeColor = Color.FromArgb(108, 117, 125),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.TopLeft
            };
            mainPanel.Controls.Add(tipsLabel, 0, 4);
            mainPanel.SetColumnSpan(tipsLabel, 2);

            // Configuration des styles
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));

            contentPanel.Controls.Add(mainPanel);
        }

        /// <summary>
        /// Étape 5: Immobilier
        /// </summary>
        private void CreateStep5_Location(Panel contentPanel)
        {
            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 3,
                ColumnCount = 2,
                Padding = new Padding(20)
            };

            // Titre
            var titleLabel = new Label
            {
                Text = "🏢 Coûts immobiliers",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 58, 64),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };
            mainPanel.Controls.Add(titleLabel, 0, 0);
            mainPanel.SetColumnSpan(titleLabel, 2);

            // Loyer par m²
            var lblRent = new Label
            {
                Text = "Loyer par m² (CAD$/mois):",
                Font = new Font("Segoe UI", 10F),
                TextAlign = ContentAlignment.MiddleRight,
                Dock = DockStyle.Fill
            };
            mainPanel.Controls.Add(lblRent, 0, 1);
            mainPanel.Controls.Add(numRentPerSqm, 1, 1);

            // Conseils
            var tipsLabel = new Label
            {
                Text = "💡 Conseil: Le loyer varie selon l'emplacement. Centre-ville: 50-100$/m², Banlieue: 20-50$/m²",
                Font = new Font("Segoe UI", 9F, FontStyle.Italic),
                ForeColor = Color.FromArgb(108, 117, 125),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.TopLeft
            };
            mainPanel.Controls.Add(tipsLabel, 0, 2);
            mainPanel.SetColumnSpan(tipsLabel, 2);

            // Configuration des styles
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));

            contentPanel.Controls.Add(mainPanel);
        }

        /// <summary>
        /// Étape 6: Validation et calcul
        /// </summary>
        private void CreateStep6_Validation(Panel contentPanel)
        {
            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 3,
                ColumnCount = 1,
                Padding = new Padding(20)
            };

            // Titre
            var titleLabel = new Label
            {
                Text = "✅ Validation et calcul",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 58, 64),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };
            mainPanel.Controls.Add(titleLabel, 0, 0);

            // Résumé des données
            var summaryPanel = CreateValidationSummary();
            mainPanel.Controls.Add(summaryPanel, 0, 1);

            // Instructions finales
            var instructionsLabel = new Label
            {
                Text = "🎯 Vérifiez vos données ci-dessus, puis cliquez sur 'Calculer' pour obtenir vos résultats détaillés.",
                Font = new Font("Segoe UI", 11F),
                ForeColor = Color.FromArgb(40, 167, 69),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            mainPanel.Controls.Add(instructionsLabel, 0, 2);

            // Configuration des styles
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));

            contentPanel.Controls.Add(mainPanel);
        }

        /// <summary>
        /// Crée le résumé de validation
        /// </summary>
        private Panel CreateValidationSummary()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(248, 249, 250),
                BorderStyle = BorderStyle.FixedSingle
            };

            var summaryText = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                BackColor = Color.FromArgb(248, 249, 250),
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10F)
            };

            // Construire le résumé
            var summary = "RÉSUMÉ DE VOTRE CONFIGURATION:\n\n";
            summary += $"📝 Projet: {txtSessionName?.Text ?? "Non défini"}\n";
            summary += $"🏪 Type: {cmbTheme?.SelectedItem?.ToString() ?? "Non défini"}\n";
            summary += $"📊 Taille: {cmbRevenueSize?.SelectedItem?.ToString() ?? "Non défini"}\n\n";
            summary += $"🍳 Cuisine: {numKitchenSize?.Value ?? 0}m² avec {numWorkstations?.Value ?? 0} postes\n";
            summary += $"👥 Personnel: {numStaffCount?.Value ?? 0} employés ({cmbExperience?.SelectedItem?.ToString() ?? "Non défini"})\n";
            summary += $"🔧 Équipement: {numEquipmentValue?.Value ?? 0:N0} CAD$ ({cmbEquipmentCondition?.SelectedItem?.ToString() ?? "Non défini"})\n";
            summary += $"🏢 Loyer: {numRentPerSqm?.Value ?? 0} CAD$/m²/mois\n";

            summaryText.Text = summary;
            panel.Controls.Add(summaryText);

            return panel;
        }
    }
}
