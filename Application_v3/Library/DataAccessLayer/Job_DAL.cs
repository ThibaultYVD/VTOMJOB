using Library.DB;
using Library.Classes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DataAccessLayer
{
    public class Job_DAL
    {
        /// <summary>
        /// Demande à la base de données toutes les Jobs ou tout les Jobs ayant le Statut Actif et l'Etat En attente
        /// </summary>
        /// <param name="arg">Renseigner true fait la sélection des Jobs suivant la condition, Renseigner false fait la sélection de tout les Jobs</param>
        /// <returns>Retourne une liste de Jobs si il y a un résultat, sinon retourne null</returns>
        public static List<Job> GetJobs(bool SelectCondition)
        {
            SqlConnection conn = DBUtils.GetDBConnection();

            try
            {
                conn.Open();
                string sql = "SELECT * FROM Jobs";
                if (SelectCondition == true)
                {
                    sql = "SELECT * FROM Jobs WHERE etat_job = 'En attente' AND statut_job = 'Actif';";
                }
                
                SqlCommand cmd = new SqlCommand(sql, conn);
                List<Job> jobList = new List<Job>();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            int job_id = Convert.ToInt32(reader.GetValue(0));
                            string etat = reader.GetString(1);
                            string statut = reader.GetString(2);
                            string fichier_src = reader.GetString(3);
                            int type_suffixe = Convert.ToInt32(reader.GetValue(4));
                            int crlf = Convert.ToInt32(reader.GetValue(5));
                            string dateheure = reader.GetString(6);
                            string date_heure_resultat = reader.GetString(7);
                            string resultat_etat = reader.GetString(8);
                            List<Destination> destinations = Destinations_DAL.GetJobDestination(job_id);
                            Job unJob = new Job(job_id, etat, statut, fichier_src, type_suffixe, crlf, dateheure, date_heure_resultat, resultat_etat, destinations);
                            jobList.Add(unJob);
                        }
                    }
                    else
                    {
                        return null;
                    }
                    return jobList;
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
        /// Demande à la base de données un Job
        /// </summary>
        /// <param name="job_id">Renseigner l'ID du Job</param>
        /// <returns></returns>
        public static Job GetJob(int job_id)
        {
            SqlConnection conn = DBUtils.GetDBConnection();

            try
            {
                conn.Open();
                string sql = "SELECT * FROM Jobs WHERE job_id = @job_id;";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@job_id", job_id);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    int id = Convert.ToInt32(reader.GetValue(0));
                    string etat = reader.GetString(1);
                    string statut = reader.GetString(2);
                    string fichier_src = reader.GetString(3);
                    int type_suffixe = Convert.ToInt32(reader.GetValue(4));
                    int crlf = Convert.ToInt32(reader.GetValue(5));
                    string dateheure = reader.GetString(6);
                    string date_heure_resultat = reader.GetString(7);
                    string resultat_etat = reader.GetString(8);
                    List<Destination> destinations = Destinations_DAL.GetJobDestination(job_id);
                    Job unJob = new Job(id, etat, statut, fichier_src, type_suffixe, crlf, dateheure, date_heure_resultat, resultat_etat, destinations);
                    return unJob;
                }
                else
                {
                    return null;
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
        /// Ajoute un nouveau Job dans la base de données
        /// </summary>
        /// <param name="etat"></param>
        /// <param name="statut"></param>
        /// <param name="fichier_src"></param>
        /// <param name="type_suffixe"></param>
        /// <param name="crlf"></param>
        /// <param name="dateheure"></param>
        /// <returns>Retourne null si l'opération a fonctionné, sinon retourne un message d'erreur</returns>
        public static string AddJob(string etat, string statut, string fichier_src, int type_suffixe, int crlf, string dateheure, string date_heure_resultat, string etat_resultat)
        {
            SqlConnection conn = DBUtils.GetDBConnection();

            try
            {
                conn.Open();
                string sql = "INSERT INTO Jobs (etat_job, statut_job, fichier_src, type_suffixe, crlf, date_heure, date_heure_resultat, etat_resultat) " +
                    "VALUES (@etat, @statut, @fichier_src, @type_suffixe, @crlf, @dateheure, @date_heure_resultat, @etat_resultat);";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@etat", etat);
                cmd.Parameters.AddWithValue("@statut", statut);
                cmd.Parameters.AddWithValue("@fichier_src", fichier_src);
                cmd.Parameters.AddWithValue("@type_suffixe", type_suffixe);
                cmd.Parameters.AddWithValue("@crlf", crlf);
                cmd.Parameters.AddWithValue("@dateheure", dateheure);
                cmd.Parameters.AddWithValue("@date_heure_resultat", date_heure_resultat);
                cmd.Parameters.AddWithValue("@etat_resultat", etat_resultat);
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

        /// <summary>
        /// Supprime un Job et toute ses destinations dans la base de données, et réinitialise l'incrémentation des tables Jobs et Destinations
        /// </summary>
        /// <param name="job_id"></param>
        /// <returns>Retourne null si l'opération a fonctionné, sinon retourne un message d'erreur</returns>
        public static string DeleteJob(int job_id)
        {
            SqlConnection conn = DBUtils.GetDBConnection();
            try
            {
                conn.Open();
                string sql = "DELETE FROM Destinations WHERE job_id = @job_id;" +
                    "DELETE FROM Jobs WHERE job_id = @job_id;" +
                    "DECLARE @max INT;" +
                    "SELECT @max = max(job_id) FROM Jobs;" +
                    "DBCC CHECKIDENT(Jobs, RESEED, @max);" +
                    "SELECT @max = max(destination_id) FROM Destinations;" +
                    "DBCC CHECKIDENT(Destinations, RESEED, @max);";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@job_id", job_id);
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

        /// <summary>
        /// Met à jour un Job
        /// </summary>
        /// <param name="job_id"></param>
        /// <param name="etat"></param>
        /// <param name="statut"></param>
        /// <param name="fichier_src"></param>
        /// <param name="type_suffixe"></param>
        /// <param name="crlf"></param>
        /// <param name="dateheure"></param>
        /// <returns>Retourne null si l'opération a fonctionné, sinon retourne un message d'erreur</returns>
        public static string UpdateJob(int job_id, string etat, string statut, string fichier_src, int type_suffixe, int crlf, string dateheure, string date_heure_resultat, string etat_resultat)
        {
            SqlConnection conn = DBUtils.GetDBConnection();
            try
            {
                conn.Open();
                string sql = "UPDATE Jobs SET etat_job = @etat, statut_job = @statut, fichier_src = @fichier_src, type_suffixe = @type_suffixe, crlf = @crlf, date_heure = @dateheure, date_heure_resultat = @date_heure_resultat, etat_resultat = @etat_resultat WHERE job_id = @job_id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@job_id", job_id);
                cmd.Parameters.AddWithValue("@etat", etat);
                cmd.Parameters.AddWithValue("@statut", statut);
                cmd.Parameters.AddWithValue("@fichier_src", fichier_src);
                cmd.Parameters.AddWithValue("@type_suffixe", type_suffixe);
                cmd.Parameters.AddWithValue("@crlf", crlf);
                cmd.Parameters.AddWithValue("@dateheure", dateheure);
                cmd.Parameters.AddWithValue("@date_heure_resultat", date_heure_resultat);
                cmd.Parameters.AddWithValue("@etat_resultat", etat_resultat);
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

        /// <summary>
        /// Demande à la base de données l'ID maximum de la table Jobs
        /// </summary>
        /// <returns>Retourne l'ID si l'opération a fonctionné, sinon retourne null</returns>
        public static int? GetMaxID()
        {
            SqlConnection conn = DBUtils.GetDBConnection();

            try
            {
                conn.Open();
                string sql = "SELECT max(job_id) FROM Jobs;";
                SqlCommand cmd = new SqlCommand(sql, conn);
                int id = 0;


                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            id = Convert.ToInt32(reader.GetValue(0));
                        }
                    }
                    else
                    {
                        return null;
                    }
                    return id;
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
    }
}

