using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Problem8
{
    class Program
    {
        static void Main(string[] args)
        {

            var ids = new List<int>();
            var newNames = new List<string>();
            TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
            SqlConnection dbCon = new SqlConnection("Server=.; " + "Database=MinionsDb;" + "Integrated Security=true");
            var inputIDs = Console.ReadLine().Split(" ").Select(int.Parse).ToList();
            var outputList = new List<string>();
            dbCon.Open();
            using (dbCon)
            {
                var selectTheMinions = new SqlCommand($"SELECT * FROM MINIONS WHERE Id IN ({string.Join(", ", inputIDs)})", dbCon);
                var reader = selectTheMinions.ExecuteReader();
                using (reader)
                {
                    while (reader.Read())
                    {
                        var id = (int)reader["Id"];
                        var name = ti.ToTitleCase((string)reader["Name"]);
                        ids.Add(id);
                        newNames.Add(name);
                    }
                }

                for (int i = 0; i < ids.Count(); i++)
                {
                    var updateTheAge = new SqlCommand($"UPDATE Minions " +
                           $"SET Age += 1, Name = @name " +
                           $"WHERE Id = @id", dbCon);
                    updateTheAge.Parameters.AddWithValue("@name", newNames[i]);
                    updateTheAge.Parameters.AddWithValue("@id", ids[i]);
                    updateTheAge.ExecuteNonQuery();
                }


                var selectEverything = new SqlCommand("SELECT Name + ' ' + CONVERT(VARCHAR(5),Age) AS Output FROM Minions", dbCon);
                var readerSelect = selectEverything.ExecuteReader();
                using (readerSelect)
                {
                    while (readerSelect.Read())
                    {
                        Console.WriteLine(readerSelect["Output"]);
                    }
                }
            }
        }
    }
}