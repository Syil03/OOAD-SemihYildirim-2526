using Microsoft.Data.SqlClient;

namespace DokterspraktijkLib.Helpers
{
    public class DatabaseHelper
    {
        private static string connectionString =
            @"Server=MSI\SQLEXPRESS;Database=DokterspraktijkDB;Trusted_Connection=True;TrustServerCertificate=True;";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}