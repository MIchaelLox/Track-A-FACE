using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TrackAFaceWinForms
{
    internal static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // Lancer directement InputForm pour tester l'application
            Application.Run(new Forms.InputForm());
            
            // Pour production, utiliser MainForm avec menu:
            // Application.Run(new MainForm());
        }
    }
}
