namespace TrackAFaceWinForms
{
    partial class MainForm
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.menuFichier = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFichierNouveau = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFichierOuvrir = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFichierSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuFichierQuitter = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAide = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAideAPropos = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuFichier,
            this.menuAide});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(800, 24);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip1";
            // 
            // menuFichier
            // 
            this.menuFichier.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuFichierNouveau,
            this.menuFichierOuvrir,
            this.menuFichierSeparator1,
            this.menuFichierQuitter});
            this.menuFichier.Name = "menuFichier";
            this.menuFichier.Size = new System.Drawing.Size(54, 20);
            this.menuFichier.Text = "&Fichier";
            // 
            // menuFichierNouveau
            // 
            this.menuFichierNouveau.Name = "menuFichierNouveau";
            this.menuFichierNouveau.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.menuFichierNouveau.Size = new System.Drawing.Size(227, 22);
            this.menuFichierNouveau.Text = "&Nouvelle Session";
            this.menuFichierNouveau.Click += new System.EventHandler(this.menuFichierNouveau_Click);
            // 
            // menuFichierOuvrir
            // 
            this.menuFichierOuvrir.Name = "menuFichierOuvrir";
            this.menuFichierOuvrir.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.menuFichierOuvrir.Size = new System.Drawing.Size(227, 22);
            this.menuFichierOuvrir.Text = "&Ouvrir Session...";
            this.menuFichierOuvrir.Click += new System.EventHandler(this.menuFichierOuvrir_Click);
            // 
            // menuFichierSeparator1
            // 
            this.menuFichierSeparator1.Name = "menuFichierSeparator1";
            this.menuFichierSeparator1.Size = new System.Drawing.Size(224, 6);
            // 
            // menuFichierQuitter
            // 
            this.menuFichierQuitter.Name = "menuFichierQuitter";
            this.menuFichierQuitter.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.menuFichierQuitter.Size = new System.Drawing.Size(227, 22);
            this.menuFichierQuitter.Text = "&Quitter";
            this.menuFichierQuitter.Click += new System.EventHandler(this.menuFichierQuitter_Click);
            // 
            // menuAide
            // 
            this.menuAide.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuAideAPropos});
            this.menuAide.Name = "menuAide";
            this.menuAide.Size = new System.Drawing.Size(43, 20);
            this.menuAide.Text = "&Aide";
            // 
            // menuAideAPropos
            // 
            this.menuAideAPropos.Name = "menuAideAPropos";
            this.menuAideAPropos.ShortcutKeys = System.Windows.Forms.Keys.F1;
            this.menuAideAPropos.Size = new System.Drawing.Size(154, 22);
            this.menuAideAPropos.Text = "À &Propos...";
            this.menuAideAPropos.Click += new System.EventHandler(this.menuAideAPropos_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainForm";
            this.Text = "Track-A-FACE - Calculateur de Coûts de Restaurant";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem menuFichier;
        private System.Windows.Forms.ToolStripMenuItem menuFichierNouveau;
        private System.Windows.Forms.ToolStripMenuItem menuFichierOuvrir;
        private System.Windows.Forms.ToolStripSeparator menuFichierSeparator1;
        private System.Windows.Forms.ToolStripMenuItem menuFichierQuitter;
        private System.Windows.Forms.ToolStripMenuItem menuAide;
        private System.Windows.Forms.ToolStripMenuItem menuAideAPropos;
    }
}

