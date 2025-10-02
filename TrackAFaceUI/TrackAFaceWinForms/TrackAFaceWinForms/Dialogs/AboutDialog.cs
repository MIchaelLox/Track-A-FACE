using System;
using System.Drawing;
using System.Windows.Forms;

namespace TrackAFaceWinForms.Dialogs
{
    /// <summary>
    /// Dialogue "À Propos" affichant les informations sur l'application
    /// </summary>
    public partial class AboutDialog : Form
    {
        public AboutDialog()
        {
            InitializeComponent();
            InitializeDialog();
        }

        private void InitializeDialog()
        {
            // Configuration du formulaire
            this.Text = "À Propos - Track-A-FACE";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Size = new Size(500, 400);
            this.BackColor = Color.White;

            // Panel d'en-tête avec fond coloré
            Panel headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(76, 175, 80) // Vert Track-A-FACE
            };

            // Logo/Titre
            Label lblTitle = new Label
            {
                Text = "Track-A-FACE",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 20),
                AutoSize = true
            };
            headerPanel.Controls.Add(lblTitle);

            // Version
            Label lblVersion = new Label
            {
                Text = "Version 1.0.0",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.White,
                Location = new Point(20, 55),
                AutoSize = true
            };
            headerPanel.Controls.Add(lblVersion);

            this.Controls.Add(headerPanel);

            // Description
            Label lblDescription = new Label
            {
                Text = "Calculateur de Coûts de Restaurant",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 33, 33),
                Location = new Point(20, 100),
                AutoSize = true
            };
            this.Controls.Add(lblDescription);

            // Texte détaillé
            Label lblDetails = new Label
            {
                Text = "Track-A-FACE est un outil professionnel pour estimer les coûts\n" +
                       "de démarrage et d'exploitation d'un restaurant.\n\n" +
                       "Fonctionnalités:\n" +
                       "• Calcul détaillé des coûts (Personnel, Équipement, Immobilier, Opérationnel)\n" +
                       "• Support de 5 thèmes de restaurants\n" +
                       "• Sauvegarde et chargement de sessions\n" +
                       "• Export CSV et PDF professionnels\n" +
                       "• Interface intuitive et moderne",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(66, 66, 66),
                Location = new Point(20, 135),
                Size = new Size(440, 150)
            };
            this.Controls.Add(lblDetails);

            // Séparateur
            Panel separator = new Panel
            {
                Location = new Point(20, 290),
                Size = new Size(440, 1),
                BackColor = Color.FromArgb(224, 224, 224)
            };
            this.Controls.Add(separator);

            // Copyright
            Label lblCopyright = new Label
            {
                Text = "© 2025 Track-A-FACE. Tous droits réservés.",
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.FromArgb(117, 117, 117),
                Location = new Point(20, 305),
                AutoSize = true
            };
            this.Controls.Add(lblCopyright);

            // Bouton OK
            Button btnOk = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Location = new Point(380, 325),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnOk.FlatAppearance.BorderSize = 0;
            this.Controls.Add(btnOk);

            // Définir le bouton par défaut
            this.AcceptButton = btnOk;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // AboutDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 361);
            this.Name = "AboutDialog";
            this.Text = "À Propos - Track-A-FACE";
            this.ResumeLayout(false);
        }
    }
}
