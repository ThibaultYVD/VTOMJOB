using Library.DB;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DataAccessLayer
{
    public class Statuts_DAL
    {
        /// <summary>
        /// Demande à la base de données tout les Statuts
        /// </summary>
        /// <returns>Retourne un tableau de string si il y a un résultat, sinon retourne null (Le tableau un une taille de 2)</returns>
        public static string[] GetStatuts()
        {
            SqlConnection conn = DBUtils.GetDBConnection();

            try
            {
                conn.Open();
                string sql = "SELECT * FROM Statuts;";
                SqlCommand cmd = new SqlCommand(sql, conn);

                string[] statuts = new string[2];
                int i = 0;
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            string statut = reader.GetString(0);
                            statuts[i] = statut;
                            i++;
                        }
                    }
                    else
                    {
                        return null;
                    }

                    return statuts;
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
