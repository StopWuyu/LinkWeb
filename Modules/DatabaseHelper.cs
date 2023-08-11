using MySql.Data.MySqlClient;
using System.Data;

namespace LinkWeb.Modules
{

    public class DatabaseHelper
    {
        private readonly string _connectionString;

        public DatabaseHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void InsertUserData(string username, string salt, string hash)
        {
            using MySqlConnection connection = new(_connectionString);
            string query = $"INSERT INTO {ModBase.GetConfig("Database:TableName")} (Username, Salt, Hash) VALUES (@Username, @Salt, @Hash)";

            using   MySqlCommand command = new(query, connection);
            command.Parameters.AddWithValue("@Username", username);
            command.Parameters.AddWithValue("@Salt", salt);
            command.Parameters.AddWithValue("@Hash", hash);

            connection.Open();
            command.ExecuteNonQuery();
        }

        public DataRow? GetUserByUsername(string username)
        {
            using MySqlConnection connection = new(_connectionString);
            string query = $"SELECT * FROM {ModBase.GetConfig("Database:TableName")} WHERE Username = @Username";

            using MySqlCommand command = new(query, connection);
            command.Parameters.AddWithValue("@Username", username);

            connection.Open();

            using MySqlDataAdapter adapter = new(command);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);

            if (dataSet.Tables[0].Rows.Count > 0)
            {
                return dataSet.Tables[0].Rows[0];
            }

            return null;
        }
    }

}
