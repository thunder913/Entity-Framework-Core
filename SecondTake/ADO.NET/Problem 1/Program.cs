using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace Problem_8
{
    class Program
    {
        static void Main(string[] args)
        {
            var minionIds = Console.ReadLine().Split().Select(int.Parse).ToList();
            var sqlIdsParsed= string.Join(", ", minionIds);
            TextInfo myTI = new CultureInfo("en-US", false).TextInfo;

            var connection = new SqlConnection("Server=.;Database=MinionsDB;Integrated Security=true");
            connection.Open();
            using (connection)
            {

                var minions = new List<Minion>();
                var getMinions = new SqlCommand($"SELECT Id, Name FROM Minions WHERE Id IN ({sqlIdsParsed})", connection);
                var reader = getMinions.ExecuteReader();

                using (reader)
                {
                    while (reader.Read())
                    {
                        minions.Add(new Minion(reader[1].ToString(), int.Parse(reader[0].ToString())));
                    }
                }
                for (int i = 0; i < minions.Count(); i++)
                {
                var updateMinion = new SqlCommand("UPDATE Minions SET Age += 1, Name = @name WHERE Id = @id", connection);
                    updateMinion.Parameters.AddWithValue("@id",minions[i].Id);
                    updateMinion.Parameters.AddWithValue("@name", myTI.ToTitleCase(minions[i].Name));
                    updateMinion.ExecuteNonQuery();
                }

                var getAllMinions = new SqlCommand("SELECT Name,Age FROM Minions", connection);
                reader = getAllMinions.ExecuteReader();
                using (reader)
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader[0]} {reader[1]}");
                    }
                }
            }
        }
    }
}
