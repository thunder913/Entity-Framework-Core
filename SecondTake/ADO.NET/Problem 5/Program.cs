using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Problem_5
{
    class Program
    {
        static void Main(string[] args)
        {
            var countryName = Console.ReadLine();

            var connection = new SqlConnection("Server=.;Database=MinionsDB;Integrated Security=true");
            connection.Open();
            using (connection)
            {
                var updateTowns = new SqlCommand("UPDATE Towns SET Name = UPPER(Name) WHERE CountryCode IN(SELECT Id FROM Countries WHERE Name = @name)", connection);
                updateTowns.Parameters.AddWithValue("@name", countryName);
                updateTowns.ExecuteNonQuery();

                var getTowns = new SqlCommand("SELECT Name FROM Towns T WHERE CountryCode IN (SELECT Id FROM Countries WHERE Name=@name)", connection);
                getTowns.Parameters.AddWithValue("@name", countryName);
                var towns = new List<string>();
                var reader = getTowns.ExecuteReader();
                using (reader)
                {
                    while (reader.Read())
                    {
                        towns.Add(reader[0].ToString());
                    }
                }

                if (towns.Count()==0)
                {
                    Console.WriteLine("No town names were affected.");
                }
                else
                {
                    Console.WriteLine($"{towns.Count()} town names were affected.");
                    Console.WriteLine($"[{String.Join(", ", towns)}]");
                }
            }
        }
    }
}
