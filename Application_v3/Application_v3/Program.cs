using Library.Classes;
using Library.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Configuration;
using Application;
using System.Data.SqlClient;
using Library.DB;

namespace Program
{
    class Program
    {

        public static async Task Main(string[] args)
        {
            List<Job> jobs = new List<Job>();
            jobs = Job_DAL.GetJobs(true);

            
            if (jobs != null)
            {
                List<Task> tasks = new List<Task>();
                foreach (Job job in jobs)
                {
                    //Si le fichier source n'existe pas
                    if (Manipulations.VerifFichierSrc(job.GetFichierSrc()) == false)
                    {
                        Job_DAL.UpdateJob(job.GetId(), "En attente", job.GetStatut(), job.GetFichierSrc(), job.GetTypeSuffixe(), job.GetCRLF(), job.GetDateHeure(), job.GetDateHeureResultat(), job.GetResultatEtat());
                    }
                    else
                    {
                        Console.WriteLine(job.afficher());
                        job.SetDateHeure(Horodatage.GetDateHeure());
                        Job_DAL.UpdateJob(job.GetId(), "En cours", job.GetStatut(), job.GetFichierSrc(), job.GetTypeSuffixe(), job.GetCRLF(), job.GetDateHeure(), job.GetDateHeureResultat(), job.GetResultatEtat());
                        tasks.Add(Task.Run(async () => await new Copier().Copy(job)));
                    }
                }
                await Task.WhenAll(tasks);
            }

            int value = Convert.ToInt32(ConfigurationManager.AppSettings["PurgeLogs"]);
            Logs_DAL.DeleteLogs(value);
            Manipulations.PurgeBackup();
        }   
    }
}
