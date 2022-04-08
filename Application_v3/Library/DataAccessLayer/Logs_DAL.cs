using Library.Classes;
using Library.DB;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DataAccessLayer
{
    public class Logs_DAL
    {
        /// <summary>
        /// Demande à la base de données tous les Logs
        /// </summary>
        /// <returns>Retourne une liste de Log si il y a un résultat, sinon retourne null</returns>
        public static List<Log> GetLogs()
        {
            SqlConnection conn = DBUtils.GetDBConnection();

            try
            {
                conn.Open();
                string sql = "SELECT * FROM Logs";
                SqlCommand cmd = new SqlCommand(sql, conn);
                List<Log> LogsList = new List<Log>();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            int log_id = Convert.ToInt32(reader.GetValue(0));
                            string horodatage = reader.GetValue(1).ToString();
                            string fichier_src = reader.GetString(2);
                            string destination = reader.GetString(3);
                            string etat_copie = reader.GetString(4);
                            string information = reader.GetString(5);
                            Log unLog = new Log(log_id, horodatage, fichier_src, destination, etat_copie,information);
                            LogsList.Add(unLog);
                        }
                    }
                    else
                    {

                        return null;
                    }

                    return LogsList;
                }
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                conn.Close();
            }
        }
        
        /// <summary>
        /// Ajoute un nouveau log
        /// </summary>
        /// <param name="horodatage"></param>
        /// <param name="job_id"></param>
        /// <param name="destination_id"></param>
        /// <param name="etat_copie"></param>
        /// <param name="information"></param>
        public static void CreateLog(string horodatage, string fichier_src, string destination, string etat_copie, string information)
        {
            SqlConnection conn = DBUtils.GetDBConnection();

            try
            {
                conn.Open();
                string sql = "INSERT INTO Logs (horodatage, fichier_src, destination, etat_copie, information) VALUES (@horodatage, @fichier_src, @destination, @etat_copie, @information);";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@horodatage", horodatage);
                cmd.Parameters.AddWithValue("@fichier_src", fichier_src);
                cmd.Parameters.AddWithValue("@destination", destination);
                cmd.Parameters.AddWithValue("@etat_copie", etat_copie);
                cmd.Parameters.AddWithValue("@information", information);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// Supprime les log vieux d'un moins nbJours
        /// </summary>
        /// <param name="nbJours">Nombre de jours avant la suppression</param>
        /// <returns>Retourne null si l'opération a fonctionné, sinon retourne un message d'erreur</returns>
        public static string DeleteLogs(int nbJours)
        {
            SqlConnection conn = DBUtils.GetDBConnection();
            try
            {
                conn.Open();
                string sql = "DELETE FROM Logs1 WHERE horodatage > GETDATE() + @nbJours";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@nbJours", nbJours);
                cmd.ExecuteNonQuery();
                return null;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                conn.Close();
            }
        }
    }
}
