using System;
using System.Data.SqlClient;

namespace Problem_2
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlConnection connection = new SqlConnection("Server=.;Database=MinionsDB;Integrated Security=true");

            connection.Open();

            using (connection)
            {
                var command = new SqlCommand("SELECT V.Name, COUNT(MV.MinionId) 'Count'  FROM Villains V "
                                        + "JOIN MinionsVillains MV ON MV.VillainId = V.Id "
                                        + "GROUP BY V.Name "
                                        + "HAVING COUNT(MV.MinionId) > 1 "
                                        + "ORDER BY Count(*) DESC", connection);

                var reader = command.ExecuteReader();
                using (reader)
                {
                    while (reader.Read())
                    {
                        Console.WriteLine(reader[0] + " - " + reader[1]);
                    }
                }
            }
        }
    }
}
