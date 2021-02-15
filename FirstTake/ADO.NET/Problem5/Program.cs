using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;

namespace Problem5
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlConnection dbCon = new SqlConnection("Server=.; " + "Database=MinionsDb;" + "Integrated Security=true");
            dbCon.Open();
            using (dbCon)
            {
                var country = Console.ReadLine();
                SqlCommand SelectAllTowns = new SqlCommand("SELECT T.Name, T.Id FROM Towns T " +
                    "JOIN Countries C ON C.Id = T.ContryCode " +
                    " WHERE C.Name = @country", dbCon);
                SelectAllTowns.Parameters.AddWithValue("@country", country);
                var reader = SelectAllTowns.ExecuteReader();

                var towns = new List<string>();
                var ids = new List<int>();
                using (reader)
                {
                    bool hasAnything = false;
                    while (reader.Read())
                    {
                        hasAnything = true;
                        int id = (int)reader["Id"];
                        string town = (string)reader["Name"];
                        town = town.ToUpper();

                        towns.Add(town);
                        ids.Add(id);
                    }

                    if (hasAnything == false)
                    {
                        Console.WriteLine("No town names were affected.");
                        return;
                    }
                }

                for (int i = 0; i < towns.Count(); i++)
                {
                    SqlCommand updateTown = new SqlCommand("UPDATE Towns " +
                                                                        "SET Name = @newName " +
                                                                        "WHERE Id = @id ", dbCon);
                    updateTown.Parameters.AddWithValue("@newName", towns[i]);
                    updateTown.Parameters.AddWithValue("@Id", ids[i]);
                    updateTown.ExecuteNonQuery();
                }

                Console.WriteLine($"{towns.Count()} town names were affected. ");
                Console.WriteLine($"[{string.Join(", ", towns)}]");
            }
        }
    }
}
