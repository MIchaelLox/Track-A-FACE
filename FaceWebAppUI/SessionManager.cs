using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Linq;

namespace FaceWebAppUI
{
    /// <summary>
    /// Gestionnaire de sessions pour Track-A-FACE
    /// </summary>
    public class SessionManager
    {
        private readonly string _sessionsDirectory;
        private readonly MainForm _mainForm;

        public SessionManager(MainForm mainForm)
        {
            _mainForm = mainForm ?? throw new ArgumentNullException(nameof(mainForm));
            _sessionsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Track-A-FACE", "Sessions");
            
            // Créer le répertoire s'il n'existe pas
            Directory.CreateDirectory(_sessionsDirectory);
        }

        /// <summary>
        /// Sauvegarde la session courante
        /// </summary>
        public bool SaveCurrentSession(string sessionName = null)
        {
            try
            {
                var sessionData = CreateSessionData(sessionName);
                var fileName = GenerateFileName(sessionData.SessionName);
                var filePath = Path.Combine(_sessionsDirectory, fileName);

                var json = JsonConvert.SerializeObject(sessionData, Formatting.Indented);
                File.WriteAllText(filePath, json);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la sauvegarde: {ex.Message}", "Erreur de sauvegarde", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Charge une session depuis un fichier
        /// </summary>
        public bool LoadSession(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    MessageBox.Show("Le fichier de session n'existe pas.", "Erreur de chargement", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                var json = File.ReadAllText(filePath);
                var sessionData = JsonConvert.DeserializeObject<SessionData>(json);

                if (sessionData == null)
                {
                    MessageBox.Show("Le fichier de session est corrompu.", "Erreur de chargement", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                LoadSessionData(sessionData);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement: {ex.Message}", "Erreur de chargement", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Affiche la boîte de dialogue de sauvegarde
        /// </summary>
        public void ShowSaveDialog()
        {
            var dialog = new SaveFileDialog
            {
                Title = "Sauvegarder la session",
                Filter = "Fichiers de session Track-A-FACE (*.taf)|*.taf|Tous les fichiers (*.*)|*.*",
                DefaultExt = "taf",
                InitialDirectory = _sessionsDirectory,
                FileName = GenerateFileName(_mainForm.GetSessionName())
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var sessionData = CreateSessionData();
                    var json = JsonConvert.SerializeObject(sessionData, Formatting.Indented);
                    File.WriteAllText(dialog.FileName, json);

                    MessageBox.Show("Session sauvegardée avec succès!", "Sauvegarde terminée", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de la sauvegarde: {ex.Message}", "Erreur de sauvegarde", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Affiche la boîte de dialogue de chargement
        /// </summary>
        public void ShowLoadDialog()
        {
            var dialog = new OpenFileDialog
            {
                Title = "Charger une session",
                Filter = "Fichiers de session Track-A-FACE (*.taf)|*.taf|Tous les fichiers (*.*)|*.*",
                InitialDirectory = _sessionsDirectory,
                Multiselect = false
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                LoadSession(dialog.FileName);
            }
        }

        /// <summary>
        /// Affiche le gestionnaire de sessions
        /// </summary>
        public void ShowSessionManager()
        {
            var sessionManagerForm = new SessionManagerForm(this, _sessionsDirectory);
            sessionManagerForm.ShowDialog(_mainForm);
        }

        /// <summary>
        /// Obtient la liste des sessions disponibles
        /// </summary>
        public List<SessionInfo> GetAvailableSessions()
        {
            var sessions = new List<SessionInfo>();

            try
            {
                var files = Directory.GetFiles(_sessionsDirectory, "*.taf");
                
                foreach (var file in files)
                {
                    try
                    {
                        var json = File.ReadAllText(file);
                        var sessionData = JsonConvert.DeserializeObject<SessionData>(json);
                        
                        if (sessionData != null)
                        {
                            sessions.Add(new SessionInfo
                            {
                                FilePath = file,
                                SessionName = sessionData.SessionName,
                                CreatedDate = sessionData.CreatedDate,
                                ModifiedDate = File.GetLastWriteTime(file),
                                RestaurantTheme = sessionData.InputData.GetValueOrDefault("restaurant_theme", "").ToString(),
                                RevenueSize = sessionData.InputData.GetValueOrDefault("revenue_size", "").ToString()
                            });
                        }
                    }
                    catch
                    {
                        // Ignorer les fichiers corrompus
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la lecture des sessions: {ex.Message}", "Erreur", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return sessions.OrderByDescending(s => s.ModifiedDate).ToList();
        }

        /// <summary>
        /// Supprime une session
        /// </summary>
        public bool DeleteSession(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la suppression: {ex.Message}", "Erreur de suppression", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Sauvegarde automatique
        /// </summary>
        public void AutoSave()
        {
            try
            {
                var autoSaveDir = Path.Combine(_sessionsDirectory, "AutoSave");
                Directory.CreateDirectory(autoSaveDir);

                var sessionData = CreateSessionData("AutoSave");
                var fileName = $"AutoSave_{DateTime.Now:yyyyMMdd_HHmmss}.taf";
                var filePath = Path.Combine(autoSaveDir, fileName);

                var json = JsonConvert.SerializeObject(sessionData, Formatting.Indented);
                File.WriteAllText(filePath, json);

                // Nettoyer les anciennes sauvegardes automatiques (garder seulement les 10 dernières)
                CleanupAutoSaves(autoSaveDir);
            }
            catch
            {
                // Ignorer les erreurs de sauvegarde automatique
            }
        }

        /// <summary>
        /// Crée les données de session
        /// </summary>
        private SessionData CreateSessionData(string sessionName = null)
        {
            var inputData = _mainForm.CollectInputData();
            
            return new SessionData
            {
                SessionName = sessionName ?? _mainForm.GetSessionName(),
                CreatedDate = DateTime.Now,
                Version = "1.0",
                InputData = inputData,
                WorkflowStep = _mainForm.GetCurrentWorkflowStep(),
                Notes = _mainForm.GetSessionNotes()
            };
        }

        /// <summary>
        /// Charge les données de session dans l'interface
        /// </summary>
        private void LoadSessionData(SessionData sessionData)
        {
            _mainForm.LoadInputData(sessionData.InputData);
            _mainForm.SetCurrentWorkflowStep(sessionData.WorkflowStep);
            _mainForm.SetSessionNotes(sessionData.Notes ?? "");
            
            MessageBox.Show($"Session '{sessionData.SessionName}' chargée avec succès!", "Chargement terminé", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Génère un nom de fichier pour la session
        /// </summary>
        private string GenerateFileName(string sessionName)
        {
            var cleanName = string.Join("_", sessionName.Split(Path.GetInvalidFileNameChars()));
            return $"{cleanName}_{DateTime.Now:yyyyMMdd_HHmmss}.taf";
        }

        /// <summary>
        /// Nettoie les anciennes sauvegardes automatiques
        /// </summary>
        private void CleanupAutoSaves(string autoSaveDir)
        {
            try
            {
                var files = Directory.GetFiles(autoSaveDir, "AutoSave_*.taf")
                    .Select(f => new FileInfo(f))
                    .OrderByDescending(f => f.LastWriteTime)
                    .Skip(10)
                    .ToList();

                foreach (var file in files)
                {
                    file.Delete();
                }
            }
            catch
            {
                // Ignorer les erreurs de nettoyage
            }
        }
    }

    /// <summary>
    /// Données de session
    /// </summary>
    public class SessionData
    {
        public string SessionName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string Version { get; set; } = "1.0";
        public Dictionary<string, object> InputData { get; set; } = new Dictionary<string, object>();
        public int WorkflowStep { get; set; }
        public string Notes { get; set; } = string.Empty;
    }

    /// <summary>
    /// Informations sur une session
    /// </summary>
    public class SessionInfo
    {
        public string FilePath { get; set; } = string.Empty;
        public string SessionName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string RestaurantTheme { get; set; } = string.Empty;
        public string RevenueSize { get; set; } = string.Empty;
    }

    /// <summary>
    /// Formulaire de gestion des sessions
    /// </summary>
    public partial class SessionManagerForm : Form
    {
        private readonly SessionManager _sessionManager;
        private readonly string _sessionsDirectory;
        private ListView _listViewSessions;
        private TextBox _txtNotes;

        public SessionManagerForm(SessionManager sessionManager, string sessionsDirectory)
        {
            _sessionManager = sessionManager;
            _sessionsDirectory = sessionsDirectory;
            InitializeComponent();
            LoadSessions();
        }

        private void InitializeComponent()
        {
            this.Text = "Gestionnaire de sessions - Track-A-FACE";
            this.Size = new System.Drawing.Size(800, 600);
            this.StartPosition = FormStartPosition.CenterParent;

            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 3,
                ColumnCount = 2,
                Padding = new System.Windows.Forms.Padding(10)
            };

            // Liste des sessions
            _listViewSessions = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                MultiSelect = false
            };

            _listViewSessions.Columns.Add("Nom de session", 200);
            _listViewSessions.Columns.Add("Type de restaurant", 120);
            _listViewSessions.Columns.Add("Taille", 100);
            _listViewSessions.Columns.Add("Créé le", 120);
            _listViewSessions.Columns.Add("Modifié le", 120);

            _listViewSessions.SelectedIndexChanged += ListViewSessions_SelectedIndexChanged;
            _listViewSessions.DoubleClick += ListViewSessions_DoubleClick;

            mainPanel.Controls.Add(_listViewSessions, 0, 0);
            mainPanel.SetColumnSpan(_listViewSessions, 2);

            // Notes
            var lblNotes = new Label
            {
                Text = "Notes:",
                Dock = DockStyle.Fill,
                TextAlign = System.Drawing.ContentAlignment.TopLeft
            };
            mainPanel.Controls.Add(lblNotes, 0, 1);

            _txtNotes = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                ReadOnly = true
            };
            mainPanel.Controls.Add(_txtNotes, 1, 1);

            // Boutons
            var buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft
            };

            var btnClose = new Button
            {
                Text = "Fermer",
                Size = new System.Drawing.Size(80, 30),
                DialogResult = DialogResult.Cancel
            };

            var btnDelete = new Button
            {
                Text = "Supprimer",
                Size = new System.Drawing.Size(80, 30),
                BackColor = System.Drawing.Color.FromArgb(220, 53, 69),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnDelete.Click += BtnDelete_Click;

            var btnLoad = new Button
            {
                Text = "Charger",
                Size = new System.Drawing.Size(80, 30),
                BackColor = System.Drawing.Color.FromArgb(0, 123, 255),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnLoad.Click += BtnLoad_Click;

            var btnRefresh = new Button
            {
                Text = "Actualiser",
                Size = new System.Drawing.Size(80, 30),
                BackColor = System.Drawing.Color.FromArgb(108, 117, 125),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnRefresh.Click += BtnRefresh_Click;

            buttonPanel.Controls.Add(btnClose);
            buttonPanel.Controls.Add(btnDelete);
            buttonPanel.Controls.Add(btnLoad);
            buttonPanel.Controls.Add(btnRefresh);

            mainPanel.Controls.Add(buttonPanel, 0, 2);
            mainPanel.SetColumnSpan(buttonPanel, 2);

            // Configuration des styles de ligne
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 70F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100F));
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            this.Controls.Add(mainPanel);
            this.CancelButton = btnClose;
        }

        private void LoadSessions()
        {
            _listViewSessions.Items.Clear();
            var sessions = _sessionManager.GetAvailableSessions();

            foreach (var session in sessions)
            {
                var item = new ListViewItem(session.SessionName);
                item.SubItems.Add(session.RestaurantTheme);
                item.SubItems.Add(session.RevenueSize);
                item.SubItems.Add(session.CreatedDate.ToString("dd/MM/yyyy HH:mm"));
                item.SubItems.Add(session.ModifiedDate.ToString("dd/MM/yyyy HH:mm"));
                item.Tag = session;

                _listViewSessions.Items.Add(item);
            }
        }

        private void ListViewSessions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_listViewSessions.SelectedItems.Count > 0)
            {
                var session = _listViewSessions.SelectedItems[0].Tag as SessionInfo;
                // Charger les notes si disponibles
                _txtNotes.Text = $"Session: {session?.SessionName}\nCréée: {session?.CreatedDate:dd/MM/yyyy HH:mm}\nModifiée: {session?.ModifiedDate:dd/MM/yyyy HH:mm}";
            }
            else
            {
                _txtNotes.Text = "";
            }
        }

        private void ListViewSessions_DoubleClick(object sender, EventArgs e)
        {
            BtnLoad_Click(sender, e);
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            if (_listViewSessions.SelectedItems.Count > 0)
            {
                var session = _listViewSessions.SelectedItems[0].Tag as SessionInfo;
                if (session != null)
                {
                    if (_sessionManager.LoadSession(session.FilePath))
                    {
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner une session à charger.", "Aucune sélection", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (_listViewSessions.SelectedItems.Count > 0)
            {
                var session = _listViewSessions.SelectedItems[0].Tag as SessionInfo;
                if (session != null)
                {
                    var result = MessageBox.Show($"Êtes-vous sûr de vouloir supprimer la session '{session.SessionName}'?", 
                        "Confirmer la suppression", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    
                    if (result == DialogResult.Yes)
                    {
                        if (_sessionManager.DeleteSession(session.FilePath))
                        {
                            LoadSessions();
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner une session à supprimer.", "Aucune sélection", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadSessions();
        }
    }
}
