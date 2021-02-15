using System;
using System.Data.SqlClient;

namespace Problem_3
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlConnection connection = new SqlConnection("Server=.;Database=MinionsDB;Integrated Security=true");
            connection.Open();

            using (connection)
            {
                var villainId = int.Parse(Console.ReadLine());

                var villainNameCommand = new SqlCommand("SELECT Name FROM Villains " +
                                                    "WHERE Id = @villainId", connection);
                villainNameCommand.Parameters.AddWithValue("@villainId", villainId);
                var villainName = villainNameCommand.ExecuteScalar();

                if (villainName==null)
                {
                    Console.WriteLine($"No villain with ID {villainId} exists in the database.");
                }
                else
                {
                    Console.WriteLine($"Villain: {villainName}");
                }

                var minionReader = new SqlCommand("SELECT M.Name, M.Age FROM Minions M " +
                    "JOIN MinionsVillains MV ON MV.MinionId = M.Id " +
                    "JOIN Villains V ON V.Id = MV.VillainId " +
                    "WHERE V.Id = @villainId " +
                    "ORDER BY M.Name",connection);
                minionReader.Parameters.AddWithValue("@villainId", villainId);
                var reader = minionReader.ExecuteReader();
                int index = 1;
                bool hasAny = false;
                using (reader)
                {
                    while (reader.Read())
                    {
                        hasAny = true;
                        Console.WriteLine($"{index++}. {reader[0]} {reader[1]}");
                    }
                }
                if (!hasAny)
                {
                    Console.WriteLine("(no minions)");
                }
            }
        }
    }
}
