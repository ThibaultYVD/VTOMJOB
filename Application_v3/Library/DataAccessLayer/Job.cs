using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Classes
{
    public class Job
    {
        private int id;
        private string etat;
        private string statut;
        private string ch_fh_src;
        private int type_suffixe;
        private List<RepDest> repDest = new List<RepDest>();

        public Job(int id, string etat, string statut, string ch_fh_src, int type_suffixe, List<RepDest> repDest)
        {
            this.id = id;
            this.etat = etat;
            this.statut = statut;
            this.ch_fh_src = ch_fh_src;
            this.type_suffixe = type_suffixe;
            this.repDest = repDest;
        }

        public Job()
        {

        }

        public int GetId()
        {
            return id;
        }
        public string GetEtat()
        {
            return etat;
        }

        public void SetEtat(string etat)
        {
            this.etat = etat;
        }

        public string GetStatut()
        {
            return statut;
        }

        public void SetStatut(string statut)
        {
            this.statut = statut;
        }

        public string GetCheminFichierSrc()
        {
            return ch_fh_src;
        }

        public void SetCheminFichierSrc(string ch_fh_src)
        {
            this.ch_fh_src = ch_fh_src;
        }

        public int GetTypeSuffixe()
        {
            return type_suffixe;
        }

        public void SetTypeSuffixe(int type_suffixe)
        {
            this.type_suffixe = type_suffixe;
        }

        public void SetJob(int id, string etat, string statut, string ch_fh_src, int type_suffixe, List<RepDest> repDest)
        {
            this.id = id;
            this.etat = etat;
            this.statut = statut;
            this.ch_fh_src = ch_fh_src;
            this.type_suffixe = type_suffixe;
            this.repDest = repDest;
        }

        public string afficher()
        {
            return $"Id: {id} \tEtat: {etat}\tStatut: {statut}\tChemin: {ch_fh_src}\tSuffixe: {type_suffixe}";
        }
    }
}
