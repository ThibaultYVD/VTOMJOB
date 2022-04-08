using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Classes
{
    public class Job
    {
        private int job_id;
        private string etat;
        private string statut;
        private string fichier_src;
        private int type_suffixe;
        private int crlf;
        private string date_heure;
        private string date_heure_resultat;
        private string resultat_etat;
        private List<Destination> destinations = new List<Destination>();

        public Job(int job_id, string etat, string statut, string fichier_src, int type_suffixe, int crlf, string date_heure, string date_heure_resultat, string resultat_etat, List<Destination> destinations)
        {
            this.job_id = job_id;
            this.etat = etat;
            this.statut = statut;
            this.fichier_src = fichier_src;
            this.type_suffixe = type_suffixe;
            this.crlf = crlf;
            this.date_heure = date_heure;
            this.date_heure_resultat = date_heure_resultat;
            this.resultat_etat = resultat_etat;
            this.destinations = destinations;
        }

        public int GetId()
        {
            return job_id;
        }

        public string GetEtat()
        {
            return etat;
        }

        public string GetStatut()
        {
            return statut;
        }

        public string GetFichierSrc()
        {
            return fichier_src;
        }

        public int GetTypeSuffixe()
        {
            return type_suffixe;
        }

        public int GetCRLF()
        {
            return crlf;
        }

        public string GetDateHeure()
        {
            return date_heure;
        }

        public string GetDateHeureResultat()
        {
            return date_heure_resultat;
        }

        public string GetResultatEtat()
        {
            return resultat_etat;
        }

        public void SetDateHeure(string date_heure)
        {
            this.date_heure = date_heure;
        }

        public List<Destination> GetJobDestinations()
        {
            return destinations;
        }

        public void SetJobDestination(List<Destination> destinations)
        {
            this.destinations = destinations;
        }

        public string afficher()
        {
            return $"Id: {job_id} \tEtat: {etat}\tStatut: {statut}\tFichier Source: {fichier_src}\tSuffixe: {type_suffixe} \t Dateheure : {date_heure}";
        }
    }
}
