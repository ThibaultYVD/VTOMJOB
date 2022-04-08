using Library;
using Library.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Library.Classes;
using System.Configuration;

namespace Application
{
    class Copier
    {
        /// <summary>
        /// Met à jour l'heure d'éxécution du job, vérifie s'il y a des erreurs en appelant CheckForError(), modifie la liste des nom des fichiers de destinations avec ModificationNom
        /// Appelle AsyncParralelCopy() pour lancer une nouvelle tâche de copie
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        public async Task Copy(Job job)
        {
            try
            {
                /*
                if(job.GetResultatEtat() == "Echec")
                {
                    Console.WriteLine("Attente...");
                    double Interval = Convert.ToDouble(ConfigurationManager.AppSettings["JobReplayMin"]);
                    Interval = Interval * 60000;
                    Thread.Sleep(Convert.ToInt32(Interval));
                }
                */
                

                if (CheckForError(job) == false)
                {
                    ModificationNom(job);
                    await AsyncParralelCopy(job); 
                }


                foreach (Destination destination in job.GetJobDestinations())
                {
                    if (destination.GetEtatCopie() == "Succès")
                    {
                        Destinations_DAL.UpdateEtatCopie(destination.GetDestinationId(), "Succès");
                        Logs_DAL.CreateLog(Horodatage.GetDateHeure(), job.GetFichierSrc(), destination.GetDestination(), "Succès", "La copie s'est déroulée avec succès");
                    }
                    else
                    {
                        Destinations_DAL.UpdateEtatCopie(destination.GetDestinationId(), "Echec");
                        Logs_DAL.CreateLog(Horodatage.GetDateHeure(), job.GetFichierSrc(), destination.GetDestination(), "Succès", "La copie est un échec");
                        //Job_DAL.UpdateJob(job.GetId(), "En attente", job.GetStatut(), job.GetFichierSrc(), job.GetTypeSuffixe(), job.GetCRLF(), job.GetDateHeure(), Horodatage.GetDateHeure(), "Echec");
                    }
                }


                var Dest =
                            from rep in Destinations_DAL.GetJobDestination(job.GetId())
                            select rep;


                var DestSucces =
                        from rep in Destinations_DAL.GetJobDestination(job.GetId())
                        where rep.etat_copie == "Succès"
                        select rep;

                List<Destination> destinationsSucces = new List<Destination>(DestSucces);

                //Si toutes les destinations ont été un succès
                if (DestSucces.Count() == Dest.Count())
                {
                    //Le Job est un succès
                    Job_DAL.UpdateJob(job.GetId(), "En attente", job.GetStatut(), job.GetFichierSrc(), job.GetTypeSuffixe(), job.GetCRLF(), job.GetDateHeure(), Horodatage.GetDateHeure(), "Succès");
                    Manipulations.MoveToBackup(job.GetFichierSrc());
                    Manipulations.ChangeAccess(job.GetJobDestinations());
                }
                else
                {
                    //Le job est un échec mais quelques copies ont pu se faire, il faudra le rejouer
                    Job_DAL.UpdateJob(job.GetId(), "En attente", job.GetStatut(), job.GetFichierSrc(), job.GetTypeSuffixe(), job.GetCRLF(), job.GetDateHeure(), Horodatage.GetDateHeure(), "Echec");
                }
            }
            catch (Exception e)
            {
                Logs_DAL.CreateLog(Horodatage.GetDateHeure(), job.GetFichierSrc(), "", "Echec", "Erreur dans Copy() : " + e.Message);
                List<Destination> destinations = Destinations_DAL.GetJobDestination(job.GetId());

                Job_DAL.UpdateJob(job.GetId(), "En attente", job.GetStatut(), job.GetFichierSrc(), job.GetTypeSuffixe(), job.GetCRLF(), job.GetDateHeure(), Horodatage.GetDateHeure(), "Echec");
            }
        }


