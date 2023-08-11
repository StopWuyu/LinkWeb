using LinkWeb.Modules;
using MySql.Data.MySqlClient;

namespace LinkWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();
            DbConfiguration dbConfig = new();
            using MySqlConnection connection = dbConfig.GetConnection();
            ModBase.Initialize();
            try
            {
                connection.Open();

                // �������ݿ�
                string dn = ModBase.GetConfig("Database:DatabaseName");
                using var createDbCommand = new MySqlCommand($"CREATE DATABASE IF NOT EXISTS {dn};", connection);
                createDbCommand.ExecuteNonQuery();

                // �л������ݿ�
                connection.ChangeDatabase(dn);

                // ������
                using var createTableCommand = new MySqlCommand(
                    $"CREATE TABLE IF NOT EXISTS {ModBase.GetConfig("Database:TableName")} (" +
                    "Id INT AUTO_INCREMENT PRIMARY KEY," +
                    "Username VARCHAR(255)," +
                    "Salt VARCHAR(255)," +
                    "Hash VARCHAR(255));", connection);
                createTableCommand.ExecuteNonQuery();

                Console.WriteLine("Database has been loaded.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            app.Run();

        }
    }
}