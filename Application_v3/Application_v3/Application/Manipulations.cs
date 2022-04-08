using Library.Classes;
using Library.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Text.CodePagesEncodingProvider;

namespace Application
{
    public class Manipulations
    {
        /// <summary>
        /// Instancie l'encodage en ANSI
        /// </summary>
        /// <returns>Retourne un Encodage</returns>
        internal static Encoding EncodingANSI()
        {
            Encoding.RegisterProvider(Instance);
            Encoding enc = Encoding.GetEncoding(1252);
            return enc;
        }



        /// <summary>
        /// Vérifie si le fichier source est opérationnel
        /// </summary>
        /// <param name="fichier_src"></param>
        /// <returns>Retourne true si le fichier est lisible ou false si le fichier est inexistant</returns>
        internal static bool VerifFichierSrc(string fichier_src)
        {
            if (File.Exists(fichier_src))
            {
                try
                {
                    var file = new FileInfo(fichier_src);
                    using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        stream.Close();
                    }
                }
                catch (IOException)
                {
                    return false;
                }

                return true;
            }
            else
            {
                return false;
            }
        }



        /// <summary>
        /// Vérifie si le répertoire de destination existe
        /// </summary>
        /// <param name="Dir"></param>
        /// <returns>Retourne true si le répertoire existe, false si ce n'est pas le cas</returns>
        internal static bool VerifDirDest(string Dir)
        {
            if (Directory.Exists(Dir))
            {
                return true;
            }
            else
            {
                return false;
            }
        }



        /// <summary>
        /// Déplace un fichier dans un dossier. Créé un répertoire dans un dossier de backup sous le nom du jour actuel si il n'existe pas déjà 
        /// </summary>
        /// <param name="file"></param>
        internal static void MoveToBackup(string file)
        {
            DateTime DT = DateTime.Now;
            string date = DT.ToString("dd-MM-yyyy");

            string filename = Path.GetFileName(file);
            string backupDir = ConfigurationManager.AppSettings["BackupDir"];

            string DirPath = Path.Combine(backupDir, date);

            if (!Directory.Exists(DirPath))
            {
                Directory.CreateDirectory(DirPath);
            }
            string destination = Path.Combine(DirPath, filename);
            File.Move(file, destination);
        }



        /// <summary>
        /// Remplace les LF en CRLF dans un document
        /// </summary>
        /// <param name="file"></param>
        internal static void ReplaceLF(string file)
        {
            StreamReader reader = new StreamReader(file, EncodingANSI());
            string text = reader.ReadToEnd();
            reader.Close();

            text = Regex.Replace(text, "\n", "\r\n");

            StreamWriter writer = new StreamWriter(file, append: false, EncodingANSI());
            writer.Write(text);
            writer.Close();
        }



        /// <summary>
        /// Change l'accès au fichier en lecture seule
        /// </summary>
        /// <param name="file"></param>
        internal static void ChangeAccess(List<Destination> destinations)
        {

            foreach (Destination dest in destinations)
            {
                try
                {
                    if (dest.GetDroitAcces() == "Lecture seule")
                    {
                        var attr = File.GetAttributes(dest.GetDestination());

                        // set read-only
                        attr = attr | FileAttributes.ReadOnly;
                        File.SetAttributes(dest.GetDestination(), attr);
                    }
                    else if (dest.GetDroitAcces() == "Lecture et écriture")
                    {
                        File.SetAttributes(dest.GetDestination(), FileAttributes.Normal);
                    }
                }
                catch (Exception e)
                {
                    Logs_DAL.CreateLog(Horodatage.GetDateHeure(), "", "", "Echec", "Erreur dans ChangeAccess() : " + e.Message);

                    dest.SetEtatCopie("Echec");
                    Destinations_DAL.UpdateEtatCopie(dest.GetDestinationId(), "Echec");

                }
            }
        }



        /// <summary>
        /// Supprime tout les répertoires plus vieux de x jours dans un dossier de backup
        /// </summary>
        public static void PurgeBackup()
        {
            string[] sousRepertoires = Directory.GetDirectories(ConfigurationManager.AppSettings["BackupDir"]);

            foreach (string dir in sousRepertoires)
            {
                string date = Path.GetDirectoryName(dir + @"\");
                var dirName = new DirectoryInfo(date).Name;
                DateTime dt = Convert.ToDateTime(dirName);
                int PurgeDays = Convert.ToInt32(ConfigurationManager.AppSettings["PurgeLogs"]);

                if (dt < DateTime.Now.AddDays(-PurgeDays))
                {
                    Directory.Delete(dir);
                }
            }
        }
    }
}
