using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TrackAFaceWinForms.Session;

namespace TrackAFaceWinForms.Dialogs
{
    /// <summary>
    /// Dialogue pour sélectionner et charger une session sauvegardée
    /// </summary>
    public partial class LoadSessionDialog : Form
    {
        private ListBox lstSessions;
        private Label lblTitle;
        private Label lblDetails;
        private Button btnLoad;
        private Button btnDelete;
        private Button btnCancel;
        private List<SessionMetadata> _sessions;
        
        public string SelectedSessionPath { get; private set; }

        public LoadSessionDialog()
        {
            InitializeComponent();
            InitializeDialog();
            LoadSessions();
        }

        private void InitializeDialog()
        {
            // Configuration du formulaire
            this.Text = "Charger une Session - Track-A-FACE";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Size = new Size(600, 500);
            this.BackColor = Color.White;

            // Panel d'en-tête
            Panel headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(33, 150, 243) // Bleu
            };

            lblTitle = new Label
            {
                Text = "Sélectionnez une session à charger",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 18),
                AutoSize = true
            };
            headerPanel.Controls.Add(lblTitle);
            this.Controls.Add(headerPanel);

            // Liste des sessions
            Label lblSessions = new Label
            {
                Text = "Sessions disponibles:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 33, 33),
                Location = new Point(20, 75),
                AutoSize = true
            };
            this.Controls.Add(lblSessions);

            lstSessions = new ListBox
            {
                Location = new Point(20, 100),
                Size = new Size(540, 200),
                Font = new Font("Segoe UI", 10),
                DrawMode = DrawMode.OwnerDrawFixed,
                ItemHeight = 60,
                BorderStyle = BorderStyle.FixedSingle
            };
            lstSessions.DrawItem += LstSessions_DrawItem;
            lstSessions.SelectedIndexChanged += LstSessions_SelectedIndexChanged;
            this.Controls.Add(lstSessions);

            // Panel de détails
            Panel detailsPanel = new Panel
            {
                Location = new Point(20, 315),
                Size = new Size(540, 80),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(245, 245, 245)
            };

            lblDetails = new Label
            {
                Text = "Sélectionnez une session pour voir les détails...",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(66, 66, 66),
                Location = new Point(10, 10),
                Size = new Size(520, 60)
            };
            detailsPanel.Controls.Add(lblDetails);
            this.Controls.Add(detailsPanel);

            // Boutons
            btnCancel = new Button
            {
                Text = "Annuler",
                DialogResult = DialogResult.Cancel,
                Location = new Point(350, 415),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(158, 158, 158),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            this.Controls.Add(btnCancel);

            btnDelete = new Button
            {
                Text = "Supprimer",
                Location = new Point(235, 415),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(244, 67, 54), // Rouge
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Enabled = false
            };
            btnDelete.FlatAppearance.BorderSize = 0;
            btnDelete.Click += BtnDelete_Click;
            this.Controls.Add(btnDelete);

            btnLoad = new Button
            {
                Text = "Charger",
                DialogResult = DialogResult.OK,
                Location = new Point(460, 415),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(76, 175, 80), // Vert
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Enabled = false
            };
            btnLoad.FlatAppearance.BorderSize = 0;
            this.Controls.Add(btnLoad);

            // Boutons par défaut
            this.AcceptButton = btnLoad;
            this.CancelButton = btnCancel;
        }

        private void LoadSessions()
        {
            try
            {
                var sessionManager = new SessionManager();
                _sessions = sessionManager.ListSessions();
                
                lstSessions.Items.Clear();
                
                if (_sessions == null || _sessions.Count == 0)
                {
                    lstSessions.Items.Add("Aucune session sauvegardée");
                    return;
                }

                // Trier par date de modification (plus récent en premier)
                _sessions = _sessions.OrderByDescending(s => s.LastModified).ToList();
                
                foreach (var session in _sessions)
                {
                    lstSessions.Items.Add(session);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erreur lors du chargement des sessions:\n\n{ex.Message}",
                    "Erreur",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void LstSessions_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            e.DrawBackground();

            var item = lstSessions.Items[e.Index];
            
            if (item is string)
            {
                // Message "Aucune session"
                using (Brush brush = new SolidBrush(Color.Gray))
                {
                    e.Graphics.DrawString(
                        item.ToString(),
                        new Font("Segoe UI", 10, FontStyle.Italic),
                        brush,
                        new PointF(e.Bounds.X + 10, e.Bounds.Y + 20)
                    );
                }
                return;
            }

            var session = item as SessionMetadata;
            if (session == null) return;

            // Fond sélectionné
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                e.Graphics.FillRectangle(
                    new SolidBrush(Color.FromArgb(200, 230, 201)), // Vert clair
                    e.Bounds
                );
            }
            else
            {
                e.Graphics.FillRectangle(Brushes.White, e.Bounds);
            }

            // Nom de la session (gras)
            e.Graphics.DrawString(
                session.SessionName,
                new Font("Segoe UI", 11, FontStyle.Bold),
                Brushes.Black,
                new PointF(e.Bounds.X + 10, e.Bounds.Y + 5)
            );

            // Date de création
            string dateText = $"Créé: {session.CreatedDate:dd/MM/yyyy HH:mm}";
            e.Graphics.DrawString(
                dateText,
                new Font("Segoe UI", 8),
                Brushes.Gray,
                new PointF(e.Bounds.X + 10, e.Bounds.Y + 28)
            );

            // Date de modification
            string modifiedText = $"Modifié: {session.LastModified:dd/MM/yyyy HH:mm}";
            e.Graphics.DrawString(
                modifiedText,
                new Font("Segoe UI", 8),
                Brushes.Gray,
                new PointF(e.Bounds.X + 10, e.Bounds.Y + 43)
            );

            // Bordure
            e.Graphics.DrawRectangle(
                new Pen(Color.FromArgb(224, 224, 224)),
                e.Bounds
            );

            e.DrawFocusRectangle();
        }

        private void LstSessions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstSessions.SelectedItem is SessionMetadata session)
            {
                // Afficher les détails
                lblDetails.Text = $"Nom: {session.SessionName}\n" +
                                 $"Créé: {session.CreatedDate:dd/MM/yyyy HH:mm}\n" +
                                 $"Modifié: {session.LastModified:dd/MM/yyyy HH:mm}\n" +
                                 $"Fichier: {Path.GetFileName(session.FilePath)}";

                SelectedSessionPath = session.FilePath;
                btnLoad.Enabled = true;
                btnDelete.Enabled = true;
            }
            else
            {
                lblDetails.Text = "Sélectionnez une session pour voir les détails...";
                SelectedSessionPath = null;
                btnLoad.Enabled = false;
                btnDelete.Enabled = false;
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (lstSessions.SelectedItem is SessionMetadata session)
            {
                var result = MessageBox.Show(
                    $"Êtes-vous sûr de vouloir supprimer la session '{session.SessionName}'?\n\n" +
                    "Cette action est irréversible.",
                    "Confirmer la suppression",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        var sessionManager = new SessionManager();
                        sessionManager.DeleteSession(session.FilePath);
                        MessageBox.Show(
                            $"Session '{session.SessionName}' supprimée avec succès.",
                            "Suppression réussie",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                        
                        // Recharger la liste
                        LoadSessions();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            $"Erreur lors de la suppression:\n\n{ex.Message}",
                            "Erreur",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }
                }
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // LoadSessionDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 461);
            this.Name = "LoadSessionDialog";
            this.Text = "Charger une Session - Track-A-FACE";
            this.ResumeLayout(false);
        }
    }
}
