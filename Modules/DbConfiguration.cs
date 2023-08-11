using MySql.Data.MySqlClient;

namespace LinkWeb.Modules
{
    public class DbConfiguration
    {

        public MySqlConnection GetConnection()
        {
            string ip = ModBase.GetConfig("Database:IP");
            int port = int.Parse(ModBase.GetConfig("Database:Port"));
            string username = ModBase.GetConfig("Database:Username");
            string password = ModBase.GetConfig("Database:Password");

            string connectionString = $"server={ip};port={port};user={username};password={password};";
            return new MySqlConnection(connectionString);
        }

        public static string GetConnectionString()
        {
            string ip = ModBase.GetConfig("Database:IP");
            int port = int.Parse(ModBase.GetConfig("Database:Port"));
            string username = ModBase.GetConfig("Database:Username");
            string password = ModBase.GetConfig("Database:Password");
            string database = ModBase.GetConfig("Database:DatabaseName");

            string connectionString = $"server={ip};port={port};user={username};password={password};database={database}";

            return connectionString;
        }
    }

}
