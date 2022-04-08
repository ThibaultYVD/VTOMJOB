using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DB
{
    internal class DBSQLServerUtils
    {
        /// <summary>
        /// Création d'une connection en récupérant les informations de connection
        /// </summary>
        /// <param name="datasource">Source des données (Serveur)</param>
        /// <param name="database"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>Retourne une connection Sql</returns>
        public static SqlConnection GetDBConnection(string datasource, string database, string username, string password)
        {
            //Connection pour SQL Server avec une authentification Windows
            string connString = @"Server=" + datasource + ";" +
                "Database=" + database + ";" +
                "Integrated Security=True;" +
                "MultipleActiveResultSets=True";

            ////Connection pour SQL Server avec une authentification Nom d'utilisateur/Mot de passe
            //string connString = @"Server=" + datasource + ";
            //Database = " + database + ";
            //Persist Security = True;
            //User ID = " + username + ";
            //Password = " + password;

            //Crée et retourne la connection
            SqlConnection conn = new SqlConnection(connString);
            return conn;
        }
    }
}