        /// <summary>
        /// Regarde si les destinations sont disponible et remplace les destinations du Job avec celles pouvant être traitées
        /// </summary>
        /// <param name="job"></param>
        /// <returns>
        /// Retourne false (pas d'erreur) si au moins une destination est disponible.
        /// Retourne true (il y a des erreurs) si toutes les destinations ne sont pas disponible
        /// </returns>
        private static bool CheckForError(Job job)
        {
            //Isole la liste de destination du Job pour les tester un par un
            List<Destination> TestDestinations = Destinations_DAL.GetJobDestination(job.GetId());
            foreach (Destination dest in TestDestinations.ToList())
            {
                //Si le répertoire n'existe pas, on le met en échec et on le supprime de la liste
                if (Manipulations.VerifDirDest(Path.GetDirectoryName(dest.GetDestination())) == false)
                {
                    //Un dossier de destination n'existe pas
                    dest.SetEtatCopie("Echec");
                    Destinations_DAL.UpdateEtatCopie(dest.GetDestinationId(), "Echec");
                    Logs_DAL.CreateLog(Horodatage.GetDateHeure(), job.GetFichierSrc(), dest.GetDestination(), "Echec", $"Le répertoire de destination " + Path.GetDirectoryName(dest.GetDestination()) + " n'existe pas");
                    //On le supprime de la liste pour que la destination ne soit pas traitée
                    TestDestinations.Remove(dest);
                }
                if(dest.GetEtatCopie() == "Succès" && job.GetResultatEtat() == "Echec")
                {
                    TestDestinations.Remove(dest);
                }
            }

            //Si il y a des destinations traitable
            if (TestDestinations.Count != 0)
            {
                //On change la liste de destination du Job avec la liste des destinations disponible pour être traité
                job.SetJobDestination(TestDestinations);
                return false;
            }
            //Si il n'y a aucune destination disponible
            else
            {
                Job_DAL.UpdateJob(job.GetId(), "En attente", job.GetStatut(), job.GetFichierSrc(), job.GetTypeSuffixe(), job.GetCRLF(), job.GetDateHeure(), Horodatage.GetDateHeure(), "Echec");
                return true;
                
            }
        }


        /// <summary>
        /// Incrémente le nom de chaque fichier en destination en fonction de la date et/ou heure
        /// </summary>
        /// <param name="job"></param>
        private void ModificationNom(Job job)
        {
            foreach (Destination destination in job.GetJobDestinations())
            {
                try
                {
                    string DirectoryName = Path.GetDirectoryName(destination.GetDestination());
                    string FileName = Path.GetFileNameWithoutExtension(destination.GetDestination());
                    string extension = Path.GetExtension(destination.GetDestination());

                    switch (job.GetTypeSuffixe())
                    {
                        case 1:
                            FileName += "_" + Horodatage.GetDate();
                            break;

                        case 2:
                            FileName += "_" + Horodatage.GetFichierDateHeure();
                            break;
                    }

                    string FullPath = Path.Combine(DirectoryName + @"\" + FileName + extension);
                    destination.SetDestination(FullPath);
                }
                catch (Exception e)
                {
                    Logs_DAL.CreateLog(Horodatage.GetDateHeure(), job.GetFichierSrc(), "", "Echec", "Erreur dans ModificationNom() : " + e.Message);
                    Job_DAL.UpdateJob(job.GetId(), "En attente", job.GetStatut(), job.GetFichierSrc(), job.GetTypeSuffixe(), job.GetCRLF(), job.GetDateHeure(), Horodatage.GetDateHeure(), "Echec");
                }
            }
        }


        /// <summary>
        /// Créé une tâche de copie asynchrone
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        private async Task AsyncParralelCopy(Job job)
        {
            List<Task> tasks = new List<Task>();
 
            foreach (Destination destination in job.GetJobDestinations())
            {
                try
                {
                    tasks.Add(Task.Run(() => CopyStream(job, destination)));

                    destination.SetEtatCopie("Succès");
                    
                }
                catch (Exception e)
                {
                    destination.SetEtatCopie("Echec");
                    
                    Logs_DAL.CreateLog(Horodatage.GetDateHeure(), job.GetFichierSrc(), destination.GetDestination(), "Echec", "Erreur dans AsyncParralelCopy() : " + e.Message);
                }
            }

            await Task.WhenAll(tasks);

            
        }
        

        /// <summary>
        /// Utilise les Stream pour copier coller le fichier, en plus de faire le traitement LF -> CRLF
        /// </summary>
        /// <param name="job"></param>
        /// <param name="destination"></param>
        private static void CopyStream(Job job, Destination destination)
        {
            try
            {
                if (File.Exists(destination.GetDestination()))
                {
                    File.SetAttributes(destination.GetDestination(), FileAttributes.Normal);
                    File.Delete(destination.GetDestination());
                }
                
                using (StreamReader reader = new StreamReader(job.GetFichierSrc(), Manipulations.EncodingANSI()))
                using (StreamWriter writer = new StreamWriter(destination.GetDestination(), append: false, Manipulations.EncodingANSI()))
                {
                    writer.Write(reader.ReadToEnd());
                }
                
                if (job.GetCRLF() == 1)
                {
                    Manipulations.ReplaceLF(destination.GetDestination());
                }
            }
            catch (Exception e)
            {
                Logs_DAL.CreateLog(Horodatage.GetDateHeure(), job.GetFichierSrc(), "", "Echec", "Erreur dans CopyStream() : " + e.Message);

                destination.SetEtatCopie("Echec");
                Destinations_DAL.UpdateEtatCopie(destination.GetDestinationId(), "Echec");


                Job_DAL.UpdateJob(job.GetId(), "En attente", job.GetStatut(), job.GetFichierSrc(), job.GetTypeSuffixe(), job.GetCRLF(), job.GetDateHeure(), Horodatage.GetDateHeure(), "Echec");
            }
        }
    }
}
