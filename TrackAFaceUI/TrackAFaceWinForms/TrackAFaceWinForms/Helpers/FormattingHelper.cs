using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackAFaceWinForms.Helpers
{
    public static class FormattingHelper
    {
        /// <summary>
        /// Formate un montant en CAD$
        /// </summary>
        public static string FormatCurrency(double amount)
        {
            return $"{amount:N2} CAD$";
        }

        /// <summary>
        /// Formate un pourcentage
        /// </summary>
        public static string FormatPercentage(double percent)
        {
            return $"{percent:F1}%";
        }

        /// <summary>
        /// Formate une date
        /// </summary>
        public static string FormatDate(DateTime date)
        {
            return date.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// Formate un nombre avec séparateur de milliers
        /// </summary>
        public static string FormatNumber(int number)
        {
            return number.ToString("N0");
        }

        /// <summary>
        /// Formate une surface en m²
        /// </summary>
        public static string FormatArea(double area)
        {
            return $"{area:N1} m²";
        }
    }
}
