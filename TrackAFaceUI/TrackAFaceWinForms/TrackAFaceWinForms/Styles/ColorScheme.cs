using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackAFaceWinForms.Styles
{
    public static class ColorScheme
    {
        // Couleurs par catégorie (utilisées dans ResultsForm)
        public static readonly Color StaffColor = Color.FromArgb(76, 175, 80);       // Vert
        public static readonly Color EquipmentColor = Color.FromArgb(33, 150, 243);  // Bleu
        public static readonly Color LocationColor = Color.FromArgb(255, 152, 0);    // Orange
        public static readonly Color OperationalColor = Color.FromArgb(156, 39, 176); // Violet
        public static readonly Color TotalColor = Color.FromArgb(244, 67, 54);       // Rouge

        // Couleurs UI
        public static readonly Color PrimaryButtonColor = Color.FromArgb(76, 175, 80);
        public static readonly Color SecondaryButtonColor = Color.FromArgb(158, 158, 158);
        public static readonly Color SuccessColor = Color.FromArgb(76, 175, 80);
        public static readonly Color ErrorColor = Color.FromArgb(244, 67, 54);
        public static readonly Color WarningColor = Color.FromArgb(255, 152, 0);
    }
}
