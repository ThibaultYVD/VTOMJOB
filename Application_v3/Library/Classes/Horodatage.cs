using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Classes
{
    public class Horodatage
    {
        public static string GetDate()
        {
            DateTime Jour;
            string jour;
            Jour = DateTime.Now;
            jour = Jour.ToString("yyyyMMd");
            return jour;
        }

        public static string GetHeure()
        {
            DateTime Heure;
            string heure;
            Heure = DateTime.Now;
            heure = Heure.ToString("HHmmss");
            return heure;
        }

        /// <summary>
        /// Récupère la date et l'heure actuel
        /// </summary>
        /// <returns>Retourne une chaine sous la forme yyyyMMdd_HHmmss</returns>
        public static string GetFichierDateHeure()
        {
            return GetDate() + "_" + GetHeure();
        }


        /// <summary>
        /// Récupère la date et l'heure actuel
        /// </summary>
        /// <returns>Retourne une chaine sous la forme dd/MM/yyyy</returns>
        public static string GetDateHeure()
        {
            DateTime DT = DateTime.Now;
            string format = "dd/MM/yyyy HH:mm:ss";
            return DT.ToString(format);
        }

    }
}
