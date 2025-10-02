using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackAFaceWinForms.Dialogs;
using TrackAFaceWinForms.Services;

namespace TrackAFaceWinForms
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            // Test diagnostic Python Bridge (optionnel - peut être commenté)
            // var bridge = new TrackAFaceWinForms.Services.PythonBridge();
            // MessageBox.Show(bridge.GetDiagnostic(), "Configuration Track-A-FACE");
        }

        /// <summary>
        /// Menu: Fichier → Nouvelle Session (Ctrl+N)
        /// </summary>
        private void menuFichierNouveau_Click(object sender, EventArgs e)
        {
            // TODO: Réinitialiser le formulaire InputForm
            MessageBox.Show(
                "Fonctionnalité à venir:\n\nRéinitialiser tous les champs et créer une nouvelle session.",
                "Nouvelle Session",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        /// <summary>
        /// Menu: Fichier → Ouvrir Session... (Ctrl+O)
        /// </summary>
        private void menuFichierOuvrir_Click(object sender, EventArgs e)
        {
            try
            {
                using (var dialog = new LoadSessionDialog())
                {
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        if (!string.IsNullOrEmpty(dialog.SelectedSessionPath))
                        {
                            MessageBox.Show(
                                $"Session sélectionnée:\n\n{dialog.SelectedSessionPath}\n\n" +
                                "Fonctionnalité à venir: Charger la session dans InputForm.",
                                "Charger Session",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                            );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erreur lors de l'ouverture de la session:\n\n{ex.Message}",
                    "Erreur",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Menu: Fichier → Quitter (Alt+F4)
        /// </summary>
        private void menuFichierQuitter_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Êtes-vous sûr de vouloir quitter Track-A-FACE?",
                "Quitter",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        /// <summary>
        /// Menu: Aide → À Propos... (F1)
        /// </summary>
        private void menuAideAPropos_Click(object sender, EventArgs e)
        {
            try
            {
                using (var dialog = new AboutDialog())
                {
                    dialog.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erreur lors de l'ouverture du dialogue À Propos:\n\n{ex.Message}",
                    "Erreur",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}
