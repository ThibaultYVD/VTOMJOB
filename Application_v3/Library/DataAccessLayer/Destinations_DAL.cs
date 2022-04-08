using Library.DB;
using Library.Classes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Library.DataAccessLayer
{
    public class Destinations_DAL
    {
        /// <summary>
        /// Demande à la base de données toutes les Destinations d'un Job
        /// </summary>
        /// <param name="id">Indiquer l'ID du Job</param>
        /// <returns>Retourne une liste de Destinations si il y a un résultat, sinon retourne null</returns>
        public static List<Destination> GetJobDestination(int job_id)
        {
            SqlConnection conn = DBUtils.GetDBConnection();

            try
            {
                conn.Open();
                string sql = "SELECT * FROM Destinations WHERE job_id = @job_id;";
                SqlCommand cmd = new SqlCommand(sql, conn);

                List<Destination> destinationsList = new List<Destination>();
                cmd.Parameters.AddWithValue("@job_id", job_id);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            int destination_id = Convert.ToInt32(reader.GetValue(0));
                            string etat_copie = reader.GetString(2);
                            string destination = reader.GetString(3);
                            string droit_acces = reader.GetString(4);
                            Destination uneDestination = new Destination(destination_id, etat_copie, destination, droit_acces);
                            destinationsList.Add(uneDestination);
                        }
                    }
                    else
                    {
                        return null;
                    }

                    return destinationsList;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
            finally
            {
                conn.Close();
            }
        }

        public static Destination GetDestination(int destination_id)
        {
            SqlConnection conn = DBUtils.GetDBConnection();

            try
            {
                conn.Open();
                string sql = "SELECT * FROM Destinations WHERE destination_id = @destination_id;";
                SqlCommand cmd = new SqlCommand(sql, conn);

                
                cmd.Parameters.AddWithValue("@destination_id", destination_id);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int id = Convert.ToInt32(reader.GetValue(0));
                        string etat_copie = reader.GetString(2);
                        string destination = reader.GetString(3);
                        string droit_acces = reader.GetString(4);
                        Destination uneDestination = new Destination(destination_id, etat_copie, destination, droit_acces);
                        return uneDestination;
                    }
                    else
                    {
                        return null;
                    }
                   
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// Ajoute une nouvelle Destination au Job
        /// </summary>
        /// <param name="id">Indiquer l'ID du Job</param>
        /// <param name="etat_copie"></param>
        /// <param name="destination"></param>
        /// <returns>Retourne null si l'opération a fonctionné, sinon retourne un message d'erreur</returns>
        public static string AddJobDestination(int job_id, string etat_copie, string destination, string droit_acces)
        {
            SqlConnection conn = DBUtils.GetDBConnection();

            try
            {
                conn.Open();
                string sql = "INSERT INTO Destinations (job_id, etat_copie, destination, droit_acces) VALUES (@job_id, @etat_copie, @destination, @droit_acces);";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@job_id", job_id);
                cmd.Parameters.AddWithValue("@etat_copie", etat_copie);
                cmd.Parameters.AddWithValue("@destination", destination);
                cmd.Parameters.AddWithValue("@droit_acces", droit_acces);
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
        /// Supprime toutes les destinations d'un Job
        /// </summary>
        /// <param name="job_id">Indiquer l'ID du Job</param>
        /// <returns>Retourne null si l'opération a fonctionné, sinon retourne un message d'erreur</returns>
        public static string DeleteJobDestination(int job_id)
        {
            SqlConnection conn = DBUtils.GetDBConnection();
            try
            {
                conn.Open();
                string sql = "DELETE FROM Destinations WHERE job_id = @job_id;" +
                    "DECLARE @max INT;" +
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
        /// Met à jour l'état de la copie de destination
        /// </summary>
        /// <param name="destination_id">Indiquer l'ID de la Destination</param>
        /// <param name="etat_copie"></param>
        /// <returns>Retourne null si l'opération a fonctionné, sinon retourne un message d'erreur</returns>
        public static string UpdateEtatCopie(int destination_id, string etat_copie)
        {
            SqlConnection conn = DBUtils.GetDBConnection();
            try
            {
                conn.Open();
                string sql = "UPDATE Destinations SET etat_copie = @etat_copie WHERE destination_id = @destination_id";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@destination_id", destination_id);
                cmd.Parameters.AddWithValue("@etat_copie", etat_copie);
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
