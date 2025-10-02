using System;
using System.Drawing;
using System.Windows.Forms;

namespace TrackAFaceWinForms.Dialogs
{
    /// <summary>
    /// Dialogue affichant une barre de progression pendant le calcul
    /// </summary>
    public partial class ProgressDialog : Form
    {
        private Label lblMessage;
        private ProgressBar progressBar;
        private Label lblStatus;

        public ProgressDialog()
        {
            InitializeComponent();
            InitializeDialog();
        }

        private void InitializeDialog()
        {
            // Configuration du formulaire
            this.Text = "Calcul en cours...";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Size = new Size(450, 200);
            this.BackColor = Color.White;
            this.ControlBox = false; // Pas de bouton fermer

            // Message principal
            lblMessage = new Label
            {
                Text = "Calcul des coûts en cours...",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 33, 33),
                Location = new Point(20, 20),
                Size = new Size(400, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lblMessage);

            // Barre de progression
            progressBar = new ProgressBar
            {
                Location = new Point(40, 70),
                Size = new Size(360, 30),
                Style = ProgressBarStyle.Marquee,
                MarqueeAnimationSpeed = 30
            };
            this.Controls.Add(progressBar);

            // Statut
            lblStatus = new Label
            {
                Text = "Communication avec le moteur Python...",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(117, 117, 117),
                Location = new Point(20, 115),
                Size = new Size(400, 40),
                TextAlign = ContentAlignment.TopCenter
            };
            this.Controls.Add(lblStatus);
        }

        /// <summary>
        /// Met à jour le message principal
        /// </summary>
        public void UpdateMessage(string message)
        {
            if (lblMessage.InvokeRequired)
            {
                lblMessage.Invoke(new Action(() => lblMessage.Text = message));
            }
            else
            {
                lblMessage.Text = message;
            }
        }

        /// <summary>
        /// Met à jour le statut
        /// </summary>
        public void UpdateStatus(string status)
        {
            if (lblStatus.InvokeRequired)
            {
                lblStatus.Invoke(new Action(() => lblStatus.Text = status));
            }
            else
            {
                lblStatus.Text = status;
            }
        }

        /// <summary>
        /// Définit le mode de progression (déterminé ou indéterminé)
        /// </summary>
        public void SetProgressMode(bool indeterminate)
        {
            if (progressBar.InvokeRequired)
            {
                progressBar.Invoke(new Action(() =>
                {
                    progressBar.Style = indeterminate
                        ? ProgressBarStyle.Marquee
                        : ProgressBarStyle.Continuous;
                }));
            }
            else
            {
                progressBar.Style = indeterminate
                    ? ProgressBarStyle.Marquee
                    : ProgressBarStyle.Continuous;
            }
        }

        /// <summary>
        /// Met à jour la valeur de progression (0-100)
        /// </summary>
        public void SetProgress(int value)
        {
            if (progressBar.InvokeRequired)
            {
                progressBar.Invoke(new Action(() => progressBar.Value = Math.Min(100, Math.Max(0, value))));
            }
            else
            {
                progressBar.Value = Math.Min(100, Math.Max(0, value));
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ProgressDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 161);
            this.Name = "ProgressDialog";
            this.Text = "Calcul en cours...";
            this.ResumeLayout(false);
        }
    }
}
