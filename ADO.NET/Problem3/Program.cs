using Microsoft.Data.SqlClient;
using System;

namespace Problem3
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlConnection dbCon = new SqlConnection("Server=.; " + "Database=MinionsDb;" + "Integrated Security=true");
            dbCon.Open();
            using (dbCon)
            {
                var villainId = int.Parse(Console.ReadLine());

                SqlCommand command = new SqlCommand("SELECT V.Name AS Villain,M.Name as MinionName,M.Age as Age FROM Minions M" +
                    "    JOIN MinionsVillains MV ON MV.MinionId = M.Id" +
                    "    JOIN Villains V ON MV.VillainId = V.Id" +
                    "    WHERE V.Id = @villainId" +
                    "    ORDER BY M.Name", dbCon);
                command.Parameters.AddWithValue("@villainId", villainId);

                var reader = command.ExecuteReader();
                using (reader) 
                {
                    int count = 1;
                    if (reader.Read())
                    {
                        Console.WriteLine((string)$"Villain: {reader["Villain"]}");
                        Console.WriteLine($"{count++}. {reader["MinionName"]} {reader["Age"]}");
                    }
                    else 
                    {
                        Console.WriteLine($"No villain with ID {villainId} exists in the database.");
                        return;
                    }
                    while (reader.Read())
                    {
                        Console.WriteLine($"{count++}. {reader["MinionName"]} {reader["Age"]}");
                    }
                }

            }
        }
    }
}
