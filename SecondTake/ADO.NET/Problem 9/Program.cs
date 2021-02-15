using System;
using System.Data.SqlClient;

namespace Problem_9
{
    class Program
    {
        static void Main(string[] args)
        {
            var connection = new SqlConnection("Server=.;Database=MinionsDB;Integrated Security=true");

            connection.Open();
            using (connection)
            {
                var id = int.Parse(Console.ReadLine());
                var command = new SqlCommand("EXEC usp_GetOlder @id", connection);
                command.Parameters.AddWithValue("@id", id);
                command.ExecuteNonQuery();

                var getMinion = new SqlCommand("SELECT Name, Age FROM Minions WHERE Id=@id", connection);
                getMinion.Parameters.AddWithValue("@id", id);
                var reader = getMinion.ExecuteReader();
                using (reader)
                {
                    reader.Read();
                    Console.WriteLine($"{reader[0]} - {reader[1]} years old");
                }
            }
        }
    }
}
