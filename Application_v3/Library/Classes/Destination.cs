using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Classes
{
    public class Destination
    {
        private int destination_id;
        public string etat_copie;
        private string destination;
        private string droit_acces;

        public Destination(int destination_id, string etat_copie, string destination, string droit_acces)
        {
            this.destination_id = destination_id;
            this.etat_copie = etat_copie;
            this.destination = destination;
            this.droit_acces = droit_acces;
        }

        public int GetDestinationId()
        {
            return destination_id;
        }

        public void SetEtatCopie(string etat_copie)
        {
            this.etat_copie = etat_copie;
        }
        public string GetEtatCopie()
        {
            return etat_copie;
        }   

        public string GetDestination()
        {
            return destination;
        }

        public void SetDestination(string destination)
        {
            this.destination = destination;
        }

        public string GetDroitAcces()
        {
            return droit_acces;
        }

        public override string ToString()
        {
            return $"ID : {destination_id}\t Etat : {etat_copie}\t Dest : {destination} \tDRoit {droit_acces}";
        }
    }
}
