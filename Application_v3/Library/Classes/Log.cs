using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Classes
{
    public class Log
    {
        private int log_id;
        private string horodatage;
        private string fichier_src;
        private string destination;
        private string etat_copie;
        private string information;

        public Log(int log_id, string horodatage, string fichier_src, string destination, string etat_copie, string information)
        {
            this.log_id = log_id;
            this.horodatage = horodatage;
            this.fichier_src = fichier_src;
            this.destination = destination;
            this.etat_copie = etat_copie;
            this.information = information;
        }

        public int GetLogID()
        {
            return log_id;
        }

        public string GetHorodatage()
        {
            return horodatage;
        }


        public string GetFichierSrc()
        {
            return fichier_src;
        }

        public string GetDestination()
        {
            return destination;
        }

        public string GetEtatCopie()
        {
            return etat_copie;
        }

        public string GetInformation()
        {
            return information;
        }

        public string toString()
        {
            return $"ID : {log_id} \t Horodatage : {horodatage} \t Fichier source : {fichier_src} \t Destination : {destination} \t Etat copie : {etat_copie} \t Information {information}";
        }

    }
}
