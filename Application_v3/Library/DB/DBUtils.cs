using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DB
{
    public class DBUtils
    {
        /// <summary>
        /// Instanciation des données de connection
        /// </summary>
        /// <returns>Retourne une connection Sql en renseignant les informations de connection</returns>
        public static SqlConnection GetDBConnection()
        {
            string datasource = "PC53-STINFO";
            string database = "vtomjob";
            string username = "";
            string password = "";

            return DBSQLServerUtils.GetDBConnection(datasource, database, username, password);
        }

        public static bool TryDBConnection()
        {
            try
            {
                SqlConnection conn = GetDBConnection();
                var command = new SqlCommand("select 1", conn);
                conn.Open();
                command.ExecuteScalar();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
